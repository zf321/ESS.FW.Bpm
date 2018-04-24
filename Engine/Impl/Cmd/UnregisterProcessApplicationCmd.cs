using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Interceptor;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///     
    /// </summary>
    public class UnregisterProcessApplicationCmd : ICommand<object>
    {
        protected internal IList<string> DeploymentIds;

        protected internal bool RemoveProcessesFromCache;

        public UnregisterProcessApplicationCmd(string deploymentId, bool removeProcessesFromCache)
            : this(new List<string>() { deploymentId }, removeProcessesFromCache)
        {
        }

        public UnregisterProcessApplicationCmd(IList<string> deploymentIds, bool removeProcessesFromCache)
        {
            this.DeploymentIds = deploymentIds;
            this.RemoveProcessesFromCache = removeProcessesFromCache;
        }

        public virtual object Execute(CommandContext commandContext)
        {
            if (DeploymentIds == null)
                throw new ProcessEngineException("Deployment Ids cannot be null.");

            commandContext.AuthorizationManager.CheckCamundaAdmin();

            context.Impl.Context.ProcessEngineConfiguration.ProcessApplicationManager.UnregisterProcessApplicationForDeployments(
                DeploymentIds, RemoveProcessesFromCache);
            return null;
        }
    }
}