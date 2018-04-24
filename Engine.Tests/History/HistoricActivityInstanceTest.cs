using System;
using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.History.Impl.Event;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Pvm.Runtime;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.History
{
    /// <summary>
    ///   
    /// </summary>
    [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryAudit)]
    [TestFixture]
    public class HistoricActivityInstanceTest : PluggableProcessEngineTestCase
    {
        //[Test] 自定义异常
        public virtual void testHistoricActivityInstanceQueryByCompleteScopeAndCanceled()
        {
            try
            {
                historyService.CreateHistoricActivityInstanceQuery()
                    /*.CompleteScope()*/
                    //.Canceled()

                    .ToList();
                Assert.Fail("It should not be possible to query by completeScope and canceled.");
            }
            catch (ProcessEngineException)
            {
                // exception expected
            }
        }

        private IQueryable<IHistoricActivityInstance> startEventTestProcess(string message)
        {
            if (message.Equals(""))
                runtimeService.StartProcessInstanceByKey("testEvents");
            else
                runtimeService.StartProcessInstanceByMessage("CAM-2365");

            return historyService.CreateHistoricActivityInstanceQuery();
        }

        [Test]
        [Deployment]
        public virtual void testBoundaryCancelEvent()
        {
            var pi = runtimeService.StartProcessInstanceByKey("process");

            var query = historyService.CreateHistoricActivityInstanceQuery();

            query = query.Where(m => m.ActivityId == "catchCancel");
            Assert.AreEqual(1, query.Count());
            Assert.NotNull(query.First()
                .EndTime);
            Assert.AreEqual("cancelBoundaryCatch".ToLower(), query.First()
                .ActivityType);

            var task = taskService.CreateTaskQuery()
                .First();
            taskService.Complete(task.Id);

            AssertProcessEnded(pi.Id);
        }

        [Test]
        [Deployment]
        public virtual void testBoundaryCompensateEvent()
        {
            var pi = runtimeService.StartProcessInstanceByKey("process");

            var query = historyService.CreateHistoricActivityInstanceQuery();

            // the compensation boundary event should not appear in history!
            query = query.Where(m => m.ActivityId == "compensate");
            Assert.AreEqual(0, query.Count());

            AssertProcessEnded(pi.Id);
        }

        [Test]
        [Deployment]
        public virtual void testBoundaryErrorEvent()
        {
            var pi = runtimeService.StartProcessInstanceByKey("process");
            var query = historyService.CreateHistoricActivityInstanceQuery();
            query = query.Where(m => m.ActivityId == "error");
            Assert.AreEqual(1, query.Count());
            Assert.NotNull(query.First()
                .EndTime);
            Assert.AreEqual("boundaryError".ToLower(), query.First()
                .ActivityType);
            var task = taskService.CreateTaskQuery()
                .First();
            taskService.Complete(task.Id);
            AssertProcessEnded(pi.Id);
        }

        //[Test] //不启用 cmmn case
        [Deployment(
            new[]
            {
                "resources/history/HistoricActivityInstanceTest.TestCaseCallActivity.bpmn20.xml",
                "resources/api/cmmn/oneTaskCase.cmmn"
            })]
        public virtual void testCaseCallActivity()
        {
            runtimeService.StartProcessInstanceByKey("process");

            var subCaseInstanceId = caseService.CreateCaseInstanceQuery()
                .First()
                .Id;


            var historicCallActivity = historyService.CreateHistoricActivityInstanceQuery()
                .Where(m => m.ActivityId == "callActivity")
                .First();

            Assert.AreEqual(subCaseInstanceId, historicCallActivity.CalledCaseInstanceId);
            Assert.IsNull(historicCallActivity.EndTime);

            var humanTaskId = caseService.CreateCaseExecutionQuery()
                .Where(m => m.ActivityId == "PI_HumanTask_1")
                .First()
                .Id;

            caseService.CompleteCaseExecution(humanTaskId);

            historicCallActivity = historyService.CreateHistoricActivityInstanceQuery()
                .Where(m => m.ActivityId == "callActivity")
                .First();

            Assert.AreEqual(subCaseInstanceId, historicCallActivity.CalledCaseInstanceId);
            Assert.NotNull(historicCallActivity.EndTime);
        }

        [Test]
        [Deployment("resources/history/HistoricActivityInstanceTest.TestBoundaryCompensateEvent.bpmn20.xml")]
        public virtual void testCompensationServiceTaskHasEndTime()
        {
            var pi = runtimeService.StartProcessInstanceByKey("process");
            var query = historyService.CreateHistoricActivityInstanceQuery();
            query = query.Where(m => m.ActivityId == "compensationServiceTask");
            Assert.AreEqual(1, query.Count());
            Assert.NotNull(query.First()
                .EndTime);
            AssertProcessEnded(pi.Id);
        }

        //[Test]//.net不支持groovy语法,
        [Deployment("resources/history/HistoricActivityInstanceTest.TestEvents.bpmn")]
        public virtual void testEndEventTypes()
        {
            var query = startEventTestProcess("");

            query = query.Where(m => m.ActivityId == "cancellationEndEvent");
            Assert.AreEqual(1, query.Count());
            Assert.AreEqual("cancelEndEvent", query.First()
                .ActivityType);

            query.Where(m => m.ActivityId == "messageEndEvent");
            Assert.AreEqual(1, query.Count());
            Assert.AreEqual("messageEndEvent", query.First()
                .ActivityType);

            query.Where(m => m.ActivityId == "errorEndEvent");
            Assert.AreEqual(1, query.Count());
            Assert.AreEqual("errorEndEvent", query.First()
                .ActivityType);

            query.Where(m => m.ActivityId == "signalEndEvent");
            Assert.AreEqual(1, query.Count());
            Assert.AreEqual("signalEndEvent", query.First()
                .ActivityType);

            query.Where(m => m.ActivityId == "terminationEndEvent");
            Assert.AreEqual(1, query.Count());
            Assert.AreEqual("terminateEndEvent", query.First()
                .ActivityType);

            query.Where(m => m.ActivityId == "noneEndEvent");
            Assert.AreEqual(1, query.Count());
            Assert.AreEqual("noneEndEvent", query.First()
                .ActivityType);
        }

        [Test]
        [Deployment]
        public virtual void testEndParallelJoin()
        {
            var pi = runtimeService.StartProcessInstanceByKey("process");

            var activityInstance = historyService.CreateHistoricActivityInstanceQuery()
                .Where(c => c.ProcessInstanceId == pi.Id)
                .Where(m => m.ActivityId == "parallelJoinEnd")

                .ToList();

            Assert.That(activityInstance.Count, Is.EqualTo(2));
            Assert.That(pi.IsEnded, Is.EqualTo(true));
        }

        [Test]
        [Deployment("resources/history/HistoricActivityInstanceTest.StartEventTypesForEventSubprocess.bpmn20.xml")]
        public virtual void testErrorEventSubprocess()
        {
            IDictionary<string, object> vars = new Dictionary<string, object>();
            vars["shouldThrowError"] = true;
            runtimeService.StartProcessInstanceByKey("process", vars);

            var historicActivity = historyService.CreateHistoricActivityInstanceQuery()
                .Where(m => m.ActivityId == "errorStartEvent")
                .First();

            Assert.AreEqual("errorStartEvent".ToLower(), historicActivity.ActivityType);
        }

        [Test]
        [Deployment(
            new[]
            {
                "resources/history/calledProcess.bpmn20.xml",
                "resources/history/HistoricActivityInstanceTest.TestCallSimpleSubProcess.bpmn20.xml"
            })]
        public virtual void testHistoricActivityInstanceCalledProcessId()
        {
            runtimeService.StartProcessInstanceByKey("callSimpleSubProcess");

            var historicActivityInstance = historyService.CreateHistoricActivityInstanceQuery()
                .Where(m => m.ActivityId == "callSubProcess")
                .First();

            var oldInstance = historyService.CreateHistoricProcessInstanceQuery()
                .Where(m => m.ProcessDefinitionKey == "calledProcess")
                .First();

            Assert.AreEqual(oldInstance.Id, historicActivityInstance.CalledProcessInstanceId);
        }

        [Test]
        [Deployment]
        public virtual void testHistoricActivityInstanceForEventsQuery()
        {
            var pi = runtimeService.StartProcessInstanceByKey("eventProcess");
            Assert.AreEqual(1, taskService.CreateTaskQuery()
                .Count());
            runtimeService.SignalEventReceived("signal");
            AssertProcessEnded(pi.Id);

            Assert.AreEqual(1, historyService.CreateHistoricActivityInstanceQuery()
                .Where(m => m.ActivityId == "noop")

                .Count());
            Assert.AreEqual(1, historyService.CreateHistoricActivityInstanceQuery()
                .Where(m => m.ActivityId == "userTask")

                .Count());
            Assert.AreEqual(1, historyService.CreateHistoricActivityInstanceQuery()
                .Where(m => m.ActivityId == "intermediate-event")

                .Count());
            Assert.AreEqual(1, historyService.CreateHistoricActivityInstanceQuery()
                .Where(m => m.ActivityId == "start")

                .Count());
            Assert.AreEqual(1, historyService.CreateHistoricActivityInstanceQuery()
                .Where(m => m.ActivityId == "end")

                .Count());

            Assert.AreEqual(1, historyService.CreateHistoricActivityInstanceQuery()
                .Where(m => m.ActivityId == "boundaryEvent")

                .Count());

            var intermediateEvent = historyService.CreateHistoricActivityInstanceQuery()
                .Where(m => m.ActivityId == "intermediate-event")
                .First();
            Assert.NotNull(intermediateEvent.StartTime);
            Assert.NotNull(intermediateEvent.EndTime);

            var startEvent = historyService.CreateHistoricActivityInstanceQuery()
                .Where(m => m.ActivityId == "start")
                .First();
            Assert.NotNull(startEvent.StartTime);
            Assert.NotNull(startEvent.EndTime);

            var endEvent = historyService.CreateHistoricActivityInstanceQuery()
                .Where(m => m.ActivityId == "end")
                .First();
            Assert.NotNull(endEvent.StartTime);
            Assert.NotNull(endEvent.EndTime);
        }

        /// <summary>
        ///     https://app.Camunda.com/jira/browse/CAM-1537
        /// </summary>
        [Test]
        [Deployment]
        public virtual void testHistoricActivityInstanceGatewayEndTimes()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("gatewayEndTimes");

            var query = taskService.CreateTaskQuery().ToList().OrderBy(m => m.Name)
                /*.OrderByTaskName()*/
                /*.Asc()*/;
            var tasks = query
                .ToList();
            taskService.Complete(tasks[0].Id);
            taskService.Complete(tasks[1].Id);

            // process instance should have finished
            Assert.NotNull(historyService.CreateHistoricProcessInstanceQuery()
                .Where(c => c.ProcessInstanceId == processInstance.Id)
                .First()
                .EndTime);
            // gateways should have end timestamps
            Assert.NotNull(historyService.CreateHistoricActivityInstanceQuery()
                .Where(m => m.ActivityId == "Gateway_0")
                .First()
                .EndTime);

            // there exists two historic activity instances for "Gateway_1" (parallel join)
            var historicActivityInstanceQuery = historyService.CreateHistoricActivityInstanceQuery()
                .Where(m => m.ActivityId == "Gateway_1");

            Assert.AreEqual(2, historicActivityInstanceQuery.Count());
            // they should have an end timestamp
            Assert.NotNull(historicActivityInstanceQuery
                .First().EndTime);
            Assert.NotNull(historicActivityInstanceQuery
                .ToList()[1].EndTime);
        }

        [Test]
        [Deployment("resources/history/HistoricActivityInstanceTest.TestHistoricActivityInstanceTimerEvent.bpmn20.xml")]
        public virtual void testHistoricActivityInstanceMessageEvent()
        {
            runtimeService.StartProcessInstanceByKey("catchSignal");

            var jobQuery = managementService.CreateJobQuery();
            Assert.AreEqual(1, jobQuery.Count());

            var eventSubscriptionQuery = runtimeService.CreateEventSubscriptionQuery();
            Assert.AreEqual(1, eventSubscriptionQuery.Count());

            runtimeService.CorrelateMessage("newInvoice");

            var taskQuery = taskService.CreateTaskQuery();
            var task = taskQuery.First();

            Assert.AreEqual("afterMessage", task.Name);

            var historicActivityInstanceQuery = historyService.CreateHistoricActivityInstanceQuery(m => m.ActivityId == "gw1");
            Assert.AreEqual(1, historicActivityInstanceQuery.Count());
            Assert.NotNull(historicActivityInstanceQuery.First()
                .EndTime);

            historicActivityInstanceQuery = historyService.CreateHistoricActivityInstanceQuery()
                .Where(m => m.ActivityId == "messageEvent");
            Assert.AreEqual(1, historicActivityInstanceQuery.Count());
            Assert.NotNull(historicActivityInstanceQuery.First()
                .EndTime);
            Assert.AreEqual("intermediateMessageCatch".ToLower(), historicActivityInstanceQuery.First()
                .ActivityType);
        }

        [Test]
        [Deployment]
        public virtual void testHistoricActivityInstanceNoop()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("noopProcess");

            var historicActivityInstance = historyService.CreateHistoricActivityInstanceQuery()
                .Where(m => m.ActivityId == "noop")
                .First();

            Assert.AreEqual("noop", historicActivityInstance.ActivityId);
            Assert.AreEqual("serviceTask", historicActivityInstance.ActivityType);
            Assert.NotNull(historicActivityInstance.ProcessDefinitionId);
            Assert.AreEqual(processInstance.Id, historicActivityInstance.ProcessInstanceId);
            Assert.AreEqual(processInstance.Id, historicActivityInstance.ExecutionId);
            Assert.NotNull(historicActivityInstance.StartTime);
            Assert.NotNull(historicActivityInstance.EndTime);
            Assert.True(historicActivityInstance.DurationInMillis >= 0);
        }

        [Test]//AssignTaskCmd
        [Deployment]
        public virtual void testHistoricActivityInstanceProperties()
        {
            // Start process instance
            runtimeService.StartProcessInstanceByKey("taskAssigneeProcess");

            // Get task list
            var historicActivityInstance = historyService.CreateHistoricActivityInstanceQuery()
                .Where(m => m.ActivityId == "theTask")
                .First();

            var task = taskService.CreateTaskQuery()
               .First();
            Assert.AreEqual(task.Id, historicActivityInstance.TaskId);
            Assert.AreEqual("kermit", historicActivityInstance.Assignee);

            // change assignee of the task
            taskService.SetAssignee(task.Id, "gonzo");
            task = taskService.CreateTaskQuery()
                .First();

            historicActivityInstance = historyService.CreateHistoricActivityInstanceQuery()
                .Where(m => m.ActivityId == "theTask")
                .First();
            Assert.AreEqual("gonzo", historicActivityInstance.Assignee);
        }

        [Test]
        [Deployment]
        public virtual void testHistoricActivityInstanceQuery()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("noopProcess");

            Assert.AreEqual(0, historyService.CreateHistoricActivityInstanceQuery()
                .Where(m => m.ActivityId == "nonExistingActivityId")

                .Count());
            Assert.AreEqual(1, historyService.CreateHistoricActivityInstanceQuery()
                .Where(m => m.ActivityId == "noop")

                .Count());

            Assert.AreEqual(0, historyService.CreateHistoricActivityInstanceQuery()
                .Where(m => m.ActivityType == "nonExistingActivityType")

                .Count());
            Assert.AreEqual(1, historyService.CreateHistoricActivityInstanceQuery()
                .Where(m => m.ActivityType == "serviceTask")

                .Count());

            Assert.AreEqual(0, historyService.CreateHistoricActivityInstanceQuery()
                .Where(m => m.ActivityName == "nonExistingActivityName")

                .Count());
            Assert.AreEqual(1, historyService.CreateHistoricActivityInstanceQuery()
                .Where(m => m.ActivityName == "No operation")

                .Count());

            Assert.AreEqual(0, historyService.CreateHistoricActivityInstanceQuery()
                .Where(m => m.TaskAssignee == "nonExistingAssignee")

                .Count());

            Assert.AreEqual(0, historyService.CreateHistoricActivityInstanceQuery()
                .Where(m => m.ExecutionId == "nonExistingExecutionId")

                .Count());

            if (processEngineConfiguration.HistoryLevel.Id >= ProcessEngineConfigurationImpl.HistorylevelActivity)
                Assert.AreEqual(3, historyService.CreateHistoricActivityInstanceQuery()
                    .Where(m => m.ExecutionId == processInstance.Id)

                    .Count());
            else
                Assert.AreEqual(0, historyService.CreateHistoricActivityInstanceQuery()
                    .Where(m => m.ExecutionId == processInstance.Id)

                    .Count());

            Assert.AreEqual(0, historyService.CreateHistoricActivityInstanceQuery()
                .Where(c => c.ProcessInstanceId == "nonExistingProcessInstanceId")

                .Count());

            if (processEngineConfiguration.HistoryLevel.Id >= ProcessEngineConfigurationImpl.HistorylevelActivity)
                Assert.AreEqual(3, historyService.CreateHistoricActivityInstanceQuery()
                    .Where(c => c.ProcessInstanceId == processInstance.Id)

                    .Count());
            else
                Assert.AreEqual(0, historyService.CreateHistoricActivityInstanceQuery()
                    .Where(c => c.ProcessInstanceId == processInstance.Id)

                    .Count());

            Assert.AreEqual(0, historyService.CreateHistoricActivityInstanceQuery()
                .Where(m => m.ProcessDefinitionId == "nonExistingProcessDefinitionId")

                .Count());

            if (processEngineConfiguration.HistoryLevel.Id >= ProcessEngineConfigurationImpl.HistorylevelActivity)
                Assert.AreEqual(3, historyService.CreateHistoricActivityInstanceQuery()
                    .Where(m => m.ProcessDefinitionId == processInstance.ProcessDefinitionId)

                    .Count());
            else
                Assert.AreEqual(0, historyService.CreateHistoricActivityInstanceQuery()
                    .Where(m => m.ProcessDefinitionId == processInstance.ProcessDefinitionId)

                    .Count());

            Assert.AreEqual(0, historyService.CreateHistoricActivityInstanceQuery(m => m.EndTime == null)
                /*.Unfinished()*/

                .Count());

            if (processEngineConfiguration.HistoryLevel.Id >= ProcessEngineConfigurationImpl.HistorylevelActivity)
                Assert.AreEqual(3, historyService.CreateHistoricActivityInstanceQuery(m => m.EndTime != null)
                    //.Finished()

                    .Count());
            else
                Assert.AreEqual(0, historyService.CreateHistoricActivityInstanceQuery(m => m.EndTime != null)
                    //.Finished()
                    .Count());

            if (processEngineConfiguration.HistoryLevel.Id >= ProcessEngineConfigurationImpl.HistorylevelActivity)
            {
                var historicActivityInstance = historyService.CreateHistoricActivityInstanceQuery()
                    .First();
                Assert.AreEqual(1, historyService.CreateHistoricActivityInstanceQuery()
                    .Where(m => m.Id == historicActivityInstance.Id)
                    .Count());
            }
        }

        [Test]
        [Deployment(
            "resources/history/HistoricActivityInstanceTest.TestHistoricActivityInstanceQueryByCompleteScope.bpmn")]
        public virtual void testHistoricActivityInstanceQueryByCanceled()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("process");

            runtimeService.DeleteProcessInstance(processInstance.Id, "test");

            var query = historyService.CreateHistoricActivityInstanceQuery(m => m.ActivityInstanceState == ActivityInstanceStateFields.Canceled.StateCode);
            //.Canceled();

            Assert.AreEqual(3, query.Count());

            var instances = query
                .ToList();

            foreach (var instance in instances)
                if (!instance.ActivityId.Equals("subprocess") && !instance.ActivityId.Equals("userTask1") &&
                    !instance.ActivityId.Equals("userTask2"))
                    Assert.Fail("Unexpected instance with activity id " + instance.ActivityId + " found.");

            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment]
        public virtual void testHistoricActivityInstanceQueryByCompleteScope()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("process");

            var tasks = taskService.CreateTaskQuery()

                .ToList();

            foreach (var task in tasks)
                taskService.Complete(task.Id);

            var query = historyService.CreateHistoricActivityInstanceQuery(m => m.ActivityInstanceState == ActivityInstanceStateFields.ScopeComplete.StateCode)
                /*.CompleteScope()*/;

            Assert.AreEqual(3, query.Count());

            var instances = query
                .ToList();

            foreach (var instance in instances)
                if (!instance.ActivityId.Equals("innerEnd") && !instance.ActivityId.Equals("end1") &&
                    !instance.ActivityId.Equals("end2"))
                    Assert.Fail("Unexpected instance with activity id " + instance.ActivityId + " found.");

            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment("resources/history/oneTaskProcess.bpmn20.xml")]
        public virtual void testHistoricActivityInstanceQueryStartFinishAfterBefore()
        {
            var startTime = DateTime.Now;

            ClockUtil.CurrentTime = startTime;
            var processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess", "businessKey123");

            var hourAgo = startTime;
            hourAgo = hourAgo.AddHours(-1);
            var hourFromNow = startTime;
            hourFromNow = hourFromNow.AddHours(1);

            // Start/end dates
            Assert.AreEqual(0, historyService.CreateHistoricActivityInstanceQuery()
                .Where(m => m.ActivityId == "theTask")
                //.FinishedBefore(hourAgo)
                .Where(m => m.EndTime < hourAgo)
                .Count());
            Assert.AreEqual(0, historyService.CreateHistoricActivityInstanceQuery()
                .Where(m => m.ActivityId == "theTask")
                //.FinishedBefore(hourFromNow)
                .Where(m => m.EndTime < hourFromNow)
                .Count());
            Assert.AreEqual(0, historyService.CreateHistoricActivityInstanceQuery()
                .Where(m => m.ActivityId == "theTask")
                //.FinishedAfter(hourAgo)
                .Where(m => m.EndTime > hourAgo)
                .Count());
            Assert.AreEqual(0, historyService.CreateHistoricActivityInstanceQuery()
                .Where(m => m.ActivityId == "theTask")
                //.FinishedAfter(hourFromNow)
                .Where(m => m.EndTime > hourAgo)
                .Count());
            Assert.AreEqual(1, historyService.CreateHistoricActivityInstanceQuery()
                .Where(m => m.ActivityId == "theTask")
                //.StartedBefore(hourFromNow)
                .Where(m => m.StartTime < hourFromNow)
                .Count());
            Assert.AreEqual(0, historyService.CreateHistoricActivityInstanceQuery()
                .Where(m => m.ActivityId == "theTask")
                //.StartedBefore(hourAgo)
                .Where(m => m.StartTime < hourAgo)
                .Count());
            Assert.AreEqual(1, historyService.CreateHistoricActivityInstanceQuery()
                .Where(m => m.ActivityId == "theTask")
                //.StartedAfter(hourAgo)
                .Where(m => m.StartTime > hourAgo)
                .Count());
            Assert.AreEqual(0, historyService.CreateHistoricActivityInstanceQuery()
                .Where(m => m.ActivityId == "theTask")
                //.StartedAfter(hourFromNow)
                .Where(m => m.StartTime > hourFromNow)
                .Count());
            Assert.AreEqual(0, historyService.CreateHistoricActivityInstanceQuery()
                .Where(m => m.ActivityId == "theTask")
                //.StartedAfter(hourFromNow)
                .Where(m => m.StartTime > hourFromNow)
                //.StartedBefore(hourAgo)
                .Where(m => m.StartTime < hourAgo)
                .Count());

            // After finishing process
            taskService.Complete(taskService.CreateTaskQuery()
                .Where(c => c.ProcessInstanceId == processInstance.Id)
                .First()
                .Id);
            Assert.AreEqual(1, historyService.CreateHistoricActivityInstanceQuery()
                .Where(m => m.ActivityId == "theTask")
                //.Finished()
                .Where(m => m.EndTime != null)
                .Count());
            Assert.AreEqual(0, historyService.CreateHistoricActivityInstanceQuery()
                .Where(m => m.ActivityId == "theTask")
                //.FinishedBefore(hourAgo)
                .Where(m => m.EndTime < hourAgo)
                .Count());
            Assert.AreEqual(1, historyService.CreateHistoricActivityInstanceQuery()
                .Where(m => m.ActivityId == "theTask")
                //.FinishedBefore(hourFromNow)
                .Where(m => m.EndTime < hourFromNow)
                .Count());
            Assert.AreEqual(1, historyService.CreateHistoricActivityInstanceQuery()
                .Where(m => m.ActivityId == "theTask")
                //.FinishedAfter(hourAgo)
                .Where(m => m.EndTime > hourAgo)
                .Count());
            Assert.AreEqual(0, historyService.CreateHistoricActivityInstanceQuery()
                .Where(m => m.ActivityId == "theTask")
                //.FinishedAfter(hourFromNow)
                .Where(m => m.EndTime > hourFromNow)
                .Count());
            Assert.AreEqual(0, historyService.CreateHistoricActivityInstanceQuery()
                .Where(m => m.ActivityId == "theTask")
                //.FinishedBefore(hourAgo)
                .Where(m => m.EndTime < hourAgo)
                //.FinishedAfter(hourFromNow)
                .Where(m => m.EndTime > hourFromNow)
                .Count());
        }

        [Test]
        [Deployment]
        public virtual void testHistoricActivityInstanceReceive()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("receiveProcess");

            var historicActivityInstance = historyService.CreateHistoricActivityInstanceQuery()
                .Where(m => m.ActivityId == "receive")
                .First();

            Assert.AreEqual("receive", historicActivityInstance.ActivityId);
            Assert.AreEqual("receiveTask", historicActivityInstance.ActivityType);
            Assert.IsNull(historicActivityInstance.EndTime);
            Assert.IsNull(historicActivityInstance.DurationInMillis);
            Assert.NotNull(historicActivityInstance.ProcessDefinitionId);
            Assert.AreEqual(processInstance.Id, historicActivityInstance.ProcessInstanceId);
            Assert.AreEqual(processInstance.Id, historicActivityInstance.ExecutionId);
            Assert.NotNull(historicActivityInstance.StartTime);

            // move clock by 1 second
            var now = ClockUtil.CurrentTime;
            ClockUtil.CurrentTime =now.AddSeconds(1);
            runtimeService.Signal(processInstance.Id);

            historicActivityInstance = historyService.CreateHistoricActivityInstanceQuery()
                .Where(m => m.ActivityId == "receive")
                .First();

            Assert.AreEqual("receive", historicActivityInstance.ActivityId);
            Assert.AreEqual("receiveTask", historicActivityInstance.ActivityType);
            Assert.NotNull(historicActivityInstance.EndTime);
            Assert.NotNull(historicActivityInstance.ProcessDefinitionId);
            Assert.AreEqual(processInstance.Id, historicActivityInstance.ProcessInstanceId);
            Assert.AreEqual(processInstance.Id, historicActivityInstance.ExecutionId);
            Assert.NotNull(historicActivityInstance.StartTime);
            Assert.True(historicActivityInstance.DurationInMillis >= 1000);
            Assert.True(((HistoricActivityInstanceEventEntity)historicActivityInstance).DurationRaw >= 1000);
        }

        [Test]//ExecuteJob
        [Deployment]
        public virtual void testHistoricActivityInstanceTimerEvent()
        {
            runtimeService.StartProcessInstanceByKey("catchSignal");

            Assert.AreEqual(1, runtimeService.CreateEventSubscriptionQuery()
                .Count());

            var jobQuery = managementService.CreateJobQuery();
            Assert.AreEqual(1, jobQuery.Count());

            var timer = jobQuery.FirstOrDefault();
            managementService.ExecuteJob(timer.Id);

            var taskQuery = taskService.CreateTaskQuery();
            var task = taskQuery.FirstOrDefault();

            Assert.AreEqual("afterTimer", task.Name);

            var historicActivityInstanceQuery = historyService.CreateHistoricActivityInstanceQuery()
                .Where(m => m.ActivityId == "gw1");
            Assert.AreEqual(1, historicActivityInstanceQuery.Count());
            Assert.NotNull(historicActivityInstanceQuery.First()
                .EndTime);

            historicActivityInstanceQuery = historyService.CreateHistoricActivityInstanceQuery()
                .Where(m => m.ActivityId == "timerEvent");
            Assert.AreEqual(1, historicActivityInstanceQuery.Count());
            Assert.NotNull(historicActivityInstanceQuery.First()
                .EndTime);
            Assert.AreEqual("intermediateTimer", historicActivityInstanceQuery.First()
                .ActivityType);
        }

        [Test]//net不支持groovy语法,
        [Deployment("resources/history/HistoricActivityInstanceTest.TestEvents.bpmn")]
        public virtual void testIntermediateCatchEventTypes()
        {
            var query = startEventTestProcess("");

            query.Where(m => m.ActivityId == "intermediateSignalCatchEvent");
            Assert.AreEqual(1, query.Count());
            Assert.AreEqual("intermediateSignalCatch", query.First()
                .ActivityType);

            query.Where(m => m.ActivityId == "intermediateMessageCatchEvent");
            Assert.AreEqual(1, query.Count());
            Assert.AreEqual("intermediateMessageCatch", query.First()
                .ActivityType);

            query.Where(m => m.ActivityId == "intermediateTimerCatchEvent");
            Assert.AreEqual(1, query.Count());
            Assert.AreEqual("intermediateTimer", query.First()
                .ActivityType);
        }

        //[Test] net不支持groovy语法,
        [Deployment("resources/history/HistoricActivityInstanceTest.TestEvents.bpmn")]
        public virtual void testIntermediateThrowEventTypes()
        {
            var query = startEventTestProcess("");

            query.Where(m => m.ActivityId == "intermediateSignalThrowEvent");
            Assert.AreEqual(1, query.Count());
            Assert.AreEqual("intermediateSignalThrow", query.First()
                .ActivityType);

            query.Where(m => m.ActivityId == "intermediateMessageThrowEvent");
            Assert.AreEqual(1, query.Count());
            Assert.AreEqual("intermediateMessageThrowEvent", query.First()
                .ActivityType);

            query.Where(m => m.ActivityId == "intermediateNoneThrowEvent");
            Assert.AreEqual(1, query.Count());
            Assert.AreEqual("intermediateNoneThrowEvent", query.First()
                .ActivityType);

            query.Where(m => m.ActivityId == "intermediateCompensationThrowEvent");
            Assert.AreEqual(1, query.Count());
            Assert.AreEqual("intermediateCompensationThrowEvent", query.First()
                .ActivityType);
        }

        [Test]
        [Deployment]
        public virtual void testInterruptingBoundaryMessageEvent()
        {
            var pi = runtimeService.StartProcessInstanceByKey("process");
            //13:56:31.107 [main] DEBUG o.c.b.e.i.p.e.E.selectExecutionsByQueryCriteria - ==>  Preparing: select distinct RES.* from ACT_RU_EXECUTION RES inner join ACT_RE_PROCDEF P on RES.PROC_DEF_ID_ = P.ID_ WHERE exists (select ID_ from ACT_RU_EVENT_SUBSCR EVT where EVT.EXECUTION_ID_ = RES.ID_ and EVT.EVENT_TYPE_ = ? and EVT.EVENT_NAME_ = ? ) order by RES.ID_ asc LIMIT ? OFFSET ? 
            //13:56:31.114[main] DEBUG o.c.b.e.i.p.e.E.selectExecutionsByQueryCriteria - ==> Parameters: message(String), newMessage(String), 2147483647(Integer), 0(Integer)
            //var execution = runtimeService.CreateExecutionQuery()
            /*.MessageEventSubscriptionName("newMessage")*/
            //.First();
            var db = runtimeService.GetDbContext();

            var _query = from a in (from e in db.Set<ExecutionEntity>() from c in db.Set<EventSubscriptionEntity>() where c.ExecutionId == e.Id && c.EventType == "message" && c.EventName == "newMessage" select e)
                        join b in db.Set<ProcessDefinitionEntity>() on a.ProcessDefinitionId equals b.Id
                        orderby a.Id
                        select a;
            var execution = _query.FirstOrDefault();
            runtimeService.MessageEventReceived("newMessage", execution.Id);

            var query = historyService.CreateHistoricActivityInstanceQuery();

           query= query.Where(m => m.ActivityId == "message");
            Assert.AreEqual(1, query.Count());
            Assert.NotNull(query.First()
                .EndTime);
            Assert.AreEqual("boundaryMessage".ToLower(), query.First()
                .ActivityType);

            var task = taskService.CreateTaskQuery()
                .First();
            taskService.Complete(task.Id);

            AssertProcessEnded(pi.Id);
        }

        [Test]
        [Deployment]
        public virtual void testInterruptingBoundarySignalEvent()
        {
            var pi = runtimeService.StartProcessInstanceByKey("process");
            //14:21:24.932 [main] DEBUG o.c.b.e.i.p.e.E.selectExecutionsByQueryCriteria - ==>  Preparing: select distinct RES.* from ACT_RU_EXECUTION RES inner join ACT_RE_PROCDEF P on RES.PROC_DEF_ID_ = P.ID_ WHERE exists (select ID_ from ACT_RU_EVENT_SUBSCR EVT where EVT.EXECUTION_ID_ = RES.ID_ and EVT.EVENT_TYPE_ = ? and EVT.EVENT_NAME_ = ? ) order by RES.ID_ asc LIMIT ? OFFSET ? 
            //14:21:24.937[main] DEBUG o.c.b.e.i.p.e.E.selectExecutionsByQueryCriteria - ==> Parameters: signal(String), newSignal(String), 2147483647(Integer), 0(Integer)
            //var execution = runtimeService.CreateExecutionQuery()
                //.SignalEventSubscriptionName("newSignal")
                //.First();
            var db = runtimeService.GetDbContext();
            var sql = from a in (from exe in db.Set<ExecutionEntity>() join sub in db.Set<EventSubscriptionEntity>() on exe.Id equals sub.ExecutionId where sub.EventType=="signal"&&sub.EventName== "newSignal" select exe)
                      join b in db.Set<ProcessDefinitionEntity>() on a.ProcessDefinitionId equals b.Id
                      orderby a.Id
                      select a;
            var execution = sql.FirstOrDefault();
            runtimeService.SignalEventReceived("newSignal", execution.Id);

            var query = historyService.CreateHistoricActivityInstanceQuery();

            query= query.Where(m => m.ActivityId == "signal");
            Assert.AreEqual(1, query.Count());
            Assert.NotNull(query.First()
                .EndTime);
            Assert.AreEqual("boundarySignal".ToLower(), query.First()
                .ActivityType);

            var task = taskService.CreateTaskQuery()
                .First();
            taskService.Complete(task.Id);

            AssertProcessEnded(pi.Id);
        }

        [Test]
        [Deployment]
        public virtual void testInterruptingBoundaryTimerEvent()
        {
            var pi = runtimeService.StartProcessInstanceByKey("process");

            var job = managementService.CreateJobQuery()
                .First();
            Assert.NotNull(job);

            managementService.ExecuteJob(job.Id);

            var query = historyService.CreateHistoricActivityInstanceQuery();

            query= query.Where(m => m.ActivityId == "timer");
            Assert.AreEqual(1, query.Count());
            Assert.NotNull(query.First()
                .EndTime);
            Assert.AreEqual("boundaryTimer".ToLower(), query.First()
                .ActivityType);

            var task = taskService.CreateTaskQuery()
                .First();
            taskService.Complete(task.Id);

            AssertProcessEnded(pi.Id);
        }

        //[Test]//自定义异常
        public virtual void testInvalidSorting()
        {
            try
            {
                historyService.CreateHistoricActivityInstanceQuery()
                    ///*.Asc()*/
                    .ToList();
                Assert.Fail();
            }
            catch (ProcessEngineException)
            {
            }

            try
            {
                historyService.CreateHistoricActivityInstanceQuery()
                    ///*.Desc()*/
                    .ToList();
                Assert.Fail();
            }
            catch (ProcessEngineException)
            {
            }

            try
            {
                historyService.CreateHistoricActivityInstanceQuery()
                    //.OrderByHistoricActivityInstanceDuration()
                    //
                    .ToList();
                Assert.Fail();
            }
            catch (ProcessEngineException)
            {
            }
        }

        [Test]
        [Deployment("resources/history/HistoricActivityInstanceTest.TestHistoricActivityInstanceReceive.bpmn20.xml")]
        public virtual void testLongRunningHistoricActivityInstanceReceive()
        {
            //const long ONE_YEAR = 1000 * 60 * 60 * 24 * 365;
            DateTime now = DateTime.Now;
            long ONE_YEAR = (now.AddYears(1) - now).Milliseconds;
            DateTime cal = now.AddSeconds(-now.Second).AddMilliseconds(-now.Millisecond);
            //cal.Set(DateTime.SECOND, 0);
            //cal.Set(DateTime.MILLISECOND, 0);

            ClockUtil.CurrentTime = cal;

            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("receiveProcess");

            IHistoricActivityInstance historicActivityInstance = historyService.CreateHistoricActivityInstanceQuery(c => c.ActivityId == "receive").First();

            Assert.AreEqual("receive", historicActivityInstance.ActivityId);
            Assert.AreEqual("receiveTask", historicActivityInstance.ActivityType);
            Assert.IsNull(historicActivityInstance.EndTime);
            Assert.IsNull(historicActivityInstance.DurationInMillis);
            Assert.NotNull(historicActivityInstance.ProcessDefinitionId);
            Assert.AreEqual(processInstance.Id, historicActivityInstance.ProcessInstanceId);
            Assert.AreEqual(processInstance.Id, historicActivityInstance.ExecutionId);
            Assert.NotNull(historicActivityInstance.StartTime);

            // move clock by 1 year
            cal.AddYears(1);
            ClockUtil.CurrentTime = cal;

            runtimeService.Signal(processInstance.Id);

            historicActivityInstance = historyService.CreateHistoricActivityInstanceQuery(c => c.ActivityId == "receive").First();

            Assert.AreEqual("receive", historicActivityInstance.ActivityId);
            Assert.AreEqual("receiveTask", historicActivityInstance.ActivityType);
            Assert.NotNull(historicActivityInstance.EndTime);
            Assert.NotNull(historicActivityInstance.ProcessDefinitionId);
            Assert.AreEqual(processInstance.Id, historicActivityInstance.ProcessInstanceId);
            Assert.AreEqual(processInstance.Id, historicActivityInstance.ExecutionId);
            Assert.NotNull(historicActivityInstance.StartTime);
            Assert.True(historicActivityInstance.DurationInMillis >= ONE_YEAR);
            Assert.True(((HistoricActivityInstanceEventEntity)historicActivityInstance).DurationRaw >= ONE_YEAR);
        }

        [Test]
        [Deployment("resources/history/HistoricActivityInstanceTest.StartEventTypesForEventSubprocess.bpmn20.xml")]
        public virtual void testMessageEventSubprocess()
        {
            IDictionary<string, object> vars = new Dictionary<string, object>();
            vars["shouldThrowError"] = false;
            runtimeService.StartProcessInstanceByKey("process", vars);

            runtimeService.CorrelateMessage("newMessage");

            var historicActivity = historyService.CreateHistoricActivityInstanceQuery()
                .Where(m => m.ActivityId == "messageStartEvent")
                .First();

            Assert.AreEqual("messageStartEvent".ToLower(), historicActivity.ActivityType);
        }

        [Test]
        [Deployment]
        public virtual void testMultiInstanceReceiveActivity()
        {
            runtimeService.StartProcessInstanceByKey("process");

            var query = historyService.CreateHistoricActivityInstanceQuery();
            var miBodyInstance = query.Where(m => m.ActivityId == "receiveTask#multiInstanceBody")
                .First();

            query= query.Where(m => m.ActivityId == "receiveTask");
            Assert.AreEqual(5, query.Count());

            var result = query
                .ToList();

            foreach (var instance in result)
                Assert.AreEqual(miBodyInstance.Id, instance.ParentActivityInstanceId);
        }

        [Test]
        [Deployment]
        public virtual void testMultiInstanceScopeActivity()
        {
            var pi = runtimeService.StartProcessInstanceByKey("process");

            var query = historyService.CreateHistoricActivityInstanceQuery();

            var miBodyInstance = query.Where(m => m.ActivityId == "userTask#multiInstanceBody")
                .First();

            query= query.Where(m => m.ActivityId == "userTask");
            Assert.AreEqual(5, query.Count());


            var result = query
                .ToList();

            foreach (var instance in result)
                Assert.AreEqual(miBodyInstance.Id, instance.ParentActivityInstanceId);

            var tasks = taskService.CreateTaskQuery()

                .ToList();
            foreach (var task in tasks)
                taskService.Complete(task.Id);

            AssertProcessEnded(pi.Id);
        }

        [Test]
        [Deployment]
        public virtual void testNonInterruptingBoundaryMessageEvent()
        {
            var pi = runtimeService.StartProcessInstanceByKey("process");

            //var execution = runtimeService.CreateExecutionQuery()
                /*.MessageEventSubscriptionName("newMessage")*/
                //.First();
            var db = runtimeService.GetDbContext();

            var _query = from a in (from e in db.Set<ExecutionEntity>() from c in db.Set<EventSubscriptionEntity>() where c.ExecutionId == e.Id && c.EventType == "message" && c.EventName == "newMessage" select e)
                         join b in db.Set<ProcessDefinitionEntity>() on a.ProcessDefinitionId equals b.Id
                         orderby a.Id
                         select a;
            var execution = _query.FirstOrDefault();

            runtimeService.MessageEventReceived("newMessage", execution.Id);

            var query = historyService.CreateHistoricActivityInstanceQuery();

            query= query.Where(m => m.ActivityId == "message");
            Assert.AreEqual(1, query.Count());
            Assert.NotNull(query.First()
                .EndTime);
            Assert.AreEqual("boundaryMessage".ToLower(), query.First()
                .ActivityType);

            var tasks = taskService.CreateTaskQuery()

                .ToList();
            foreach (var task in tasks)
                taskService.Complete(task.Id);

            AssertProcessEnded(pi.Id);
        }

        [Test]
        [Deployment]
        public virtual void testNonInterruptingBoundarySignalEvent()
        {
            var pi = runtimeService.StartProcessInstanceByKey("process");

            //var execution = runtimeService.CreateExecutionQuery()
                //.SignalEventSubscriptionName("newSignal")
                // .First();
            var db = runtimeService.GetDbContext();

            var _query = from a in (from e in db.Set<ExecutionEntity>() from c in db.Set<EventSubscriptionEntity>() where c.ExecutionId == e.Id && c.EventType == "signal" && c.EventName == "newSignal" select e)
                         join b in db.Set<ProcessDefinitionEntity>() on a.ProcessDefinitionId equals b.Id
                         orderby a.Id
                         select a;
            var execution = _query.FirstOrDefault();

            runtimeService.SignalEventReceived("newSignal", execution.Id);

            var query = historyService.CreateHistoricActivityInstanceQuery();

            query= query.Where(m => m.ActivityId == "signal");
            Assert.AreEqual(1, query.Count());
            Assert.NotNull(query.First()
                .EndTime);
            Assert.AreEqual("boundarySignal".ToLower(), query.First()
                .ActivityType);

            var tasks = taskService.CreateTaskQuery()

                .ToList();
            foreach (var task in tasks)
                taskService.Complete(task.Id);

            AssertProcessEnded(pi.Id);
        }

        [Test]
        [Deployment]
        public virtual void testNonInterruptingBoundaryTimerEvent()
        {
            var pi = runtimeService.StartProcessInstanceByKey("process");

            var job = managementService.CreateJobQuery()
                .First();
            Assert.NotNull(job);

            managementService.ExecuteJob(job.Id);

            var query = historyService.CreateHistoricActivityInstanceQuery();

            query= query.Where(m => m.ActivityId == "timer");
            Assert.AreEqual(1, query.Count());
            Assert.NotNull(query.First()
                .EndTime);
            Assert.AreEqual("boundaryTimer".ToLower(), query.First()
                .ActivityType);

            var tasks = taskService.CreateTaskQuery()

                .ToList();
            foreach (var task in tasks)
                taskService.Complete(task.Id);

            AssertProcessEnded(pi.Id);
        }

        [Test]
        [Deployment("resources/history/oneTaskProcess.bpmn20.xml")]
        public virtual void testProcessDefinitionKeyProperty()
        {
            // given
            var key = "oneTaskProcess";
            var ProcessInstanceId = runtimeService.StartProcessInstanceByKey(key)
                .Id;

            // when
            var activityInstance = historyService.CreateHistoricActivityInstanceQuery()
                .Where(c => c.ProcessInstanceId == ProcessInstanceId)
                .Where(m => m.ActivityId == "theTask")
                .First();

            // then
            Assert.NotNull(activityInstance.ProcessDefinitionKey);
            Assert.AreEqual(key, activityInstance.ProcessDefinitionKey);
        }

        [Test]
        [Deployment]
        public virtual void testScopeActivity()
        {
            var pi = runtimeService.StartProcessInstanceByKey("process");

            var query = historyService.CreateHistoricActivityInstanceQuery();

            query= query.Where(m => m.ActivityId == "userTask");
            Assert.AreEqual(1, query.Count());

            var historicActivityInstance = query.First();

            Assert.AreEqual(pi.Id, historicActivityInstance.ParentActivityInstanceId);

            var task = taskService.CreateTaskQuery()
                .First();
            taskService.Complete(task.Id);

            AssertProcessEnded(pi.Id);
        }

        [Test]
        [Deployment("resources/history/HistoricActivityInstanceTest.StartEventTypesForEventSubprocess.bpmn20.xml")]
        public virtual void testSignalEventSubprocess()
        {
            IDictionary<string, object> vars = new Dictionary<string, object>();
            vars["shouldThrowError"] = false;
            runtimeService.StartProcessInstanceByKey("process", vars);

            runtimeService.SignalEventReceived("newSignal");

            var historicActivity = historyService.CreateHistoricActivityInstanceQuery()
                .Where(m => m.ActivityId == "signalStartEvent")
                .First();

            Assert.AreEqual("signalStartEvent".ToLower(), historicActivity.ActivityType);
        }

        [Test]
        [Deployment]
        public virtual void testSorting()
        {
            runtimeService.StartProcessInstanceByKey("process");

            var expectedActivityInstances = -1;
            if (processEngineConfiguration.HistoryLevel.Id >= ProcessEngineConfigurationImpl.HistorylevelActivity)
                expectedActivityInstances = 2;
            else
                expectedActivityInstances = 0;

            Assert.AreEqual(expectedActivityInstances, historyService.CreateHistoricActivityInstanceQuery()
                //.OrderByHistoricActivityInstanceId()
                /*.Asc()*/

                .Count());
            Assert.AreEqual(expectedActivityInstances, historyService.CreateHistoricActivityInstanceQuery()
                //.OrderByHistoricActivityInstanceStartTime()
                /*.Asc()*/

                .Count());
            Assert.AreEqual(expectedActivityInstances, historyService.CreateHistoricActivityInstanceQuery()
                /*.OrderByHistoricActivityInstanceEndTime()*/
                /*.Asc()*/

                .Count());
            Assert.AreEqual(expectedActivityInstances, historyService.CreateHistoricActivityInstanceQuery()
                //.OrderByHistoricActivityInstanceDuration()
                /*.Asc()*/

                .Count());
            Assert.AreEqual(expectedActivityInstances, historyService.CreateHistoricActivityInstanceQuery()
                //.OrderByExecutionId()
                /*.Asc()*/

                .Count());
            Assert.AreEqual(expectedActivityInstances, historyService.CreateHistoricActivityInstanceQuery()
                /*.OrderByProcessDefinitionId()*/
                /*.Asc()*/

                .Count());
            Assert.AreEqual(expectedActivityInstances, historyService.CreateHistoricActivityInstanceQuery()
                //.OrderByProcessInstanceId()
                /*.Asc()*/

                .Count());

            Assert.AreEqual(expectedActivityInstances, historyService.CreateHistoricActivityInstanceQuery()
                //.OrderByHistoricActivityInstanceId()
                /*.Desc()*/

                .Count());
            Assert.AreEqual(expectedActivityInstances, historyService.CreateHistoricActivityInstanceQuery()
                //.OrderByHistoricActivityInstanceStartTime()
                /*.Desc()*/

                .Count());
            Assert.AreEqual(expectedActivityInstances, historyService.CreateHistoricActivityInstanceQuery()
                /*.OrderByHistoricActivityInstanceEndTime()*/
                /*.Desc()*/

                .Count());
            Assert.AreEqual(expectedActivityInstances, historyService.CreateHistoricActivityInstanceQuery()
                //.OrderByHistoricActivityInstanceDuration()
                /*.Desc()*/

                .Count());
            Assert.AreEqual(expectedActivityInstances, historyService.CreateHistoricActivityInstanceQuery()
                //.OrderByExecutionId()
                /*.Desc()*/

                .Count());
            Assert.AreEqual(expectedActivityInstances, historyService.CreateHistoricActivityInstanceQuery()
                /*.OrderByProcessDefinitionId()*/
                /*.Desc()*/

                .Count());
            Assert.AreEqual(expectedActivityInstances, historyService.CreateHistoricActivityInstanceQuery()
                //.OrderByProcessInstanceId()
                /*.Desc()*/

                .Count());

            Assert.AreEqual(expectedActivityInstances, historyService.CreateHistoricActivityInstanceQuery()
                //.OrderByHistoricActivityInstanceId()
                /*.Asc()*/
                .Count());
            Assert.AreEqual(expectedActivityInstances, historyService.CreateHistoricActivityInstanceQuery()
                //.OrderByHistoricActivityInstanceStartTime()
                /*.Asc()*/
                .Count());
            Assert.AreEqual(expectedActivityInstances, historyService.CreateHistoricActivityInstanceQuery()
                /*.OrderByHistoricActivityInstanceEndTime()*/
                /*.Asc()*/
                .Count());
            Assert.AreEqual(expectedActivityInstances, historyService.CreateHistoricActivityInstanceQuery()
                //.OrderByHistoricActivityInstanceDuration()
                /*.Asc()*/
                .Count());
            Assert.AreEqual(expectedActivityInstances, historyService.CreateHistoricActivityInstanceQuery()
                //.OrderByExecutionId()
                /*.Asc()*/
                .Count());
            Assert.AreEqual(expectedActivityInstances, historyService.CreateHistoricActivityInstanceQuery()
                /*.OrderByProcessDefinitionId()*/
                /*.Asc()*/
                .Count());
            Assert.AreEqual(expectedActivityInstances, historyService.CreateHistoricActivityInstanceQuery()
                //.OrderByProcessInstanceId()
                /*.Asc()*/
                .Count());

            Assert.AreEqual(expectedActivityInstances, historyService.CreateHistoricActivityInstanceQuery()
                //.OrderByHistoricActivityInstanceId()
                /*.Desc()*/
                .Count());
            Assert.AreEqual(expectedActivityInstances, historyService.CreateHistoricActivityInstanceQuery()
                //.OrderByHistoricActivityInstanceStartTime()
                /*.Desc()*/
                .Count());
            Assert.AreEqual(expectedActivityInstances, historyService.CreateHistoricActivityInstanceQuery()
                /*.OrderByHistoricActivityInstanceEndTime()*/
                /*.Desc()*/
                .Count());
            Assert.AreEqual(expectedActivityInstances, historyService.CreateHistoricActivityInstanceQuery()
                //.OrderByHistoricActivityInstanceDuration()
                /*.Desc()*/
                .Count());
            Assert.AreEqual(expectedActivityInstances, historyService.CreateHistoricActivityInstanceQuery()
                //.OrderByExecutionId()
                /*.Desc()*/
                .Count());
            Assert.AreEqual(expectedActivityInstances, historyService.CreateHistoricActivityInstanceQuery()
                /*.OrderByProcessDefinitionId()*/
                /*.Desc()*/
                .Count());
            Assert.AreEqual(expectedActivityInstances, historyService.CreateHistoricActivityInstanceQuery()
                //.OrderByProcessInstanceId()
                /*.Desc()*/
                .Count());
        }

        //[Test]//.net不支持groovy语法,
        [Deployment("resources/history/HistoricActivityInstanceTest.TestEvents.bpmn")]
        public virtual void testStartEventTypes()
        {
            var query = startEventTestProcess("");

            query.Where(m => m.ActivityId == "timerStartEvent");
            Assert.AreEqual(1, query.Count());
            Assert.AreEqual("startTimerEvent", query.First()
                .ActivityType);

            query.Where(m => m.ActivityId == "noneStartEvent");
            Assert.AreEqual(1, query.Count());
            Assert.AreEqual("startEvent", query.First()
                .ActivityType);

            query = startEventTestProcess("CAM-2365");
            query.Where(m => m.ActivityId == "messageStartEvent");
            Assert.AreEqual(1, query.Count());
            Assert.AreEqual("messageStartEvent", query.First()
                .ActivityType);
        }

        [Test]
        [Deployment("resources/history/HistoricActivityInstanceTest.StartEventTypesForEventSubprocess.bpmn20.xml")]
        public virtual void testTimerEventSubprocess()
        {
            IDictionary<string, object> vars = new Dictionary<string, object>();
            vars["shouldThrowError"] = false;
            runtimeService.StartProcessInstanceByKey("process", vars);

            var timerJob = managementService.CreateJobQuery()
                .First();
            managementService.ExecuteJob(timerJob.Id);

            var historicActivity = historyService.CreateHistoricActivityInstanceQuery()
                .Where(m => m.ActivityId == "timerStartEvent")
                .First();

            Assert.AreEqual("startTimerEvent".ToLower(), historicActivity.ActivityType);
        }

        [Test]
        [Deployment("resources/history/HistoricActivityInstanceTest.TestBoundaryCancelEvent.bpmn20.xml")]
        public virtual void testTransaction()
        {
            var pi = runtimeService.StartProcessInstanceByKey("process");

            var query = historyService.CreateHistoricActivityInstanceQuery();

            query= query.Where(m => m.ActivityId == "transaction");
            Assert.AreEqual(1, query.Count());
            Assert.NotNull(query.First()
                .EndTime);

            var task = taskService.CreateTaskQuery()
                .First();
            taskService.Complete(task.Id);

            AssertProcessEnded(pi.Id);
        }

        [Test]
        [Deployment]
        public virtual void testUserTaskStillRunning()
        {
            runtimeService.StartProcessInstanceByKey("nonInterruptingEvent");

            var jobQuery = managementService.CreateJobQuery();
            Assert.AreEqual(1, jobQuery.Count());

            managementService.ExecuteJob(jobQuery.First()
                .Id);

            var historicActivityInstanceQuery = historyService.CreateHistoricActivityInstanceQuery()
                .Where(m => m.ActivityId == "userTask");
            Assert.AreEqual(1, historicActivityInstanceQuery.Count());
            Assert.IsNull(historicActivityInstanceQuery.First()
                .EndTime);

            historicActivityInstanceQuery = historyService.CreateHistoricActivityInstanceQuery()
                .Where(m => m.ActivityId == "end1");
            Assert.AreEqual(0, historicActivityInstanceQuery.Count());

            historicActivityInstanceQuery = historyService.CreateHistoricActivityInstanceQuery()
                .Where(m => m.ActivityId == "timer");
            Assert.AreEqual(1, historicActivityInstanceQuery.Count());
            Assert.NotNull(historicActivityInstanceQuery.First()
                .EndTime);

            historicActivityInstanceQuery = historyService.CreateHistoricActivityInstanceQuery()
                .Where(m => m.ActivityId == "end2");
            Assert.AreEqual(1, historicActivityInstanceQuery.Count());
            Assert.NotNull(historicActivityInstanceQuery.First()
                .EndTime);
        }
    }
}