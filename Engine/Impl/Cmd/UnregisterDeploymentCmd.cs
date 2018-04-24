using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Interceptor;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    /// </summary>
    public class UnregisterDeploymentCmd : ICommand<object>
    {
        protected internal IList<string> DeploymentIds;

        public UnregisterDeploymentCmd(IList<string> deploymentIds)
        {
            this.DeploymentIds = deploymentIds;
        }

        public UnregisterDeploymentCmd(string deploymentId) : this(new List<string> {deploymentId})
        {
        }

        public virtual object Execute(CommandContext commandContext)
        {
            foreach (var it in DeploymentIds)
                context.Impl.Context.ProcessEngineConfiguration.RegisteredDeployments.Remove(it);
            return null;
        }
    }
}