using System;
using System.IO;
using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///     Gives access to a deployed process diagram, e.g., a PNG image, through a
    ///     stream of bytes.
    ///     
    /// </summary>
    [Serializable]
    public class GetDeploymentProcessDiagramCmd : ICommand<Stream>
    {
        private const long SerialVersionUid = 1L;

        protected internal string ProcessDefinitionId;

        public GetDeploymentProcessDiagramCmd(string processDefinitionId)
        {
            if (ReferenceEquals(processDefinitionId, null) || (processDefinitionId.Length < 1))
                throw new ProcessEngineException("The process definition id is mandatory, but '" + processDefinitionId +
                                                 "' has been provided.");
            this.ProcessDefinitionId = processDefinitionId;
        }
        
        public virtual Stream Execute(CommandContext commandContext)
        {
            ProcessDefinitionEntity processDefinition = context.Impl.Context.ProcessEngineConfiguration.DeploymentCache.FindDeployedProcessDefinitionById(ProcessDefinitionId);

            foreach (ICommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
            {
                checker.CheckReadProcessDefinition(processDefinition);
            }
            
            string deploymentId = processDefinition.DeploymentId;
            string resourceName = processDefinition.DiagramResourceName;

            if (string.ReferenceEquals(resourceName, null))
            {
                return null;
            }
            else
            {

                Stream processDiagramStream = commandContext.RunWithoutAuthorization(()=> new GetDeploymentResourceCmd(deploymentId, resourceName).Execute(commandContext));

                return processDiagramStream;
            }
        }
        
    }
}