using NUnit.Framework;

namespace Engine.Tests.Standalone.Scripting
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class ScriptBeanAccessTest : ResourceProcessEngineTestCase
    {
        public ScriptBeanAccessTest() : base("resources/standalone/scripting/camunda.cfg.xml")
        {
        }

        [Test]
        [Deployment]
        public virtual void TestConfigurationBeanAccess()
        {
            var pi = runtimeService.StartProcessInstanceByKey("ScriptBeanAccess");
            Assert.AreEqual("myValue", runtimeService.GetVariable(pi.Id, "myVariable"));
        }
    }
}