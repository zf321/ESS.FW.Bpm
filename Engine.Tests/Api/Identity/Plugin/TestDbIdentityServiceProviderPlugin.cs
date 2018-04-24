/// 

using Engine.Tests.Api.Identity.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Digest;

namespace Engine.Tests.Api.Identity.Plugin
{
    /// <summary>
    ///     
    /// </summary>
    public class TestDbIdentityServiceProviderPlugin : IProcessEnginePlugin
    {
        //internal TestDbIdentityServiceProviderFactory testFactory;

        public void PreInit(ProcessEngineConfigurationImpl processEngineConfiguration)
        {
            //processEngineConfiguration.IdentityProviderSessionFactory = testFactory;
            processEngineConfiguration.PasswordEncryptor = new ShaHashDigest();
        }

        public void PostInit(ProcessEngineConfigurationImpl processEngineConfiguration)
        {
            processEngineConfiguration.SaltGenerator = new MyConstantSaltGenerator("");
        }

        public void PostProcessEngineBuild(IProcessEngine processEngine)
        {
            // nothing to do here
        }
    }
}