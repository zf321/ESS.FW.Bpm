using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.Shared.Entities.Bpm;

namespace ESS.FW.Bpm.Engine.Impl.DB.EntityManager.Operation
{
    /// <summary>
    ///     An operation on a single DbEntity
    ///     
    /// </summary>
    public class DbEntityOperation : DbOperation
    {
        /// <summary>
        ///     The entity the operation is performed on.
        /// </summary>
        protected internal IDbEntity entity;

        protected internal ISet<string> flushRelevantEntityReferences;

        public virtual IDbEntity Entity
        {
            get { return entity; }
            set
            {
                entityType = value.GetType();
                entity = value;
            }
        }

        private bool _failed = false;

        public override bool Failed => _failed;

        public void SetFailed(bool failed)
        {
            _failed = failed;
        }


        public virtual ISet<string> FlushRelevantEntityReferences
        {
            set { flushRelevantEntityReferences = value; }
            get { return flushRelevantEntityReferences; }
        }

        public override void Recycle()
        {
            entity = null;
            base.Recycle();
        }


        public override string ToString()
        {
            return operationType + " " + ClassNameUtil.GetClassNameWithoutPackage(entity) + "[" + entity.Id + "]";
        }

        public override int GetHashCode()
        {
            const int prime = 31;
            var result = 1;
            result = prime*result + (entity == null ? 0 : entity.GetHashCode());
            result = prime*result + (operationType == null ? 0 : operationType.GetHashCode());
            return result;
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;
            if (obj == null)
                return false;
            if (GetType() != obj.GetType())
                return false;
            var other = (DbEntityOperation) obj;
            if (entity == null)
            {
                if (other.entity != null)
                    return false;
            }
            else if (!entity.Equals(other.entity))
            {
                return false;
            }
            if (operationType != other.operationType)
                return false;
            return true;
        }
    }
}