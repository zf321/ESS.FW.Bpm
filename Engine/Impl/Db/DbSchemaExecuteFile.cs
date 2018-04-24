using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;

namespace ESS.FW.Bpm.Engine.Impl.DB
{
    /// <summary>
    /// </summary>
    public class DbSchemaExecuteFile
    {
        protected internal static readonly EnginePersistenceLogger LOG = ProcessEngineLogger.PersistenceLogger;

        /// <param name="args"> </param>
        public static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                throw LOG.InvokeSchemaResourceToolException(args.Length);
            }
            
            var configurationFileResourceName = args[0];
            var schemaFileResourceName = args[1];

            EnsureUtil.EnsureNotNull("Process engine configuration file name cannot be null",
                "configurationFileResourceName", configurationFileResourceName);
            EnsureUtil.EnsureNotNull("Schema resource file name cannot be null", "schemaFileResourceName",
                schemaFileResourceName);

            var configuration =
                (ProcessEngineConfigurationImpl)
                ProcessEngineConfiguration.CreateProcessEngineConfigurationFromResource(
                    configurationFileResourceName);
            var processEngine = configuration.BuildProcessEngine();

            configuration.CommandExecutorTxRequired.Execute(new CommandAnonymousInnerClass(schemaFileResourceName));

            processEngine.Close();
        }

        private class CommandAnonymousInnerClass : ICommand<object>
        {
            private readonly string _schemaFileResourceName;

            public CommandAnonymousInnerClass(string schemaFileResourceName)
            {
                _schemaFileResourceName = schemaFileResourceName;
            }


            public virtual object Execute(CommandContext commandContext)
            {
                //commandContext.DbSqlSession.executeSchemaResource(schemaFileResourceName);
                return null;
            }
        }
    }
}