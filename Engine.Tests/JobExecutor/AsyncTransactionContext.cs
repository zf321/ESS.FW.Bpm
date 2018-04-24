using System.Threading;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Cfg.Standalone;
using ESS.FW.Bpm.Engine.Impl.Interceptor;

namespace Engine.Tests.JobExecutor
{
    /// <summary>
    /// </summary>
    public class AsyncTransactionContext : StandaloneTransactionContext
    {
        public AsyncTransactionContext(CommandContext commandContext) : base(commandContext)
        {
        }
        
        protected internal virtual void FireTransactionEvent(TransactionJavaStatus transactionState)
        {
            var thread = new Thread(() => { FireTransactionEventAsync(transactionState); });

            thread.Start();
            //try
            //{
            thread.Join();
            //}
            //catch (InterruptedException e)
            //{
            //  throw new ProcessEngineException(e);
            //}
        }
        
        protected internal virtual void FireTransactionEventAsync(TransactionJavaStatus transactionState)
        {
            base.FireTransactionEvent(transactionState);
        }
    }
}