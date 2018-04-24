
using ESS.FW.Bpm.Engine.Dmn.engine.@delegate;
using ESS.FW.Bpm.Engine.Variable.Context;

namespace ESS.FW.Bpm.Engine.Dmn.engine.impl.evaluation
{
    /// <summary>
    ///     Evaluates decisions with a specific kind of decision logic and generates the
    ///     result.
    /// </summary>
    public interface IDmnDecisionLogicEvaluationHandler
    {
        /// <summary>
        ///     Evaluate a decision with the given <seealso cref="IVariableContext" />.
        /// </summary>
        /// <param name="decision">
        ///     the decision to evaluate
        /// </param>
        /// <param name="variableContext">
        ///     the available variable context
        /// </param>
        /// <returns> the event which represents the evaluation </returns>
        IDmnDecisionLogicEvaluationEvent Evaluate(IDmnDecision decision, IVariableContext variableContext);

        /// <summary>
        ///     Generates the decision evaluation result of the given event.
        /// </summary>
        /// <param name="event">
        ///     which represents the evaluation
        /// </param>
        /// <returns> the result of the decision evaluation </returns>
        IDmnDecisionResult GenerateDecisionResult(IDmnDecisionLogicEvaluationEvent @event);
    }
}