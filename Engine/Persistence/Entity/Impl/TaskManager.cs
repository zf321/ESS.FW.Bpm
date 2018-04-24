using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Impl.Cfg.Auth;
using ESS.FW.Bpm.Engine.Task;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Impl.DB;
using System.Linq;
using Autofac.Features.AttributeFilters;

using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.DB.EntityManager;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.DataAccess;
using ESS.FW.Common.Components;
using ESS.FW.Common.Extensions;
using Microsoft.Extensions.Logging;
using Context = ESS.FW.Bpm.Engine.context.Impl.Context;
using ESS.FW.Common.Utilities;

namespace ESS.FW.Bpm.Engine.Persistence.Entity.Impl
{

    /// <summary>
    ///  
    /// </summary>
    [Component]
    public class TaskManager : AbstractManagerNet<TaskEntity>, ITaskManager
    {
        public TaskManager(DbContext dbContex, ILoggerFactory loggerFactory, IDGenerator idGenerator) : base(dbContex, loggerFactory, idGenerator)
        {
        }

        public IList<TaskEntity> FindTasksByCandidateUser(string userId)
        {
            var groupIds = from g in DbContext.Set<GroupEntity>()
                           join m in DbContext.Set<MembershipEntity>() on g.Id equals m.GroupId
                           join u in DbContext.Set<UserEntity>() on m.UserId equals u.Id
                           where u.Id == userId
                           select g.Id;

            return (from u in DbContext.Set<TaskEntity>()
                    join i in DbContext.Set<IdentityLinkEntity>() on u.Id equals i.TaskId
                    where (u.AssigneeWithoutCascade == null && i.Type == "candidate" && (i.UserId == userId || groupIds.Contains(i.GroupId)))
                    select u).Distinct().ToList()
                ;
        }
        public IList<TaskEntity> FindTasksByCandidateGroup(params string[] groupIds)
        {
            return (from u in DbContext.Set<TaskEntity>()
                    join i in DbContext.Set<IdentityLinkEntity>() on u.Id equals i.TaskId
                    where (u.AssigneeWithoutCascade == null && i.Type == "candidate" && groupIds.Contains(i.GroupId))
                    select u).ToList()
                ;
        }

        public virtual void InsertTask(TaskEntity task)
        {
            if (task.Revision == 0)
            {
                task.Revision = task.RevisionNext;
            }
            Add(task);
            CreateDefaultAuthorizations(task);
        }

        public virtual void UpdateTask(TaskEntity task)
        {
            Update(task);
            CreateDefaultAuthorizations(task);
        }

        public virtual void DeleteTasksByProcessInstanceId(string processInstanceId, string deleteReason, bool cascade, bool skipCustomListeners)
        {
            //IList<TaskEntity> tasks = (IList)DbEntityManager.createTaskQuery().processInstanceId(processInstanceId).list();
            IList<TaskEntity> tasks = Find(m => m.ProcessInstanceId == processInstanceId).ToList();
            string reason = (deleteReason == null || deleteReason.Length == 0) ? TaskEntity.DeleteReasonDeleted : deleteReason;

            foreach (TaskEntity task in tasks)
            {
                task.Delete(reason, cascade, skipCustomListeners);
            }
        }

        public virtual void DeleteTasksByCaseInstanceId(string caseInstanceId, string deleteReason, bool cascade)
        {

            //IList<TaskEntity> tasks = (IList)DbEntityManager.createTaskQuery().caseInstanceId(caseInstanceId).list();
            IList<TaskEntity> tasks = Find(m => m.CaseInstanceId == caseInstanceId).ToList();
            string reason = (deleteReason == null || deleteReason.Length == 0) ? TaskEntity.DeleteReasonDeleted : deleteReason;

            foreach (TaskEntity task in tasks)
            {
                task.Delete(reason, cascade, false);
            }
        }

