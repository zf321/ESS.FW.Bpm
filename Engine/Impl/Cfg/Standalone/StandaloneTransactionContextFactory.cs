using ESS.FW.Bpm.Engine.Impl.Interceptor;

namespace ESS.FW.Bpm.Engine.Impl.Cfg.Standalone
{
    /// <summary>
    ///
    /// </summary>
    public class StandaloneTransactionContextFactory : ITransactionContextFactory
    {
        public virtual ITransactionContext OpenTransactionContext(CommandContext commandContext)
        {
            return new StandaloneTransactionContext(commandContext);
        }
    }
}