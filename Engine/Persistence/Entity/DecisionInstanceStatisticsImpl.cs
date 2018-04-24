using System;
using ESS.FW.Bpm.Engine.History;
using ESS.Shared.Entities.Bpm;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{
    public class DecisionInstanceStatisticsImpl : IHistoricDecisionInstanceStatistics,IDbEntity
	{
	    public  int Evaluations { get; set; }


	    public  string DecisionDefinitionKey { get; set; }
        public string Id { get; set; }

        public object GetPersistentState()
        {
            throw new NotImplementedException();
        }
    }

}