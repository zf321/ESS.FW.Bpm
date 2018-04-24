using ESS.FW.Bpm.Engine.Impl.Cfg;

namespace ESS.FW.Bpm.Engine.Impl.DB
{
    /// <summary>
    ///      
    /// </summary>
    public class DbSchemaCreate
    {
        public static void Main(string[] args)
        {
            ProcessEngineConfiguration.CreateProcessEngineConfigurationFromResourceDefault()
                .SetDatabaseSchemaUpdate(ProcessEngineConfigurationImpl.DbSchemaUpdateCreate)
                .BuildProcessEngine();
        }
    }
}