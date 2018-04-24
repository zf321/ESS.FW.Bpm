using System;
using ESS.FW.Bpm.Engine.Variable.Type;
using ESS.FW.Bpm.Engine.Variable.Value;

namespace ESS.FW.Bpm.Engine.History
{
    /// <summary>
    ///     Represents one output variable of a decision evaluation.
    ///     
    /// </summary>
    public interface IHistoricDecisionOutputInstance
    {
        /// <summary>
        ///     The unique identifier of this historic decision output instance.
        /// </summary>
        string Id { get; }

        /// <summary>
        ///     The unique identifier of the historic decision instance.
        /// </summary>
        string DecisionInstanceId { get; }

        /// <summary>
        ///     The unique identifier of the clause that the value is assigned for.
        ///     Can be <code>null</code> if the decision is not implemented as decision table.
        /// </summary>
        string ClauseId { get; }

        /// <summary>
        ///     The name of the clause that the value is assigned for.
        ///     Can be <code>null</code> if the decision is not implemented as decision table.
        /// </summary>
        string ClauseName { get; }

        /// <summary>
        ///     The unique identifier of the rule that is matched.
        ///     Can be <code>null</code> if the decision is not implemented as decision table.
        /// </summary>
        string RuleId { get; }

        /// <summary>
        ///     The order of the rule that is matched.
        ///     Can be <code>null</code> if the decision is not implemented as decision table.
        /// </summary>
        int? RuleOrder { get; }

        /// <summary>
        ///     The name of the output variable.
        /// </summary>
        string VariableName { get; }

        /// <summary>
        ///     Returns the type name of the variable
        /// </summary>
        string TypeName { get; }

        /// <summary>
        ///     Returns the value of this variable instance.
        /// </summary>
        object Value { get; }

        /// <summary>
        ///     Returns the <seealso cref="TypedValue" /> for this value.
        /// </summary>
        ITypedValue TypedValue { get; }

        /// <summary>
        ///     If the variable value could not be loaded, this returns the error message.
        /// </summary>
        /// <returns> an error message indicating why the variable value could not be loaded. </returns>
        string ErrorMessage { get; }
    }
}