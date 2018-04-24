using System;

using ESS.FW.Bpm.Engine.Dmn.engine;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Deploy.Cache;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Variable;


namespace ESS.FW.Bpm.Engine.Dmn.Impl.Cmd
{

    /// <summary>
    ///     Evaluates the decision with the given key or id.
    ///     If the decision definition key given then specify the version and tenant-id.
    ///     If no version is provided then the latest version is taken.
    /// </summary>
    public class EvaluateDecisionCmd : ICommand<IDmnDecisionResult>
    {
        protected internal string DecisionDefinitionId;

        protected internal string DecisionDefinitionKey;
        protected internal string DecisionDefinitionTenantId;
        protected internal bool IsTenandIdSet;
        protected internal IVariableMap Variables;
        protected internal int? Version;

        public EvaluateDecisionCmd(DecisionEvaluationBuilderImpl builder)
        {
            DecisionDefinitionKey = builder.DecisionDefinitionKey;
            DecisionDefinitionId = builder.DecisionDefinitionId;
            Version = builder._Version;//.Version;
            Variables = Variable.Variables.FromMap(builder._Variables);
            DecisionDefinitionTenantId = builder._DecisionDefinitionTenantId;
            IsTenandIdSet = builder.TenantIdSet;
        }

        public virtual IDmnDecisionResult Execute(CommandContext commandContext)
        {
            EnsureUtil.EnsureOnlyOneNotNull("either decision definition id or key must be set", DecisionDefinitionId,
                DecisionDefinitionKey);

            var decisionDefinition = GetDecisionDefinition(commandContext);

            foreach (var checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
            {
                checker.CheckEvaluateDecision(decisionDefinition);
            }

            return DoEvaluateDecision(decisionDefinition, Variables);
        }

        protected internal virtual IDmnDecisionResult DoEvaluateDecision(IDecisionDefinition decisionDefinition,
            IVariableMap variables)
        {
            try
            {
                return DecisionEvaluationUtil.EvaluateDecision(decisionDefinition, variables);
            }
            catch (System.Exception e)
            {
                throw new ProcessEngineException(
                    "Exception while evaluating decision with key '" + DecisionDefinitionKey + "'", e);
            }
        }

        protected internal virtual IDecisionDefinition GetDecisionDefinition(CommandContext commandContext)
        {
            DeploymentCache deploymentCache = commandContext.ProcessEngineConfiguration.DeploymentCache;

            if (!ReferenceEquals(DecisionDefinitionId, null))
            {
                return FindById(deploymentCache);
            }
            return FindByKey(deploymentCache);

        }

        protected internal virtual IDecisionDefinition FindById(DeploymentCache deploymentCache)
        {
            return deploymentCache.FindDeployedDecisionDefinitionById(DecisionDefinitionId);
        }

        protected internal virtual IDecisionDefinition FindByKey(DeploymentCache deploymentCache)
        {
            IDecisionDefinition decisionDefinition = null;

            if (Version == null && !IsTenandIdSet)
            {
                decisionDefinition = deploymentCache.FindDeployedLatestDecisionDefinitionByKey(DecisionDefinitionKey);
            }
            else if (Version == null && IsTenandIdSet)
            {
                decisionDefinition =
                    deploymentCache.FindDeployedLatestDecisionDefinitionByKeyAndTenantId(DecisionDefinitionKey,
                        DecisionDefinitionTenantId);
            }
            else if (Version != null && !IsTenandIdSet)
            {
                decisionDefinition = deploymentCache.FindDeployedDecisionDefinitionByKeyAndVersion(
                    DecisionDefinitionKey, Version);
            }
            else if (Version != null && IsTenandIdSet)
            {
                decisionDefinition =
                    deploymentCache.FindDeployedDecisionDefinitionByKeyVersionAndTenantId(DecisionDefinitionKey, Version,
                        DecisionDefinitionTenantId);
            }

            return decisionDefinition;
        }
    }
}