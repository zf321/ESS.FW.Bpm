using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.Impl.Cfg
{
    /// <summary>
    ///     <seealso cref="IProcessEnginePlugin" /> that provides composite behavior. When registered on an engine
    ///     configuration,
    ///     all plugins added to this composite will be triggered on preInit/postInit/postProcessEngineBuild.
    ///     <para>
    ///         Use to encapsulate common behavior (like engine configuration).
    ///     </para>
    /// </summary>
    public class CompositeProcessEnginePlugin : AbstractProcessEnginePlugin
    {
        protected internal readonly IList<IProcessEnginePlugin> plugins;

        /// <summary>
        ///     New instance with empty list.
        /// </summary>
        public CompositeProcessEnginePlugin()
        {
            plugins = new List<IProcessEnginePlugin>();
        }


        /// <summary>
        ///     New instance with vararg.
        /// </summary>
        /// <param name="plugin"> first plugin </param>
        /// <param name="additionalPlugins"> additional vararg plugins </param>
        public CompositeProcessEnginePlugin(IProcessEnginePlugin plugin, params IProcessEnginePlugin[] additionalPlugins)
            : this()
        {
            AddProcessEnginePlugin(plugin, additionalPlugins);
        }

        /// <summary>
        ///     New instance with initial plugins.
        /// </summary>
        /// <param name="plugins"> the initial plugins. Must not be null. </param>
        public CompositeProcessEnginePlugin(IList<IProcessEnginePlugin> plugins) : this()
        {
            AddProcessEnginePlugins(plugins);
        }

        /// <summary>
        ///     Get all plugins.
        /// </summary>
        /// <returns> the configured plugins </returns>
        public virtual IList<IProcessEnginePlugin> Plugins
        {
            get { return plugins; }
        }

        /// <summary>
        ///     Add one (or more) plugins.
        /// </summary>
        /// <param name="plugin"> first plugin </param>
        /// <param name="additionalPlugins"> additional vararg plugins </param>
        /// <returns> self for fluent usage </returns>
        public virtual CompositeProcessEnginePlugin AddProcessEnginePlugin(IProcessEnginePlugin plugin,
            params IProcessEnginePlugin[] additionalPlugins)
        {
            return AddProcessEnginePlugins(ToList(plugin, additionalPlugins));
        }

        /// <summary>
        ///     Add collection of plugins.
        ///     If collection is not sortable, order of plugin execution can not be guaranteed.
        /// </summary>
        /// <param name="plugins"> plugins to add </param>
        /// <returns> self for fluent usage </returns>
        public virtual CompositeProcessEnginePlugin AddProcessEnginePlugins(ICollection<IProcessEnginePlugin> plugins)
        {
            ((List<IProcessEnginePlugin>) this.plugins).AddRange(plugins);

            return this;
        }

        public override void PreInit(ProcessEngineConfigurationImpl processEngineConfiguration)
        {
            foreach (var plugin in plugins)
                plugin.PreInit(processEngineConfiguration);
        }

        public override void PostInit(ProcessEngineConfigurationImpl processEngineConfiguration)
        {
            foreach (var plugin in plugins)
                plugin.PostInit(processEngineConfiguration);
        }

        public override void PostProcessEngineBuild(IProcessEngine processEngine)
        {
            foreach (var plugin in plugins)
                plugin.PostProcessEngineBuild(processEngine);
        }

        public override string ToString()
        {
            return GetType().Name + plugins;
        }


        private static IList<IProcessEnginePlugin> ToList(IProcessEnginePlugin plugin,
            params IProcessEnginePlugin[] additionalPlugins)
        {
            IList<IProcessEnginePlugin> plugins = new List<IProcessEnginePlugin>();
            plugins.Add(plugin);
            if ((additionalPlugins != null) && (additionalPlugins.Length > 0))
                ((List<IProcessEnginePlugin>) plugins).AddRange(additionalPlugins);
            return plugins;
        }
    }
}