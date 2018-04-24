using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Repository;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{


    /// <summary>
    /// </summary>
    public class RegisterDeploymentCmd : ICommand<object>
    {
        protected internal string DeploymentId;

        public RegisterDeploymentCmd(string deploymentId)
        {
            this.DeploymentId = deploymentId;
        }

        public virtual object Execute(CommandContext commandContext)
        {
            IDeployment deployment = commandContext.DeploymentManager.FindDeploymentById(DeploymentId);

            EnsureUtil.EnsureNotNull("Deployment " + DeploymentId + " does not exist", "deployment", deployment);

            commandContext.AuthorizationManager.CheckCamundaAdmin();

            context.Impl.Context.ProcessEngineConfiguration.RegisteredDeployments.Add(DeploymentId);
            return null;
        }
    }
}