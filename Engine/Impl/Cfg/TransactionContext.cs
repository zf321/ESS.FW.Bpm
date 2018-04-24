using System.Transactions;

namespace ESS.FW.Bpm.Engine.Impl.Cfg
{
    /// <summary>
    ///     The transaction context is an abstraction for different transaction management strategies
    ///     existing the Java Ecosystem. Provides transaction lifecycle management and management of transaction listeners.
    ///     Note: not every Technology or environment may provide a full implementation of this interface.
    ///     
    /// </summary>
    public interface ITransactionContext
    {
        bool IsTransactionActive { get; }

        /// <summary>
        ///     Commit the current transaction.
        /// </summary>
        void Commit();

        /// <summary>
        ///     Rollback the current transaction.
        /// </summary>
        void Rollback();


        /// <summary>
        ///     Add a <seealso cref="ITransactionListener" /> to the current transaction.
        /// </summary>
        /// <param name="transactionState">
        ///     the transaction state for which the <seealso cref="ITransactionListener" /> should be
        ///     added.
        /// </param>
        /// <param name="transactionListener"> the <seealso cref="ITransactionListener" /> to add. </param>
        void AddTransactionListener(TransactionJavaStatus transactionState, ITransactionListener transactionListener);
    }
}