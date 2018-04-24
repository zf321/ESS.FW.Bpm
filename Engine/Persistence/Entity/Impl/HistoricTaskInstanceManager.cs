using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.History.Impl;
using ESS.FW.Bpm.Engine.History.Impl.Event;
using ESS.FW.Bpm.Engine.History.Impl.Producer;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using Microsoft.Extensions.Logging;
using ESS.FW.DataAccess;
using System.Linq;
using Autofac.Features.AttributeFilters;

using ESS.FW.Common.Components;

namespace ESS.FW.Bpm.Engine.Persistence.Entity.Impl
{

    /// <summary>
    ///   
    /// </summary>
    [Component]
    public class HistoricTaskInstanceManager : AbstractHistoricManagerNet<HistoricTaskInstanceEventEntity>, IHistoricTaskInstanceManager
    {
        protected ITaskManager taskManager;
        public HistoricTaskInstanceManager(DbContext dbContex, ILoggerFactory loggerFactory, ITaskManager _taskManager, IDGenerator idGenerator) : base(dbContex, loggerFactory, idGenerator)
        {
            taskManager = _taskManager;
        }

        public virtual void DeleteHistoricTaskInstancesByProcessInstanceId(string processInstanceId)
        {
            DeleteHistoricTaskInstances("processInstanceId", processInstanceId);
        }

        public virtual void DeleteHistoricTaskInstancesByCaseInstanceId(string caseInstanceId)
        {
            DeleteHistoricTaskInstances("caseInstanceId", caseInstanceId);
        }

        public virtual void DeleteHistoricTaskInstancesByCaseDefinitionId(string caseDefinitionId)
        {
            DeleteHistoricTaskInstances("caseDefinitionId", caseDefinitionId);
        }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") protected void deleteHistoricTaskInstances(String key, String value)
        protected internal virtual void DeleteHistoricTaskInstances(string key, string value)
        {
            if (HistoryEnabled)
            {

                //IDictionary<string, string> @params = new Dictionary<string, string>();
                //@params[key] = value;
                //IList<string> taskInstanceIds =ListExt.ConvertToListT<string>(DbEntityManager.SelectList("selectHistoricTaskInstanceIdsByParameters", @params)) ;
                //foreach (string taskInstanceId in taskInstanceIds)
                //{
                //    DeleteHistoricTaskInstanceById(taskInstanceId);
                //}
                IList<string> taskInstanceIds = null;
                switch (key)
                {
                    case "processInstanceId":
                        taskInstanceIds = Find(m => m.ProcessInstanceId == value).Select(m => m.Id).ToList();
                        break;
                    case "processDefinitionId":
                        taskInstanceIds = Find(m => m.ProcessDefinitionId == value).Select(m => m.Id).ToList();
                        break;
                    case "caseInstanceId":
                        taskInstanceIds = Find(m => m.CaseInstanceId == value).Select(m => m.Id).ToList();
                        break;
                    case "caseDefinitionId":
                        taskInstanceIds = Find(m => m.CaseDefinitionId == value).Select(m => m.Id).ToList();
                        break;
                }
                foreach (string taskInstanceId in taskInstanceIds)
                {
                    DeleteHistoricTaskInstanceById(taskInstanceId);
                }
            }
        }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public long findHistoricTaskInstanceCountByQueryCriteria(final org.camunda.bpm.engine.impl.HistoricTaskInstanceQueryImpl historicTaskInstanceQuery)
        //public virtual long FindHistoricTaskInstanceCountByQueryCriteria(HistoricTaskInstanceQueryImpl historicTaskInstanceQuery)
        //{
        //    if (HistoryEnabled)
        //    {
        //        ConfigureQuery(historicTaskInstanceQuery);
        //        throw new System.NotImplementedException();
        //        //return (long) DbEntityManager.SelectOne("selectHistoricTaskInstanceCountByQueryCriteria",historicTaskInstanceQuery);
        //    }

