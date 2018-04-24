using ESS.FW.Bpm.Engine.Impl.Interceptor;

namespace ESS.FW.Bpm.Engine.Impl.DB
{
    /// <summary>
    ///      
    /// </summary>
    public class DbSchemaPrune
    {
        public static void Main(string[] args)
        {
            var processEngine = (ProcessEngineImpl) ProcessEngines.DefaultProcessEngine;
            //ICommandExecutor commandExecutor = processEngine.ProcessEngineConfiguration.CommandExecutorTxRequired;
            //commandExecutor.execute(new CommandAnonymousInnerClass());
        }

        private class CommandAnonymousInnerClass : ICommand<object>
        {
            public virtual object Execute(CommandContext commandContext)
            {
                //commandContext.getSession(typeof (PersistenceSession)).dbSchemaPrune();
                return null;
            }
        }
    }
}