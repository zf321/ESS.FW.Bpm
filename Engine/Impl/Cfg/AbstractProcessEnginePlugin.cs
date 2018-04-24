namespace ESS.FW.Bpm.Engine.Impl.Cfg
{
    /// <summary>
    ///     <para>Adapter class for implementing process engine plugins</para>
    ///     
    /// </summary>
    public class AbstractProcessEnginePlugin : IProcessEnginePlugin
    {
        public virtual void PreInit(ProcessEngineConfigurationImpl processEngineConfiguration)
        {
        }

        public virtual void PostInit(ProcessEngineConfigurationImpl processEngineConfiguration)
        {
        }

        public virtual void PostProcessEngineBuild(IProcessEngine processEngine)
        {
        }

        public override string ToString()
        {
            return GetType().Name;
        }
    }
}