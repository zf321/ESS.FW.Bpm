
using ESS.FW.Bpm.Engine.Persistence;
using ESS.FW.Common.Components;
using ESS.FW.DataAccess;
using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.Configuration
{
    /// <summary>
    ///     configuration class bpm.
    /// </summary>
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// </summary>
        /// <returns></returns>
        public static FW.Common.Configurations.Configuration UseBpm(
            this FW.Common.Configurations.Configuration configuration)
        {
            IProcessEngine processEngine = 
            Engine.ProcessEngineConfiguration.CreateStandaloneInMemProcessEngineConfiguration().BuildProcessEngine();
            ObjectContainer.Current.RegisterInstance<IProcessEngine,IProcessEngine>(processEngine);
            ObjectContainer.Current.RegisterGeneric(typeof(IRepository<,>), typeof(AbstractManagerNet<>),
                LifeStyle.Transient);
            return configuration;
        }
    }
}