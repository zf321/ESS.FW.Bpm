using System.Collections.Generic;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.History.Impl.Event;
using ESS.FW.DataAccess;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{
    public interface IHistoricCaseInstanceManager :IRepository<HistoricCaseInstanceEventEntity,string>
    {
        void DeleteHistoricCaseInstanceByCaseDefinitionId(string caseDefinitionId);
        void DeleteHistoricCaseInstanceById(string historicCaseInstanceId);
        HistoricCaseInstanceEventEntity FindHistoricCaseInstance(string caseInstanceId);
        long FindHistoricCaseInstanceCountByNativeQuery(IDictionary<string, object> parameterMap);
        HistoricCaseInstanceEventEntity FindHistoricCaseInstanceEvent(string eventId);
        IList<IHistoricCaseInstance> FindHistoricCaseInstancesByNativeQuery(IDictionary<string, object> parameterMap, int firstResult, int maxResults);
    }
}