        public virtual void DeleteTask(TaskEntity task, string deleteReason, bool cascade, bool skipCustomListeners)
        {
            if (!task.Deleted)
            {
                task.Deleted = true;

                CommandContext commandContext = Context.CommandContext;
                string taskId = task.Id;

                IList<TaskEntity> subTasks = FindTasksByParentTaskId(taskId);
                foreach (TaskEntity subTask in subTasks)
                {
                    subTask.Delete(deleteReason, cascade, skipCustomListeners);
                }

                task.DeleteIdentityLinks();

                commandContext.VariableInstanceManager.DeleteVariableInstanceByTask(task);

                if (cascade)
                {
                    commandContext.HistoricTaskInstanceManager.DeleteHistoricTaskInstanceById(taskId);
                }
                else
                {
                    commandContext.HistoricTaskInstanceManager.MarkTaskInstanceEnded(taskId, deleteReason);
                    if (TaskEntity.DeleteReasonCompleted.Equals(deleteReason))
                    {
                        task.CreateHistoricTaskDetails(UserOperationLogEntryFields.OperationTypeComplete);
                    }
                    else
                    {
                        task.CreateHistoricTaskDetails(UserOperationLogEntryFields.OperationTypeDelete);
                    }
                }

                DeleteAuthorizations(Resources.Task, taskId);
                Delete(task);
            }
        }


        public virtual TaskEntity FindTaskById(string id)
        {
            EnsureUtil.EnsureNotNull("Invalid task id", "id", id);
            return Get(id);
        }

        public virtual IList<TaskEntity> FindTasksByExecutionId(string executionId)
        {

            return Find(m => m.ExecutionId == executionId).ToList();
        }

        public virtual TaskEntity FindTaskByCaseExecutionId(string caseExecutionId)
        {

            return Single(m => m.CaseExecutionId == caseExecutionId);
        }

        public virtual IList<TaskEntity> FindTasksByProcessInstanceId(string processInstanceId)
        {

            return Find(m => m.ProcessInstanceId == processInstanceId).ToList();
        }


        public virtual IList<TaskEntity> FindTasksByParentTaskId(string parentTaskId)
        {
            return Find(c => c.ParentTaskIdWithoutCascade == parentTaskId).ToList();
        }

        public virtual void UpdateTaskSuspensionStateByProcessDefinitionId(string processDefinitionId, ISuspensionState suspensionState)
        {
            //IDictionary<string, object> parameters = new Dictionary<string, object>();
            //parameters["processDefinitionId"] = processDefinitionId;
            //parameters["suspensionState"] = suspensionState.StateCode;
            //DbEntityManager.Update(typeof(TaskEntity), "updateTaskSuspensionStateByParameters", ConfigureParameterizedQuery(parameters));
            Find(m => m.ProcessDefinitionId == processDefinitionId).ForEach(d =>
            {
                d.Revision = d.RevisionNext;
                d.SuspensionState = suspensionState.StateCode;
            });
        }

        public virtual void UpdateTaskSuspensionStateByProcessInstanceId(string processInstanceId, ISuspensionState suspensionState)
        {
            //throw new NotImplementedException();
            //IDictionary<string, object> parameters = new Dictionary<string, object>();
            //parameters["processInstanceId"] = processInstanceId;
            //parameters["suspensionState"] = suspensionState.StateCode;
            //DbEntityManager.Update(typeof(TaskEntity), "updateTaskSuspensionStateByParameters", ConfigureParameterizedQuery(parameters));
            Find(m => m.ProcessInstanceId == processInstanceId).ForEach(d =>
            {
                d.Revision = d.RevisionNext;
                d.SuspensionState = suspensionState.StateCode;
            });
        }

