using System;
using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.History
{
    /// <summary>
    ///     Represents one evaluation of a decision.
    ///     
    ///     
    /// </summary>
    public interface IHistoricDecisionInstance
    {
        /// <summary>
        ///     The unique identifier of this historic decision instance.
        /// </summary>
        string Id { get; }

        /// <summary>
        ///     The decision definition reference.
        /// </summary>
        string DecisionDefinitionId { get; }

        /// <summary>
        ///     The unique identifier of the decision definition
        /// </summary>
        string DecisionDefinitionKey { get; }

        /// <summary>
        ///     The name of the decision definition
        /// </summary>
        string DecisionDefinitionName { get; }

        /// <summary>
        ///     Time when the decision was evaluated.
        /// </summary>
        DateTime EvaluationTime { get; }

        /// <summary>
        ///     The corresponding key of the process definition in case the decision was evaluated inside a process.
        /// </summary>
        string ProcessDefinitionKey { get; }

        /// <summary>
        ///     The corresponding id of the process definition in case the decision was evaluated inside a process.
        /// </summary>
        string ProcessDefinitionId { get; }

        /// <summary>
        ///     The corresponding process instance in case the decision was evaluated inside a process.
        /// </summary>
        string ProcessInstanceId { get; }

        /// <summary>
        ///     The corresponding key of the case definition in case the decision was evaluated inside a case.
        /// </summary>
        string CaseDefinitionKey { get; }

        /// <summary>
        ///     The corresponding id of the case definition in case the decision was evaluated inside a case.
        /// </summary>
        string CaseDefinitionId { get; }

        /// <summary>
        ///     The corresponding case instance in case the decision was evaluated inside a case.
        /// </summary>
        string CaseInstanceId { get; }

        /// <summary>
        ///     The corresponding activity in case the decision was evaluated inside a process or a case.
        /// </summary>
        string ActivityId { get; }

        /// <summary>
        ///     The corresponding activity instance in case the decision was evaluated inside a process or a case.
        /// </summary>
        string ActivityInstanceId { get; }

        /// <summary>
        ///     The user ID in case the decision was evaluated by an authenticated user using the decision service
        ///     outside of an execution context.
        /// </summary>
        string UserId { get; }

        /// <summary>
        ///     The input values of the evaluated decision. The fetching of the input values must be enabled on the query.
        /// </summary>
        /// <exception cref="ProcessEngineException">
        ///     if the input values are not fetched.
        /// </exception>
        /// <seealso cref= HistoricDecisionInstanceQuery# includeInputs
        /// (
        /// )
        /// </seealso>
        IList<IHistoricDecisionInputInstance> Inputs { get; }

        /// <summary>
        ///     The output values of the evaluated decision. The fetching of the output values must be enabled on the query.
        /// </summary>
        /// <exception cref="ProcessEngineException">
        ///     if the output values are not fetched.
        /// </exception>
        /// <seealso cref= HistoricDecisionInstanceQuery# includeOutputs
        /// (
        /// )
        /// </seealso>
        IList<IHistoricDecisionOutputInstance> Outputs { get; }

        /// <summary>
        ///     The result of the collect operation if the hit policy 'collect' was used for the decision.
        /// </summary>
        double? CollectResultValue { get; }

        /// <summary>
        ///     The unique identifier of the historic decision instance of the evaluated root decision.
        ///     Can be <code>null</code> if this instance is the root decision instance of the evaluation.
        /// </summary>
        string RootDecisionInstanceId { get; }

        /// <summary>
        ///     The id of the related decision requirements definition. Can be
        ///     <code>null</code> if the decision has no relations to other decisions.
        /// </summary>
        string DecisionRequirementsDefinitionId { get; }

        /// <summary>
        ///     The key of the related decision requirements definition. Can be
        ///     <code>null</code> if the decision has no relations to other decisions.
        /// </summary>
        string DecisionRequirementsDefinitionKey { get; }

        /// <summary>
        ///     The id of the tenant this historic decision instance belongs to. Can be <code>null</code>
        ///     if the historic decision instance belongs to no single tenant.
        /// </summary>
        string TenantId { get; }
    }
}