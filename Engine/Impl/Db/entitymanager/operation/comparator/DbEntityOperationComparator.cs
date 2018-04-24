using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.Impl.DB.EntityManager.Operation.Comparator
{
    /// <summary>
    ///     
    /// </summary>
    public class DbEntityOperationComparator : IComparer<DbEntityOperation>
    {
        public virtual int Compare(DbEntityOperation firstOperation, DbEntityOperation secondOperation)
        {
            if (firstOperation.Equals(secondOperation))
                return 0;

            var firstEntity = firstOperation.Entity;
            var secondEntity = secondOperation.Entity;

            return firstEntity.Id.CompareTo(secondEntity.Id);
        }
    }
}