        public virtual void UpdateTaskSuspensionStateByProcessDefinitionKey(string processDefinitionKey, ISuspensionState suspensionState)
        {
            //IDictionary<string, object> parameters = new Dictionary<string, object>();
            //parameters["processDefinitionKey"] = processDefinitionKey;
            //parameters["isProcessDefinitionTenantIdSet"] = false;
            //parameters["suspensionState"] = suspensionState.StateCode;

            var predicateProc = ParameterBuilder.True<ProcessDefinitionEntity>();
            var predicateTask = ParameterBuilder.True<TaskEntity>();
            if (!string.IsNullOrEmpty(processDefinitionKey))
            {
                predicateProc = predicateProc.AndParam(c => c.Key == processDefinitionKey);
                predicateTask = predicateTask.AndParam(c => DbContext.Set<ProcessDefinitionEntity>().Where(predicateProc).Select(p => p.Id).ToList().Contains(c.ProcessDefinitionId));
            }

            Find(predicateTask).ForEach(d =>
            {
                d.Revision = d.RevisionNext;
                d.SuspensionState = suspensionState.StateCode;
            });
        }

        public virtual void UpdateTaskSuspensionStateByProcessDefinitionKeyAndTenantId(string processDefinitionKey, string processDefinitionTenantId, ISuspensionState suspensionState)
        {
            //IDictionary<string, object> parameters = new Dictionary<string, object>();
            //parameters["processDefinitionKey"] = processDefinitionKey;
            //parameters["isProcessDefinitionTenantIdSet"] = true;
            //parameters["processDefinitionTenantId"] = processDefinitionTenantId;
            //parameters["suspensionState"] = suspensionState.StateCode;
            var predicateTask = ParameterBuilder.True<TaskEntity>();
            var predicateProc = ParameterBuilder.True<ProcessDefinitionEntity>();
            if (!string.IsNullOrEmpty(processDefinitionKey))
            {
                predicateProc = predicateProc.AndParam(c => c.Key == processDefinitionKey);
                predicateProc = !string.IsNullOrEmpty(processDefinitionTenantId) ? predicateProc.AndParam(c => c.TenantId == processDefinitionTenantId) : predicateProc.AndParam(c => c.TenantId == null);
                predicateTask = predicateTask.AndParam(c => DbContext.Set<ProcessDefinitionEntity>().Where(predicateProc).Select(p => p.Id).ToList().Contains(c.ProcessDefinitionId));
            }

            Find(predicateTask).ForEach(d =>
            {
                d.Revision = d.RevisionNext;
                d.SuspensionState = suspensionState.StateCode;
            });
        }

        public virtual void UpdateTaskSuspensionStateByCaseExecutionId(string caseExecutionId, ISuspensionState suspensionState)
        {
            //IDictionary<string, object> parameters = new Dictionary<string, object>();
            //parameters["caseExecutionId"] = caseExecutionId;
            //parameters["suspensionState"] = suspensionState.StateCode;
            //throw new NotImplementedException();
            //DbEntityManager.Update(typeof(TaskEntity), "updateTaskSuspensionStateByParameters", ConfigureParameterizedQuery(parameters));
            var list = Find(m => m.CaseExecutionId == caseExecutionId);
            foreach (var item in list)
            {
                item.Revision = item.Revision + 1;
                item.SuspensionState = suspensionState.StateCode;
            }
        }

        // helper ///////////////////////////////////////////////////////////

        protected internal virtual void CreateDefaultAuthorizations(TaskEntity task)
        {
            if (AuthorizationEnabled)
            {
                IResourceAuthorizationProvider provider = ResourceAuthorizationProvider;
                AuthorizationEntity[] authorizations = provider.NewTask(task);
                SaveDefaultAuthorizations(authorizations);
            }
        }

        //protected internal virtual void ConfigureQuery(TaskQueryImpl query)
        //{
        //    AuthorizationManager.ConfigureTaskQuery(query);
        //    TenantManager.ConfigureQuery(query);
        //}

        //protected internal virtual ListQueryParameterObject ConfigureParameterizedQuery(object parameter)
        //{
        //    throw new NotImplementedException();
        //    //return TenantManager.ConfigureQuery(parameter);
        //}

    }

}