using System;
using System.IO;
using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///     Gives access to a deployed process model, e.g., a BPMN 2.0 XML file, through
    ///     a stream of bytes.
    ///     
    /// </summary>
    [Serializable]
    public class GetDeploymentProcessModelCmd : ICommand<Stream>
    {
        private const long SerialVersionUid = 1L;
        protected internal string ProcessDefinitionId;

        public GetDeploymentProcessModelCmd(string processDefinitionId)
        {
            if (ReferenceEquals(processDefinitionId, null) || (processDefinitionId.Length < 1))
                throw new ProcessEngineException("The process definition id is mandatory, but '" + processDefinitionId +
                                                 "' has been provided.");
            this.ProcessDefinitionId = processDefinitionId;
        }
        
        public virtual Stream Execute(CommandContext commandContext)
        {
            ProcessDefinitionEntity processDefinition = context.Impl.Context.ProcessEngineConfiguration.DeploymentCache.FindDeployedProcessDefinitionById(ProcessDefinitionId);

            foreach (var checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
            {
                checker.CheckReadProcessDefinition(processDefinition);
            }
            
            string deploymentId = processDefinition.DeploymentId;
            string resourceName = processDefinition.ResourceName;

            System.IO.Stream processModelStream = commandContext.RunWithoutAuthorization(()=> new GetDeploymentResourceCmd(deploymentId, resourceName).Execute(commandContext));
            return processModelStream;
        }
        
    }
}