using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Form.Impl.Validator;
using ESS.FW.Bpm.Engine.Impl.Cfg;

namespace Engine.Tests.Api.Authorization.Service
{

    /// <summary>
    /// 
    /// 
    /// </summary>
    public class MyFormFieldValidator : MyDelegationService, IFormFieldValidator
    {
        public bool Validate(object submittedValue, IFormFieldValidatorContext validatorContext)
        {
            ProcessEngineConfigurationImpl processEngineConfiguration = ESS.FW.Bpm.Engine.context.Impl.Context.ProcessEngineConfiguration;
            IIdentityService identityService = processEngineConfiguration.IdentityService;
            IRuntimeService runtimeService = processEngineConfiguration.RuntimeService;

            logAuthentication(identityService);
            logInstancesCount(runtimeService);

            return true;
        }

        //public virtual bool validate(object submittedValue, IFormFieldValidatorContext validatorContext)
        //{
        //    ProcessEngineConfigurationImpl processEngineConfiguration = Context.ProcessEngineConfiguration;
        //    IIdentityService identityService = processEngineConfiguration.IdentityService;
        //    IRuntimeService runtimeService = processEngineConfiguration.RuntimeService;

        //    logAuthentication(identityService);
        //    logInstancesCount(runtimeService);

        //    return true;
        //}

    }

}