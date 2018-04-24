using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Dmn.Impl.Entity.Repository;
using ESS.FW.Bpm.Engine.exception.dmn;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Deploy.Cache;
using ESS.FW.Bpm.Model.Dmn;

namespace ESS.FW.Bpm.Engine.Dmn.Impl.Cmd
{

    /// <summary>
    ///     Gives access to a deployed DMN model instance which can be accessed by the
    ///     DMN model API.
    /// </summary>
    public class GetDeploymentDmnModelInstanceCmd : ICommand<IDmnModelInstance>
    {
        protected internal string DecisionDefinitionId;

        public GetDeploymentDmnModelInstanceCmd(string decisionDefinitionId)
        {
            this.DecisionDefinitionId = decisionDefinitionId;
        }

        public virtual IDmnModelInstance Execute(CommandContext commandContext)
        {
            EnsureUtil.EnsureNotNull("decisionDefinitionId", DecisionDefinitionId);

            DeploymentCache deploymentCache = Context.ProcessEngineConfiguration.DeploymentCache;

            DecisionDefinitionEntity decisionDefinition =
                deploymentCache.FindDeployedDecisionDefinitionById(DecisionDefinitionId);

            foreach (ICommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
            {
                checker.CheckReadDecisionDefinition(decisionDefinition);
            }

            IDmnModelInstance modelInstance =
                deploymentCache.FindDmnModelInstanceForDecisionDefinition(DecisionDefinitionId);

            EnsureUtil.EnsureNotNull(typeof(DmnModelInstanceNotFoundException),
                "No DMN model instance found for decision definition id " + DecisionDefinitionId, "modelInstance",
                modelInstance);
            return modelInstance;
            return null;
        }
    }
}