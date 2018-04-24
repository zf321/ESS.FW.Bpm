using ESS.FW.Bpm.Engine.Application;
using ESS.FW.Bpm.Engine.Application.Impl;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    public class RegisterProcessApplicationNetCmd : ICommand<IProcessApplicationRegistration>
    {
        private DeploymentEntity _deloyment;
        public RegisterProcessApplicationNetCmd(DeploymentEntity deloyment)
        {
            this._deloyment = deloyment;
        }
        public IProcessApplicationRegistration Execute(CommandContext commandContext)
        {
            #region 源码实现
            //commandContext.AuthorizationManager.CheckCamundaAdmin();

            ////JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            ////ORIGINAL LINE: final org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl processEngineConfiguration = org.camunda.bpm.engine.impl.context.Context.GetProcessEngineConfiguration();
            //var processEngineConfiguration = context.Impl.Context.ProcessEngineConfiguration;
            ////JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            ////ORIGINAL LINE: final org.camunda.bpm.engine.impl.application.ProcessApplicationManager processApplicationManager = processEngineConfiguration.getProcessApplicationManager();
            //var processApplicationManager = processEngineConfiguration.ProcessApplicationManager;

            //return processApplicationManager.RegisterProcessApplicationForDeployments(DeploymentsToRegister, Reference);
            #endregion
            commandContext.AuthorizationManager.CheckCamundaAdmin();
            var processEngineConfiguration = context.Impl.Context.ProcessEngineConfiguration;
            var processApplicationManager = processEngineConfiguration.ProcessApplicationManager;
            var processEngineName = context.Impl.Context.ProcessEngineConfiguration.ProcessEngineName;
            return processApplicationManager.RegisterProcessApplicationForDeployments(_deloyment.Id, _deloyment);
            //var registration = new DefaultProcessApplicationRegistration(null, new HashSet<string>() { _deloyment.Id},processEngineName);
            // add to registration map
            //foreach (var deploymentId in deploymentsToRegister)
            //    RegistrationsByDeploymentId[deploymentId] = registration;
            
            //return registration;
        }
        private ISet<string> GetDeploymentsToRegister()
        {
             return new HashSet<string>() { _deloyment.Id };
        }
    }
}
