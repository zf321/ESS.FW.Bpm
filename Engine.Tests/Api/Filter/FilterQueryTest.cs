//using System.Collections.Generic;
//using System.Linq;
//using ESS.FW.Bpm.Engine.Filter;
//using ESS.FW.Bpm.Engine.Persistence.Entity;
//using NUnit.Framework;

//namespace ESS.FW.Bpm.Engine.Tests.Api.Filter
//{
//    /// <summary>
//    /// </summary>
//    [TestFixture]
//    public class FilterQueryTest : PluggableProcessEngineTestCase
//    {
//        protected internal IList<string> filterIds = new List<string>();

//        [SetUp]
//        public virtual void setUp()
//        {
//            saveFilter("b", "b");
//            saveFilter("d", "d");
//            saveFilter("a", "a");
//            saveFilter("c_", "c");
//        }

//        protected internal virtual void saveFilter(string name, string owner)
//        {
//            var filter = filterService.NewTaskFilter()
//                .SetName(name)
//                .SetOwner(owner);
//            filterService.SaveFilter(filter);
//            filterIds.Add(filter.Id);
//        }
//        [TearDown]
//        public virtual void tearDown()
//        {
//            // Delete all filters
//            foreach (var filter in filterService.CreateFilterQuery()
                
//                .ToList())
//                filterService.DeleteFilter(filter.Id);
//        }

//        [Test]
//        public virtual void testQueryNoCriteria()
//        {
//            var query = filterService.CreateFilterQuery();
//            Assert.AreEqual(4, query.Count());
//            Assert.AreEqual(4, query
//                .Count());
//            try
//            {
//                query.First();
//                Assert.Fail("Exception expected");
//            }
//            catch (ProcessEngineException)
//            {
//                // expected
//            }
//        }

//        [Test]
//        public virtual void testQueryByFilterId()
//        {
//            var query = filterService.CreateFilterQuery()
//                .FilterId(filterIds[0]);
//            Assert.NotNull(query.First());
//            Assert.AreEqual(1, query
//                .Count());
//            Assert.AreEqual(1, query.Count());
//        }

//        [Test]
//        public virtual void testQueryByInvalidFilterId()
//        {
//            var query = filterService.CreateFilterQuery()
//                .FilterId("invalid");
//            Assert.IsNull(query.First());
//            Assert.AreEqual(0, query
//                .Count());
//            Assert.AreEqual(0, query.Count());

//            try
//            {
//                filterService.CreateFilterQuery()
//                    .FilterId(null);
//                Assert.Fail("Exception expected");
//            }
//            catch (ProcessEngineException)
//            {
//                // expected
//            }
//        }

//        [Test]
//        public virtual void testQueryByResourceType()
//        {
//            var query = filterService.CreateFilterQuery()
//                .FilterResourceType(EntityTypes.Task);
//            try
//            {
//                query.First();
//                Assert.Fail("Exception expected");
//            }
//            catch (ProcessEngineException)
//            {
//                // expected
//            }
//            Assert.AreEqual(4, query
//                .Count());
//            Assert.AreEqual(4, query.Count());
//        }

//        [Test]
//        public virtual void testQueryByInvalidResourceType()
//        {
//            var query = filterService.CreateFilterQuery()
//                .FilterResourceType("invalid");
//            Assert.IsNull(query.First());
//            Assert.AreEqual(0, query
//                .Count());
//            Assert.AreEqual(0, query.Count());

//            try
//            {
//                filterService.CreateFilterQuery()
//                    .FilterResourceType(null);
//                Assert.Fail("Exception expected");
//            }
//            catch (ProcessEngineException)
//            {
//                // expected
//            }
//        }

//        [Test]
//        public virtual void testQueryByName()
//        {
//            var query = filterService.CreateFilterQuery()
//                .FilterName("a");
//            Assert.NotNull(query.First());
//            Assert.AreEqual(1, query
//                .Count());
//            Assert.AreEqual(1, query.Count());
//        }

//        [Test]
//        public virtual void testQueryByNameLike()
//        {
//            var query = filterService.CreateFilterQuery()
//                .FilterNameLike("%\\_");
//            Assert.NotNull(query.First());
//            Assert.AreEqual(1, query
//                .Count());
//            Assert.AreEqual(1, query.Count());
//        }

//        [Test]
//        public virtual void testQueryByInvalidName()
//        {
//            var query = filterService.CreateFilterQuery()
//                .FilterName("invalid");
//            Assert.IsNull(query.First());
//            Assert.AreEqual(0, query
//                .Count());
//            Assert.AreEqual(0, query.Count());

