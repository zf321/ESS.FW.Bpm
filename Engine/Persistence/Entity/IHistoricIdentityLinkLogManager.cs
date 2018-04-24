using ESS.FW.Bpm.Engine.History.Impl.Event;
using ESS.FW.DataAccess;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{
    public interface IHistoricIdentityLinkLogManager:IRepository<HistoricIdentityLinkLogEventEntity,string>
    {
        void DeleteHistoricIdentityLinksLogByProcessDefinitionId(string processDefId);
        void DeleteHistoricIdentityLinksLogByTaskId(string taskId);
    }
}