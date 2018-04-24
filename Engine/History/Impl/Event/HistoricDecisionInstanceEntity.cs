using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Impl.DB;
using ESS.FW.Bpm.Engine.context.Impl;

namespace ESS.FW.Bpm.Engine.History.Impl.Event
{
    /// <summary>
    ///     History entry for an evaluated decision.
    /// </summary>
    [Serializable]
    public class HistoricDecisionInstanceEntity : HistoryEvent, IHistoricDecisionInstance
    {
        private const long SerialVersionUid = 1L;

        protected internal static readonly EnginePersistenceLogger Log = ProcessEngineLogger.PersistenceLogger;

        protected internal IList<IHistoricDecisionInputInstance> inputs;
        protected internal IList<IHistoricDecisionOutputInstance> outputs;

        public virtual string DecisionDefinitionId { get; set; }


        public virtual string DecisionDefinitionKey { get; set; }


        public virtual string DecisionDefinitionName { get; set; }


        public virtual string ActivityInstanceId { get; set; }


        public virtual string ActivityId { get; set; }


        public virtual DateTime EvaluationTime { get; set; }


        public virtual string UserId { get; set; }


        public virtual string TenantId { get; set; }


        public virtual IList<IHistoricDecisionInputInstance> Inputs
        {
            get
            {
                if (inputs != null)
                    return inputs;
                throw Log.HistoricDecisionInputInstancesNotFetchedException();
            }
            set { inputs = value; }
        }

        public virtual IList<IHistoricDecisionOutputInstance> Outputs
        {
            get
            {
                if (outputs != null)
                    return outputs;
                throw Log.HistoricDecisionOutputInstancesNotFetchedException();
            }
            set
            {
                outputs =
                value;
            }
        }

        public virtual double? CollectResultValue { get; set; }


        public virtual string RootDecisionInstanceId { get; set; }


        public virtual string DecisionRequirementsDefinitionId { get; set; }


        public virtual string DecisionRequirementsDefinitionKey { get; set; }


        public virtual void Delete()
        {
            Context.CommandContext.GetDbEntityManager<HistoricDecisionInstanceEntity>().Delete(this);
        }

        public virtual void AddInput(IHistoricDecisionInputInstance decisionInputInstance)
        {
            if (inputs == null)
                inputs = new List<IHistoricDecisionInputInstance>();
            inputs.Add(decisionInputInstance);
        }

        public virtual void AddOutput(IHistoricDecisionOutputInstance decisionOutputInstance)
        {
            if (outputs == null)
                outputs = new List<IHistoricDecisionOutputInstance>();
            outputs.Add(decisionOutputInstance);
        }
    }
}