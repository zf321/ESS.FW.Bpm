using System.Collections.Generic;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.History.Impl.Event;
using ESS.FW.DataAccess;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{
    public interface IHistoricCaseActivityInstanceManager:IRepository<HistoricCaseActivityInstanceEventEntity, string>
    {
        void DeleteHistoricCaseActivityInstancesByCaseInstanceId(string historicCaseInstanceId);
        HistoricCaseActivityInstanceEventEntity FindHistoricCaseActivityInstance(string caseActivityId, string caseInstanceId);
        long FindHistoricCaseActivityInstanceCountByNativeQuery(IDictionary<string, object> parameterMap);
        IList<IHistoricCaseActivityInstance> FindHistoricCaseActivityInstancesByNativeQuery(IDictionary<string, object> parameterMap, int firstResult, int maxResults);
        void InsertHistoricCaseActivityInstance(HistoricCaseActivityInstanceEventEntity historicCaseActivityInstance);
    }
}