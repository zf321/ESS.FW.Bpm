using ESS.FW.Bpm.Engine.Impl.Interceptor;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///     
    /// </summary>
    public class GetProcessApplicationForDeploymentCmd : ICommand<string>
    {
        protected internal string DeploymentId;

        public GetProcessApplicationForDeploymentCmd(string deploymentId)
        {
            this.DeploymentId = deploymentId;
        }

        public virtual string Execute(CommandContext commandContext)
        {
            commandContext.AuthorizationManager.CheckCamundaAdmin();

            var reference =
                context.Impl.Context.ProcessEngineConfiguration.ProcessApplicationManager.GetProcessApplicationForDeployment(
                    DeploymentId);

            if (reference != null)
                return reference.Name;
            return null;
        }
    }
}