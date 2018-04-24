using NUnit.Framework;

namespace Engine.Tests.Api.Mgmt
{
    /// <summary>
    /// </summary>
    public class ManagementServiceTableCountTest : PluggableProcessEngineTestCase
    {
        [Test]
        public virtual void testTableCount()
        {
            var tableCount = managementService.TableCount;

            var tablePrefix = processEngineConfiguration.DatabaseTablePrefix;

            //commenting out this Assertion as there is no much sense to check the quantity of records, not the presence/absence of specific ones
            //when additional row was added within CAM-7539, the test started failing when testing old engine (7.6) with new database (7.7)
            //Assert.AreEqual(new Long(5), tableCount.Get(tablePrefix + "ACT_GE_PROPERTY"));

            Assert.AreEqual(new long?(0), tableCount[tablePrefix + "ACT_GE_BYTEARRAY"]);
            Assert.AreEqual(new long?(0), tableCount[tablePrefix + "ACT_RE_DEPLOYMENT"]);
            Assert.AreEqual(new long?(0), tableCount[tablePrefix + "ACT_RU_EXECUTION"]);
            Assert.AreEqual(new long?(0), tableCount[tablePrefix + "ACT_ID_GROUP"]);
            Assert.AreEqual(new long?(0), tableCount[tablePrefix + "ACT_ID_MEMBERSHIP"]);
            Assert.AreEqual(new long?(0), tableCount[tablePrefix + "ACT_ID_USER"]);
            Assert.AreEqual(new long?(0), tableCount[tablePrefix + "ACT_RE_PROCDEF"]);
            Assert.AreEqual(new long?(0), tableCount[tablePrefix + "ACT_RU_TASK"]);
            Assert.AreEqual(new long?(0), tableCount[tablePrefix + "ACT_RU_IDENTITYLINK"]);
        }
    }
}