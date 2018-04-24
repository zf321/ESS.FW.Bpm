using ESS.FW.Bpm.Engine.History.Impl.Event;
using ESS.FW.DataAccess;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{
    public interface IHistoricIncidentManager:IRepository<HistoricIncidentEntity,string>
    {
        void DeleteHistoricIncidentsByJobDefinitionId(string jobDefinitionId);
        void DeleteHistoricIncidentsByProcessDefinitionId(string processDefinitionId);
        void DeleteHistoricIncidentsByProcessInstanceId(string processInstanceId);
    }
}