using System.Transactions;

namespace ESS.FW.Bpm.Engine.Impl.Cfg
{
    /// <summary>
    ///     
    /// </summary>
    public class TransactionLogger : ProcessEngineLogger
    {
        public virtual ProcessEngineException ExceptionWhileInteractingWithTransaction(string operation, System.Exception e)
        {
            throw new ProcessEngineException(ExceptionMessage("001", "{0} while {1}", e.GetType().Name, operation), e);
        }

        public virtual void DebugTransactionOperation(string @string)
        {
            LogDebug("002", @string);
        }

        public virtual void ExceptionWhileFiringEvent(TransactionJavaStatus state, System.Exception exception)
        {
            LogError("003", "Exception while firing event {1}: {2}", state, exception.Message, exception);
        }

        public virtual void DebugFiringEventRolledBack()
        {
            LogDebug("004", "Firing event rolled back");
        }
    }
}