using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Variable.Type;
using ESS.FW.Bpm.Engine.Variable.Value;
using ESS.FW.Bpm.Model.Dmn;

namespace ESS.FW.Bpm.Engine.Dmn.engine.@delegate
{
    /// <summary>
    ///     Event which represents the evaluation of a decision table
    /// </summary>
    public interface IDmnDecisionTableEvaluationEvent : IDmnDecisionLogicEvaluationEvent
    {
        /// <returns> the evaluated decision table </returns>
        IDmnDecision DecisionTable { get; }

        /// <returns> the inputs on which the decision table was evaluated </returns>
        IList<IDmnEvaluatedInput> Inputs { get; }

        /// <returns> the matching rules of the decision table evaluation </returns>
        IList<IDmnEvaluatedDecisionRule> MatchingRules { get; }

        /// <returns>
        ///     the result name of the collect operation if the <seealso cref="HitPolicy#COLLECT" /> was used with an
        ///     aggregator otherwise null
        /// </returns>
        string CollectResultName { get; }

        /// <returns>
        ///     the result value of the collect operation if the <seealso cref="HitPolicy#COLLECT" /> was used with an
        ///     aggregator otherwise null
        /// </returns>
        ITypedValue CollectResultValue { get; }

        /// <returns> the number of executed decision elements during the evaluation </returns>
        long ExecutedDecisionElements { get; }
    }
}