//            try
//            {
//                filterService.CreateFilterQuery()
//                    .FilterName(null);
//                Assert.Fail("Exception expected");
//            }
//            catch (ProcessEngineException)
//            {
//                // expected
//            }
//        }

//        [Test]
//        public virtual void testQueryByOwner()
//        {
//            var query = filterService.CreateFilterQuery()
//                .FilterOwner("a");
//            Assert.NotNull(query.First());
//            Assert.AreEqual(1, query
//                .Count());
//            Assert.AreEqual(1, query.Count());
//        }

//        [Test]
//        public virtual void testQueryByInvalidOwner()
//        {
//            var query = filterService.CreateFilterQuery()
//                .FilterOwner("invalid");
//            Assert.IsNull(query.First());
//            Assert.AreEqual(0, query
//                .Count());
//            Assert.AreEqual(0, query.Count());

//            try
//            {
//                filterService.CreateFilterQuery()
//                    .FilterOwner(null);
//                Assert.Fail("Exception expected");
//            }
//            catch (ProcessEngineException)
//            {
//                // expected
//            }
//        }

//        [Test]
//        public virtual void testQueryPaging()
//        {
//            var query = filterService.CreateFilterQuery();

//            Assert.AreEqual(4, query.ListPage(0, int.MaxValue)
//                .Count());

//            // Verifying the un-paged results
//            Assert.AreEqual(4, query.Count());
//            Assert.AreEqual(4, query
//                .Count());

//            // Verifying paged results
//            Assert.AreEqual(2, query/*.ListPage(0, 2)*/
//                .Count());
//            Assert.AreEqual(2, query.ListPage(2, 2)
//                .Count());
//            Assert.AreEqual(1, query.ListPage(3, 1)
//                .Count());

//            // Verifying odd usages
//            Assert.AreEqual(0, query.ListPage(-1, -1)
//                .Count());
//            Assert.AreEqual(0, query.ListPage(4, 2)
//                .Count()); // 4 is the last index with a result
//            Assert.AreEqual(4, query.ListPage(0, 15)
//                .Count()); // there are only 4 tasks
//        }

//        [Test]
//        public virtual void testQuerySorting()
//        {
//            IList<string> sortedIds = new List<string>(filterIds);
//            //sortedIds.Sort();
//            //Assert.AreEqual(4, filterService.CreateFilterQuery().OrderByFilterId()/*.Asc()*/.Count());
//            //Assert.That(filterService.CreateFilterQuery().OrderByFilterId()/*.Asc()*/, contains(hasProperty("id", equalTo(sortedIds[0])), hasProperty("id", equalTo(sortedIds[1])), hasProperty("id", equalTo(sortedIds[2])), hasProperty("id", equalTo(sortedIds[3]))));

//            //Assert.AreEqual(4, filterService.CreateFilterQuery().OrderByFilterResourceType()/*.Asc()*/.Count());
//            //Assert.That(filterService.CreateFilterQuery().OrderByFilterResourceType()/*.Asc()*/, contains(hasProperty("resourceType", equalTo(EntityTypes.Resources.Task)), hasProperty("resourceType", equalTo(EntityTypes.Resources.Task)), hasProperty("resourceType", equalTo(EntityTypes.Resources.Task)), hasProperty("resourceType", equalTo(EntityTypes.Resources.Task))));

//            //Assert.AreEqual(4, filterService.CreateFilterQuery().OrderByFilterName()/*.Asc()*/.Count());
//            //Assert.That(filterService.CreateFilterQuery().OrderByFilterName()/*.Asc()*/, contains(hasProperty("name", equalTo("a")), hasProperty("name", equalTo("b")), hasProperty("name", equalTo("c_")), hasProperty("name", equalTo("d"))));

//            //Assert.AreEqual(4, filterService.CreateFilterQuery().OrderByFilterOwner()/*.Asc()*/.Count());
//            //Assert.That(filterService.CreateFilterQuery().OrderByFilterOwner()/*.Asc()*/, contains(hasProperty("owner", equalTo("a")), hasProperty("owner", equalTo("b")), hasProperty("owner", equalTo("c")), hasProperty("owner", equalTo("d"))));

//            //Assert.AreEqual(4, filterService.CreateFilterQuery().OrderByFilterId()/*.Desc()*/.Count());
//            //Assert.That(filterService.CreateFilterQuery().OrderByFilterId()/*.Desc()*/, contains(hasProperty("id", equalTo(sortedIds[3])), hasProperty("id", equalTo(sortedIds[2])), hasProperty("id", equalTo(sortedIds[1])), hasProperty("id", equalTo(sortedIds[0]))));

