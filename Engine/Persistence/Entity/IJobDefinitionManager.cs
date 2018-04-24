using ESS.FW.DataAccess;
using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{
    public interface IJobDefinitionManager:IRepository<JobDefinitionEntity,string>
    {
        void DeleteJobDefinitionsByProcessDefinitionId(string id);
        JobDefinitionEntity FindById(string jobDefinitionId);
        IList<JobDefinitionEntity> FindByProcessDefinitionId(string processDefinitionId);
        void UpdateJobDefinitionSuspensionStateById(string jobDefinitionId, ISuspensionState suspensionState);
        void UpdateJobDefinitionSuspensionStateByProcessDefinitionId(string processDefinitionId, ISuspensionState suspensionState);
        void UpdateJobDefinitionSuspensionStateByProcessDefinitionKey(string processDefinitionKey, ISuspensionState suspensionState);
        void UpdateJobDefinitionSuspensionStateByProcessDefinitionKeyAndTenantId(string processDefinitionKey, string processDefinitionTenantId, ISuspensionState suspensionState);
    }
}