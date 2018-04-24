using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Dmn;
using ESS.FW.Bpm.Engine.exception;

namespace ESS.FW.Bpm.Engine
{
    /// <summary>
    ///     Service to evaluate decisions inside the DMN engine.
    ///     
    /// </summary>
    public interface IDecisionService
    {
        /// <summary>
        ///     Evaluates the decision with the given id.
        /// </summary>
        /// <param name="decisionDefinitionId">
        ///     the id of the decision definition, cannot be null.
        /// </param>
        /// <param name="variables">
        ///     the input values of the decision.
        /// </param>
        /// <returns>
        ///     the result of the evaluation.
        /// </returns>
        /// <exception cref="NotFoundException">
        ///     when no decision definition is deployed with the given id.
        /// </exception>
        /// <exception cref="NotValidException">
        ///     when the given decision definition id is null.
        /// </exception>
        /// <exception cref="AuthorizationException">
        ///     if the user has no <seealso cref="Permissions#CREATE_INSTANCE" /> permission
        ///     on <seealso cref="Resources#DECISION_DEFINITION" />.
        /// </exception>
        IDmnDecisionTableResult EvaluateDecisionTableById(string decisionDefinitionId,
            IDictionary<string, object> variables);

        /// <summary>
        ///     Evaluates the decision with the given key in the latest version.
        /// </summary>
        /// <param name="decisionDefinitionKey">
        ///     the key of the decision definition, cannot be null.
        /// </param>
        /// <param name="variables">
        ///     the input values of the decision.
        /// </param>
        /// <returns>
        ///     the result of the evaluation.
        /// </returns>
        /// <exception cref="NotFoundException">
        ///     when no decision definition is deployed with the given key.
        /// </exception>
        /// <exception cref="NotValidException">
        ///     when the given decision definition key is null.
        /// </exception>
        /// <exception cref="AuthorizationException">
        ///     if the user has no <seealso cref="Permissions#CREATE_INSTANCE" /> permission
        ///     on <seealso cref="Resources#DECISION_DEFINITION" />.
        /// </exception>
        IDmnDecisionTableResult EvaluateDecisionTableByKey(string decisionDefinitionKey,
            IDictionary<string, object> variables);

        /// <summary>
        ///     Evaluates the decision with the given key in the specified version. If no
        ///     version is provided then the latest version of the decision definition is
        ///     taken.
        /// </summary>
        /// <param name="decisionDefinitionKey">
        ///     the key of the decision definition, cannot be null.
        /// </param>
        /// <param name="version">
        ///     the version of the decision definition. If <code>null</code> then
        ///     the latest version is taken.
        /// </param>
        /// <param name="variables">
        ///     the input values of the decision.
        /// </param>
        /// <returns>
        ///     the result of the evaluation.
        /// </returns>
        /// <exception cref="NotFoundException">
        ///     when no decision definition is deployed with the given key and
        ///     version.
        /// </exception>
        /// <exception cref="NotValidException">
        ///     when the given decision definition key is null.
        /// </exception>
        /// <exception cref="AuthorizationException">
        ///     if the user has no <seealso cref="Permissions#CREATE_INSTANCE" /> permission
        ///     on <seealso cref="Resources#DECISION_DEFINITION" />.
        /// </exception>
        IDmnDecisionTableResult EvaluateDecisionTableByKeyAndVersion(string decisionDefinitionKey, int? version,
            IDictionary<string, object> variables);

        /// <summary>
        ///     Returns a fluent builder to evaluate the decision table with the given key.
        ///     The builder can be used to set further properties and specify evaluation
        ///     instructions.
        /// </summary>
        /// <param name="decisionDefinitionKey">
        ///     the key of the decision definition, cannot be <code>null</code>.
        /// </param>
        /// <returns>
        ///     a builder to evaluate a decision table
        /// </returns>
        /// <seealso cref= # evaluateDecisionByKey( String
        /// )
        /// </seealso>
        IDecisionEvaluationBuilder EvaluateDecisionTableByKey(string decisionDefinitionKey);

        /// <summary>
        ///     Returns a fluent builder to evaluate the decision table with the given id.
        ///     The builder can be used to set further properties and specify evaluation
        ///     instructions.
        /// </summary>
        /// <param name="decisionDefinitionId">
        ///     the id of the decision definition, cannot be <code>null<code>.
        /// </param>
        /// <returns>
        ///     a builder to evaluate a decision table
        /// </returns>
        /// <seealso cref= # evaluateDecisionById( String
        /// )
        /// </seealso>
        IDecisionEvaluationBuilder EvaluateDecisionTableById(string decisionDefinitionId);

        /// <summary>
        ///     Returns a fluent builder to evaluate the decision with the given key.
        ///     The builder can be used to set further properties and specify evaluation
        ///     instructions.
        /// </summary>
        /// <param name="decisionDefinitionKey">
        ///     the key of the decision definition, cannot be <code>null</code>.
        /// </param>
        /// <returns> a builder to evaluate a decision </returns>
        IDecisionsEvaluationBuilder EvaluateDecisionByKey(string decisionDefinitionKey);

        /// <summary>
        ///     Returns a fluent builder to evaluate the decision with the given id.
        ///     The builder can be used to set further properties and specify evaluation
        ///     instructions.
        /// </summary>
        /// <param name="decisionDefinitionId">
        ///     the id of the decision definition, cannot be <code>null<code>.
        /// </param>
        /// <returns> a builder to evaluate a decision </returns>
        IDecisionsEvaluationBuilder EvaluateDecisionById(string decisionDefinitionId);
    }
}