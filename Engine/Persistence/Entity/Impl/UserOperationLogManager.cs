using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.History.Impl.Event;
using ESS.FW.Bpm.Engine.History.Impl.Producer;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.oplog;
using ESS.FW.DataAccess;
using Microsoft.EntityFrameworkCore;
using Autofac.Features.AttributeFilters;

using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Persistence.Entity.Impl;
using Microsoft.Extensions.Logging;
using ESS.FW.Common.Components;

namespace ESS.FW.Bpm.Engine.Persistence.Entity.Impl
{

    /// <summary>
	/// Manager for <seealso cref="UserOperationLogEntryEventEntity"/> that also provides a generic and some specific log methods.
	/// 
	///  Danny Gräf
	/// </summary>
    [Component]
    public class UserOperationLogManager : AbstractHistoricManagerNet<UserOperationLogEntryEventEntity>, IUserOperationLogManager
    {
        protected DbContext dbContex;
        protected IExecutionManager processInstanceManager;
        protected IProcessDefinitionManager processDefinitionManager;
        protected IJobManager jobManager;
        protected IJobDefinitionManager jobDefinitionManager;
        protected ITaskManager taskManager;
        public UserOperationLogManager(
            DbContext dbContex,
            IExecutionManager _processInstanceManager,
            IProcessDefinitionManager _processDefinitionManager,
            ILoggerFactory loggerFactory,
            IJobManager _jobManager,
            IJobDefinitionManager _jobDefinitionManager,
            ITaskManager _taskManager
            , IDGenerator idGenerator) : base(dbContex, loggerFactory, idGenerator)
        {
            processInstanceManager = _processInstanceManager;
            processDefinitionManager = _processDefinitionManager;
            jobManager = _jobManager;
            jobDefinitionManager = _jobDefinitionManager;
            taskManager = _taskManager;
        }
        public virtual IUserOperationLogEntry FindOperationLogById(string entryId)
        {
            //return DbEntityManager.SelectById<UserOperationLogEntryEventEntity>(typeof(UserOperationLogEntryEventEntity), entryId);
            return Get(entryId);//.SelectById(entryId);
        }

        //	  public virtual long FindOperationLogEntryCountByQueryCriteria(UserOperationLogQueryImpl query)
        //	  {
        //		AuthorizationManager.ConfigureUserOperationLogQuery(query);
        //            //return (long) DbEntityManager.SelectOne("selectUserOperationLogEntryCountByQueryCriteria", query);
        //            throw new NotImplementedException();
        //	  }

        ////JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        ////ORIGINAL LINE: @SuppressWarnings("unchecked") public java.Util.List<org.camunda.bpm.engine.history.UserOperationLogEntry> findOperationLogEntriesByQueryCriteria(org.camunda.bpm.engine.impl.UserOperationLogQueryImpl query, org.camunda.bpm.engine.impl.Page page)
        //	  public virtual IList<IUserOperationLogEntry> FindOperationLogEntriesByQueryCriteria(UserOperationLogQueryImpl query, Page page)
        //	  {
        //		//AuthorizationManager.ConfigureUserOperationLogQuery(query);
        //		//return ListExt.ConvertToListT<IUserOperationLogEntry>( DbEntityManager.SelectList("selectUserOperationLogEntriesByQueryCriteria", query, page));
        //            throw new NotImplementedException();
        //	  }

        public virtual void DeleteOperationLogEntryById(string entryId)
        {
            if (HistoryLevelFullEnabled)
            {
                //DbEntityManager.Delete(typeof(UserOperationLogEntryEventEntity), "deleteUserOperationLogEntryById", entryId);
                Delete(entryId);
            }
        }

        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
        //ORIGINAL LINE: protected void fireUserOperationLog(final org.camunda.bpm.engine.impl.oplog.UserOperationLogContext context)
        protected  void FireUserOperationLog(UserOperationLogContext context)
        {
            if (context.UserId == null)
            {
                context.UserId = AuthenticatedUserId;
            }

            HistoryEventProcessor.ProcessHistoryEvents(new HistoryEventCreatorAnonymousInnerClassHelper(this, context));
        }

        private class HistoryEventCreatorAnonymousInnerClassHelper : HistoryEventProcessor.HistoryEventCreator
        {
            private readonly UserOperationLogManager _outerInstance;

            private UserOperationLogContext _context;

            public HistoryEventCreatorAnonymousInnerClassHelper(UserOperationLogManager outerInstance, UserOperationLogContext context)
            {
                this._outerInstance = outerInstance;
                this._context = context;
            }

            public override IList<HistoryEvent> CreateHistoryEvents(IHistoryEventProducer producer)
            {
                return producer.CreateUserOperationLogEvents(_context);
            }
        }

        public virtual void LogUserOperations(UserOperationLogContext context)
        {
            if (UserOperationLogEnabled)
            {
                FireUserOperationLog(context);
            }
        }

