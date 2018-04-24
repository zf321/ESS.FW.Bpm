using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.Event.Timer
{
    [TestFixture]
    public class BoundaryTimerEventFullHistoryTest : ResourceProcessEngineTestCase
    {

        public BoundaryTimerEventFullHistoryTest() : base("resources/standalone/history/fullhistory.camunda.cfg.xml")
        {
        }

        [Test]
        [Deployment]
        public virtual void testSetProcessVariablesFromTaskWhenTimerOnTask()
        {
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("timerVariablesProcess");
            runtimeService.SetVariable(processInstance.Id, "myVar", 123456L);
        }

    }

}