using System;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Deploy.Cache;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Model.Bpmn;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{

    /// <summary>
    ///     Gives access to a deploy BPMN model instance which can be accessed by
    ///     the BPMN model API.
    ///     
    /// </summary>
    [Serializable]
    public class GetDeploymentBpmnModelInstanceCmd : ICommand<IBpmnModelInstance>
    {
        private const long SerialVersionUid = 1L;
        protected internal string ProcessDefinitionId;

        public GetDeploymentBpmnModelInstanceCmd(string processDefinitionId)
        {
            if (ReferenceEquals(processDefinitionId, null) || (processDefinitionId.Length < 1))
                throw new ProcessEngineException("The process definition id is mandatory, but '" + processDefinitionId +
                                                 "' has been provided.");
            this.ProcessDefinitionId = processDefinitionId;
        }

        public virtual IBpmnModelInstance Execute(CommandContext commandContext)
        {
            var configuration = context.Impl.Context.ProcessEngineConfiguration;
            DeploymentCache deploymentCache = configuration.DeploymentCache;

            ProcessDefinitionEntity processDefinition =
                deploymentCache.FindDeployedProcessDefinitionById(ProcessDefinitionId);

            foreach (var checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
            {
                checker.CheckReadProcessDefinition(processDefinition);
            }

            IBpmnModelInstance modelInstance =
                deploymentCache.FindBpmnModelInstanceForProcessDefinition(ProcessDefinitionId);

            EnsureUtil.EnsureNotNull("no BPMN model instance found for process definition id " + ProcessDefinitionId,
                "modelInstance", modelInstance);
            return modelInstance;
        }
    }
}