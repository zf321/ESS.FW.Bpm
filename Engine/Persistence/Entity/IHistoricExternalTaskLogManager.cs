using ESS.FW.Bpm.Engine.Externaltask;
using ESS.FW.Bpm.Engine.History.Impl.Event;
using ESS.FW.DataAccess;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{
    public interface IHistoricExternalTaskLogManager:IRepository<HistoricExternalTaskLogEntity, string>
    {
        void DeleteHistoricExternalTaskLogsByProcessInstanceId(string processInstanceId);
        HistoricExternalTaskLogEntity FindHistoricExternalTaskLogById(string historicExternalTaskLogId);
        void FireExternalTaskCreatedEvent(IExternalTask externalTask);
        void FireExternalTaskDeletedEvent(IExternalTask externalTask);
        void FireExternalTaskFailedEvent(IExternalTask externalTask);
        void FireExternalTaskSuccessfulEvent(IExternalTask externalTask);
    }
}