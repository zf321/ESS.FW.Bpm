namespace ESS.FW.Bpm.Engine.Impl.Cfg
{
    /// <summary>
    ///     <para>A process engine plugin allows customizing the process engine</para>
    ///     
    /// </summary>
    public interface IProcessEnginePlugin
    {
        /// <summary>
        ///     <para>Invoked before the process engine configuration is initialized.</para>
        /// </summary>
        /// <param name="processEngineConfiguration">
        ///     the process engine configuation
        /// </param>
        void PreInit(ProcessEngineConfigurationImpl processEngineConfiguration);

        /// <summary>
        ///     <para>
        ///         Invoked after the process engine configuration is initialized.
        ///         and before the process engine is built.
        ///     </para>
        /// </summary>
        /// <param name="processEngineConfiguration">
        ///     the process engine configuation
        /// </param>
        void PostInit(ProcessEngineConfigurationImpl processEngineConfiguration);

        /// <summary>
        ///     <para>Invoked after the process engine has been built.</para>
        /// </summary>
        /// <param name="processEngine"> </param>
        void PostProcessEngineBuild(IProcessEngine processEngine);
    }
}