        //    return 0;
        //}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.Util.List<org.camunda.bpm.engine.history.HistoricTaskInstance> findHistoricTaskInstancesByQueryCriteria(final org.camunda.bpm.engine.impl.HistoricTaskInstanceQueryImpl historicTaskInstanceQuery, final org.camunda.bpm.engine.impl.Page page)
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
        //public virtual IList<IHistoricTaskInstance> FindHistoricTaskInstancesByQueryCriteria(HistoricTaskInstanceQueryImpl historicTaskInstanceQuery, Page page)
        //{
        //    if (HistoryEnabled)
        //    {
        //        ConfigureQuery(historicTaskInstanceQuery);
        //        return DbEntityManager.SelectList("selectHistoricTaskInstancesByQueryCriteria", historicTaskInstanceQuery, page);
        //    }

        //    return Collections.EMPTY_LIST;
        //}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public HistoricTaskInstanceEntity findHistoricTaskInstanceById(final String taskId)
        public virtual HistoricTaskInstanceEventEntity FindHistoricTaskInstanceById(string taskId)
        {
            EnsureUtil.EnsureNotNull("Invalid historic task id", "taskId", taskId);

            if (HistoryEnabled)
            {
                //return (HistoricTaskInstanceEntity) DbEntityManager.SelectOne("selectHistoricTaskInstance", taskId);
                return Single(m => m.Id == taskId);
            }

            return null;
        }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void deleteHistoricTaskInstanceById(final String taskId)
        public virtual void DeleteHistoricTaskInstanceById(string taskId)
        {
            if (HistoryEnabled)
            {
                HistoricTaskInstanceEventEntity historicTaskInstance = FindHistoricTaskInstanceById(taskId);
                if (historicTaskInstance != null)
                {
                    CommandContext commandContext = context.Impl.Context.CommandContext;

                    commandContext.HistoricDetailManager.DeleteHistoricDetailsByTaskId(taskId);

                    commandContext.HistoricVariableInstanceManager.DeleteHistoricVariableInstancesByTaskId(taskId);

                    commandContext.CommentManager.DeleteCommentsByTaskId(taskId);

                    commandContext.AttachmentManager.DeleteAttachmentsByTaskId(taskId);

                    commandContext.HistoricIdentityLinkManager.DeleteHistoricIdentityLinksLogByTaskId(taskId);

                    Delete(historicTaskInstance);
                }
            }
        }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.Util.List<org.camunda.bpm.engine.history.HistoricTaskInstance> findHistoricTaskInstancesByNativeQuery(final java.Util.Map<String, Object> parameterMap, final int firstResult, final int maxResults)
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
        public virtual IList<IHistoricTaskInstance> FindHistoricTaskInstancesByNativeQuery(IDictionary<string, object> parameterMap, int firstResult, int maxResults)
        {
            //return ListExt.ConvertToListT<IHistoricTaskInstance>(DbEntityManager.SelectListWithRawParameter("selectHistoricTaskInstanceByNativeQuery", parameterMap, firstResult, maxResults)) ;
            throw new System.NotImplementedException();
        }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public long findHistoricTaskInstanceCountByNativeQuery(final java.Util.Map<String, Object> parameterMap)
        public virtual long FindHistoricTaskInstanceCountByNativeQuery(IDictionary<string, object> parameterMap)
        {
            //return (long) DbEntityManager.SelectOne("selectHistoricTaskInstanceCountByNativeQuery", parameterMap);
            throw new System.NotImplementedException();
        }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void updateHistoricTaskInstance(final TaskEntity taskEntity)
        public virtual void UpdateHistoricTaskInstance(TaskEntity taskEntity)
        {
            ProcessEngineConfigurationImpl configuration = context.Impl.Context.ProcessEngineConfiguration;

            IHistoryLevel historyLevel = configuration.HistoryLevel;
            if (historyLevel.IsHistoryEventProduced(HistoryEventTypes.TaskInstanceUpdate, taskEntity))
            {

                HistoryEventProcessor.ProcessHistoryEvents(new HistoryEventCreatorAnonymousInnerClassHelper(this, taskEntity));
            }
        }

