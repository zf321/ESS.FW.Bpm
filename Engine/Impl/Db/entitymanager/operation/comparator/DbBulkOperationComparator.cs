using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.Impl.DB.EntityManager.Operation.Comparator
{
    /// <summary>
    ///     Orders bulk operations according to the lexicographical ordering of their statement names
    ///     
    /// </summary>
    public class DbBulkOperationComparator : IComparer<DbBulkOperation>
    {
        public virtual int Compare(DbBulkOperation firstOperation, DbBulkOperation secondOperation)
        {
            if (firstOperation.Equals(secondOperation))
                return 0;

            // order by statement
            var statementOrder = firstOperation.Statement.CompareTo(secondOperation.Statement);

            if (statementOrder == 0)
                return firstOperation.GetHashCode() < secondOperation.GetHashCode() ? -1 : 1;
            return statementOrder;
        }
    }
}