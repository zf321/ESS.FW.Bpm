using System.Collections.Generic;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.History.Impl.Event;
using ESS.FW.DataAccess;

namespace org.camunda.bpm.engine.impl.history.@event
{
    public interface IHistoricDecisionInstanceManager :IRepository<HistoricDecisionInstanceEntity, string>
    {
        void DeleteHistoricDecisionInstancesByDecisionDefinitionId(string decisionDefinitionId);
        void DeleteHistoricHistoricInstanceByInstanceId(string historicDecisionInstanceId);
        HistoricDecisionInstanceEntity FindHistoricDecisionInstance(string historicDecisionInstanceId);
        long FindHistoricDecisionInstanceCountByNativeQuery(IDictionary<string, object> parameterMap);
        IList<IHistoricDecisionInstance> FindHistoricDecisionInstancesByNativeQuery(IDictionary<string, object> parameterMap, int firstResult, int maxResults);
        void InsertHistoricDecisionInstances(HistoricDecisionEvaluationEvent @event);
    }
}