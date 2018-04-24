using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.DB.EntityManager.Operation.Comparator;

namespace ESS.FW.Bpm.Engine.Impl.DB.EntityManager.Operation
{
//JAVA TO C# CONVERTER TODO ITask: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.db.entitymanager.Operation.DbOperationType.DELETE;
//JAVA TO C# CONVERTER TODO ITask: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.db.entitymanager.Operation.DbOperationType.Insert;

    /// <summary>
    ///     Manages a set of <seealso cref="DbOperation database operations" />.
    ///     
    /// </summary>
    public class DbOperationManager
    {
        // comparators ////////////////

        public static IComparer<Type> InsertTypeComparator = new EntityTypeComparatorForInserts();
        public static IComparer<Type> ModificationTypeComparator = new EntityTypeComparatorForModifications();
        public static IComparer<DbEntityOperation> InsertOperationComparator = new DbEntityOperationComparator();
        public static IComparer<DbEntityOperation> ModificationOperationComparator = new DbEntityOperationComparator();
        public static IComparer<DbBulkOperation> BulkOperationComparator = new DbBulkOperationComparator();

        /// <summary>
        ///     bulk modifications (DELETE, UPDATE) on an entity collection
        /// </summary>
        public SortedDictionary<Type, SortedSet<DbBulkOperation>> BulkOperations =
            new SortedDictionary<Type, SortedSet<DbBulkOperation>>(ModificationTypeComparator);

        /// <summary>
        ///     Deletes of a single entity
        /// </summary>
        public SortedDictionary<Type, SortedSet<DbEntityOperation>> Deletes =
            new SortedDictionary<Type, SortedSet<DbEntityOperation>>(ModificationTypeComparator);

        // pre-sorted operation maps //////////////

        /// <summary>
        ///     INSERTs
        /// </summary>
        public SortedDictionary<Type, SortedSet<DbEntityOperation>> Inserts =
            new SortedDictionary<Type, SortedSet<DbEntityOperation>>(InsertTypeComparator);

        /// <summary>
        ///     UPDATEs of a single entity
        /// </summary>
        public SortedDictionary<Type, SortedSet<DbEntityOperation>> Updates =
            new SortedDictionary<Type, SortedSet<DbEntityOperation>>(ModificationTypeComparator);

        public virtual bool AddOperation(DbEntityOperation newOperation)
        {
            if (newOperation.OperationType == DbOperationType.Insert)
                return GetInsertsForType(newOperation.EntityType, true).Add(newOperation);
            if (newOperation.OperationType == DbOperationType.Delete)
                return GetDeletesByType(newOperation.EntityType, true).Add(newOperation);
            // UPDATE
            return GetUpdatesByType(newOperation.EntityType, true).Add(newOperation);
        }

        protected internal virtual SortedSet<DbEntityOperation> GetDeletesByType(Type type, bool create)
        {
            var deletesByType = Deletes[type];
            if ((deletesByType == null) && create)
            {
                deletesByType = new SortedSet<DbEntityOperation>(ModificationOperationComparator);
                Deletes[type] = deletesByType;
            }
            return deletesByType;
        }

        protected internal virtual SortedSet<DbEntityOperation> GetUpdatesByType(Type type, bool create)
        {
            var updatesByType = Updates[type];
            if ((updatesByType == null) && create)
            {
                updatesByType = new SortedSet<DbEntityOperation>(ModificationOperationComparator);
                Updates[type] = updatesByType;
            }
            return updatesByType;
        }

        protected internal virtual SortedSet<DbEntityOperation> GetInsertsForType(Type type, bool create)
        {
            var insertsByType = Inserts[type];
            if ((insertsByType == null) && create)
            {
                insertsByType = new SortedSet<DbEntityOperation>(InsertOperationComparator);
                Inserts[type] = insertsByType;
            }
            return insertsByType;
        }

        public virtual bool AddOperation(DbBulkOperation newOperation)
        {
            var bulksByType = BulkOperations[newOperation.EntityType];
            if (bulksByType == null)
            {
                bulksByType = new SortedSet<DbBulkOperation>(BulkOperationComparator);
                BulkOperations[newOperation.EntityType] = bulksByType;
            }

            return bulksByType.Add(newOperation);
        }

