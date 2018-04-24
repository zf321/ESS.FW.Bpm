using System.Transactions;
using ESS.FW.Bpm.Engine.Impl.Interceptor;

namespace ESS.FW.Bpm.Engine.Impl.Cfg.Tx
{
    /// <summary>
    ///     
    /// </summary>
    public class TxTransactionContextFactory : ITransactionContextFactory
    {

        public virtual ITransactionContext OpenTransactionContext(CommandContext commandContext)
        {
            //return new TxTransactionContext(new TransactionScope());
            return new TxTransactionContext(new CommittableTransaction());
        }
    }
}