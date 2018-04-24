using System;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Repository;

namespace ESS.FW.Bpm.Engine.Dmn.Impl.Cmd
{
    /// <summary>
    ///     Gives access to a deployed decision definition instance.
    /// </summary>
    [Serializable]
    public class GetDeploymentDecisionDefinitionCmd : ICommand<IDecisionDefinition>
    {
        
        protected internal string DecisionDefinitionId;

        public GetDeploymentDecisionDefinitionCmd(string decisionDefinitionId)
        {
            this.DecisionDefinitionId = decisionDefinitionId;
        }

        public virtual IDecisionDefinition Execute(CommandContext commandContext)
        {
            EnsureUtil.EnsureNotNull("decisionDefinitionId", DecisionDefinitionId);
            //DeploymentCache deploymentCache = Context.ProcessEngineConfiguration.DeploymentCache;
            //DecisionDefinitionEntity decisionDefinition =
            //    deploymentCache.findDeployedDecisionDefinitionById(decisionDefinitionId);

            //foreach (ICommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
            //{
            //    checker.checkReadDecisionDefinition(decisionDefinition);
            //}

            //return decisionDefinition;
            return null;
        }
    }
}