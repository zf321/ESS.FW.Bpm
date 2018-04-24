using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Dmn.engine;
using ESS.FW.Bpm.Engine.exception;

namespace ESS.FW.Bpm.Engine.Dmn
{
    /// <summary>
    ///     Fluent builder to evaluate a decision.
    /// </summary>
    public interface IDecisionsEvaluationBuilder
    {
        /// <summary>
        ///     Specify the id of the tenant the decision definition belongs to. Can only be
        ///     used when the definition is referenced by <code>key</code> and not by <code>id</code>.
        /// </summary>
        IDecisionsEvaluationBuilder DecisionDefinitionTenantId(string tenantId);

        /// <summary>
        ///     Specify that the decision definition belongs to no tenant. Can only be
        ///     used when the definition is referenced by <code>key</code> and not by <code>id</code>.
        /// </summary>
        IDecisionsEvaluationBuilder DecisionDefinitionWithoutTenantId();

        /// <summary>
        ///     Set the version of the decision definition. If <code>null</code> then
        ///     the latest version is taken.
        /// </summary>
        IDecisionsEvaluationBuilder Version(int? version);

        /// <summary>
        ///     Set the input values of the decision.
        /// </summary>
        IDecisionsEvaluationBuilder Variables(IDictionary<string, object> variables);

        /// <summary>
        ///     Evaluates the decision.
        /// </summary>
        /// <returns>
        ///     the result of the evaluation.
        /// </returns>
        /// <exception cref="NotFoundException">
        ///     when no decision definition is deployed with the given id / key.
        /// </exception>
        /// <exception cref="NotValidException">
        ///     when the given decision definition id / key is null.
        /// </exception>
        /// <exception cref="AuthorizationException">
        ///     if the user has no <seealso cref="Permissions#CREATE_INSTANCE" /> permission
        ///     on <seealso cref="Resources#DECISION_DEFINITION" />.
        /// </exception>
        IDmnDecisionResult Evaluate();
    }
}