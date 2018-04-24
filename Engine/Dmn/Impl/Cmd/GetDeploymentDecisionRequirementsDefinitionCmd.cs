using System;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Repository;

namespace ESS.FW.Bpm.Engine.Dmn.Impl.Cmd
{

    /// <summary>
    ///     Gives access to a deployed decision requirements definition instance.
    /// </summary>
    [Serializable]
    public class GetDeploymentDecisionRequirementsDefinitionCmd : ICommand<IDecisionRequirementsDefinition>
    {
        
        protected internal string DecisionRequirementsDefinitionId;

        public GetDeploymentDecisionRequirementsDefinitionCmd(string decisionRequirementsDefinitionId)
        {
            this.DecisionRequirementsDefinitionId = decisionRequirementsDefinitionId;
        }

        public virtual IDecisionRequirementsDefinition Execute(CommandContext commandContext)
        {
            EnsureUtil.EnsureNotNull("decisionRequirementsDefinitionId", DecisionRequirementsDefinitionId);
            //DeploymentCache deploymentCache = Context.ProcessEngineConfiguration.DeploymentCache;
            //DecisionRequirementsDefinitionEntity decisionRequirementsDefinition =
            //    deploymentCache.findDeployedDecisionRequirementsDefinitionById(decisionRequirementsDefinitionId);

            foreach (var checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
            {
                //checker.checkReadDecisionRequirementsDefinition(decisionRequirementsDefinition);
            }

            //return decisionRequirementsDefinition;
            return null;
        }
    }
}