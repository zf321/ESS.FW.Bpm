//using System.Collections.Generic;
//using ESS.FW.Bpm.Engine.Impl.Cfg;
//using ESS.FW.Bpm.Engine.Impl.DB;
//using ESS.FW.Bpm.Engine.Impl.DB.EntityManager;
//using ESS.FW.Bpm.Engine.Impl.DB.EntityManager.Operation;
//using ESS.FW.Bpm.Engine.Persistence.Entity;
//using NUnit.Framework;

//namespace ESS.FW.Bpm.Engine.Tests.Standalone.Db.EntityManager
//{
//    /// <summary>
//    /// </summary>
//    [TestFixture]
//    public class DbOperationsOrderingTest
//    {
//        protected internal ExposingDbEntityManager EntityManager;

//        // setup some entities
//        internal ExecutionEntity Execution1;
//        internal ExecutionEntity Execution2;
//        internal ExecutionEntity Execution3;
//        internal ExecutionEntity Execution4;
//        internal ExecutionEntity Execution5;
//        internal ExecutionEntity Execution6;
//        internal ExecutionEntity Execution7;
//        internal ExecutionEntity Execution8;

//        internal TaskEntity Task1;
//        internal TaskEntity Task2;
//        internal TaskEntity Task3;
//        internal TaskEntity Task4;

//        internal VariableInstanceEntity Variable1;
//        internal VariableInstanceEntity Variable2;
//        internal VariableInstanceEntity Variable3;
//        internal VariableInstanceEntity Variable4;


//        [SetUp]
//        public virtual void Setup()
//        {
//            var idGenerator = new TestIdGenerator();
//            EntityManager = new ExposingDbEntityManager(idGenerator, null);

//            Execution1 = new ExecutionEntity();
//            Execution1.Id = "101";
//            Execution2 = new ExecutionEntity();
//            Execution2.Id = "102";
//            Execution3 = new ExecutionEntity();
//            Execution3.Id = "103";
//            Execution4 = new ExecutionEntity();
//            Execution4.Id = "104";
//            Execution5 = new ExecutionEntity();
//            Execution5.Id = "105";
//            Execution6 = new ExecutionEntity();
//            Execution6.Id = "106";
//            Execution7 = new ExecutionEntity();
//            Execution7.Id = "107";
//            Execution8 = new ExecutionEntity();
//            Execution8.Id = "108";

//            Task1 = new TaskEntity();
//            Task1.Id = "104";
//            Task2 = new TaskEntity();
//            Task2.Id = "105";
//            Task3 = new TaskEntity();
//            Task3.Id = "106";
//            Task4 = new TaskEntity();
//            Task4.Id = "107";

//            Variable1 = new VariableInstanceEntity();
//            Variable1.Id = "108";
//            Variable2 = new VariableInstanceEntity();
//            Variable2.Id = "109";
//            Variable3 = new VariableInstanceEntity();
//            Variable3.Id = "110";
//            Variable4 = new VariableInstanceEntity();
//            Variable4.Id = "111";
//        }

//        protected internal virtual void AssertHappensAfter(IDbEntity entity1, IDbEntity entity2,
//            IList<DbOperation> operations)
//        {
//            var idx1 = IndexOfEntity(entity1, operations);
//            var idx2 = IndexOfEntity(entity2, operations);
//            Assert.True(idx1 > idx2, "operation for " + entity1 + " should be executed after operation for " + entity2);
//        }

//        protected internal virtual void AssertHappensBefore(IDbEntity entity1, IDbEntity entity2,
//            IList<DbOperation> operations)
//        {
//            var idx1 = IndexOfEntity(entity1, operations);
//            var idx2 = IndexOfEntity(entity2, operations);
//            Assert.True(idx1 < idx2, "operation for " + entity1 + " should be executed before operation for " + entity2);
//        }

//        protected internal virtual int IndexOfEntity(IDbEntity entity, IList<DbOperation> operations)
//        {
//            for (var i = 0; i < operations.Count; i++)
//                if (entity == ((DbEntityOperation) operations[i]).Entity)
//                    return i;
//            return -1;
//        }

//        public class ExposingDbEntityManager : DbEntityManager
//        {
//            public ExposingDbEntityManager(IDGenerator idGenerator, IPersistenceSession persistenceSession)
//                : base(idGenerator, persistenceSession)
//            {
//            }

//            /// <summary>
//            ///     Expose this method for test purposes
//            /// </summary>
//            public virtual void FlushEntityCache()
//            {
//                base.FlushEntityCache();
//            }
//        }

