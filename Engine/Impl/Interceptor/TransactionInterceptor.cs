using System;
using ESS.FW.Bpm.Engine.Impl.Cmd;
using System.Transactions;
using ESS.FW.Bpm.Model.Bpmn.instance;

namespace ESS.FW.Bpm.Engine.Impl.Interceptor
{
    /// <summary>
    ///     
    /// </summary>
    public class TransactionInterceptor : CommandInterceptor
    {
        private static readonly CommandLogger Log = ProcessEngineLogger.CmdLogger;
        private readonly bool _requiresNew;
        private TransactionScope _transactionScope;

        public TransactionInterceptor(bool requiresNew)
        {
            this._requiresNew = requiresNew;
        }

        private bool Existing
        {
            get
            {
                //try
                //{
                return _transactionScope != null;// != Status.STATUS_NO_TRANSACTION;
                //}
                //catch (SystemException e)
                //{
                //    throw new TransactionException("Unable to retrieve transaction status", e);
                //}
            }
        }

        public override T Execute<T>(ICommand<T> command)
        {
            var existing = Existing;
            var isNew = !existing || _requiresNew;

            if (isNew)
                DoBegin();
            T result;
            try
            {
                result = Next.Execute(command);
            }
            catch (System.Exception ex)
            {
                DoRollback(isNew);
                throw;
            }
            if (isNew)
                DoCommit();
            return result;

        }

        private void DoBegin()
        {
            try
            {
                var transOptions = new TransactionOptions
                {
                    IsolationLevel = IsolationLevel.ReadCommitted,
                    Timeout = new TimeSpan(0, 10, 0)
                };
                _transactionScope = new TransactionScope(TransactionScopeOption.Required,
                    transOptions,
                    EnterpriseServicesInteropOption.Automatic);
            }
            catch (NotSupportedException e)
            {
                throw new TransactionException("Unable to begin transaction", e);
            }
            catch (SystemException e)
            {
                throw new TransactionException("Unable to begin transaction", e);
            }
        }

        private void DoCommit()
        {
            try
            {
                _transactionScope.Complete();
                _transactionScope.Dispose();
                _transactionScope = null;
            }
            catch (TransactionException e)
            {
                throw new TransactionException("Unable to commit transaction", e);
            }
            catch (System.Exception)
            {
                DoRollback(true);
                throw;
            }
        }

        private void DoRollback(bool isNew)
        {
            try
            {
                if (isNew)
                {
                    _transactionScope.Dispose();
                    _transactionScope = null;
                }
            }
            catch (SystemException e)
            {
                Log.ExceptionWhileRollingBackTransaction(e);
            }
        }

    }
}