        public virtual void LogTaskOperations(string operation, TaskEntity task, IList<PropertyChange> propertyChanges)
        {
            if (UserOperationLogEnabled)
            {
                UserOperationLogContext context = new UserOperationLogContext();
                UserOperationLogContextEntryBuilder entryBuilder = UserOperationLogContextEntryBuilder.Entry(operation, EntityTypes.Task).InContextOf(task, propertyChanges);

                context.AddEntry(entryBuilder.Create());
                FireUserOperationLog(context);
            }
        }

        public virtual void LogLinkOperation(string operation, TaskEntity task, PropertyChange propertyChange)
        {
            if (UserOperationLogEnabled)
            {
                UserOperationLogContext context = new UserOperationLogContext();
                UserOperationLogContextEntryBuilder entryBuilder = UserOperationLogContextEntryBuilder.Entry(operation, EntityTypes.IdentityLink).InContextOf(task, new List<PropertyChange>() { propertyChange });

                context.AddEntry(entryBuilder.Create());
                FireUserOperationLog(context);
            }
        }

        public virtual void LogProcessInstanceOperation(string operation, string processInstanceId, string processDefinitionId, string processDefinitionKey, IList<PropertyChange> propertyChanges)
        {
            if (UserOperationLogEnabled)
            {

                UserOperationLogContext context = new UserOperationLogContext();
                UserOperationLogContextEntryBuilder entryBuilder = UserOperationLogContextEntryBuilder.Entry(operation, EntityTypes.ProcessInstance).PropertyChanges(propertyChanges).ProcessInstanceId(processInstanceId).ProcessDefinitionId(processDefinitionId).ProcessDefinitionKey(processDefinitionKey);

                if (processInstanceId != null)
                {
                    ExecutionEntity instance = processInstanceManager.FindExecutionById(processInstanceId);

                    if (instance != null)
                    {
                        entryBuilder.InContextOf(instance);
                    }
                }
                else if (processDefinitionId != null)
                {
                    ProcessDefinitionEntity definition = processDefinitionManager.FindLatestProcessDefinitionById(processDefinitionId);
                    if (definition != null)
                    {
                        entryBuilder.InContextOf(definition);
                    }
                }

                context.AddEntry(entryBuilder.Create());
                FireUserOperationLog(context);
            }
        }

        public virtual void LogProcessDefinitionOperation(string operation, string processDefinitionId, string processDefinitionKey, PropertyChange propertyChange)
        {
            if (UserOperationLogEnabled)
            {

                UserOperationLogContext context = new UserOperationLogContext();
                UserOperationLogContextEntryBuilder entryBuilder = UserOperationLogContextEntryBuilder.Entry(operation, EntityTypes.ProcessDefinition).PropertyChanges(propertyChange).ProcessDefinitionId(processDefinitionId).ProcessDefinitionKey(processDefinitionKey);

                if (processDefinitionId != null)
                {
                    ProcessDefinitionEntity definition = processDefinitionManager.FindLatestProcessDefinitionById(processDefinitionId);
                    entryBuilder.InContextOf(definition);
                }

                context.AddEntry(entryBuilder.Create());

                FireUserOperationLog(context);
            }
        }

        public virtual void LogJobOperation(string operation, string jobId, string jobDefinitionId, string processInstanceId, string processDefinitionId, string processDefinitionKey, PropertyChange propertyChange)
        {
            if (UserOperationLogEnabled)
            {

                UserOperationLogContext context = new UserOperationLogContext();
                UserOperationLogContextEntryBuilder entryBuilder = UserOperationLogContextEntryBuilder.Entry(operation, EntityTypes.Job).JobId(jobId).JobDefinitionId(jobDefinitionId).ProcessDefinitionId(processDefinitionId).ProcessDefinitionKey(processDefinitionKey).PropertyChanges(propertyChange);

                if (jobId != null)
                {
                    JobEntity job = jobManager.FindJobById(jobId);
                    // Backward compatibility
                    if (job != null)
                    {
                        entryBuilder.InContextOf(job);
                    }
                }
                else

                {
                    if (jobDefinitionId != null)
                    {
                        JobDefinitionEntity jobDefinition = jobDefinitionManager.FindById(jobDefinitionId);
                        // Backward compatibility
                        if (jobDefinition != null)
                        {
                            entryBuilder.InContextOf(jobDefinition);
                        }
                    }
                    else if (processInstanceId != null)
                    {
                        ExecutionEntity processInstance = processInstanceManager.FindExecutionById(processInstanceId);
                        // Backward compatibility
                        if (processInstance != null)
                        {
                            entryBuilder.InContextOf(processInstance);
                        }
                    }
                    else if (processDefinitionId != null)
                    {
                        ProcessDefinitionEntity definition = processDefinitionManager.FindLatestProcessDefinitionById(processDefinitionId);
                        // Backward compatibility
                        if (definition != null)
                        {
                            entryBuilder.InContextOf(definition);
                        }
                    }
                }

                context.AddEntry(entryBuilder.Create());
                FireUserOperationLog(context);
            }
        }

