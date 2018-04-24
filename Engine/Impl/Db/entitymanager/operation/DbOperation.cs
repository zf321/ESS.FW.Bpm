using System;

namespace ESS.FW.Bpm.Engine.Impl.DB.EntityManager.Operation
{
    /// <summary>
    ///     A database operation.
    ///     
    /// </summary>
    public abstract class DbOperation : IRecyclable
    {
        /// <summary>
        ///     The type of the DbEntity this operation is executed on.
        /// </summary>
        protected internal Type entityType;

        /// <summary>
        ///     The type of the operation.
        /// </summary>
        protected internal DbOperationType operationType;

        // getters / setters //////////////////////////////////////////

        public abstract bool Failed { get; }

        public virtual Type EntityType
        {
            get { return entityType; }
            set { entityType = value; }
        }


        public virtual DbOperationType OperationType
        {
            get { return operationType; }
            set { operationType = value; }
        }

        public virtual void Recycle()
        {
            // clean out the object state
            //operationType = null;
            entityType = null;
        }
    }
}