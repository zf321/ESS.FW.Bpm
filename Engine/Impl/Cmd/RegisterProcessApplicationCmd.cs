using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Application;
using ESS.FW.Bpm.Engine.Impl.Interceptor;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///     
    /// </summary>
    public class RegisterProcessApplicationCmd : ICommand<IProcessApplicationRegistration>
    {
        protected internal IList<string> DeploymentsToRegister;

        protected internal IProcessApplicationReference Reference;

        public RegisterProcessApplicationCmd(string deploymentId, IProcessApplicationReference reference)
            : this(new List<string>(){deploymentId}, reference)
        {
        }

        public RegisterProcessApplicationCmd(IList<string> deploymentsToRegister,
            IProcessApplicationReference appReference)
        {
            this.DeploymentsToRegister = deploymentsToRegister;
            Reference = appReference;
        }

        public virtual IProcessApplicationRegistration Execute(CommandContext commandContext)
        {
            commandContext.AuthorizationManager.CheckCamundaAdmin();
            
            var processEngineConfiguration = context.Impl.Context.ProcessEngineConfiguration;
            var processApplicationManager = processEngineConfiguration.ProcessApplicationManager;

            return processApplicationManager.RegisterProcessApplicationForDeployments(DeploymentsToRegister, Reference);
        }
    }
}