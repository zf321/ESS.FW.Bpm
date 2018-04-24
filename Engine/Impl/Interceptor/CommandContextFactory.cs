

using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Common.Components;

namespace ESS.FW.Bpm.Engine.Impl.Interceptor
{
    /// <summary>
    ///      
    /// </summary>
    public class CommandContextFactory
    {
        protected internal ProcessEngineConfigurationImpl processEngineConfiguration;

        // getters and setters //////////////////////////////////////////////////////

        public virtual ProcessEngineConfigurationImpl ProcessEngineConfiguration
        {
            get { return processEngineConfiguration; }
            set { processEngineConfiguration = value; }
        }

        public virtual CommandContext CreateCommandContext()
        {
            IScope scope = ObjectContainer.BeginLifetimeScope();
            return new CommandContext(processEngineConfiguration, scope);
        }
    }
}