using System;
using System.IO;
using ESS.FW.Bpm.Engine.Impl.Cmd;
using ESS.FW.Bpm.Engine.Impl.Interceptor;

namespace ESS.FW.Bpm.Engine.Dmn.Impl.Cmd
{
    /// <summary>
    ///     Gives access to a deployed decision requirements model, e.g., a DMN 1.1 XML file, through a stream of bytes.
    /// </summary>
    [Serializable]
    public class GetDeploymentDecisionRequirementsModelCmd : ICommand<Stream>
    {
        
        protected internal string DecisionRequirementsDefinitionId;

        public GetDeploymentDecisionRequirementsModelCmd(string decisionRequirementsDefinitionId)
        {
            this.DecisionRequirementsDefinitionId = decisionRequirementsDefinitionId;
        }
        
        public virtual Stream Execute(CommandContext commandContext)
        {
            var decisionRequirementsDefinition =
                new GetDeploymentDecisionRequirementsDefinitionCmd(DecisionRequirementsDefinitionId).Execute(
                    commandContext);
            
            var deploymentId = decisionRequirementsDefinition.DeploymentId;
            var resourceName = decisionRequirementsDefinition.ResourceName;

            return
                commandContext.RunWithoutAuthorization(()=> new GetDeploymentResourceCmd(deploymentId, resourceName).Execute(commandContext));
        }
        
    }
}