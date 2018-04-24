using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.SendTask
{

    public class SendTaskTest : PluggableProcessEngineTestCase
    {

        [Test]
        [Deployment]
        public virtual void TestJavaDelegate()
        {
            DummyActivityBehavior.WasExecuted = true;
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("sendTaskJavaDelegate");

            AssertProcessEnded(processInstance.Id);
            Assert.True(DummyActivityBehavior.WasExecuted);
        }

        [Test]
        [Deployment]
        public virtual void TestActivityName()
        {
            DummyActivityBehavior.WasExecuted = false;

            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("process");

            runtimeService.Signal(processInstance.Id);

            AssertProcessEnded(processInstance.Id);

            Assert.True(DummyActivityBehavior.WasExecuted);

            Assert.NotNull(DummyActivityBehavior.CurrentActivityName);
            Assert.AreEqual("Task", DummyActivityBehavior.CurrentActivityName);

            Assert.NotNull(DummyActivityBehavior.CurrentActivityId);
            Assert.AreEqual("task", DummyActivityBehavior.CurrentActivityId);
        }

    }

}