using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Impl.Javax.EL;
using NUnit.Framework;

namespace Engine.Tests.Standalone.EL
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class ExpressionBeanAccessTest : ResourceProcessEngineTestCase
    {
        public ExpressionBeanAccessTest() : base("resources/standalone/el/camunda.cfg.xml")
        {
        }

        [Test]
        [Deployment]
        public virtual void TestConfigurationBeanAccess()
        {
            // Exposed bean returns 'I'm exposed' when to-string is called in first service-task
            var pi = runtimeService.StartProcessInstanceByKey("expressionBeanAccess");
            Assert.AreEqual("I'm exposed", runtimeService.GetVariable(pi.Id, "exposedBeanResult"));

            // After signaling, an expression tries to use a bean that is present in the configuration but
            // is not added to the beans-list
            try
            {
                runtimeService.Signal(pi.Id);
                Assert.Fail("Exception expected");
            }
            catch (ProcessEngineException ae)
            {
                Assert.NotNull(ae.InnerException);
                Assert.True(ae.InnerException is PropertyNotFoundException);
            }
        }
    }
}