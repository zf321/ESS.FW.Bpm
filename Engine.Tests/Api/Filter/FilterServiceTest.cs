using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Filter;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using NUnit.Framework;

namespace Engine.Tests.Api.Filter
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class FilterServiceTest : PluggableProcessEngineTestCase
    {
        protected internal IFilter filter;
        [SetUp]
        public virtual void setUp()
        {
            //filter = filterService.NewTaskFilter().SetName("name").SetOwner("owner").SetQuery(taskService.CreateTaskQuery()).SetProperties(new Dictionary<string, object>());
            Assert.IsNull(filter.Id);
            filterService.SaveFilter(filter);
            Assert.NotNull(filter.Id);
        }
        [TearDown]
        public virtual void tearDown()
        {
            // Delete all existing filters
            foreach (var filter in filterService.CreateTaskFilterQuery()
                
                .ToList())
                filterService.DeleteFilter(filter.Id);
        }

        [Test]
        public virtual void testCreateFilter()
        {
            Assert.NotNull(filter);

            var filter2 = filterService.GetFilter(filter.Id);
            Assert.NotNull(filter2);

            compareFilter(filter, filter2);
        }

        [Test]
        public virtual void testCreateInvalidFilter()
        {
            try
            {
                filter.SetName(null);
                Assert.Fail("Exception expected");
            }
            catch (ProcessEngineException)
            {
                // expected
            }

            try
            {
                filter.SetName("");
                Assert.Fail("Exception expected");
            }
            catch (ProcessEngineException)
            {
                // expected
            }

            try
            {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: filter.SetQuery((org.Camunda.bpm.Engine.query.Query<?, ?>) null);
                //filter.Query = (IQuery<object, object>) null;
                Assert.Fail("Exception expected");
            }
            catch (ProcessEngineException)
            {
                // expected
            }
        }

        [Test]
        public virtual void testUpdateFilter()
        {
            filter.SetName("newName");
            filter.SetOwner("newOwner");
            //filter.Query = taskService.CreateTaskQuery();
            filter.SetProperties(new Dictionary<string, object>());

            filterService.SaveFilter(filter);

            var filter2 = filterService.GetFilter(filter.Id);

            compareFilter(filter, filter2);
        }

        [Test]
        public virtual void testExtendFilter()
        {
            var extendingQuery = taskService.CreateTaskQuery(c=>c.Name =="newName")
               ;// .TaskOwner("newOwner");
            //IFilter newFilter = filter.Extend(extendingQuery);
            //Assert.IsNull(newFilter.Id);

            //TaskQueryImpl filterQuery = newFilter.Query;
            //Assert.AreEqual("newName", filterQuery.Name);
            //Assert.AreEqual("newOwner", filterQuery.Owner);
        }

        [Test]
        public virtual void testQueryFilter()
        {
            var filter2 = filterService.CreateTaskFilterQuery()
                //.FilterId(filter.Id)
                //.FilterName("name")
                //.FilterOwner("owner")
                .First();

            compareFilter(filter, filter2);

            filter2 = filterService.CreateTaskFilterQuery()
               // .FilterNameLike("%m%")
                .First();

            compareFilter(filter, filter2);
        }

        [Test]
        public virtual void testQueryUnknownFilter()
        {
            var unknownFilter = filterService.CreateTaskFilterQuery()
               // .FilterId("unknown")
                .First();

            Assert.IsNull(unknownFilter);

            unknownFilter = filterService.CreateTaskFilterQuery()
               // .FilterId(filter.Id)
               // .FilterName("invalid")
                .First();

            Assert.IsNull(unknownFilter);
        }

        [Test]
        public virtual void testDeleteFilter()
        {
            filterService.DeleteFilter(filter.Id);

            filter = filterService.GetFilter(filter.Id);
            Assert.IsNull(filter);
        }

        [Test]
        public virtual void testDeleteUnknownFilter()
        {
            filterService.DeleteFilter(filter.Id);
            var Count = filterService.CreateFilterQuery()
                .Count();
            Assert.AreEqual(0, Count);

            try
            {
                filterService.DeleteFilter(filter.Id);
                Assert.Fail("Exception expected");
            }
            catch (ProcessEngineException)
            {
                // expected
            }
        }
        public static void compareFilter(IFilter filter1, IFilter filter2)
        {
            Assert.NotNull(filter1);
            Assert.NotNull(filter2);
            Assert.AreEqual(filter1.Id, filter2.Id);
            Assert.AreEqual(filter1.ResourceType, filter2.ResourceType);
            Assert.AreEqual(filter1.Name, filter2.Name);
            Assert.AreEqual(filter1.Owner, filter2.Owner);
            Assert.AreEqual(((FilterEntity) filter1).QueryInternal, ((FilterEntity) filter2).QueryInternal);
            Assert.AreEqual(filter1.Properties, filter2.Properties);
        }
    }
}