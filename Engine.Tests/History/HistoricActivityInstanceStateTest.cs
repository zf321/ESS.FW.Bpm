using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Impl.Pvm.Runtime;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.History
{
    /// <summary>
    /// </summary>
    [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryAudit)]
    [TestFixture]
    public class HistoricActivityInstanceStateTest : PluggableProcessEngineTestCase
    {
        [Test]
        [Deployment(new[] { "resources/history/HistoricActivityInstanceStateTest.TestCancelProcessInstanceInUserTask.bpmn", "resources/history/HistoricActivityInstanceStateTest.TestEndTerminateEventWithCallActivity.bpmn" })]
        public virtual void testEndTerminateEventCancelWithCallActivity()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("process1");
            runtimeService.CorrelateMessage("continue");
            AssertProcessEnded(processInstance.Id);

            var allInstances = AllActivityInstances;

            AssertIsCanceledActivityInstances(allInstances, "callActivity", 1);
            AssertNonCompletingActivityInstance(allInstances, "callActivity");

            AssertIsCanceledActivityInstances(allInstances, "userTask", 1);
            AssertNonCompletingActivityInstance(allInstances, "userTask");

            AssertNonCanceledActivityInstance(allInstances, "terminateEnd");
            AssertIsCompletingActivityInstances(allInstances, "terminateEnd", 1);
        }

        private void AssertIsCanceledActivityInstances(IList<IHistoricActivityInstance> allInstances, string activityId)
        {
            AssertIsCanceledActivityInstances(allInstances, activityId, -1);
        }

        private void AssertIsCanceledActivityInstances(IList<IHistoricActivityInstance> allInstances, string activityId,
            int Count)
        {
            AssertCorrectCanceledState(allInstances, activityId, Count, true);
        }

        private void AssertNonCanceledActivityInstance(IList<IHistoricActivityInstance> instances, string activityId)
        {
            AssertNonCanceledActivityInstance(instances, activityId, -1);
        }

        private void AssertNonCanceledActivityInstance(IList<IHistoricActivityInstance> instances, string activityId,
            int Count)
        {
            AssertCorrectCanceledState(instances, activityId, Count, false);
        }

        private void AssertCorrectCanceledState(IList<IHistoricActivityInstance> allInstances, string activityId,
            int expectedCount, bool canceled)
        {
            var found = 0;

            foreach (var instance in allInstances)
                if (instance.ActivityId == activityId)
                {
                    found++;
                    Assert.AreEqual(canceled, instance.Canceled,
                        string.Format("expect <{0}> to be {1}canceled", activityId, canceled ? "" : "non-"));
                }

            Assert.True(found > 0, "contains entry for activity <" + activityId + ">");

            if (expectedCount != -1)
                Assert.True(found == expectedCount,
                    "contains <" + expectedCount + "> entries for activity <" + activityId + ">");
        }

        private void AssertIsCompletingActivityInstances(IList<IHistoricActivityInstance> allInstances,
            string activityId)
        {
            AssertIsCompletingActivityInstances(allInstances, activityId, -1);
        }

        private void AssertIsCompletingActivityInstances(IList<IHistoricActivityInstance> allInstances,
            string activityId, int Count)
        {
            AssertCorrectCompletingState(allInstances, activityId, Count, true);
        }

        private void AssertNonCompletingActivityInstance(IList<IHistoricActivityInstance> instances, string activityId)
        {
            AssertNonCompletingActivityInstance(instances, activityId, -1);
        }

        private void AssertNonCompletingActivityInstance(IList<IHistoricActivityInstance> instances, string activityId,
            int Count)
        {
            AssertCorrectCompletingState(instances, activityId, Count, false);
        }

        private void AssertCorrectCompletingState(IList<IHistoricActivityInstance> allInstances, string activityId,
            int expectedCount, bool completing)
        {
            var found = 0;

            foreach (var instance in allInstances)
                if (instance.ActivityId == activityId)
                {
                    found++;
                    Assert.AreEqual(completing, instance.CompleteScope,
                        string.Format("expect <{0}> to be {1}completing", activityId, completing ? "" : "non-"));
                }

            Assert.True(found > 0, "contains entry for activity <" + activityId + ">");

            if (expectedCount != -1)
                Assert.True(found == expectedCount,
                    "contains <" + expectedCount + "> entries for activity <" + activityId + ">");
        }

        private IList<IHistoricActivityInstance> EndActivityInstances
        {
            get
            {
                return historyService.CreateHistoricActivityInstanceQuery()
                    /*.OrderByHistoricActivityInstanceEndTime()*/
                    /*.Asc()*/
                    /*.CompleteScope()*/
                    .OrderBy(m=>m.EndTime)
                    .Where(m=>m.ActivityInstanceState== ActivityInstanceStateFields.ScopeComplete.StateCode)
                    .ToList();
            }
        }

        private IList<IHistoricActivityInstance> AllActivityInstances
        {
            get
            {
                return historyService.CreateHistoricActivityInstanceQuery().OrderBy(m=>m.StartTime)
                    //.OrderByHistoricActivityInstanceStartTime()
                    /*.Asc()*/

                    .ToList();
            }
        }

        private IProcessInstance startProcess()
        {
            return runtimeService.StartProcessInstanceByKey("process");
        }

        [Test]
        [Deployment]
        public virtual void testBoundaryErrorCancel()
        {
            var processInstance = startProcess();
            runtimeService.CorrelateMessage("continue");
            AssertProcessEnded(processInstance.Id);


            var allInstances = AllActivityInstances;

            AssertNonCanceledActivityInstance(allInstances, "start");
            AssertNonCompletingActivityInstance(allInstances, "start");

            AssertNonCanceledActivityInstance(allInstances, "subprocessStart");
            AssertNonCompletingActivityInstance(allInstances, "subprocessStart");

            AssertNonCanceledActivityInstance(allInstances, "gtw");
            AssertNonCompletingActivityInstance(allInstances, "gtw");

            AssertIsCanceledActivityInstances(allInstances, "subprocess", 1);
            AssertNonCompletingActivityInstance(allInstances, "subprocess");

            AssertIsCanceledActivityInstances(allInstances, "errorSubprocessEnd", 1);
            AssertNonCompletingActivityInstance(allInstances, "errorSubprocessEnd");

            AssertIsCanceledActivityInstances(allInstances, "userTask", 1);
            AssertNonCompletingActivityInstance(allInstances, "userTask");

            AssertNonCanceledActivityInstance(allInstances, "subprocessBoundary");
            AssertNonCompletingActivityInstance(allInstances, "subprocessBoundary");

            AssertNonCanceledActivityInstance(allInstances, "endAfterBoundary");
            AssertIsCompletingActivityInstances(allInstances, "endAfterBoundary", 1);
        }

        [Test]
        [Deployment]
        public virtual void testBoundarySignalCancel()
        {
            var processInstance = startProcess();

            // should wait in IUser task
            Assert.IsFalse(processInstance.IsEnded);

            // signal sub process
            runtimeService.SignalEventReceived("interrupt");

            var allInstances = AllActivityInstances;

            AssertNonCompletingActivityInstance(allInstances, "subprocess");
            AssertIsCanceledActivityInstances(allInstances, "subprocess", 1);

            AssertIsCanceledActivityInstances(allInstances, "userTask", 1);
            AssertNonCompletingActivityInstance(allInstances, "userTask");

            AssertNonCanceledActivityInstance(allInstances, "subprocessBoundary");
            AssertNonCompletingActivityInstance(allInstances, "subprocessBoundary");

            AssertNonCanceledActivityInstance(allInstances, "endAfterBoundary");
            AssertIsCompletingActivityInstances(allInstances, "endAfterBoundary", 1);
        }

        [Test]
        [Deployment]
        public virtual void testCancelProcessInstanceInSubprocess()
        {
            var processInstance = startProcess();

            runtimeService.DeleteProcessInstance(processInstance.Id, "test");

            var allInstances = AllActivityInstances;

            AssertIsCanceledActivityInstances(allInstances, "userTask", 1);
            AssertNonCompletingActivityInstance(allInstances, "userTask");

            AssertIsCanceledActivityInstances(allInstances, "subprocess", 1);
            AssertNonCompletingActivityInstance(allInstances, "subprocess");
        }

        [Test]
        [Deployment]
        public virtual void testCancelProcessInstanceInUserTask()
        {
            var processInstance = startProcess();

            runtimeService.DeleteProcessInstance(processInstance.Id, "test");

            var allInstances = AllActivityInstances;

            AssertIsCanceledActivityInstances(allInstances, "userTask", 1);
            AssertNonCompletingActivityInstance(allInstances, "userTask");
        }

        [Test]
        [Deployment]
        public virtual void testCancelProcessWithParallelGateway()
        {
            var processInstance = startProcess();

            runtimeService.DeleteProcessInstance(processInstance.Id, "test");

            var allInstances = AllActivityInstances;

            AssertIsCanceledActivityInstances(allInstances, "userTask1", 1);
            AssertNonCompletingActivityInstance(allInstances, "userTask1");

            AssertIsCanceledActivityInstances(allInstances, "userTask2", 1);
            AssertNonCompletingActivityInstance(allInstances, "userTask2");

            AssertIsCanceledActivityInstances(allInstances, "subprocess", 1);
            AssertNonCompletingActivityInstance(allInstances, "subprocess");
        }

        [Test]
        [Deployment]
        public virtual void testEndParallelJoin()
        {
            startProcess();

            var allInstances = AllActivityInstances;

            AssertNonCompletingActivityInstance(allInstances, "task1", 1);
            AssertNonCanceledActivityInstance(allInstances, "task1");

            AssertNonCompletingActivityInstance(allInstances, "task2", 1);
            AssertNonCanceledActivityInstance(allInstances, "task2");

            AssertIsCompletingActivityInstances(allInstances, "parallelJoinEnd", 2);
            AssertNonCanceledActivityInstance(allInstances, "parallelJoinEnd");
        }

        [Test]
        [Deployment]
        public virtual void testEndTerminateEventCancel()
        {
            var processInstance = startProcess();
            runtimeService.CorrelateMessage("continue");
            AssertProcessEnded(processInstance.Id);

            var allInstances = AllActivityInstances;

            AssertIsCanceledActivityInstances(allInstances, "userTask", 1);
            AssertNonCompletingActivityInstance(allInstances, "userTask");

            AssertNonCanceledActivityInstance(allInstances, "terminateEnd");
            AssertIsCompletingActivityInstances(allInstances, "terminateEnd", 1);
        }

        [Test]
        [Deployment]
        public virtual void testEndTerminateEventCancelInSubprocess()
        {
            var processInstance = startProcess();
            runtimeService.CorrelateMessage("continue");
            AssertProcessEnded(processInstance.Id);

            var allInstances = AllActivityInstances;

            AssertNonCompletingActivityInstance(allInstances, "subprocess");
            AssertNonCanceledActivityInstance(allInstances, "subprocess");

            AssertIsCanceledActivityInstances(allInstances, "userTask", 1);
            AssertNonCompletingActivityInstance(allInstances, "userTask");

            AssertNonCanceledActivityInstance(allInstances, "terminateEnd");
            AssertIsCompletingActivityInstances(allInstances, "terminateEnd", 1);

            AssertIsCompletingActivityInstances(allInstances, "end", 1);
            AssertNonCanceledActivityInstance(allInstances, "end");
        }

        [Test]
        [Deployment]
        public virtual void testEndTerminateEventCancelWithSubprocess()
        {
            var processInstance = startProcess();
            runtimeService.CorrelateMessage("continue");
            AssertProcessEnded(processInstance.Id);

            var allInstances = AllActivityInstances;

            AssertIsCanceledActivityInstances(allInstances, "subprocess", 1);
            AssertNonCompletingActivityInstance(allInstances, "subprocess");

            AssertIsCanceledActivityInstances(allInstances, "userTask", 1);
            AssertNonCompletingActivityInstance(allInstances, "userTask");

            AssertNonCanceledActivityInstance(allInstances, "terminateEnd");
            AssertIsCompletingActivityInstances(allInstances, "terminateEnd", 1);
        }

        [Test]
        [Deployment]
        public virtual void testEventSubprocessErrorCancel()
        {
            var processInstance = startProcess();
            runtimeService.CorrelateMessage("continue");
            AssertProcessEnded(processInstance.Id);

            var allInstances = AllActivityInstances;

            AssertIsCanceledActivityInstances(allInstances, "userTask", 1);
            AssertNonCompletingActivityInstance(allInstances, "userTask");

            AssertIsCanceledActivityInstances(allInstances, "errorEnd", 1);
            AssertNonCompletingActivityInstance(allInstances, "errorEnd");

            AssertNonCanceledActivityInstance(allInstances, "eventSubprocessStart");
            AssertNonCompletingActivityInstance(allInstances, "eventSubprocessStart");

            AssertNonCanceledActivityInstance(allInstances, "eventSubprocessEnd");
            AssertIsCompletingActivityInstances(allInstances, "eventSubprocessEnd", 1);
        }

        [Test]
        [Deployment]
        public virtual void testEventSubprocessMessageCancel()
        {
            startProcess();

            runtimeService.CorrelateMessage("message");

            Assert.IsNull(runtimeService.CreateProcessInstanceQuery()
                .FirstOrDefault());

            var allInstances = AllActivityInstances;

            AssertIsCanceledActivityInstances(allInstances, "userTask", 1);
            AssertNonCompletingActivityInstance(allInstances, "userTask");

            AssertNonCanceledActivityInstance(allInstances, "eventSubprocessStart");
            AssertNonCompletingActivityInstance(allInstances, "eventSubprocessStart");

            AssertNonCanceledActivityInstance(allInstances, "eventSubprocessEnd");
            AssertIsCompletingActivityInstances(allInstances, "eventSubprocessEnd", 1);
        }

        [Test]
        [Deployment]
        public virtual void testEventSubprocessSignalCancel()
        {
            var processInstance = startProcess();
            runtimeService.CorrelateMessage("continue");
            AssertProcessEnded(processInstance.Id);

            var allInstances = AllActivityInstances;

            AssertIsCanceledActivityInstances(allInstances, "userTask", 1);
            AssertNonCompletingActivityInstance(allInstances, "userTask");

            // fails due to CAM-4527: end execution listeners are executed twice for the signal end event
            //    AssertIsCanceledActivityInstances(allInstances, "signalEnd", 1);
            //    AssertNonCompletingActivityInstance(allInstances, "signalEnd");

            AssertNonCanceledActivityInstance(allInstances, "eventSubprocessStart");
            AssertNonCompletingActivityInstance(allInstances, "eventSubprocessStart");

            AssertNonCanceledActivityInstance(allInstances, "eventSubprocessEnd");
            AssertIsCompletingActivityInstances(allInstances, "eventSubprocessEnd", 1);
        }

        [Test]
        [Deployment]
        public virtual void testIntermediateTask()
        {
            startProcess();

            var allInstances = AllActivityInstances;

            AssertNonCompletingActivityInstance(allInstances, "intermediateTask", 1);
            AssertNonCanceledActivityInstance(allInstances, "intermediateTask");

            AssertIsCompletingActivityInstances(allInstances, "end", 1);
            AssertNonCanceledActivityInstance(allInstances, "end");
        }

        [Test]
        [Deployment]
        public virtual void testParallelMultiInstanceSubProcess()
        {
            startProcess();

            var activityInstances = EndActivityInstances;

            Assert.AreEqual(7, activityInstances.Count);

            var allInstances = AllActivityInstances;

            AssertIsCompletingActivityInstances(allInstances, "intermediateSubprocess", 3);
            AssertNonCanceledActivityInstance(allInstances, "intermediateSubprocess");

            AssertIsCompletingActivityInstances(allInstances, "subprocessEnd", 3);
            AssertNonCanceledActivityInstance(allInstances, "subprocessEnd");

            AssertNonCompletingActivityInstance(allInstances, "intermediateSubprocess#multiInstanceBody", 1);
            AssertNonCanceledActivityInstance(allInstances, "intermediateSubprocess#multiInstanceBody");

            AssertIsCompletingActivityInstances(allInstances, "end", 1);
            AssertNonCanceledActivityInstance(allInstances, "end");
        }

        [Test]
        [Deployment]
        public virtual void testSequentialMultiInstanceSubProcess()
        {
            startProcess();

            var activityInstances = EndActivityInstances;

            Assert.AreEqual(7, activityInstances.Count);

            var allInstances = AllActivityInstances;

            AssertIsCompletingActivityInstances(allInstances, "intermediateSubprocess", 3);
            AssertNonCanceledActivityInstance(allInstances, "intermediateSubprocess");

            AssertIsCompletingActivityInstances(allInstances, "subprocessEnd", 3);
            AssertNonCanceledActivityInstance(allInstances, "subprocessEnd");

            AssertNonCompletingActivityInstance(allInstances, "intermediateSubprocess#multiInstanceBody", 1);
            AssertNonCanceledActivityInstance(allInstances, "intermediateSubprocess#multiInstanceBody");

            AssertIsCompletingActivityInstances(allInstances, "end", 1);
            AssertNonCanceledActivityInstance(allInstances, "end");
        }

        [Test]
        [Deployment]
        public virtual void testSimpleSubProcess()
        {
            startProcess();

            var allInstances = AllActivityInstances;

            AssertNonCompletingActivityInstance(allInstances, "intermediateSubprocess", 1);
            AssertNonCanceledActivityInstance(allInstances, "intermediateSubprocess");

            AssertIsCompletingActivityInstances(allInstances, "subprocessEnd", 1);
            AssertNonCanceledActivityInstance(allInstances, "subprocessEnd");

            AssertIsCompletingActivityInstances(allInstances, "end", 1);
            AssertNonCanceledActivityInstance(allInstances, "end");
        }

        [Test]
        [Deployment]
        public virtual void testSingleEndActivity()
        {
            startProcess();

            var allInstances = AllActivityInstances;

            AssertNonCompletingActivityInstance(allInstances, "start", 1);
            AssertNonCanceledActivityInstance(allInstances, "start");

            AssertIsCompletingActivityInstances(allInstances, "end", 1);
            AssertNonCanceledActivityInstance(allInstances, "end");
        }

        [Test]
        [Deployment]
        public virtual void testSingleEndEvent()
        {
            startProcess();

            var allInstances = AllActivityInstances;

            AssertNonCompletingActivityInstance(allInstances, "start", 1);
            AssertNonCanceledActivityInstance(allInstances, "start");

            AssertIsCompletingActivityInstances(allInstances, "end", 1);
            AssertNonCanceledActivityInstance(allInstances, "end");
        }

        [Test]
        [Deployment]
        public virtual void testSingleEndEventAfterParallelJoin()
        {
            startProcess();

            var allInstances = AllActivityInstances;

            AssertNonCompletingActivityInstance(allInstances, "parallelJoin", 2);
            AssertNonCanceledActivityInstance(allInstances, "parallelJoin");

            AssertIsCompletingActivityInstances(allInstances, "end", 1);
            AssertNonCanceledActivityInstance(allInstances, "end");
        }

        [Test]
        [Deployment]
        public virtual void testSingleEndEventAndSingleEndActivity()
        {
            startProcess();

            var allInstances = AllActivityInstances;

            AssertNonCompletingActivityInstance(allInstances, "parallelSplit", 1);
            AssertNonCanceledActivityInstance(allInstances, "parallelSplit");

            AssertIsCompletingActivityInstances(allInstances, "end1");
            AssertNonCanceledActivityInstance(allInstances, "end1");

            AssertIsCompletingActivityInstances(allInstances, "end2");
            AssertNonCanceledActivityInstance(allInstances, "end2");
        }

        [Test]
        [Deployment]
        public virtual void testTwoEndActivities()
        {
            startProcess();

            var allInstances = AllActivityInstances;

            AssertNonCompletingActivityInstance(allInstances, "parallelSplit", 1);
            AssertNonCanceledActivityInstance(allInstances, "parallelSplit");

            AssertIsCompletingActivityInstances(allInstances, "end1", 1);
            AssertNonCanceledActivityInstance(allInstances, "end1");

            AssertIsCompletingActivityInstances(allInstances, "end2", 1);
            AssertNonCanceledActivityInstance(allInstances, "end2");
        }

        [Test]
        [Deployment]
        public virtual void testTwoEndEvents()
        {
            startProcess();

            var allInstances = AllActivityInstances;

            AssertNonCompletingActivityInstance(allInstances, "parallelSplit", 1);
            AssertNonCanceledActivityInstance(allInstances, "parallelSplit", 1);

            AssertIsCompletingActivityInstances(allInstances, "end1", 1);
            AssertNonCanceledActivityInstance(allInstances, "end1");

            AssertIsCompletingActivityInstances(allInstances, "end2", 1);
            AssertNonCanceledActivityInstance(allInstances, "end2");
        }
    }
}