        public virtual void LogJobDefinitionOperation(string operation, string jobDefinitionId, string processDefinitionId, string processDefinitionKey, PropertyChange propertyChange)
        {
            if (UserOperationLogEnabled)
            {
                UserOperationLogContext context = new UserOperationLogContext();
                UserOperationLogContextEntryBuilder entryBuilder = UserOperationLogContextEntryBuilder.Entry(operation, EntityTypes.JobDefinition).JobDefinitionId(jobDefinitionId).ProcessDefinitionId(processDefinitionId).ProcessDefinitionKey(processDefinitionKey).PropertyChanges(propertyChange);

                if (jobDefinitionId != null)
                {
                    JobDefinitionEntity jobDefinition = jobDefinitionManager.FindById(jobDefinitionId);
                    // Backward compatibility
                    if (jobDefinition != null)
                    {
                        entryBuilder.InContextOf(jobDefinition);
                    }
                }
                else if (processDefinitionId != null)
                {
                    ProcessDefinitionEntity definition = processDefinitionManager.FindLatestProcessDefinitionById(processDefinitionId);
                    // Backward compatibility
                    if (definition != null)
                    {
                        entryBuilder.InContextOf(definition);
                    }
                }

                context.AddEntry(entryBuilder.Create());

                FireUserOperationLog(context);
            }
        }

        public virtual void LogAttachmentOperation(string operation, TaskEntity task, PropertyChange propertyChange)
        {
            if (UserOperationLogEnabled)
            {
                UserOperationLogContext context = new UserOperationLogContext();

                UserOperationLogContextEntryBuilder entryBuilder = UserOperationLogContextEntryBuilder.Entry(operation, EntityTypes.Attachment).InContextOf(task, new List<PropertyChange>() { propertyChange });
                context.AddEntry(entryBuilder.Create());

                FireUserOperationLog(context);
            }
        }

        public virtual void LogAttachmentOperation(string operation, ExecutionEntity processInstance, PropertyChange propertyChange)
        {
            if (UserOperationLogEnabled)
            {
                UserOperationLogContext context = new UserOperationLogContext();

                UserOperationLogContextEntryBuilder entryBuilder = UserOperationLogContextEntryBuilder.Entry(operation, EntityTypes.Attachment).InContextOf(processInstance, new List<PropertyChange>() { propertyChange });
                context.AddEntry(entryBuilder.Create());

                FireUserOperationLog(context);
            }
        }

        public virtual void LogVariableOperation(string operation, string executionId, string taskId, PropertyChange propertyChange)
        {
            if (UserOperationLogEnabled)
            {

                UserOperationLogContext context = new UserOperationLogContext();

                UserOperationLogContextEntryBuilder entryBuilder = UserOperationLogContextEntryBuilder.Entry(operation, EntityTypes.Variable).PropertyChanges(propertyChange);

                if (executionId != null)
                {
                    ExecutionEntity execution = processInstanceManager.FindExecutionById(executionId);
                    entryBuilder.InContextOf(execution);
                }
                else if (taskId != null)
                {
                    TaskEntity task = taskManager.FindTaskById(taskId);
                    entryBuilder.InContextOf(task, new List<PropertyChange>() { propertyChange });
                }

                context.AddEntry(entryBuilder.Create());
                FireUserOperationLog(context);
            }
        }

        public virtual void LogDeploymentOperation(string operation, string deploymentId, IList<PropertyChange> propertyChanges)
        {
            if (UserOperationLogEnabled)
            {

                UserOperationLogContext context = new UserOperationLogContext();

                UserOperationLogContextEntryBuilder entryBuilder = UserOperationLogContextEntryBuilder.Entry(operation, EntityTypes.Deployment).DeploymentId(deploymentId).PropertyChanges(propertyChanges);

                context.AddEntry(entryBuilder.Create());
                FireUserOperationLog(context);
            }

        }

        protected internal virtual bool UserOperationLogEnabled
        {
            get
            {
                return HistoryLevelFullEnabled && ((UserOperationLogEnabledOnCommandContext && UserAuthenticated) || !WriteUserOperationLogOnlyWithLoggedInUser());
            }
        }

        protected internal virtual bool UserAuthenticated
        {
            get
            {
                string userId = AuthenticatedUserId;
                return userId != null && userId.Length > 0;
            }
        }

        protected internal virtual string AuthenticatedUserId
        {
            get
            {
                CommandContext commandContext = context.Impl.Context.CommandContext;
                return commandContext.AuthenticatedUserId;
            }
        }

        protected internal virtual bool WriteUserOperationLogOnlyWithLoggedInUser()
        {
            return context.Impl.Context.CommandContext.RestrictUserOperationLogToAuthenticatedUsers;
        }

        protected internal virtual bool UserOperationLogEnabledOnCommandContext
        {
            get
            {
                return context.Impl.Context.CommandContext.UserOperationLogEnabled;
            }
        }

        public virtual void LogBatchOperation(string operation, string batchId, PropertyChange propertyChange)
        {
            if (UserOperationLogEnabled)
            {
                UserOperationLogContext context = new UserOperationLogContext();
                UserOperationLogContextEntryBuilder entryBuilder = UserOperationLogContextEntryBuilder.Entry(operation, EntityTypes.Batch).BatchId(batchId).PropertyChanges(propertyChange);

                context.AddEntry(entryBuilder.Create());

                FireUserOperationLog(context);
            }
        }

    }

}