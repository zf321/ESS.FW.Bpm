using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.Dmn.engine
{
    /// <summary>
    ///     A decision of the DMN Engine.
    ///     <para>
    ///         Decisions can be implement in different ways. To check if the decision is implemented
    ///         as a Decision Table see <seealso cref="#isDecisionTable()" />.
    ///     </para>
    /// </summary>
    public interface IDmnDecision
    {
        /// <summary>
        ///     The unique identifier of the decision if exists.
        /// </summary>
        /// <returns> the identifier or null if not set </returns>
        string Key { get; }

        /// <summary>
        ///     The human readable name of the decision if exists.
        /// </summary>
        /// <returns> the name or null if not set </returns>
        string Name { get; }

        /// <summary>
        ///     是否决策表
        ///     Checks if the decision logic is implemented as Decision Table.
        /// </summary>
        /// <returns> true if the decision logic is implement as Decision Table, otherwise false </returns>
        bool DecisionTable { get; }

        /// <summary>
        ///     决策逻辑
        ///     Returns the decision logic of the decision (e.g., a decision table).
        /// </summary>
        /// <returns> the containing decision logic </returns>
        IDmnDecisionLogic DecisionLogic { get; }

        /// <summary>
        ///     需要的决策
        ///     Returns the required decisions of this decision.
        /// </summary>
        /// <returns> the required decisions or an empty collection if not exists. </returns>
        ICollection<IDmnDecision> RequiredDecisions { get; }
    }
}