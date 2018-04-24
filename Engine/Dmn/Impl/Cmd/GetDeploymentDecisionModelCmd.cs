using System;
using System.IO;
using ESS.FW.Bpm.Engine.Impl.Cmd;
using ESS.FW.Bpm.Engine.Impl.Interceptor;

namespace ESS.FW.Bpm.Engine.Dmn.Impl.Cmd
{
    /// <summary>
    ///     Gives access to a deployed decision model, e.g., a DMN 1.0 XML file, through a stream of bytes.
    /// </summary>
    [Serializable]
    public class GetDeploymentDecisionModelCmd : ICommand<Stream>
    {
        
        protected internal string DecisionDefinitionId;

        public GetDeploymentDecisionModelCmd(string decisionDefinitionId)
        {
            this.DecisionDefinitionId = decisionDefinitionId;
        }
        
        public virtual Stream Execute(CommandContext commandContext)
        {
            var decisionDefinition =
                new GetDeploymentDecisionDefinitionCmd(DecisionDefinitionId).Execute(commandContext);
            
            var deploymentId = decisionDefinition.DeploymentId;
            var resourceName = decisionDefinition.ResourceName;

            return
                commandContext.RunWithoutAuthorization(()=> new GetDeploymentResourceCmd(deploymentId, resourceName).Execute(commandContext));
        }
        
    }
}