        private class HistoryEventCreatorAnonymousInnerClassHelper : HistoryEventProcessor.HistoryEventCreator
        {
            private readonly HistoricTaskInstanceManager _outerInstance;

            private TaskEntity _taskEntity;

            public HistoryEventCreatorAnonymousInnerClassHelper(HistoricTaskInstanceManager outerInstance, TaskEntity taskEntity)
            {
                this._outerInstance = outerInstance;
                this._taskEntity = taskEntity;
            }

            public override HistoryEvent CreateHistoryEvent(IHistoryEventProducer producer)
            {
                return producer.CreateTaskInstanceUpdateEvt(_taskEntity);
            }
        }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void markTaskInstanceEnded(String taskId, final String deleteReason)
        public virtual void MarkTaskInstanceEnded(string taskId, string deleteReason)
        {
            ProcessEngineConfigurationImpl configuration = context.Impl.Context.ProcessEngineConfiguration;

            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final TaskEntity taskEntity = org.camunda.bpm.engine.impl.context.Context.getCommandContext().getDbEntityManager().selectById(TaskEntity.class, taskId);
            //TaskEntity taskEntity = context.Impl.Context.CommandContext.DbEntityManager.SelectById< TaskEntity>(typeof(TaskEntity), taskId);
            TaskEntity taskEntity = taskManager.FindTaskById(taskId);
            IHistoryLevel historyLevel = configuration.HistoryLevel;
            if (historyLevel.IsHistoryEventProduced(HistoryEventTypes.TaskInstanceComplete, taskEntity))
            {

                HistoryEventProcessor.ProcessHistoryEvents(new HistoryEventCreatorAnonymousInnerClassHelper2(this, deleteReason, taskEntity));
            }
        }

        private class HistoryEventCreatorAnonymousInnerClassHelper2 : HistoryEventProcessor.HistoryEventCreator
        {
            private readonly HistoricTaskInstanceManager _outerInstance;

            private string _deleteReason;
            private TaskEntity _taskEntity;

            public HistoryEventCreatorAnonymousInnerClassHelper2(HistoricTaskInstanceManager outerInstance, string deleteReason, TaskEntity taskEntity)
            {
                this._outerInstance = outerInstance;
                this._deleteReason = deleteReason;
                this._taskEntity = taskEntity;
            }

            public override HistoryEvent CreateHistoryEvent(IHistoryEventProducer producer)
            {
                return producer.CreateTaskInstanceCompleteEvt(_taskEntity, _deleteReason);
            }
        }


//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void createHistoricTask(final TaskEntity task)
        public virtual void CreateHistoricTask(TaskEntity task)
        {
            ProcessEngineConfigurationImpl configuration = context.Impl.Context.ProcessEngineConfiguration;

            IHistoryLevel historyLevel = configuration.HistoryLevel;
            if (historyLevel.IsHistoryEventProduced(HistoryEventTypes.TaskInstanceCreate, task))
            {

                HistoryEventProcessor.ProcessHistoryEvents(new HistoryEventCreatorAnonymousInnerClassHelper3(this, task));

            }
        }

        private class HistoryEventCreatorAnonymousInnerClassHelper3 : HistoryEventProcessor.HistoryEventCreator
        {
            private readonly HistoricTaskInstanceManager _outerInstance;

            private TaskEntity _task;

            public HistoryEventCreatorAnonymousInnerClassHelper3(HistoricTaskInstanceManager outerInstance, TaskEntity task)
            {
                this._outerInstance = outerInstance;
                this._task = task;
            }

            public override HistoryEvent CreateHistoryEvent(IHistoryEventProducer producer)
            {
                return producer.CreateTaskInstanceCreateEvt(_task);
            }
        }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected void configureQuery(final org.camunda.bpm.engine.impl.HistoricTaskInstanceQueryImpl query)
        //protected internal virtual void ConfigureQuery(HistoricTaskInstanceQueryImpl query)
        //{
        //    AuthorizationManager.ConfigureHistoricTaskInstanceQuery(query);
        //    TenantManager.ConfigureQuery(query);
        //}

    }
}