using ESS.FW.Bpm.Model.Dmn.instance;

namespace ESS.FW.Bpm.Engine.Dmn.engine.impl.spi.transform
{
    /// <summary>
    ///     Listener for a DMN transformation
    /// </summary>
    public interface IDmnTransformListener
    {
        /// <summary>
        ///     Notified after a DMN decision was transformed
        /// </summary>
        /// <param name="decision"> the decision from the DMN model instance </param>
        /// <param name="dmnDecision"> the transformed <seealso cref="IDmnDecision" /> </param>
        void transformDecision(IDecision decision, IDmnDecision dmnDecision);

        /// <summary>
        ///     Notified after a DMN decision table input was transformed
        /// </summary>
        /// <param name="input"> the input from the DMN model instance </param>
        /// <param name="dmnInput"> the transformed <seealso cref="DmnDecisionTableInputImpl" /> </param>
        void transformDecisionTableInput(IInput input, DmnDecisionTableInputImpl dmnInput);

        /// <summary>
        ///     Notified after a DMN decision table output was transformed
        /// </summary>
        /// <param name="output"> the output from the DMN model instance </param>
        /// <param name="dmnOutput"> the transformed <seealso cref="DmnDecisionTableOutputImpl" /> </param>
        void transformDecisionTableOutput(IOutput output, DmnDecisionTableOutputImpl dmnOutput);

        /// <summary>
        ///     Notified after a DMN decision table rule was transformed
        /// </summary>
        /// <param name="rule"> the rule from the DMN model instance </param>
        /// <param name="dmnRule"> the transformed <seealso cref="DmnDecisionTableRuleImpl" /> </param>
        void transformDecisionTableRule(IRule rule, DmnDecisionTableRuleImpl dmnRule);

        /// <summary>
        ///     Notified after a Decision Requirements Graph was transformed
        /// </summary>
        /// <param name="definitions"> </param>
        /// <param name="dmnDecisionGraph"> </param>
        void transformDecisionRequirementsGraph(IDefinitions definitions,
            IDmnDecisionRequirementsGraph dmnDecisionRequirementsGraph);
    }
}