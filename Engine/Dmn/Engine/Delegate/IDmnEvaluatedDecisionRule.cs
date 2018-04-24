using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.Dmn.engine.@delegate
{
    /// <summary>
    ///     A evaluated decision rule.
    /// </summary>
    public interface IDmnEvaluatedDecisionRule
    {
        /// <returns> the id of the decision rule or null if not set </returns>
        string Id { get; }

        /// <returns> the evaluated output entries for the decision rule </returns>
        IDictionary<string, IDmnEvaluatedOutput> OutputEntries { get; }
    }
}