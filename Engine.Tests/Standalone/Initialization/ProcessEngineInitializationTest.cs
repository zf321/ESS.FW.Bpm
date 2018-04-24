using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using NUnit.Framework;

namespace Engine.Tests.Standalone.Initialization
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class ProcessEngineInitializationTest : PvmTestCase
    {
        [Test]
        public virtual void TestCustomDefaultRetries()
        {
            var configuration =
                ProcessEngineConfiguration.CreateProcessEngineConfigurationFromResource(
                    "resources/standalone/initialization/customdefaultretries.Camunda.cfg.xml");

            Assert.AreEqual(5, configuration.DefaultNumberOfRetries);
        }

        [Test]
        public virtual void TestDefaultRetries()
        {
            var configuration =
                ProcessEngineConfiguration.CreateProcessEngineConfigurationFromResource(
                    "resources/standalone/initialization/defaultretries.Camunda.cfg.xml");

            Assert.AreEqual(JobEntity.DefaultRetries, configuration.DefaultNumberOfRetries);
        }

        [Test]
        public virtual void TestNoTables()
        {
            try
            {
                ProcessEngineConfiguration.CreateProcessEngineConfigurationFromResource(
                    "resources/standalone/initialization/notables.Camunda.cfg.xml").BuildProcessEngine();
                Assert.Fail("expected exception");
            }
            catch (System.Exception e)
            {
                // OK
                AssertTextPresent(
                    "ENGINE-03057 There are no Camunda tables in the database. Hint: Set <property name=\"databaseSchemaUpdate\" to value=\"true\" or value=\"create-drop\" (use create-drop for testing only!) in bean processEngineConfiguration in camunda.cfg.xml for automatic schema creation",
                    e.Message);
            }
        }
    }
}