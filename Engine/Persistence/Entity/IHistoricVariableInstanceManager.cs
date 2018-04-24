using System.Collections.Generic;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.DataAccess;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{
    public interface IHistoricVariableInstanceManager: IRepository<HistoricVariableInstanceEntity,string>
    {
        void DeleteHistoricVariableInstanceByCaseInstanceId(string historicCaseInstanceId);
        void DeleteHistoricVariableInstanceByProcessInstanceId(string historicProcessInstanceId);
        void DeleteHistoricVariableInstancesByTaskId(string taskId);
        HistoricVariableInstanceEntity FindHistoricVariableInstanceByVariableInstanceId(string variableInstanceId);
        IList<IHistoricVariableInstance> FindHistoricVariableInstancesByCaseInstanceId(string caseInstanceId);
        IList<IHistoricVariableInstance> FindHistoricVariableInstancesByProcessInstanceId(string processInstanceId);
    }
}