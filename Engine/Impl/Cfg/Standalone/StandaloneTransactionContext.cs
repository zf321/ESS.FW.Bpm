using System;
using System.Collections.Generic;
using System.Transactions;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Impl.DB;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.DataAccess;

namespace ESS.FW.Bpm.Engine.Impl.Cfg.Standalone
{
    /// <summary>
    ///     
    /// </summary>
    public class StandaloneTransactionContext : ITransactionContext
    {
        private static readonly TransactionLogger Log = ProcessEngineLogger.TxLogger;

        protected internal CommandContext CommandContext;
        private TransactionJavaStatus _lastTransactionState;
        protected internal IDictionary<TransactionJavaStatus, IList<ITransactionListener>> StateTransactionListeners;

        public StandaloneTransactionContext(CommandContext commandContext)
        {
            this.CommandContext = commandContext;
        }

        protected internal virtual TransactionJavaStatus LastTransactionState
        {
            set { _lastTransactionState = value; }
        }

        //6.5
        public virtual void AddTransactionListener(TransactionJavaStatus transactionState,
            ITransactionListener transactionListener)
        {
            if (StateTransactionListeners == null)
                StateTransactionListeners = new Dictionary<TransactionJavaStatus, IList<ITransactionListener>>();
            var transactionListeners = StateTransactionListeners.ContainsKey(transactionState) ? StateTransactionListeners[transactionState] : null;
            if (transactionListeners == null)
            {
                transactionListeners = new List<ITransactionListener>();
                StateTransactionListeners[transactionState] = transactionListeners;
            }
            transactionListeners.Add(transactionListener);
        }

        public virtual void Commit()
        {
            Log.DebugTransactionOperation("firing event committing...");

            FireTransactionEvent(TransactionJavaStatus.Committing);

            Log.DebugTransactionOperation("committing the persistence session...");
            //清空缓存
            //CommandContext.DbEntityCache.Clear();
            Log.LogDebug("StandaloneTransactionContext数据库保存----------UnitOfWork.Commit()", null);
            CommandContext.DbContext.SaveChanges();
            //PersistenceProvider.Commit();

            Log.DebugTransactionOperation("firing event committed...");

            FireTransactionEvent(TransactionJavaStatus.Committed);
        }

        public virtual void Rollback()
        {
            try
            {
                try
                {
                    Log.DebugTransactionOperation("firing event rollback...");
                    FireTransactionEvent(TransactionJavaStatus.RollingBack);
                }
                catch (System.Exception exception)
                {
                    Log.ExceptionWhileFiringEvent(TransactionJavaStatus.RollingBack, exception);
                    Context.CommandInvocationContext.TrySetThrowable(exception);
                }
                finally
                {
                    Log.DebugTransactionOperation("rolling back the persistence session...");
                    //PersistenceProvider.Rollback();
                    CommandContext.DbContext.Dispose();
                }
            }
            catch (System.Exception exception)
            {
                Log.ExceptionWhileFiringEvent(TransactionJavaStatus.RollingBack, exception);
                Context.CommandInvocationContext.TrySetThrowable(exception);
            }
            finally
            {
                Log.DebugFiringEventRolledBack();
                FireTransactionEvent(TransactionJavaStatus.RolledBack);
            }
        }

        public virtual bool IsTransactionActive
        {
            get
            {
                return TransactionJavaStatus.RollingBack != _lastTransactionState && TransactionJavaStatus.RolledBack != _lastTransactionState;
            }
        }

        protected internal virtual void FireTransactionEvent(TransactionJavaStatus transactionState)
        {
            LastTransactionState = transactionState;
            if (StateTransactionListeners == null)
                return;
            var transactionListeners = StateTransactionListeners.ContainsKey(transactionState) ? StateTransactionListeners[transactionState] : null;
            if (transactionListeners == null)
                return;
            foreach (var transactionListener in transactionListeners)
                transactionListener.Execute(CommandContext);
        }
    }
}