using System;
using ESS.FW.Bpm.Engine.Form;
using ESS.FW.Bpm.Engine.Form.Impl.Handler;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Deploy.Cache;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    

    /// <summary>
    ///      
    /// </summary>
    [Serializable]
    public class GetStartFormCmd : ICommand<IStartFormData>
    {
        private const long SerialVersionUid = 1L;
        protected internal string ProcessDefinitionId;

        public GetStartFormCmd(string processDefinitionId)
        {
            this.ProcessDefinitionId = processDefinitionId;
        }

        public virtual IStartFormData Execute(CommandContext commandContext)
        {
            var processEngineConfiguration = context.Impl.Context.ProcessEngineConfiguration;
            DeploymentCache deploymentCache = processEngineConfiguration.DeploymentCache;
            ProcessDefinitionEntity processDefinition = deploymentCache.FindDeployedProcessDefinitionById(ProcessDefinitionId);
            EnsureUtil.EnsureNotNull("No process definition found for id '" + ProcessDefinitionId + "'", "processDefinition", processDefinition);

            foreach (var checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
            {
                checker.CheckReadProcessDefinition(processDefinition);
            }

            IStartFormHandler startFormHandler = processDefinition.StartFormHandler;
            EnsureUtil.EnsureNotNull("No startFormHandler defined in process '" + ProcessDefinitionId + "'", "startFormHandler", startFormHandler);

            return startFormHandler.CreateStartFormData(processDefinition);
        }
    }
}