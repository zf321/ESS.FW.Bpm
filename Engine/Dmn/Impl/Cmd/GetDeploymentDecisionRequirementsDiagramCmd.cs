using System;
using System.IO;
using ESS.FW.Bpm.Engine.Impl.Cmd;
using ESS.FW.Bpm.Engine.Impl.Interceptor;

namespace ESS.FW.Bpm.Engine.Dmn.Impl.Cmd
{
    /// <summary>
    ///     Gives access to a deployed decision requirements diagram, e.g., a PNG image, through a stream of bytes.
    /// </summary>
    [Serializable]
    public class GetDeploymentDecisionRequirementsDiagramCmd : ICommand<Stream>
    {
        

        protected internal string DecisionRequirementsDefinitionId;

        public GetDeploymentDecisionRequirementsDiagramCmd(string decisionRequirementsDefinitionId)
        {
            this.DecisionRequirementsDefinitionId = decisionRequirementsDefinitionId;
        }
        
        public virtual Stream Execute(CommandContext commandContext)
        {
            var decisionRequirementsDefinition =
                new GetDeploymentDecisionRequirementsDefinitionCmd(DecisionRequirementsDefinitionId).Execute(
                    commandContext);
            
            var deploymentId = decisionRequirementsDefinition.DeploymentId;
            var resourceName = decisionRequirementsDefinition.DiagramResourceName;

            if (!ReferenceEquals(resourceName, null))
            {
                return
                    commandContext.RunWithoutAuthorization(()=> new GetDeploymentResourceCmd(deploymentId, resourceName).Execute(commandContext));
            }
            return null;
        }
        
    }
}