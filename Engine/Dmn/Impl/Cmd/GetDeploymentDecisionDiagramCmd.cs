using System;
using System.IO;
using ESS.FW.Bpm.Engine.Impl.Cmd;
using ESS.FW.Bpm.Engine.Impl.Interceptor;

namespace ESS.FW.Bpm.Engine.Dmn.Impl.Cmd
{
    /// <summary>
    ///     Gives access to a deployed decision diagram, e.g., a PNG image, through a stream of bytes.
    /// </summary>
    [Serializable]
    public class GetDeploymentDecisionDiagramCmd : ICommand<Stream>
    {
        

        protected internal string DecisionDefinitionId;

        public GetDeploymentDecisionDiagramCmd(string decisionDefinitionId)
        {
            this.DecisionDefinitionId = decisionDefinitionId;
        }
        
        public virtual Stream Execute(CommandContext commandContext)
        {
            var decisionDefinition =
                new GetDeploymentDecisionDefinitionCmd(DecisionDefinitionId).Execute(commandContext);
            
            var deploymentId = decisionDefinition.DeploymentId;
            var resourceName = decisionDefinition.DiagramResourceName;

            if (!ReferenceEquals(resourceName, null))
            {
                return
                    commandContext.RunWithoutAuthorization(() => new GetDeploymentResourceCmd(deploymentId, resourceName).Execute(commandContext));
            }
            return null;
        }
    }
}