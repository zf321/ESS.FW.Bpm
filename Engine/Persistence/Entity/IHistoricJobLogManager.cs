using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.DataAccess;
using ESS.FW.Bpm.Engine.History.Impl.Event;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{
    public interface IHistoricJobLogManager:IRepository<HistoricJobLogEventEntity,string>
    {
        void DeleteHistoricJobLogById(string id);
        void DeleteHistoricJobLogByJobId(string jobId);
        void DeleteHistoricJobLogsByDeploymentId(string deploymentId);
        void deleteHistoricJobLogsByHandlerType(string handlerType);
        void DeleteHistoricJobLogsByHandlerType(string handlerType);
        void DeleteHistoricJobLogsByJobDefinitionId(string jobDefinitionId);
        void DeleteHistoricJobLogsByProcessDefinitionId(string processDefinitionId);
        void DeleteHistoricJobLogsByProcessInstanceId(string processInstanceId);
        HistoricJobLogEventEntity FindHistoricJobLogById(string historicJobLogId);
        IList<IHistoricJobLog> FindHistoricJobLogsByDeploymentId(string deploymentId);
        void FireJobCreatedEvent(IJob job);
        void FireJobDeletedEvent(IJob job);
        void FireJobFailedEvent(IJob job, System.Exception exception);
        void FireJobSuccessfulEvent(IJob job);
    }
}