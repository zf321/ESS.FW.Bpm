using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Cfg.Standalone;
using ESS.FW.Bpm.Engine.Impl.Interceptor;

namespace Engine.Tests.JobExecutor
{
    /// <summary>
    /// </summary>
    public class AsyncTransactionContextFactory : StandaloneTransactionContextFactory
    {
        public override ITransactionContext OpenTransactionContext(CommandContext commandContext)
        {
            return new AsyncTransactionContext(commandContext);
        }
    }
}