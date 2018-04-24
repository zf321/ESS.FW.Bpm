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
    ///     
    /// </summary>
    [Serializable]
    public class GetRenderedStartFormCmd : ICommand<object>
    {
        private const long SerialVersionUid = 1L;
        protected internal string FormEngineName;
        protected internal string ProcessDefinitionId;

        public GetRenderedStartFormCmd(string processDefinitionId, string formEngineName)
        {
            this.ProcessDefinitionId = processDefinitionId;
            this.FormEngineName = formEngineName;
        }

        public virtual object Execute(CommandContext commandContext)
        {
            var processEngineConfiguration = context.Impl.Context.ProcessEngineConfiguration;
            DeploymentCache deploymentCache = processEngineConfiguration.DeploymentCache;
            ProcessDefinitionEntity processDefinition = deploymentCache.FindDeployedProcessDefinitionById(ProcessDefinitionId);
            EnsureUtil.EnsureNotNull("Process Definition '" + ProcessDefinitionId + "' not found", "processDefinition", processDefinition);

            foreach (var checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
            {
                checker.CheckReadProcessDefinition(processDefinition);
            }
            IStartFormHandler startFormHandler = processDefinition.StartFormHandler;
            if (startFormHandler == null)
            {
                return null;
            }

            var formEngine = context.Impl.Context.ProcessEngineConfiguration.FormEngines[FormEngineName];

            EnsureUtil.EnsureNotNull("No formEngine '" + FormEngineName + "' defined process engine configuration",
                "formEngine", formEngine);

            IStartFormData startForm = startFormHandler.CreateStartFormData(processDefinition);

            return formEngine.RenderStartForm(startForm);
        }
    }
}