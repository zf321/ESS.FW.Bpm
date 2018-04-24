using System;
using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.History.Impl.Event
{
    /// <summary>
    ///     Container for history entities which belongs to a decision evaluation. Only
    ///     the containing history entities should be persisted.
    /// </summary>
    [Serializable]
    public class HistoricDecisionEvaluationEvent : HistoryEvent
    {
        private const long SerialVersionUid = 1L;

        public virtual HistoricDecisionInstanceEntity RootHistoricDecisionInstance { get; set; }


        public virtual ICollection<HistoricDecisionInstanceEntity> RequiredHistoricDecisionInstances { get; set; } = new List<HistoricDecisionInstanceEntity>();
    }
}