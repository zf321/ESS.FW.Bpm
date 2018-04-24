using System.Collections.Generic;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.History.Impl.Event;
using System.Linq;
using ESS.FW.DataAccess;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{
    public interface IHistoricProcessInstanceManager: IRepository<HistoricProcessInstanceEventEntity, string>
    {
        void DeleteHistoricProcessInstanceById(string historicProcessInstanceId);
        void DeleteHistoricProcessInstanceByProcessDefinitionId(string processDefinitionId);
        HistoricProcessInstanceEventEntity FindHistoricProcessInstance(string processInstanceId);
        long FindHistoricProcessInstanceCountByNativeQuery(IDictionary<string, object> parameterMap);
        HistoricProcessInstanceEventEntity FindHistoricProcessInstanceEvent(string eventId);
        List<string> FindHistoricProcessInstanceIdsForCleanup(int batchSize);
        long FindHistoricProcessInstanceIdsForCleanupCount();
        IList<IHistoricProcessInstance> FindHistoricProcessInstancesByNativeQuery(IDictionary<string, object> parameterMap, int firstResult, int maxResults);
        IQueryable<HistoricProcessInstanceEventEntity> VariableValueEquals(string key, string value);
        IQueryable<HistoricProcessInstanceEventEntity> VariableValueEquals(IDictionary<string,string> dic);
        IQueryable<HistoricProcessInstanceEventEntity> ProcessInstanceIds(IEnumerable<string> ids);
        IQueryable<HistoricProcessInstanceEventEntity> VariableValueNotEquals(string key, string value);
        IQueryable<HistoricProcessInstanceEventEntity> VariableValueGreaterThan(string key,string value);
        IQueryable<HistoricProcessInstanceEventEntity> VariableValueLessThan(string key, string value);
        IQueryable<HistoricProcessInstanceEventEntity> VariableValueGreaterThanOrEqual(string key, string value);
        IQueryable<HistoricProcessInstanceEventEntity> VariableValueLessThanOrEqual(string key, string value);
        IQueryable<HistoricProcessInstanceEventEntity> VariableValueLike(string v1, string v2);
    }
}