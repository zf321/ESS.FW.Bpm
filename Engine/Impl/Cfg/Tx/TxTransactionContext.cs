using System;
using System.Transactions;

namespace ESS.FW.Bpm.Engine.Impl.Cfg.Tx
{
    /// <summary>
    /// </summary>
    public class TxTransactionContext : ITransactionContext
    {

        public static readonly TransactionLogger Log = ProcessEngineLogger.TxLogger;

        //private readonly TransactionScope _transactionScope;
        private readonly CommittableTransaction _committableTransaction;

        //public TxTransactionContext(TransactionScope transactionScope)
        //{
        //    this._transactionScope = transactionScope;
        //}
        public TxTransactionContext(CommittableTransaction committableTransaction)
        {
            this._committableTransaction = committableTransaction;
        }

        public  void Commit()
        {
            Log.LogDebug("TxTransactionContext的Commit", "----------TxTransactionContext.Commit---------------");
            //_transactionScope.Complete();
            _committableTransaction.Commit();
            // managed transaction, ignore
        }

        public  void Rollback()
        {
            Log.LogDebug("TxTransactionContext的Rollback", "----------TxTransactionContext.Rollback---------------");
            // managed transaction, mark rollback-only if not done so already.
            try
            {
                //_transactionScope.Dispose();
                _committableTransaction.Rollback();
            }
            catch (System.Exception e)
            {
                throw Log.ExceptionWhileInteractingWithTransaction("setting transaction rollback only", e);
            }
        }

        protected internal virtual Transaction Transaction
        {
            get
            {
                try
                {
                    return _committableTransaction;
                }
                catch (System.Exception e)
                {
                    throw Log.ExceptionWhileInteractingWithTransaction("getting transaction", e);
                }
            }
        }
        
        public virtual void AddTransactionListener(TransactionJavaStatus transactionState, ITransactionListener transactionListener)
        {
            throw new NotImplementedException();
            //Transaction transaction = Transaction;
            //CommandContext commandContext = Context.CommandContext;
            //try
            //{
            //    transaction.RegisterSynchronization(new TransactionStateSynchronization(transactionState, transactionListener, commandContext));
            //}
            //catch (Exception e)
            //{
            //    throw Log.ExceptionWhileInteractingWithTransaction("registering synchronization", e);
            //}
        }

        //public class TransactionStateSynchronization //: Synchronization
        //{

        //    protected internal readonly ITransactionListener transactionListener;
        //    protected internal readonly TransactionStatus transactionState;
        //    internal readonly CommandContext commandContext;

        //    public TransactionStateSynchronization(TransactionStatus transactionState, ITransactionListener transactionListener, CommandContext commandContext)
        //    {
        //        this.transactionState = transactionState;
        //        this.transactionListener = transactionListener;
        //        this.commandContext = commandContext;
        //    }

        //    public virtual void BeforeCompletion()
        //    {
        //        if (TransactionStatus.COMMITTING.Equals(transactionState) || TransactionStatus.ROLLINGBACK.Equals(transactionState))
        //        {
        //            transactionListener.Execute(commandContext);
        //        }
        //    }

        //    public virtual void AfterCompletion(int status)
        //    {
        //        if (Status.STATUS_ROLLEDBACK == status && TransactionStatus.ROLLED_BACK.Equals(transactionState))
        //        {
        //            transactionListener.Execute(commandContext);
        //        }
        //        else if (Status.STATUS_COMMITTED == status && TransactionStatus.COMMITTED.Equals(transactionState))
        //        {
        //            transactionListener.Execute(commandContext);
        //        }
        //    }

        //}
        /// <summary>
        /// 判断当前Transaction是否环境事务Transaction.Current
        /// </summary>
        public virtual bool IsTransactionActive
        {
            get
            {
                try
                {
                    return Transaction.Current == Transaction;
                    //throw new NotImplementedException("TxTransactionContext没用到？");
                    //return Transaction.Current.TransactionInformation.Status != TransactionStatus.Active;
                }
                catch (SystemException e)
                {
                    throw Log.ExceptionWhileInteractingWithTransaction("getting transaction state", e);
                }
            }
        }
    }
}
