using ESS.Shared.Entities.Bpm;
using System;
using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.Impl.DB.EntityManager.Cache
{
    /// <summary>
    ///     A cached entity
    ///     
    /// </summary>
    public class CachedDbEntity : IRecyclable
    {
        protected internal object Copy;

        protected internal IDbEntity DbEntity;

        protected internal DbEntityState entityState;

        /// <summary>
        ///     Ids of referenced entities of the same entity type
        /// </summary>
        protected internal ISet<string> flushRelevantEntityReferences;

        /// <summary>
        ///     Allows checking whether this entity is dirty.
        /// </summary>
        /// <returns> true if the entity is dirty (state has changed since it was put into the cache) </returns>
        public virtual bool Dirty
        {
            get { return !DbEntity.GetPersistentState().Equals(Copy); }
        }

        public virtual ISet<string> FlushRelevantEntityReferences
        {
            get { return flushRelevantEntityReferences; }
        }

        // getters / setters ////////////////////////////

        public virtual IDbEntity Entity
        {
            get { return DbEntity; }
            set { DbEntity = value; }
        }


        public virtual DbEntityState EntityState
        {
            get { return entityState; }
            set { entityState = value; }
        }


        public virtual Type EntityType
        {
            get { return DbEntity.GetType(); }
        }

        public virtual void Recycle()
        {
            // clean out state
            DbEntity = null;
            Copy = null;
            //entityState = DbEntityState.;null
        }

        public virtual void ForceSetDirty()
        {
            // set the value of the Copy to some value which will always be different from the new entity state.
            Copy = -1;
        }

        public virtual void MakeCopy()
        {
            Copy = DbEntity.GetPersistentState();
        }

        public override string ToString()
        {
            return entityState + " " + DbEntity.GetType().Name + "[" + DbEntity.Id + "]";
        }

        public virtual void DetermineEntityReferences()
        {
            if (DbEntity is IHasDbReferences)
                flushRelevantEntityReferences = ((IHasDbReferences) DbEntity).ReferencedEntityIds;
            else
                flushRelevantEntityReferences = null;
        }

        public virtual bool AreFlushRelevantReferencesDetermined()
        {
            return flushRelevantEntityReferences != null;
        }
    }
}