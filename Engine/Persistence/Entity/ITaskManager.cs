using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Task;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{
    public interface ITaskManager
    {
        void DeleteTask(TaskEntity task, string deleteReason, bool cascade, bool skipCustomListeners);
        void DeleteTasksByCaseInstanceId(string caseInstanceId, string deleteReason, bool cascade);
        void DeleteTasksByProcessInstanceId(string processInstanceId, string deleteReason, bool cascade, bool skipCustomListeners);
        TaskEntity FindTaskByCaseExecutionId(string caseExecutionId);
        TaskEntity FindTaskById(string id);
        IList<TaskEntity> FindTasksByExecutionId(string executionId);
        IList<TaskEntity> FindTasksByParentTaskId(string parentTaskId);
        IList<TaskEntity> FindTasksByProcessInstanceId(string processInstanceId);
        IList<TaskEntity> FindTasksByCandidateUser(string userId);
        IList<TaskEntity> FindTasksByCandidateGroup(params string[] groupId);
        void InsertTask(TaskEntity task);
        void UpdateTask(TaskEntity task);
        void UpdateTaskSuspensionStateByCaseExecutionId(string caseExecutionId, ISuspensionState suspensionState);
        void UpdateTaskSuspensionStateByProcessDefinitionId(string processDefinitionId, ISuspensionState suspensionState);
        void UpdateTaskSuspensionStateByProcessDefinitionKey(string processDefinitionKey, ISuspensionState suspensionState);
        void UpdateTaskSuspensionStateByProcessDefinitionKeyAndTenantId(string processDefinitionKey, string processDefinitionTenantId, ISuspensionState suspensionState);
        void UpdateTaskSuspensionStateByProcessInstanceId(string processInstanceId, ISuspensionState suspensionState);
    }
}