        public virtual IList<DbOperation> CalculateFlush()
        {
            IList<DbOperation> flush = new List<DbOperation>();
            // first INSERTs
            AddSortedInserts(flush);
            // then UPDATEs + Deletes
            AddSortedModifications(flush);
            return flush;
        }

        /// <summary>
        ///     Adds the insert operations to the flush (in correct order).
        /// </summary>
        /// <param name="operationsForFlush">  </param>
        protected internal virtual void AddSortedInserts(IList<DbOperation> flush)
        {
            foreach (var operationsForType in Inserts)
                if (operationsForType.Key.IsSubclassOf(typeof(IHasDbReferences)))
                    ((List<DbOperation>) flush).AddRange(SortByReferences(operationsForType.Value));
                else
                    ((List<DbOperation>) flush).AddRange(operationsForType.Value);
        }

        /// <summary>
        ///     Adds a correctly ordered list of UPDATE and DELETE operations to the flush.
        /// </summary>
        /// <param name="flush">  </param>
        protected internal virtual void AddSortedModifications(IList<DbOperation> flush)
        {
            // calculate sorted set of all modified entity types
            var modifiedEntityTypes = new SortedSet<Type>(ModificationTypeComparator);
            modifiedEntityTypes.UnionWith(Updates.Keys);
            modifiedEntityTypes.UnionWith(Deletes.Keys);
            modifiedEntityTypes.UnionWith(BulkOperations.Keys);

            foreach (var type in modifiedEntityTypes)
            {
                // first perform entity UPDATES
                AddSortedModificationsForType(type, Updates[type], flush);
                // next perform entity Deletes
                AddSortedModificationsForType(type, Deletes[type], flush);
                // last perform bulk operations
                var bulkOperationsForType = BulkOperations[type];
                if (bulkOperationsForType != null)
                    ((List<DbOperation>) flush).AddRange(bulkOperationsForType);
            }
        }

        protected internal virtual void AddSortedModificationsForType(Type type,
            SortedSet<DbEntityOperation> preSortedOperations, IList<DbOperation> flush)
        {
            if (preSortedOperations != null)
                if (type.IsSubclassOf(typeof(IHasDbReferences)))
                    ((List<DbOperation>) flush).AddRange(SortByReferences(preSortedOperations));
                else
                    ((List<DbOperation>) flush).AddRange(preSortedOperations);
        }


        /// <summary>
        ///     Assumptions:
        ///     a) all operations in the set work on entities such that the entities implement <seealso cref="IHasDbReferences" />.
        ///     b) all operations in the set work on the same type (ie. all operations are INSERTs or Deletes).
        /// </summary>
        protected internal virtual IList<DbEntityOperation> SortByReferences(SortedSet<DbEntityOperation> preSorted)
        {
            // copy the pre-sorted set and apply final sorting to list
            IList<DbEntityOperation> opList = new List<DbEntityOperation>(preSorted);

            for (var i = 0; i < opList.Count; i++)
            {
                var currentOperation = opList[i];
                var currentEntity = currentOperation.Entity;
                var currentReferences = currentOperation.FlushRelevantEntityReferences;

                // check whether this operation must be placed after another operation
                var moveTo = i;
                for (var k = i + 1; k < opList.Count; k++)
                {
                    var otherOperation = opList[k];
                    var otherEntity = otherOperation.Entity;
                    var otherReferences = otherOperation.FlushRelevantEntityReferences;

                    if (currentOperation.OperationType == DbOperationType.Insert)
                    {
                        // if we reference the other entity, we need to be inserted after that entity
                        if ((currentReferences != null) && currentReferences.Contains(otherEntity.Id))
                        {
                            moveTo = k;
                            break; // we can only reference a single entity
                        }
                    }
                    else
                    {
                        // UPDATE or DELETE

                        // if the other entity has a reference to us, we must be placed after the other entity
                        if ((otherReferences != null) && otherReferences.Contains(currentEntity.Id))
                            moveTo = k;
                    }
                }

                if (moveTo > i)
                {
                    //opList.Remove(i);
                    opList.Insert(moveTo, currentOperation);
                    i--;
                }
            }

            return opList;
        }
    }
}