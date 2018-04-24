using System.Collections.Generic;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.History.Impl.Event;
using ESS.FW.DataAccess;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{
    public interface IHistoricDetailManager : IRepository<HistoricDetailEventEntity, string>
    {
        void DeleteHistoricDetailsByCaseInstanceId(string historicCaseInstanceId);
        void DeleteHistoricDetailsByProcessCaseInstanceId(string historicProcessInstanceId, string historicCaseInstanceId);
        void DeleteHistoricDetailsByProcessInstanceId(string historicProcessInstanceId);
        void DeleteHistoricDetailsByTaskId(string taskId);
        IList<IHistoricDetail> FindHistoricDetailsByCaseInstanceId(string caseInstanceId);
        IList<IHistoricDetail> FindHistoricDetailsByProcessInstanceId(string processInstanceId);
        IList<IHistoricDetail> FindHistoricDetailsByTaskId(string taskId);
    }
}