//        [Test]
//        public virtual void TestDeleteReferenceOrdering()
//        {
//            // given
//            Execution1.ParentExecution = Execution2;
//            EntityManager.DbEntityCache.PutPersistent(Execution1);
//            EntityManager.DbEntityCache.PutPersistent(Execution2);

//            // when deleting the entities
//            EntityManager.Delete(Execution1);
//            EntityManager.Delete(Execution2);

//            EntityManager.FlushEntityCache();

//            // then the flush is based on the persistent relationships
//            var deleteOperations = EntityManager.DbOperationManager.CalculateFlush();
//            AssertHappensBefore(Execution1, Execution2, deleteOperations);
//        }

//        [Test]
//        public virtual void TestDeleteReferenceOrderingAfterTransientUpdate()
//        {
//            // given
//            Execution1.ParentExecution = Execution2;
//            EntityManager.DbEntityCache.PutPersistent(Execution1);
//            EntityManager.DbEntityCache.PutPersistent(Execution2);

//            // when reverting the relation in memory
//            Execution1.ParentExecution = null;
//            Execution2.ParentExecution = Execution1;

//            // and deleting the entities
//            EntityManager.Delete(Execution1);
//            EntityManager.Delete(Execution2);

//            EntityManager.FlushEntityCache();

//            // then the flush is based on the persistent relationships
//            var deleteOperations = EntityManager.DbOperationManager.CalculateFlush();
//            AssertHappensBefore(Execution1, Execution2, deleteOperations);
//        }

//        [Test]
//        public virtual void TestInsertIdOrdering()
//        {
//            EntityManager.Insert(Execution1);
//            EntityManager.Insert(Execution2);

//            EntityManager.FlushEntityCache();
//            var insertOperations = EntityManager.DbOperationManager.CalculateFlush();
//            AssertHappensAfter(Execution2, Execution1, insertOperations);
//        }

//        [Test]
//        public virtual void TestInsertReferenceOrdering()
//        {
//            Execution2.ParentExecution = Execution3;

//            EntityManager.Insert(Execution2);
//            EntityManager.Insert(Execution3);

//            // the parent (3) is inserted before the child (2)
//            EntityManager.FlushEntityCache();
//            var flush = EntityManager.DbOperationManager.CalculateFlush();
//            AssertHappensAfter(Execution2, Execution3, flush);
//        }


//        [Test]
//        public virtual void TestInsertReferenceOrderingAndIdOrdering()
//        {
//            Execution2.ParentExecution = Execution3;

//            EntityManager.Insert(Execution2);
//            EntityManager.Insert(Execution3);
//            EntityManager.Insert(Execution1);

//            // the parent (3) is inserted before the child (2)
//            EntityManager.FlushEntityCache();
//            var flush = EntityManager.DbOperationManager.CalculateFlush();
//            AssertHappensAfter(Execution2, Execution3, flush);
//            AssertHappensAfter(Execution3, Execution1, flush);
//            AssertHappensAfter(Execution2, Execution1, flush);
//        }

//        [Test]
//        public virtual void TestInsertReferenceOrderingMultipleTrees()
//        {
//            // tree1
//            Execution3.ParentExecution = Execution4;
//            Execution2.ParentExecution = Execution4;
//            Execution5.ParentExecution = Execution3;

//            // tree2
//            Execution1.ParentExecution = Execution8;

//            EntityManager.Insert(Execution8);
//            EntityManager.Insert(Execution6);
//            EntityManager.Insert(Execution2);
//            EntityManager.Insert(Execution5);
//            EntityManager.Insert(Execution1);
//            EntityManager.Insert(Execution4);
//            EntityManager.Insert(Execution7);
//            EntityManager.Insert(Execution3);

//            // the parent (3) is inserted before the child (2)
//            EntityManager.FlushEntityCache();
//            var insertOperations = EntityManager.DbOperationManager.CalculateFlush();
//            AssertHappensAfter(Execution3, Execution4, insertOperations);
//            AssertHappensAfter(Execution2, Execution4, insertOperations);
//            AssertHappensAfter(Execution5, Execution3, insertOperations);
//            AssertHappensAfter(Execution1, Execution8, insertOperations);
//        }

//        [Test]
//        public virtual void TestInsertSingleEntity()
//        {
//            EntityManager.Insert(Execution1);
//            EntityManager.FlushEntityCache();

//            var flush = EntityManager.DbOperationManager.CalculateFlush();
//            Assert.AreEqual(1, flush.Count);
//        }
//    }
//}