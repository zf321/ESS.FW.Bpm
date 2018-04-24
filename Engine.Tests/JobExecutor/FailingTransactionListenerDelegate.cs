using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Interceptor;

namespace Engine.Tests.JobExecutor
{
    public class FailingTransactionListenerDelegate : IJavaDelegate
    {
        public void Execute(IBaseDelegateExecution execution)
        {
            Context.CommandContext.TransactionContext.AddTransactionListener(TransactionJavaStatus.Committing,
                new TransactionListenerAnonymousInnerClass());
        }

        private class TransactionListenerAnonymousInnerClass : ITransactionListener
        {            
            public void Execute(CommandContext context)
            {
                throw new System.Exception("exception in transaction listener");
            }
        }
    }
}