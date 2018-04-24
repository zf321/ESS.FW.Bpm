using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Interceptor;

namespace ESS.FW.Bpm.Engine.Impl.Cfg
{
    /// <summary>
    ///     
    /// </summary>
    public class StandaloneProcessEngineConfiguration : ProcessEngineConfigurationImpl
    {
       protected internal override ICollection<CommandInterceptor> DefaultCommandInterceptorsTxRequired
        {
            get
            {
                IList<CommandInterceptor> defaultCommandInterceptorsTxRequired = new List<CommandInterceptor>();
                defaultCommandInterceptorsTxRequired.Add(new LogInterceptor());
                defaultCommandInterceptorsTxRequired.Add(new ProcessApplicationContextInterceptor(this));
                defaultCommandInterceptorsTxRequired.Add(new CommandContextInterceptor(commandContextFactory, this));
                return defaultCommandInterceptorsTxRequired;
            }
        }

        protected internal override ICollection<CommandInterceptor> DefaultCommandInterceptorsTxRequiresNew
        {
            get
            {
                IList<CommandInterceptor> defaultCommandInterceptorsTxRequired = new List<CommandInterceptor>();
                defaultCommandInterceptorsTxRequired.Add(new LogInterceptor());
                defaultCommandInterceptorsTxRequired.Add(new ProcessApplicationContextInterceptor(this));
                defaultCommandInterceptorsTxRequired.Add(new CommandContextInterceptor(commandContextFactory, this, true));
                return defaultCommandInterceptorsTxRequired;
            }
        }
    }
}