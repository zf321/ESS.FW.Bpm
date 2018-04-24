using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.Event.Link
{
    [TestFixture]
    public class LinkEventTest : PluggableProcessEngineTestCase
    {
        
        [Test]
        [Deployment]
        public virtual void testValidEventLink()
        {
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("linkEventValid");

            IList<string> activeActivities = runtimeService.GetActiveActivityIds(pi.Id);
            // assert that now the first receive task is active
            Assert.AreEqual((new string[] { "waitAfterLink1" }), activeActivities);

            runtimeService.Signal(pi.Id);

            activeActivities = runtimeService.GetActiveActivityIds(pi.Id);
            // assert that now the second receive task is active
            Assert.AreEqual((new string[] { "waitAfterLink2" }), activeActivities);

            runtimeService.Signal(pi.Id);
            AssertProcessEnded(pi.Id);

            // validate history
            if (processEngineConfiguration.HistoryLevel.Id >= ProcessEngineConfigurationImpl.HistorylevelActivity)
            {
                IList<IHistoricActivityInstance> activities = historyService.CreateHistoricActivityInstanceQuery(c=>c.ProcessInstanceId == pi.Id).OrderBy(m=>m.ActivityId)/*.OrderByActivityId()*//*.Asc()*/.ToList();
                Assert.AreEqual(4, activities.Count);
                Assert.AreEqual("EndEvent_1", activities[0].ActivityId);
                Assert.AreEqual("StartEvent_1", activities[1].ActivityId);
                Assert.AreEqual("waitAfterLink1", activities[2].ActivityId);
                Assert.AreEqual("waitAfterLink2", activities[3].ActivityId);
            }

        }

        [Test]
        [Deployment]
        public virtual void testEventLinkMultipleSources()
        {
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("linkEventValid");
            IList<string> activeActivities = runtimeService.GetActiveActivityIds(pi.Id);

            // assert that the link event was triggered and that we are
            Assert.AreEqual((new string[] { "WaitAfterLink", "WaitAfterLink" }), activeActivities);

            runtimeService.DeleteProcessInstance(pi.Id, "test done");

            // validate history
            if (processEngineConfiguration.HistoryLevel.Id >= ProcessEngineConfigurationImpl.HistorylevelActivity)
            {
                IList<IHistoricActivityInstance> activities = historyService.CreateHistoricActivityInstanceQuery(c=>c.ProcessInstanceId == pi.Id).OrderBy(m=>m.ActivityId)/*.OrderByActivityId()*//*.Asc()*/.ToList();
                Assert.AreEqual(5, activities.Count);
                Assert.AreEqual("ManualTask_1", activities[0].ActivityId);
                Assert.AreEqual("ParallelGateway_1", activities[1].ActivityId);
                Assert.AreEqual("StartEvent_1", activities[2].ActivityId);
                Assert.AreEqual("WaitAfterLink", activities[3].ActivityId);
                Assert.AreEqual("WaitAfterLink", activities[4].ActivityId);
            }

        }
        [Test]
        public virtual void testInvalidEventLinkMultipleTargets()
        {
            try
            {
                repositoryService.CreateDeployment().AddClasspathResource("resources/bpmn/event/link/LinkEventTest.testInvalidEventLinkMultipleTargets.bpmn20.xml").Deploy();
                Assert.Fail("process should not deploy because it contains multiple event link targets which is invalid in the BPMN 2.0 spec");
            }
            catch (System.Exception ex)
            {
                Assert.True(ex.Message.Contains("Multiple Intermediate Catch Events with the same link event name ('LinkA') are not allowed"));
            }
        }
        [Test]
        public virtual void testCatchLinkEventAfterEventBasedGatewayNotAllowed()
        {
            try
            {
                repositoryService.CreateDeployment().AddClasspathResource("resources/bpmn/event/link/LinkEventTest.testCatchLinkEventAfterEventBasedGatewayNotAllowed.bpmn20.xml").Deploy();
                Assert.Fail("process should not deploy because it contains multiple event link targets which is invalid in the BPMN 2.0 spec");
            }
            catch (System.Exception ex)
            {
                Assert.True(ex.Message.Contains("IntermediateCatchLinkEvent is not allowed after an EventBasedGateway."));
            }
        }
    }

}