using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Filter;
using NUnit.Framework;

namespace Engine.Tests.Api.Filter
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class FilterPropertiesTest : PluggableProcessEngineTestCase
    {
        protected internal IFilter filter;
        protected internal string nestedJsonObject = "{\"id\":\"nested\"}";
        protected internal string nestedJsonArray = "[\"a\",\"b\"]";

        [SetUp]
        public virtual void setUp()
        {
            filter = filterService.NewTaskFilter("name")
                .SetOwner("owner")
                .SetProperties(new Dictionary<string, object>());
        }
        [TearDown]
        protected internal virtual void tearDown()
        {
            if (filter.Id != null)
                filterService.DeleteFilter(filter.Id);
        }


        [Test]
        public virtual void testPropertiesFromNull()
        {
            filter.SetProperties(null);
            Assert.IsNull(filter.Properties);

            filter.SetProperties(null);
            Assert.IsNull(filter.Properties);
        }

        [Test]
        public virtual void testPropertiesFromMap()
        {
            IDictionary<string, object> properties = new Dictionary<string, object>();
            properties["color"] = "#123456";
            properties["priority"] = 42;
            properties["userDefined"] = true;
            properties["object"] = nestedJsonObject;
            properties["array"] = nestedJsonArray;
            filter.SetProperties(properties);

            AssertTestProperties();
        }

        protected internal virtual void AssertTestProperties()
        {
            filterService.SaveFilter(filter);
            filter = filterService.GetFilter(filter.Id);

            var properties = filter.Properties;
            Assert.AreEqual(5, properties.Count);
            Assert.AreEqual("#123456", properties["color"]);
            Assert.AreEqual(42, properties["priority"]);
            Assert.AreEqual(true, properties["userDefined"]);
            Assert.AreEqual(nestedJsonObject, properties["object"]);
            Assert.AreEqual(nestedJsonArray, properties["array"]);
        }

        [Test]
        public virtual void testNullProperty()
        {
            // given
            IDictionary<string, object> properties = new Dictionary<string, object>();
            properties["null"] = null;
            filter.SetProperties(properties);
            filterService.SaveFilter(filter);

            // when
            filter = filterService.GetFilter(filter.Id);

            // then
            var persistentProperties = filter.Properties;
            Assert.AreEqual(1, persistentProperties.Count);
            Assert.True(persistentProperties.ContainsKey("null"));
            Assert.IsNull(persistentProperties["null"]);
        }
    }
}