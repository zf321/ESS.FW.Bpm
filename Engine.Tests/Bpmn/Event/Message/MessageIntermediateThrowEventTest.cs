using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.Event.Message
{
    [TestFixture]
    public class MessageIntermediateThrowEventTest : PluggableProcessEngineTestCase
    {

        [Test]
        [Deployment]
        public virtual void testSingleIntermediateThrowMessageEvent()
        {

            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("process");
            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment]
        public virtual void testSingleIntermediateThrowMessageEventServiceTaskBehavior()
        {

            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("process");

            AssertProcessEnded(processInstance.Id);
            Assert.True(DummyServiceTask.wasExecuted);
        }

    }

}