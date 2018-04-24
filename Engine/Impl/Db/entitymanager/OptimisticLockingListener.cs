using System;
using ESS.FW.Bpm.Engine.Impl.DB.EntityManager.Operation;

namespace ESS.FW.Bpm.Engine.Impl.DB.EntityManager
{
    /// <summary>
    ///     Allows registering a listener which is notified when an
    ///     <seealso cref="DbOperationType#UPDATE" /> or <seealso cref="DbOperationType#DELETE" />
    ///     could not be performed.
    ///     
    /// </summary>
    public interface IOptimisticLockingListener
    {
        /// <summary>
        ///     The type of the entity for which this listener should be notified.
        ///     If the implementation returns 'null', the listener is notified for all
        ///     entity types.
        /// </summary>
        /// <returns> the entity type for which the listener should be notified. </returns>
        Type EntityType { get; }

        /// <summary>
        ///     Signifies that an operation failed due to optimistic locking.
        /// </summary>
        /// <param name="operation"> the failed operation. </param>
        void FailedOperation(DbOperation operation);
    }
}