using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.Dmn.engine
{
    /// <summary>
    ///     Container of <seealso cref="IDmnDecision" />s which belongs to the same decision
    ///     requirements graph (i.e. DMN resource).
    /// </summary>
    public interface IDmnDecisionRequirementsGraph
    {
        /// <summary>
        ///     The unique identifier of the diagram if exists.
        /// </summary>
        /// <returns> the identifier or null if not set </returns>
        string Key { get; }

        /// <summary>
        ///     The human readable name of the diagram if exists.
        /// </summary>
        /// <returns> the name or null if not set </returns>
        string Name { get; }

        /// <summary>
        ///     Gets the containing decisions.
        /// </summary>
        /// <returns> the containing decisions </returns>
        ICollection<IDmnDecision> Decisions { get; }

        /// <summary>
        ///     Get the keys of the containing decisions.
        /// </summary>
        /// <returns> the decision keys. </returns>
        ISet<string> DecisionKeys { get; }

        /// <summary>
        ///     Gets the containing decision with the given key.
        /// </summary>
        /// <param name="key">
        ///     the identifier of the decision
        /// </param>
        /// <returns> the decision or null if not exists </returns>
        IDmnDecision getDecision(string key);
    }
}