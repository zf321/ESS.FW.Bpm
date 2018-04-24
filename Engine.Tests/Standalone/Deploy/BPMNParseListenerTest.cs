using System.Linq;
using NUnit.Framework;

namespace Engine.Tests.Standalone.Deploy
{


    using ResourceProcessEngineTestCase = ResourceProcessEngineTestCase;


    /// <summary>
    /// 
    /// </summary>
    [TestFixture]
    public class BPMNParseListenerTest : ResourceProcessEngineTestCase
    {

        public BPMNParseListenerTest() : base("resources/standalone/deploy/bpmn.parse.listener.Camunda.cfg.xml")
        {
        }

        [Test]
        [Deployment]
        public virtual void TestAlterProcessDefinitionKeyWhenDeploying()
        {
            // Check if process-definition has different key
            Assert.AreEqual(0, repositoryService.CreateProcessDefinitionQuery(c => c.Key == "oneTaskProcess").Count());
            Assert.AreEqual(1, repositoryService.CreateProcessDefinitionQuery(c => c.Key == "oneTaskProcess-modified").Count());
        }
    }

}