using System;

using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Variable;


namespace ESS.FW.Bpm.Engine.Dmn.Impl.Cmd
{

    /// <summary>
    ///     Evaluates the decision table with the given key or id.
    ///     If the decision definition key given then specify the version and tenant-id.
    ///     If no version is provided then the latest version is taken.
    ///     
    /// </summary>
    public class EvaluateDecisionTableCmd : ICommand<IDmnDecisionTableResult>
    {
        protected internal string DecisionDefinitionId;

        protected internal string DecisionDefinitionKey;
        protected internal string DecisionDefinitionTenantId;
        protected internal bool IsTenandIdSet;
        protected internal IVariableMap Variables;
        protected internal int? Version;

        public EvaluateDecisionTableCmd(DecisionTableEvaluationBuilderImpl builder)
        {
            DecisionDefinitionKey = builder.DecisionDefinitionKey;
            DecisionDefinitionId = builder.DecisionDefinitionId;
            Version = builder.Version;
            Variables = Variable.Variables.FromMap(builder.Variables);
            DecisionDefinitionTenantId = builder.DecisionDefinitionTenantId;
            IsTenandIdSet = builder.TenantIdSet;
        }

        public IDmnDecisionTableResult Execute(CommandContext commandContext)
        {
            throw new NotImplementedException();
        }

        //public virtual IDmnDecisionTableResult execute(CommandContext commandContext)
        //{
        //    ensureOnlyOneNotNull("either decision definition id or key must be set", decisionDefinitionId,
        //        decisionDefinitionKey);

        //    var decisionDefinition = getDecisionDefinition(commandContext);

        //    foreach (var checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
        //    {

        //        checker.checkEvaluateDecision(decisionDefinition);
        //    }

        //    return doEvaluateDecision(decisionDefinition, variables);
        //}

        //protected internal virtual IDmnDecisionTableResult doEvaluateDecision(DecisionDefinition decisionDefinition,
        //    IVariableMap variables)
        //{
        //    try
        //    {
        //        return evaluateDecisionTable(decisionDefinition, variables);
        //    }
        //    catch (Exception e)
        //    {
        //        throw new ProcessEngineException(
        //            "Exception while evaluating decision with key '" + decisionDefinitionKey + "'", e);
        //    }
        //}

        //protected internal virtual DecisionDefinition getDecisionDefinition(CommandContext commandContext)
        //{
        //    DeploymentCache deploymentCache = commandContext.ProcessEngineConfiguration.DeploymentCache;

        //    if (!ReferenceEquals(decisionDefinitionId, null))
        //    {
        //        return findById(deploymentCache);
        //    }
        //    return findByKey(deploymentCache);
        //}

        //protected internal virtual DecisionDefinition findById(DeploymentCache deploymentCache)
        //{
        //    return deploymentCache.findDeployedDecisionDefinitionById(decisionDefinitionId);
        //}

        //protected internal virtual DecisionDefinition findByKey(DeploymentCache deploymentCache)
        //{
        //    DecisionDefinition decisionDefinition = null;

        //    if (version == null && !isTenandIdSet)
        //    {
        //        decisionDefinition = deploymentCache.findDeployedLatestDecisionDefinitionByKey(decisionDefinitionKey);
        //    }
        //    else if (version == null && isTenandIdSet)
        //    {
        //        decisionDefinition =
        //            deploymentCache.findDeployedLatestDecisionDefinitionByKeyAndTenantId(decisionDefinitionKey,
        //                decisionDefinitionTenantId);
        //    }
        //    else if (version != null && !isTenandIdSet)
        //    {
        //        decisionDefinition = deploymentCache.findDeployedDecisionDefinitionByKeyAndVersion(
        //            decisionDefinitionKey, version);
        //    }
        //    else if (version != null && isTenandIdSet)
        //    {
        //        decisionDefinition =
        //            deploymentCache.findDeployedDecisionDefinitionByKeyVersionAndTenantId(decisionDefinitionKey, version,
        //                decisionDefinitionTenantId);
        //    }

        //    return decisionDefinition;
        //}
    }
}