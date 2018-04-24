using System.Collections.Generic;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.History.Impl.Event;
using ESS.FW.DataAccess;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{
    public interface IHistoricActivityInstanceManager : IRepository<HistoricActivityInstanceEventEntity, string>
    {
        void DeleteHistoricActivityInstancesByProcessInstanceId(string historicProcessInstanceId);
        HistoricActivityInstanceEventEntity FindHistoricActivityInstance(string activityId, string processInstanceId);
        long FindHistoricActivityInstanceCountByNativeQuery(IDictionary<string, object> parameterMap);
        IList<IHistoricActivityInstance> FindHistoricActivityInstancesByNativeQuery(IDictionary<string, object> parameterMap, int firstResult, int maxResults);
        void InsertHistoricActivityInstance(HistoricActivityInstanceEventEntity historicActivityInstance);
    }
}