//            //Assert.AreEqual(4, filterService.CreateFilterQuery().OrderByFilterResourceType()/*.Desc()*/.Count());
//            //Assert.That(filterService.CreateFilterQuery().OrderByFilterResourceType()/*.Desc()*/, contains(hasProperty("resourceType", equalTo(EntityTypes.Resources.Task)), hasProperty("resourceType", equalTo(EntityTypes.Resources.Task)), hasProperty("resourceType", equalTo(EntityTypes.Resources.Task)), hasProperty("resourceType", equalTo(EntityTypes.Resources.Task))));

//            //Assert.AreEqual(4, filterService.CreateFilterQuery().OrderByFilterName()/*.Desc()*/.Count());
//            //Assert.That(filterService.CreateFilterQuery().OrderByFilterName()/*.Desc()*/, contains(hasProperty("name", equalTo("d")), hasProperty("name", equalTo("c_")), hasProperty("name", equalTo("b")), hasProperty("name", equalTo("a"))));

//            //Assert.AreEqual(4, filterService.CreateFilterQuery().OrderByFilterOwner()/*.Desc()*/.Count());
//            //Assert.That(filterService.CreateFilterQuery().OrderByFilterOwner()/*.Desc()*/, contains(hasProperty("name", equalTo("d")), hasProperty("name", equalTo("c_")), hasProperty("name", equalTo("b")), hasProperty("name", equalTo("a"))));

//            Assert.AreEqual(1, filterService.CreateFilterQuery()
//                .OrderByFilterId()
//                .FilterName("a")
//                /*.Asc()*/
                
//                .Count());
//            Assert.AreEqual(1, filterService.CreateFilterQuery()
//                .OrderByFilterId()
//                .FilterName("a")
//                /*.Desc()*/
                
//                .Count());
//        }

//        [Test]
//        public virtual void testNativeQuery()
//        {
//            var tablePrefix = processEngineConfiguration.DatabaseTablePrefix;
//            Assert.AreEqual(tablePrefix + "ACT_RU_FILTER", managementService.GetTableName(typeof(IFilter)));
//            Assert.AreEqual(tablePrefix + "ACT_RU_FILTER", managementService.GetTableName(typeof(FilterEntity)));
//            Assert.AreEqual(4, taskService.CreateNativeTaskQuery()
//                .Sql("SELECT * FROM " + managementService.GetTableName(typeof(IFilter)))
                
//                .Count);
//            Assert.AreEqual(4, taskService.CreateNativeTaskQuery()
//                .Sql("SELECT Count(*) FROM " + managementService.GetTableName(typeof(IFilter)))
//                .Count());

//            Assert.AreEqual(16, taskService.CreateNativeTaskQuery()
//                .Sql("SELECT Count(*) FROM " + tablePrefix + "ACT_RU_FILTER F1, " + tablePrefix + "ACT_RU_FILTER F2")
//                .Count());

//            // select with distinct
//            Assert.AreEqual(4, taskService.CreateNativeTaskQuery()
//                .Sql("SELECT F1.* FROM " + tablePrefix + "ACT_RU_FILTER F1")
                
//                .Count);

//            Assert.AreEqual(1, taskService.CreateNativeTaskQuery()
//                .Sql("SELECT Count(*) FROM " + managementService.GetTableName(typeof(IFilter)) +
//                     " F WHERE F.NAME_ = 'a'")
//                .Count());
//            Assert.AreEqual(1, taskService.CreateNativeTaskQuery()
//                .Sql("SELECT * FROM " + managementService.GetTableName(typeof(IFilter)) + " F WHERE F.NAME_ = 'a'")
                
//                .Count);

//            // use parameters
//            Assert.AreEqual(1, taskService.CreateNativeTaskQuery()
//                .Sql("SELECT Count(*) FROM " + managementService.GetTableName(typeof(IFilter)) +
//                     " F WHERE F.NAME_ = #{filterName}")
//                .Parameter("filterName", "a")
//                .Count());
//        }

//        [Test]
//        public virtual void testNativeQueryPaging()
//        {
//            var tablePrefix = processEngineConfiguration.DatabaseTablePrefix;
//            Assert.AreEqual(tablePrefix + "ACT_RU_FILTER", managementService.GetTableName(typeof(IFilter)));
//            Assert.AreEqual(tablePrefix + "ACT_RU_FILTER", managementService.GetTableName(typeof(FilterEntity)));
//            Assert.AreEqual(3, taskService.CreateNativeTaskQuery()
//                .Sql("SELECT * FROM " + managementService.GetTableName(typeof(IFilter)))
//                .ListPage(0, 3)
//                .Count);
//            Assert.AreEqual(2, taskService.CreateNativeTaskQuery()
//                .Sql("SELECT * FROM " + managementService.GetTableName(typeof(IFilter)))
//                .ListPage(2, 2)
//                .Count);
//        }
//    }
//}