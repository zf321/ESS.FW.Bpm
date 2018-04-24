using System;
using System.IO;
using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Diagram;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Repository;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///     Provides positions and dimensions of elements in a process diagram as
    ///     provided by <seealso cref="GetDeploymentProcessDiagramCmd" />.
    ///     This command requires a process model and a diagram image to be deployed.
    ///     
    /// </summary>
    [Serializable]
    public class GetDeploymentProcessDiagramLayoutCmd : ICommand<DiagramLayout>
    {
        private const long SerialVersionUid = 1L;
        protected internal string ProcessDefinitionId;

        public GetDeploymentProcessDiagramLayoutCmd(string processDefinitionId)
        {
            if (ReferenceEquals(processDefinitionId, null) || (processDefinitionId.Length < 1))
                throw new ProcessEngineException("The process definition id is mandatory, but '" + processDefinitionId +
                                                 "' has been provided.");
            this.ProcessDefinitionId = processDefinitionId;
        }
        
        public virtual DiagramLayout Execute(CommandContext commandContext)
        {
            ProcessDefinitionEntity processDefinition = context.Impl.Context.ProcessEngineConfiguration.DeploymentCache.FindDeployedProcessDefinitionById(ProcessDefinitionId);

            foreach (ICommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
            {
                checker.CheckReadProcessDefinition(processDefinition);
            }

            System.IO.Stream processModelStream = commandContext.RunWithoutAuthorization(()=> new GetDeploymentProcessModelCmd(ProcessDefinitionId).Execute(commandContext));

            System.IO.Stream processDiagramStream = commandContext.RunWithoutAuthorization(()=> new GetDeploymentProcessDiagramCmd(ProcessDefinitionId).Execute(commandContext));

            return (new ProcessDiagramLayoutFactory()).GetProcessDiagramLayout(processModelStream, processDiagramStream);
        }
        
    }
}