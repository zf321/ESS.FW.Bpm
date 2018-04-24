using System.Collections.Generic;
using ESS.FW.DataAccess;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{
    public interface IExternalTaskManager : IRepository<ExternalTaskEntity,string>
    {
        void Delete(ExternalTaskEntity externalTask);
        ExternalTaskEntity FindExternalTaskById(string id);
        IList<ExternalTaskEntity> FindExternalTasksByExecutionId(string id);
        IList<ExternalTaskEntity> FindExternalTasksByProcessInstanceId(string processInstanceId);
        IList<ExternalTaskEntity> SelectExternalTasksForTopics(ICollection<string> topics, int maxResults, bool usePriority);
        void UpdateExternalTaskSuspensionStateByProcessDefinitionId(string processDefinitionId, ISuspensionState suspensionState);
        void UpdateExternalTaskSuspensionStateByProcessDefinitionKey(string processDefinitionKey, ISuspensionState suspensionState);
        void UpdateExternalTaskSuspensionStateByProcessDefinitionKeyAndTenantId(string processDefinitionKey, string processDefinitionTenantId, ISuspensionState suspensionState);
        void UpdateExternalTaskSuspensionStateByProcessInstanceId(string processInstanceId, ISuspensionState suspensionState);
    }
}