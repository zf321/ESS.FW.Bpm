using ESS.FW.Bpm.Engine;
using NUnit.Framework;

namespace Engine.Tests.Standalone.Initialization
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class NoDbConnectionTest : PvmTestCase
    {
        private bool ContainsSqlException(System.Exception e)
        {
            if (e == null)
                return false;
            //if (e is SQLException)
            //{
            //  return true;
            //}
            return ContainsSqlException(e.InnerException);
        }

        [Test]
        public virtual void TestNoDbConnection()
        {
            try
            {
                ProcessEngineConfiguration.CreateProcessEngineConfigurationFromResource(
                    "resources/standalone/initialization/nodbconnection.Camunda.cfg.xml").BuildProcessEngine();
                Assert.Fail("expected exception");
            }
            catch (System.Exception e)
            {
                Assert.True(ContainsSqlException(e));
            }
        }
    }
}