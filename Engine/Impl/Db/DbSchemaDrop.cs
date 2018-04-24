using ESS.FW.Bpm.Engine.Impl.Interceptor;

namespace ESS.FW.Bpm.Engine.Impl.DB
{
    /// <summary>
    ///      
    /// </summary>
    public class DbSchemaDrop
    {
        public static void Main(string[] args)
        {
            var processEngine = (ProcessEngineImpl) ProcessEngines.DefaultProcessEngine;
            //CommandExecutor commandExecutor = processEngine.ProcessEngineConfiguration.CommandExecutorTxRequired;
            //commandExecutor.execute(new CommandAnonymousInnerClass());
            processEngine.Close();
        }

        private class CommandAnonymousInnerClass : ICommand<object>
        {
            public virtual object Execute(CommandContext commandContext)
            {
                //commandContext.getSession(typeof (PersistenceSession)).dbSchemaDrop();
                return null;
            }
        }
    }
}