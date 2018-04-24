using System;
using System.Linq;
using Engine.Tests.Api.Runtime;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.History.Impl.Event;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;
using ESS.FW.Bpm.Engine.Impl.util;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using NUnit.Framework;

namespace Engine.Tests.History
{
    /// <summary>
    /// </summary>
    [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull)]
    [TestFixture]
    public class HistoricJobLogTest : PluggableProcessEngineTestCase
    {
        [Test][Deployment( "resources/history/HistoricJobLogTest.TestAsyncContinuationWithLongId.bpmn20.xml") ]
        public virtual void testSuccessfulHistoricJobLogEntryStoredForLongActivityId()
        {
            runtimeService.StartProcessInstanceByKey("process", ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                .PutValue("Assert.Fail", false));

            var job = managementService.CreateJobQuery()
                .First();

            managementService.ExecuteJob(job.Id);

            var historicJob = historyService.CreateHistoricJobLogQuery(c=>c.State == JobStateFields.Successful.StateCode)
                .First();
            Assert.NotNull(historicJob);
            Assert.AreEqual(
                "serviceTaskIdIsReallyLongAndItShouldBeMoreThan64CharsSoItWill" +
                "BlowAnyActivityIdColumnWhereSizeIs64OrLessSoWeAlignItTo255LikeEverywhereElse", historicJob.ActivityId);
        }

        [Test][Deployment( "resources/history/HistoricJobLogTest.TestStartTimerEvent.bpmn20.xml") ]
        public virtual void testStartTimerEventJobHandlerType()
        {
            var job = managementService.CreateJobQuery()
                .First();

            var historicJob = historyService.CreateHistoricJobLogQuery(c=>c.JobId == job.Id)
                .First();

            Assert.NotNull(historicJob);

            Assert.AreEqual(job.Id, historicJob.JobId);
            Assert.AreEqual(job.JobDefinitionId, historicJob.JobDefinitionId);
            Assert.AreEqual("theStart", historicJob.ActivityId);
            Assert.AreEqual(TimerStartEventJobHandler.TYPE, historicJob.JobDefinitionType);
            Assert.AreEqual("CYCLE: 0 0/5 * * * ?", historicJob.JobDefinitionConfiguration);
            Assert.AreEqual(job.Duedate, historicJob.JobDueDate);
        }

        [Test][Deployment( "resources/history/HistoricJobLogTest.TestStartTimerEventInsideEventSubProcess.bpmn20.xml") ]
        public virtual void testStartTimerEventInsideEventSubProcessJobHandlerType()
        {
            runtimeService.StartProcessInstanceByKey("process");

            var job = managementService.CreateJobQuery()
                .First();

            var historicJob = historyService.CreateHistoricJobLogQuery(c=>c.JobId == job.Id)
                .First();

            Assert.NotNull(historicJob);

            Assert.AreEqual(job.Id, historicJob.JobId);
            Assert.AreEqual(job.JobDefinitionId, historicJob.JobDefinitionId);
            Assert.AreEqual("subprocessStartEvent", historicJob.ActivityId);
            Assert.AreEqual(TimerStartEventSubprocessJobHandler.TYPE, historicJob.JobDefinitionType);
            Assert.AreEqual("DURATION: PT1M", historicJob.JobDefinitionConfiguration);
            Assert.AreEqual(job.Duedate, historicJob.JobDueDate);
        }

        [Test][Deployment( "resources/history/HistoricJobLogTest.TestIntermediateTimerEvent.bpmn20.xml") ]
        public virtual void testIntermediateTimerEventJobHandlerType()
        {
            runtimeService.StartProcessInstanceByKey("process");

            var job = managementService.CreateJobQuery()
                .First();

            var historicJob = historyService.CreateHistoricJobLogQuery(c=>c.JobId == job.Id)
                .First();

            Assert.NotNull(historicJob);

            Assert.AreEqual(job.Id, historicJob.JobId);
            Assert.AreEqual(job.JobDefinitionId, historicJob.JobDefinitionId);
            Assert.AreEqual("timer", historicJob.ActivityId);
            Assert.AreEqual(TimerCatchIntermediateEventJobHandler.TYPE, historicJob.JobDefinitionType);
            Assert.AreEqual("DURATION: PT1M", historicJob.JobDefinitionConfiguration);
            Assert.AreEqual(job.Duedate, historicJob.JobDueDate);
        }

        [Test][Deployment( "resources/history/HistoricJobLogTest.TestBoundaryTimerEvent.bpmn20.xml") ]
        public virtual void testBoundaryTimerEventJobHandlerType()
        {
            runtimeService.StartProcessInstanceByKey("process");

            var job = managementService.CreateJobQuery()
                .First();

            var historicJob = historyService.CreateHistoricJobLogQuery(c=>c.JobId == job.Id)
                .First();

            Assert.NotNull(historicJob);

            Assert.AreEqual(job.Id, historicJob.JobId);
            Assert.AreEqual(job.JobDefinitionId, historicJob.JobDefinitionId);
            Assert.AreEqual("timer", historicJob.ActivityId);
            Assert.AreEqual(TimerExecuteNestedActivityJobHandler.TYPE, historicJob.JobDefinitionType);
            Assert.AreEqual("DURATION: PT5M", historicJob.JobDefinitionConfiguration);
            Assert.AreEqual(job.Duedate, historicJob.JobDueDate);
        }

        [Test][Deployment( new [] { "resources/history/HistoricJobLogTest.TestCatchingSignalEvent.bpmn20.xml", "resources/history/HistoricJobLogTest.TestThrowingSignalEventAsync.bpmn20.xml" })]
        public virtual void testCatchingSignalEventJobHandlerType()
        {
            runtimeService.StartProcessInstanceByKey("catchSignal");
            runtimeService.StartProcessInstanceByKey("throwSignal");

            var job = managementService.CreateJobQuery()
                .First();

            var historicJob = historyService.CreateHistoricJobLogQuery(c=>c.JobId == job.Id)
                .First();

            Assert.NotNull(historicJob);

            Assert.IsNull(historicJob.JobDueDate);

            Assert.AreEqual(job.Id, historicJob.JobId);
            Assert.AreEqual(job.JobDefinitionId, historicJob.JobDefinitionId);
            Assert.AreEqual("signalEvent", historicJob.ActivityId);
            Assert.AreEqual(ProcessEventJobHandler.TYPE, historicJob.JobDefinitionType);
            Assert.IsNull(historicJob.JobDefinitionConfiguration);
        }

        [Test][Deployment( new []{ "resources/history/HistoricJobLogTest.TestCatchingSignalEvent.bpmn20.xml", "resources/history/HistoricJobLogTest.TestThrowingSignalEventAsync.bpmn20.xml" }) ]
        public virtual void testCatchingSignalEventActivityId()
        {
            // given + when (1)
            var ProcessInstanceId = runtimeService.StartProcessInstanceByKey("catchSignal")
                .Id;
            runtimeService.StartProcessInstanceByKey("throwSignal");

            var jobId = managementService.CreateJobQuery()
                .First()
                .Id;

            // then (1)

            var historicJob = historyService.CreateHistoricJobLogQuery(c=>c.JobId == jobId && c.State == JobStateFields.Created.StateCode)
                .First();
            Assert.NotNull(historicJob);

            Assert.AreEqual("signalEvent", historicJob.ActivityId);

            // when (2)
            try
            {
                managementService.ExecuteJob(jobId);
                Assert.Fail();
            }
            catch (System.Exception)
            {
                // expected
            }

            // then (2)
            historicJob = historyService.CreateHistoricJobLogQuery(c=>c.JobId == jobId&& c.State == JobStateFields.Failed.StateCode)
                .First();
            Assert.NotNull(historicJob);

            Assert.AreEqual("signalEvent", historicJob.ActivityId);

            // when (3)
            runtimeService.SetVariable(ProcessInstanceId, "Assert.Fail", false);
            managementService.ExecuteJob(jobId);

            // then (3)

            historicJob = historyService.CreateHistoricJobLogQuery(c=>c.JobId == jobId && c.State == JobStateFields.Successful.StateCode)
                .First();
            Assert.NotNull(historicJob);

            Assert.AreEqual("signalEvent", historicJob.ActivityId);
        }

        [Test][Deployment( new []{ "resources/history/HistoricJobLogTest.TestSuperProcessWithCallActivity.bpmn20.xml", "resources/history/HistoricJobLogTest.TestSubProcessWithErrorEndEvent.bpmn20.xml" }) ]
        public virtual void testErrorEndEventInterruptingCallActivity()
        {
            // given
            var id = runtimeService.StartProcessInstanceByKey("process")
                .Id;

            var query = historyService.CreateHistoricJobLogQuery();
            Assert.AreEqual(2, query.Count());

            // serviceTask1
            var serviceTask1JobId = managementService.CreateJobQuery(c=> c.ActivityId =="serviceTask1")
                .First()
                .Id;

            var serviceTask1Query = historyService.CreateHistoricJobLogQuery(c=>c.JobId == serviceTask1JobId);
            var serviceTask1CreatedQuery = historyService.CreateHistoricJobLogQuery(c=>c.JobId == serviceTask1JobId&& c.State == JobStateFields.Created.StateCode);
            var serviceTask1DeletedQuery =
                historyService.CreateHistoricJobLogQuery(
                    c => c.JobId == serviceTask1JobId && c.State == JobStateFields.Deleted.StateCode);
            var serviceTask1SuccessfulQuery = historyService.CreateHistoricJobLogQuery(c=>c.JobId == serviceTask1JobId && c.State == JobStateFields.Successful.StateCode);

            Assert.AreEqual(1, serviceTask1Query.Count());
            Assert.AreEqual(1, serviceTask1CreatedQuery.Count());
            Assert.AreEqual(0, serviceTask1DeletedQuery.Count());
            Assert.AreEqual(0, serviceTask1SuccessfulQuery.Count());

            // serviceTask2
            var serviceTask2JobId = managementService.CreateJobQuery(c=> c.ActivityId =="serviceTask2")
                .First()
                .Id;

            var serviceTask2Query = historyService.CreateHistoricJobLogQuery(c=>c.JobId == serviceTask2JobId);
            var serviceTask2CreatedQuery = historyService.CreateHistoricJobLogQuery(c=>c.JobId == serviceTask2JobId&& c.State == JobStateFields.Created.StateCode);
            var serviceTask2DeletedQuery = historyService.CreateHistoricJobLogQuery(c=>c.JobId == serviceTask2JobId&& c.State == JobStateFields.Deleted.StateCode);
            var serviceTask2SuccessfulQuery = historyService.CreateHistoricJobLogQuery(c=>c.JobId == serviceTask2JobId && c.State == JobStateFields.Successful.StateCode);

            Assert.AreEqual(1, serviceTask2Query.Count());
            Assert.AreEqual(1, serviceTask2CreatedQuery.Count());
            Assert.AreEqual(0, serviceTask2DeletedQuery.Count());
            Assert.AreEqual(0, serviceTask2SuccessfulQuery.Count());

            // when
            managementService.ExecuteJob(serviceTask1JobId);

            // then
            Assert.AreEqual(4, query.Count());

            // serviceTask1
            Assert.AreEqual(2, serviceTask1Query.Count());
            Assert.AreEqual(1, serviceTask1CreatedQuery.Count());
            Assert.AreEqual(0, serviceTask1DeletedQuery.Count());
            Assert.AreEqual(1, serviceTask1SuccessfulQuery.Count());

            var serviceTask1CreatedJobLogEntry = serviceTask1CreatedQuery.First();
            Assert.AreEqual(3, serviceTask1CreatedJobLogEntry.JobRetries);

            var serviceTask1SuccessfulJobLogEntry = serviceTask1SuccessfulQuery.First();
            Assert.AreEqual(3, serviceTask1SuccessfulJobLogEntry.JobRetries);

            // serviceTask2
            Assert.AreEqual(2, serviceTask2Query.Count());
            Assert.AreEqual(1, serviceTask2CreatedQuery.Count());
            Assert.AreEqual(1, serviceTask2DeletedQuery.Count());
            Assert.AreEqual(0, serviceTask2SuccessfulQuery.Count());

            var serviceTask2CreatedJobLogEntry = serviceTask2CreatedQuery.First();
            Assert.AreEqual(3, serviceTask2CreatedJobLogEntry.JobRetries);

            var serviceTask2DeletedJobLogEntry = serviceTask2DeletedQuery.First();
            Assert.AreEqual(3, serviceTask2DeletedJobLogEntry.JobRetries);

            // there should be one task after the boundary event
            Assert.AreEqual(1, taskService.CreateTaskQuery()
                .Count());
        }

        [Test]
        public virtual void testgetJobExceptionStacktraceUnexistingJobId()
        {
            try
            {
                historyService.GetHistoricJobLogExceptionStacktrace("unexistingjob");
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException re)
            {
                AssertTextPresent("No historic job log found with id unexistingjob", re.Message);
            }
        }

        [Test]
        public virtual void testgetJobExceptionStacktraceNullJobId()
        {
            try
            {
                historyService.GetHistoricJobLogExceptionStacktrace(null);
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException re)
            {
                AssertTextPresent("historicJobLogId is null", re.Message);
            }
        }

        /// <summary>
        ///     returns a random of the given Count using characters [0-1]
        /// </summary>
        protected internal static string randomString(int numCharacters)
        {
            //TODO
            return "";
            //return (new System.Numerics.BigInteger(numCharacters, new Random())).ToString(2);
        }


        [Test]
        public virtual void testDeleteByteArray()
        {
            const string processDefinitionId = "myProcessDefition";

            processEngineConfiguration.CommandExecutorTxRequiresNew.Execute(new CommandAnonymousInnerClass(this,
                processDefinitionId));

            Assert.AreEqual(1234, historyService.CreateHistoricJobLogQuery()
                .Count());

            processEngineConfiguration.CommandExecutorTxRequiresNew.Execute(new CommandAnonymousInnerClass2(this,
                processDefinitionId));

            Assert.AreEqual(0, historyService.CreateHistoricJobLogQuery()
                .Count());
        }

        private class CommandAnonymousInnerClass : ICommand<object>
        {
            private readonly HistoricJobLogTest outerInstance;

            private readonly string processDefinitionId;

            public CommandAnonymousInnerClass(HistoricJobLogTest outerInstance, string processDefinitionId)
            {
                this.outerInstance = outerInstance;
                this.processDefinitionId = processDefinitionId;
            }


            public virtual object Execute(CommandContext commandContext)
            {
                for (var i = 0; i < 1234; i++)
                {
                    var log = new HistoricJobLogEventEntity();
                    log.JobId = i.ToString();
                    log.TimeStamp = DateTime.Now;
                    log.JobDefinitionType = MessageEntity.TYPE;
                    log.ProcessDefinitionId = processDefinitionId;


                    var aByteValue = StringUtil.ToByteArray("abc");
                    var byteArray = ExceptionUtil.CreateJobExceptionByteArray(aByteValue);
                    log.ExceptionByteArrayId = byteArray.Id;

                    commandContext.HistoricJobLogManager.Add(log);
                }

                return null;
            }
        }

        private class CommandAnonymousInnerClass2 : ICommand<object>
        {
            private readonly HistoricJobLogTest outerInstance;

            private readonly string processDefinitionId;

            public CommandAnonymousInnerClass2(HistoricJobLogTest outerInstance, string processDefinitionId)
            {
                this.outerInstance = outerInstance;
                this.processDefinitionId = processDefinitionId;
            }


            public virtual object Execute(CommandContext commandContext)
            {
                commandContext.HistoricJobLogManager.DeleteHistoricJobLogsByProcessDefinitionId(processDefinitionId);
                return null;
            }
        }

        [Test]
        [Deployment("resources/history/HistoricJobLogTest.TestAsyncContinuation.bpmn20.xml")]
        public virtual void testAsyncAfterJobHandlerType()
        {
            runtimeService.StartProcessInstanceByKey("process", ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                .PutValue("Assert.Fail", false));

            var job = managementService.CreateJobQuery()
                .First();

            managementService.ExecuteJob(job.Id);

            var anotherJob = managementService.CreateJobQuery()
                .First();

            Assert.IsFalse(job.Id.Equals(anotherJob.Id));

            var historicJob = historyService.CreateHistoricJobLogQuery(c=>c.JobId == anotherJob.Id)
                .First();

            Assert.NotNull(historicJob);

            Assert.IsNull(historicJob.JobDueDate);

            Assert.AreEqual(anotherJob.JobDefinitionId, historicJob.JobDefinitionId);
            Assert.AreEqual("serviceTask", historicJob.ActivityId);
            Assert.AreEqual(AsyncContinuationJobHandler.TYPE, historicJob.JobDefinitionType);
            Assert.AreEqual(MessageJobDeclaration.AsyncAfter, historicJob.JobDefinitionConfiguration);
        }

        [Test]
        [Deployment("resources/history/HistoricJobLogTest.TestAsyncContinuation.bpmn20.xml")]
        public virtual void testAsyncBeforeJobHandlerType()
        {
            runtimeService.StartProcessInstanceByKey("process");

            var job = managementService.CreateJobQuery()
                .First();

            var historicJob = historyService.CreateHistoricJobLogQuery(c=>c.JobId == job.Id)
                .First();

            Assert.NotNull(historicJob);

            Assert.IsNull(historicJob.JobDueDate);

            Assert.AreEqual(job.JobDefinitionId, historicJob.JobDefinitionId);
            Assert.AreEqual("serviceTask", historicJob.ActivityId);
            Assert.AreEqual(AsyncContinuationJobHandler.TYPE, historicJob.JobDefinitionType);
            Assert.AreEqual(MessageJobDeclaration.AsyncBefore, historicJob.JobDefinitionConfiguration);
        }

        [Test]
        [Deployment("resources/history/HistoricJobLogTest.TestAsyncContinuation.bpmn20.xml")]
        public virtual void testCreateHistoricJobLogProperties()
        {
            runtimeService.StartProcessInstanceByKey("process");

            var job = managementService.CreateJobQuery()
                .First();

            var historicJob = historyService.CreateHistoricJobLogQuery()
                //.CreationLog()
                .First();
            Assert.NotNull(historicJob);

            Assert.NotNull(historicJob.TimeStamp);

            Assert.IsNull(historicJob.JobExceptionMessage);

            Assert.AreEqual(job.Id, historicJob.JobId);
            Assert.AreEqual(job.JobDefinitionId, historicJob.JobDefinitionId);
            Assert.AreEqual("serviceTask", historicJob.ActivityId);
            Assert.AreEqual(AsyncContinuationJobHandler.TYPE, historicJob.JobDefinitionType);
            Assert.AreEqual(MessageJobDeclaration.AsyncBefore, historicJob.JobDefinitionConfiguration);
            Assert.AreEqual(job.Duedate, historicJob.JobDueDate);
            Assert.AreEqual(job.Retries, historicJob.JobRetries);
            Assert.AreEqual(job.ExecutionId, historicJob.ExecutionId);
            Assert.AreEqual(job.ProcessInstanceId, historicJob.ProcessInstanceId);
            Assert.AreEqual(job.ProcessDefinitionId, historicJob.ProcessDefinitionId);
            Assert.AreEqual(job.ProcessDefinitionKey, historicJob.ProcessDefinitionKey);
            Assert.AreEqual(job.DeploymentId, historicJob.DeploymentId);
            Assert.AreEqual(job.Priority, historicJob.JobPriority);

            Assert.True(historicJob.CreationLog);
            Assert.IsFalse(historicJob.FailureLog);
            Assert.IsFalse(historicJob.SuccessLog);
            Assert.IsFalse(historicJob.DeletionLog);
        }

        [Test]
        [Deployment("resources/history/HistoricJobLogTest.TestAsyncContinuation.bpmn20.xml")]
        public virtual void testDeletedHistoricJobLogProperties()
        {
            var ProcessInstanceId = runtimeService.StartProcessInstanceByKey("process")
                .Id;

            var job = managementService.CreateJobQuery()
                .First();

            runtimeService.DeleteProcessInstance(ProcessInstanceId, null);

            var historicJob = historyService.CreateHistoricJobLogQuery()
                //.DeletionLog()
                .First();
            Assert.NotNull(historicJob);

            Assert.NotNull(historicJob.TimeStamp);

            Assert.IsNull(historicJob.JobExceptionMessage);

            Assert.AreEqual(job.Id, historicJob.JobId);
            Assert.AreEqual(job.JobDefinitionId, historicJob.JobDefinitionId);
            Assert.AreEqual("serviceTask", historicJob.ActivityId);
            Assert.AreEqual(AsyncContinuationJobHandler.TYPE, historicJob.JobDefinitionType);
            Assert.AreEqual(MessageJobDeclaration.AsyncBefore, historicJob.JobDefinitionConfiguration);
            Assert.AreEqual(job.Duedate, historicJob.JobDueDate);
            Assert.AreEqual(job.Retries, historicJob.JobRetries);
            Assert.AreEqual(job.ExecutionId, historicJob.ExecutionId);
            Assert.AreEqual(job.ProcessInstanceId, historicJob.ProcessInstanceId);
            Assert.AreEqual(job.ProcessDefinitionId, historicJob.ProcessDefinitionId);
            Assert.AreEqual(job.ProcessDefinitionKey, historicJob.ProcessDefinitionKey);
            Assert.AreEqual(job.DeploymentId, historicJob.DeploymentId);
            Assert.AreEqual(job.Priority, historicJob.JobPriority);

            Assert.IsFalse(historicJob.CreationLog);
            Assert.IsFalse(historicJob.FailureLog);
            Assert.IsFalse(historicJob.SuccessLog);
            Assert.True(historicJob.DeletionLog);
        }

        [Test]
        [Deployment("resources/history/HistoricJobLogTest.TestAsyncContinuation.bpmn20.xml")]
        public virtual void testDeletedJob()
        {
            // given
            runtimeService.StartProcessInstanceByKey("process");

            var jobId = managementService.CreateJobQuery()
                .First()
                .Id;

            var query = historyService.CreateHistoricJobLogQuery(c=>c.JobId == jobId);
            var createdQuery = historyService.CreateHistoricJobLogQuery(c=>c.JobId == jobId&& c.State == JobStateFields.Created.StateCode);
            var deletedQuery = historyService.CreateHistoricJobLogQuery(c=>c.JobId == jobId&& c.State == JobStateFields.Deleted.StateCode);

            // there exists one historic job log entry
            Assert.AreEqual(1, query.Count());
            Assert.AreEqual(1, createdQuery.Count());
            Assert.AreEqual(0, deletedQuery.Count());

            // when
            managementService.DeleteJob(jobId);

            // then
            Assert.AreEqual(2, query.Count());
            Assert.AreEqual(1, createdQuery.Count());
            Assert.AreEqual(1, deletedQuery.Count());

            var createdJobLogEntry = createdQuery.First();
            Assert.AreEqual(3, createdJobLogEntry.JobRetries);

            var deletedJobLogEntry = deletedQuery.First();
            Assert.AreEqual(3, deletedJobLogEntry.JobRetries);
        }

        [Test]
        [Deployment("resources/history/HistoricJobLogTest.TestAsyncContinuation.bpmn20.xml")]
        public virtual void testDeletedProcessInstance()
        {
            // given
            var ProcessInstanceId = runtimeService.StartProcessInstanceByKey("process")
                .Id;

            var jobId = managementService.CreateJobQuery()
                .First()
                .Id;

            var query = historyService.CreateHistoricJobLogQuery(c=>c.JobId == jobId);
            var createdQuery = historyService.CreateHistoricJobLogQuery(c=>c.JobId == jobId&& c.State == JobStateFields.Created.StateCode);
            var deletedQuery = historyService.CreateHistoricJobLogQuery(c=>c.JobId == jobId&& c.State == JobStateFields.Deleted.StateCode);

            // there exists one historic job log entry
            Assert.AreEqual(1, query.Count());
            Assert.AreEqual(1, createdQuery.Count());
            Assert.AreEqual(0, deletedQuery.Count());

            // when
            runtimeService.DeleteProcessInstance(ProcessInstanceId, null);

            // then
            Assert.AreEqual(2, query.Count());
            Assert.AreEqual(1, createdQuery.Count());
            Assert.AreEqual(1, deletedQuery.Count());

            var createdJobLogEntry = createdQuery.First();
            Assert.AreEqual(3, createdJobLogEntry.JobRetries);

            var deletedJobLogEntry = deletedQuery.First();
            Assert.AreEqual(3, deletedJobLogEntry.JobRetries);
        }

        [Test]
        [Deployment]
        public virtual void testDifferentExceptions()
        {
            // given
            var ProcessInstanceId = runtimeService.StartProcessInstanceByKey("process")
                .Id;

            var jobId = managementService.CreateJobQuery()
                .First()
                .Id;

            // when (1)
            try
            {
                managementService.ExecuteJob(jobId);
                Assert.Fail();
            }
            catch (System.Exception)
            {
                // expected
            }

            // then (1)
            var serviceTask1FailedHistoricJobLog = historyService.CreateHistoricJobLogQuery()
                //.FailureLog()
                .First();

            var serviceTask1FailedHistoricJobLogId = serviceTask1FailedHistoricJobLog.Id;

            Assert.AreEqual(FirstFailingDelegate.FIRST_EXCEPTION_MESSAGE,
                serviceTask1FailedHistoricJobLog.JobExceptionMessage);

            var serviceTask1Stacktrace =
                historyService.GetHistoricJobLogExceptionStacktrace(serviceTask1FailedHistoricJobLogId);
            Assert.NotNull(serviceTask1Stacktrace);
            AssertTextPresent(FirstFailingDelegate.FIRST_EXCEPTION_MESSAGE, serviceTask1Stacktrace);
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
            AssertTextPresent(typeof(FirstFailingDelegate).FullName, serviceTask1Stacktrace);

            // when (2)
            runtimeService.SetVariable(ProcessInstanceId, "firstFail", false);
            try
            {
                managementService.ExecuteJob(jobId);
                Assert.Fail();
            }
            catch (System.Exception)
            {
                // expected
            }

            // then (2)
            var serviceTask2FailedHistoricJobLog = historyService.CreateHistoricJobLogQuery(c=>c.State == JobStateFields.Failed.StateCode)
                //.OrderByJobRetries()
                ///*.Desc()*/
                
                .ToList()[1];

            var serviceTask2FailedHistoricJobLogId = serviceTask2FailedHistoricJobLog.Id;

            Assert.AreEqual(SecondFailingDelegate.SECOND_EXCEPTION_MESSAGE,
                serviceTask2FailedHistoricJobLog.JobExceptionMessage);

            var serviceTask2Stacktrace =
                historyService.GetHistoricJobLogExceptionStacktrace(serviceTask2FailedHistoricJobLogId);
            Assert.NotNull(serviceTask2Stacktrace);
            AssertTextPresent(SecondFailingDelegate.SECOND_EXCEPTION_MESSAGE, serviceTask2Stacktrace);
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
            AssertTextPresent(typeof(SecondFailingDelegate).FullName, serviceTask2Stacktrace);

            Assert.IsFalse(serviceTask1Stacktrace.Equals(serviceTask2Stacktrace));
        }

        [Test]
        [Deployment("resources/history/HistoricJobLogTest.TestAsyncContinuation.bpmn20.xml")]
        public virtual void testExceptionStacktrace()
        {
            // given
            runtimeService.StartProcessInstanceByKey("process");

            var jobId = managementService.CreateJobQuery()
                .First()
                .Id;

            // when
            try
            {
                managementService.ExecuteJob(jobId);
                Assert.Fail();
            }
            catch (System.Exception)
            {
                // expected
            }

            // then
            var failedHistoricJobLogId = historyService.CreateHistoricJobLogQuery(c=>c.State == JobStateFields.Failed.StateCode)
                .First()
                .Id;

            var stacktrace = historyService.GetHistoricJobLogExceptionStacktrace(failedHistoricJobLogId);
            Assert.NotNull(stacktrace);
            AssertTextPresent(FailingDelegate.EXCEPTION_MESSAGE, stacktrace);
        }

        [Test]
        [Deployment("resources/history/HistoricJobLogTest.TestAsyncContinuation.bpmn20.xml")]
        public virtual void testFailedHistoricJobLogProperties()
        {
            runtimeService.StartProcessInstanceByKey("process");

            var job = managementService.CreateJobQuery()
                .First();

            try
            {
                managementService.ExecuteJob(job.Id);
                Assert.Fail();
            }
            catch (System.Exception)
            {
                // expected
            }

            var historicJob = historyService.CreateHistoricJobLogQuery(c=>c.State == JobStateFields.Failed.StateCode)
                .First();
            Assert.NotNull(historicJob);

            Assert.NotNull(historicJob.TimeStamp);

            Assert.AreEqual(job.Id, historicJob.JobId);
            Assert.AreEqual(job.JobDefinitionId, historicJob.JobDefinitionId);
            Assert.AreEqual("serviceTask", historicJob.ActivityId);
            Assert.AreEqual(AsyncContinuationJobHandler.TYPE, historicJob.JobDefinitionType);
            Assert.AreEqual(MessageJobDeclaration.AsyncBefore, historicJob.JobDefinitionConfiguration);
            Assert.AreEqual(job.Duedate, historicJob.JobDueDate);
            Assert.AreEqual(3, historicJob.JobRetries);
            Assert.AreEqual(job.ExecutionId, historicJob.ExecutionId);
            Assert.AreEqual(job.ProcessInstanceId, historicJob.ProcessInstanceId);
            Assert.AreEqual(job.ProcessDefinitionId, historicJob.ProcessDefinitionId);
            Assert.AreEqual(job.ProcessDefinitionKey, historicJob.ProcessDefinitionKey);
            Assert.AreEqual(job.DeploymentId, historicJob.DeploymentId);
            Assert.AreEqual(FailingDelegate.EXCEPTION_MESSAGE, historicJob.JobExceptionMessage);
            Assert.AreEqual(job.Priority, historicJob.JobPriority);

            Assert.IsFalse(historicJob.CreationLog);
            Assert.True(historicJob.FailureLog);
            Assert.IsFalse(historicJob.SuccessLog);
            Assert.IsFalse(historicJob.DeletionLog);
        }

        [Test]
        [Deployment("resources/history/HistoricJobLogTest.TestAsyncContinuation.bpmn20.xml")]
        public virtual void testFailedJobEvents()
        {
            // given
            runtimeService.StartProcessInstanceByKey("process");

            var jobId = managementService.CreateJobQuery()
                .First()
                .Id;

            var query = historyService.CreateHistoricJobLogQuery(c=>c.JobId == jobId);
            var createdQuery = historyService.CreateHistoricJobLogQuery(c=>c.JobId == jobId&& c.State == JobStateFields.Created.StateCode);
            var failedQuery = historyService.CreateHistoricJobLogQuery(c=>c.JobId == jobId && c.State == JobStateFields.Failed.StateCode)
                //.OrderByJobRetries()
                ///*.Desc()*/
                ;

            // there exists one historic job log entry
            Assert.AreEqual(1, query.Count());
            Assert.AreEqual(1, createdQuery.Count());
            Assert.AreEqual(0, failedQuery.Count());

            // when (1)
            try
            {
                managementService.ExecuteJob(jobId);
                Assert.Fail();
            }
            catch (System.Exception)
            {
                // expected
            }

            // then (1)
            Assert.AreEqual(2, query.Count());
            Assert.AreEqual(1, createdQuery.Count());
            Assert.AreEqual(1, failedQuery.Count());

            var createdJobLogEntry = createdQuery.First();
            Assert.AreEqual(3, createdJobLogEntry.JobRetries);

            var failedJobLogEntry = failedQuery.First();
            Assert.AreEqual(3, failedJobLogEntry.JobRetries);

            // when (2)
            try
            {
                managementService.ExecuteJob(jobId);
                Assert.Fail();
            }
            catch (System.Exception)
            {
                // expected
            }

            // then (2)
            Assert.AreEqual(3, query.Count());
            Assert.AreEqual(1, createdQuery.Count());
            Assert.AreEqual(2, failedQuery.Count());

            createdJobLogEntry = createdQuery.First();
            Assert.AreEqual(3, createdJobLogEntry.JobRetries);

            failedJobLogEntry = failedQuery
                .First();
            Assert.AreEqual(3, failedJobLogEntry.JobRetries);

            failedJobLogEntry = failedQuery
                .ToList()[1];
            Assert.AreEqual(2, failedJobLogEntry.JobRetries);

            // when (3)
            try
            {
                managementService.ExecuteJob(jobId);
                Assert.Fail();
            }
            catch (System.Exception)
            {
                // expected
            }

            // then (3)
            Assert.AreEqual(4, query.Count());
            Assert.AreEqual(1, createdQuery.Count());
            Assert.AreEqual(3, failedQuery.Count());

            createdJobLogEntry = createdQuery.First();
            Assert.AreEqual(3, createdJobLogEntry.JobRetries);

            failedJobLogEntry = failedQuery
                .First();
            Assert.AreEqual(3, failedJobLogEntry.JobRetries);

            failedJobLogEntry = failedQuery
                .ToList()[1];
            Assert.AreEqual(2, failedJobLogEntry.JobRetries);

            failedJobLogEntry = failedQuery
                .ToList()[2];
            Assert.AreEqual(1, failedJobLogEntry.JobRetries);

            // when (4)
            try
            {
                managementService.ExecuteJob(jobId);
                Assert.Fail();
            }
            catch (System.Exception)
            {
                // expected
            }

            // then (4)
            Assert.AreEqual(5, query.Count());
            Assert.AreEqual(1, createdQuery.Count());
            Assert.AreEqual(4, failedQuery.Count());

            createdJobLogEntry = createdQuery.First();
            Assert.AreEqual(3, createdJobLogEntry.JobRetries);

            failedJobLogEntry = failedQuery
                .First();
            Assert.AreEqual(3, failedJobLogEntry.JobRetries);

            failedJobLogEntry = failedQuery
                .ToList()[1];
            Assert.AreEqual(2, failedJobLogEntry.JobRetries);

            failedJobLogEntry = failedQuery
                .ToList()[2];
            Assert.AreEqual(1, failedJobLogEntry.JobRetries);

            failedJobLogEntry = failedQuery
                .ToList()[3];
            Assert.AreEqual(0, failedJobLogEntry.JobRetries);
        }

        [Test]
        [Deployment("resources/history/HistoricJobLogTest.TestAsyncContinuation.bpmn20.xml")]
        public virtual void testFailedJobEventsExecutedByJobExecutor()
        {
            // given
            runtimeService.StartProcessInstanceByKey("process");

            var jobId = managementService.CreateJobQuery()
                .First()
                .Id;

            var query = historyService.CreateHistoricJobLogQuery(c=>c.JobId == jobId);
            var createdQuery = historyService.CreateHistoricJobLogQuery(c=>c.JobId == jobId&& c.State == JobStateFields.Created.StateCode);
            var failedQuery =
                historyService.CreateHistoricJobLogQuery(
                    c => c.JobId == jobId && c.State == JobStateFields.Failed.StateCode);
                //.OrderByJobRetries()
                ///*.Desc()*/;

            // there exists one historic job log entry
            Assert.AreEqual(1, query.Count());
            Assert.AreEqual(1, createdQuery.Count());
            Assert.AreEqual(0, failedQuery.Count());

            // when (1)
            ExecuteAvailableJobs();

            // then (1)
            Assert.AreEqual(4, query.Count());
            Assert.AreEqual(1, createdQuery.Count());
            Assert.AreEqual(3, failedQuery.Count());

            var createdJobLogEntry = createdQuery.First();
            Assert.AreEqual(3, createdJobLogEntry.JobRetries);

            var failedJobLogEntry = failedQuery
                .First();
            Assert.AreEqual(3, failedJobLogEntry.JobRetries);

            failedJobLogEntry = failedQuery
                .ToList()[1];
            Assert.AreEqual(2, failedJobLogEntry.JobRetries);

            failedJobLogEntry = failedQuery
                .ToList()[2];
            Assert.AreEqual(1, failedJobLogEntry.JobRetries);

            // when (2)
            try
            {
                managementService.ExecuteJob(jobId);
                Assert.Fail();
            }
            catch (System.Exception)
            {
                // expected
            }

            // then (2)
            Assert.AreEqual(5, query.Count());
            Assert.AreEqual(1, createdQuery.Count());
            Assert.AreEqual(4, failedQuery.Count());

            createdJobLogEntry = createdQuery.First();
            Assert.AreEqual(3, createdJobLogEntry.JobRetries);

            failedJobLogEntry = failedQuery
                .First();
            Assert.AreEqual(3, failedJobLogEntry.JobRetries);

            failedJobLogEntry = failedQuery
                .ToList()[1];
            Assert.AreEqual(2, failedJobLogEntry.JobRetries);

            failedJobLogEntry = failedQuery
                .ToList()[2];
            Assert.AreEqual(1, failedJobLogEntry.JobRetries);

            failedJobLogEntry = failedQuery
                .ToList()[3];
            Assert.AreEqual(0, failedJobLogEntry.JobRetries);
        }

        [Test]
        [Deployment("resources/history/HistoricJobLogTest.TestAsyncContinuation.bpmn20.xml")]
        public virtual void testSuccessfulAndFailedJobEvents()
        {
            // given
            var ProcessInstanceId = runtimeService.StartProcessInstanceByKey("process")
                .Id;

            var jobId = managementService.CreateJobQuery()
                .First()
                .Id;

            var query = historyService.CreateHistoricJobLogQuery(c=>c.JobId == jobId);
            var createdQuery = historyService.CreateHistoricJobLogQuery(c=>c.JobId == jobId&& c.State == JobStateFields.Created.StateCode);
            var failedQuery =
                historyService.CreateHistoricJobLogQuery(
                    c => c.JobId == jobId && c.State == JobStateFields.Failed.StateCode);
                //.OrderByJobRetries()
                ///*.Desc()*/;
            var succeededQuery = historyService.CreateHistoricJobLogQuery(c=>c.JobId == jobId && c.State == JobStateFields.Successful.StateCode);

            // there exists one historic job log entry
            Assert.AreEqual(1, query.Count());
            Assert.AreEqual(1, createdQuery.Count());
            Assert.AreEqual(0, failedQuery.Count());
            Assert.AreEqual(0, succeededQuery.Count());

            // when (1)
            try
            {
                managementService.ExecuteJob(jobId);
                Assert.Fail();
            }
            catch (System.Exception)
            {
                // expected
            }

            // then (1)
            Assert.AreEqual(2, query.Count());
            Assert.AreEqual(1, createdQuery.Count());
            Assert.AreEqual(1, failedQuery.Count());
            Assert.AreEqual(0, succeededQuery.Count());

            var createdJobLogEntry = createdQuery.First();
            Assert.AreEqual(3, createdJobLogEntry.JobRetries);

            var failedJobLogEntry = failedQuery.First();
            Assert.AreEqual(3, failedJobLogEntry.JobRetries);

            // when (2)
            try
            {
                managementService.ExecuteJob(jobId);
                Assert.Fail();
            }
            catch (System.Exception)
            {
                // expected
            }

            // then (2)
            Assert.AreEqual(3, query.Count());
            Assert.AreEqual(1, createdQuery.Count());
            Assert.AreEqual(2, failedQuery.Count());
            Assert.AreEqual(0, succeededQuery.Count());

            createdJobLogEntry = createdQuery.First();
            Assert.AreEqual(3, createdJobLogEntry.JobRetries);

            failedJobLogEntry = failedQuery
                .First();
            Assert.AreEqual(3, failedJobLogEntry.JobRetries);

            failedJobLogEntry = failedQuery
                .ToList()[1];
            Assert.AreEqual(2, failedJobLogEntry.JobRetries);

            // when (3)
            runtimeService.SetVariable(ProcessInstanceId, "Assert.Fail", false);
            managementService.ExecuteJob(jobId);

            // then (3)
            Assert.AreEqual(4, query.Count());
            Assert.AreEqual(1, createdQuery.Count());
            Assert.AreEqual(2, failedQuery.Count());
            Assert.AreEqual(1, succeededQuery.Count());

            createdJobLogEntry = createdQuery.First();
            Assert.AreEqual(3, createdJobLogEntry.JobRetries);

            failedJobLogEntry = failedQuery
                .First();
            Assert.AreEqual(3, failedJobLogEntry.JobRetries);

            failedJobLogEntry = failedQuery
                .ToList()[1];
            Assert.AreEqual(2, failedJobLogEntry.JobRetries);

            var succeededJobLogEntry = succeededQuery.First();
            Assert.AreEqual(1, succeededJobLogEntry.JobRetries);
        }

        [Test]
        [Deployment("resources/history/HistoricJobLogTest.TestAsyncContinuation.bpmn20.xml")]
        public virtual void testSuccessfulHistoricJobLogProperties()
        {
            runtimeService.StartProcessInstanceByKey("process", ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                .PutValue("Assert.Fail", false));

            var job = managementService.CreateJobQuery()
                .First();

            managementService.ExecuteJob(job.Id);

            var historicJob = historyService.CreateHistoricJobLogQuery(c=>c.State == JobStateFields.Successful.StateCode)
                .First();
            Assert.NotNull(historicJob);

            Assert.NotNull(historicJob.TimeStamp);

            Assert.IsNull(historicJob.JobExceptionMessage);

            Assert.AreEqual(job.Id, historicJob.JobId);
            Assert.AreEqual(job.JobDefinitionId, historicJob.JobDefinitionId);
            Assert.AreEqual("serviceTask", historicJob.ActivityId);
            Assert.AreEqual(AsyncContinuationJobHandler.TYPE, historicJob.JobDefinitionType);
            Assert.AreEqual(MessageJobDeclaration.AsyncBefore, historicJob.JobDefinitionConfiguration);
            Assert.AreEqual(job.Duedate, historicJob.JobDueDate);
            Assert.AreEqual(job.Retries, historicJob.JobRetries);
            Assert.AreEqual(job.ExecutionId, historicJob.ExecutionId);
            Assert.AreEqual(job.ProcessInstanceId, historicJob.ProcessInstanceId);
            Assert.AreEqual(job.ProcessDefinitionId, historicJob.ProcessDefinitionId);
            Assert.AreEqual(job.ProcessDefinitionKey, historicJob.ProcessDefinitionKey);
            Assert.AreEqual(job.DeploymentId, historicJob.DeploymentId);
            Assert.AreEqual(job.Priority, historicJob.JobPriority);

            Assert.IsFalse(historicJob.CreationLog);
            Assert.IsFalse(historicJob.FailureLog);
            Assert.True(historicJob.SuccessLog);
            Assert.IsFalse(historicJob.DeletionLog);
        }

        [Test]
        [Deployment("resources/history/HistoricJobLogTest.TestAsyncContinuation.bpmn20.xml")]
        public virtual void testSuccessfulJobEvent()
        {
            // given
            runtimeService.StartProcessInstanceByKey("process", ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                .PutValue("Assert.Fail", false));

            var jobId = managementService.CreateJobQuery()
                .First()
                .Id;

            var query = historyService.CreateHistoricJobLogQuery(c=>c.JobId == jobId);
            var createdQuery = historyService.CreateHistoricJobLogQuery(c=>c.JobId == jobId&& c.State == JobStateFields.Created.StateCode);
            var succeededQuery = historyService.CreateHistoricJobLogQuery(c=>c.JobId == jobId && c.State == JobStateFields.Successful.StateCode);

            // there exists one historic job log entry
            Assert.AreEqual(1, query.Count());
            Assert.AreEqual(1, createdQuery.Count());
            Assert.AreEqual(0, succeededQuery.Count());

            // when
            managementService.ExecuteJob(jobId);

            // then
            Assert.AreEqual(2, query.Count());
            Assert.AreEqual(1, createdQuery.Count());
            Assert.AreEqual(1, succeededQuery.Count());

            var createdJobLogEntry = createdQuery.First();
            Assert.AreEqual(3, createdJobLogEntry.JobRetries);

            var succeededJobLogEntry = succeededQuery.First();
            Assert.AreEqual(3, succeededJobLogEntry.JobRetries);
        }

        [Test]
        [Deployment("resources/history/HistoricJobLogTest.TestAsyncContinuation.bpmn20.xml")]
        public virtual void testSuccessfulJobEventExecutedByJobExecutor()
        {
            // given
            runtimeService.StartProcessInstanceByKey("process", ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                .PutValue("Assert.Fail", false));

            var jobId = managementService.CreateJobQuery()
                .First()
                .Id;

            var query = historyService.CreateHistoricJobLogQuery(c=>c.JobId == jobId);
            var createdQuery = historyService.CreateHistoricJobLogQuery(c=>c.JobId == jobId&& c.State == JobStateFields.Created.StateCode);
            var succeededQuery = historyService.CreateHistoricJobLogQuery(c=>c.JobId == jobId && c.State == JobStateFields.Successful.StateCode);

            // there exists one historic job log entry
            Assert.AreEqual(1, query.Count());
            Assert.AreEqual(1, createdQuery.Count());
            Assert.AreEqual(0, succeededQuery.Count());

            // when
            ExecuteAvailableJobs();

            // then
            Assert.AreEqual(2, query.Count());
            Assert.AreEqual(1, createdQuery.Count());
            Assert.AreEqual(1, succeededQuery.Count());

            var createdJobLogEntry = createdQuery.First();
            Assert.AreEqual(3, createdJobLogEntry.JobRetries);

            var succeededJobLogEntry = succeededQuery.First();
            Assert.AreEqual(3, succeededJobLogEntry.JobRetries);
        }

        [Test]
        [Deployment]
        public virtual void testTerminateEndEvent()
        {
            // given
            var id = runtimeService.StartProcessInstanceByKey("process")
                .Id;

            var serviceTask1JobId = managementService.CreateJobQuery(c=> c.ActivityId =="serviceTask1")
                .First()
                .Id;

            var query = historyService.CreateHistoricJobLogQuery();
            Assert.AreEqual(2, query.Count());

            // serviceTask1
            var serviceTask1Query = historyService.CreateHistoricJobLogQuery(c=>c.JobId == serviceTask1JobId);
            var serviceTask1CreatedQuery = historyService.CreateHistoricJobLogQuery(c=>c.JobId == serviceTask1JobId&& c.State == JobStateFields.Created.StateCode);
            var serviceTask1DeletedQuery = historyService.CreateHistoricJobLogQuery(c=>c.JobId == serviceTask1JobId&& c.State == JobStateFields.Deleted.StateCode);
            var serviceTask1SuccessfulQuery = historyService.CreateHistoricJobLogQuery(c=>c.JobId == serviceTask1JobId && c.State == JobStateFields.Successful.StateCode);

            Assert.AreEqual(1, serviceTask1Query.Count());
            Assert.AreEqual(1, serviceTask1CreatedQuery.Count());
            Assert.AreEqual(0, serviceTask1DeletedQuery.Count());
            Assert.AreEqual(0, serviceTask1SuccessfulQuery.Count());

            // serviceTask2
            var serviceTask2JobId = managementService.CreateJobQuery(c=> c.ActivityId =="serviceTask2")
                .First()
                .Id;

            var serviceTask2Query = historyService.CreateHistoricJobLogQuery(c=>c.JobId == serviceTask2JobId);
            var serviceTask2CreatedQuery = historyService.CreateHistoricJobLogQuery(c=>c.JobId == serviceTask2JobId&& c.State == JobStateFields.Created.StateCode);
            var serviceTask2DeletedQuery = historyService.CreateHistoricJobLogQuery(c=>c.JobId == serviceTask2JobId&& c.State == JobStateFields.Deleted.StateCode);
            var serviceTask2SuccessfulQuery = historyService.CreateHistoricJobLogQuery(c=>c.JobId == serviceTask2JobId && c.State == JobStateFields.Successful.StateCode);

            Assert.AreEqual(1, serviceTask2Query.Count());
            Assert.AreEqual(1, serviceTask2CreatedQuery.Count());
            Assert.AreEqual(0, serviceTask2DeletedQuery.Count());
            Assert.AreEqual(0, serviceTask2SuccessfulQuery.Count());

            // when
            managementService.ExecuteJob(serviceTask1JobId);

            // then
            Assert.AreEqual(4, query.Count());

            // serviceTas1
            Assert.AreEqual(2, serviceTask1Query.Count());
            Assert.AreEqual(1, serviceTask1CreatedQuery.Count());
            Assert.AreEqual(0, serviceTask1DeletedQuery.Count());
            Assert.AreEqual(1, serviceTask1SuccessfulQuery.Count());

            var serviceTask1CreatedJobLogEntry = serviceTask1CreatedQuery.First();
            Assert.AreEqual(3, serviceTask1CreatedJobLogEntry.JobRetries);

            var serviceTask1SuccessfulJobLogEntry = serviceTask1SuccessfulQuery.First();
            Assert.AreEqual(3, serviceTask1SuccessfulJobLogEntry.JobRetries);

            // serviceTask2
            Assert.AreEqual(2, serviceTask2Query.Count());
            Assert.AreEqual(1, serviceTask2CreatedQuery.Count());
            Assert.AreEqual(1, serviceTask2DeletedQuery.Count());
            Assert.AreEqual(0, serviceTask2SuccessfulQuery.Count());

            var serviceTask2CreatedJobLogEntry = serviceTask2CreatedQuery.First();
            Assert.AreEqual(3, serviceTask2CreatedJobLogEntry.JobRetries);

            var serviceTask2DeletedJobLogEntry = serviceTask2DeletedQuery.First();
            Assert.AreEqual(3, serviceTask2DeletedJobLogEntry.JobRetries);
        }

        [Test]
        [Deployment]
        public virtual void testThrowExceptionMessageTruncation()
        {
            // given
            var exceptionMessage = randomString(10000);
            var @delegate = new ThrowExceptionWithOverlongMessageDelegate(exceptionMessage);

            runtimeService.StartProcessInstanceByKey("process", ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                .PutValue("delegate", @delegate));
            var job = managementService.CreateJobQuery()
                .First();

            // when
            try
            {
                managementService.ExecuteJob(job.Id);
                Assert.Fail();
            }
            catch (System.Exception)
            {
                // expected
            }

            // then
            var failedHistoricJobLog = historyService.CreateHistoricJobLogQuery(c=>c.State == JobStateFields.Failed.StateCode)
                .First();

            Assert.NotNull(failedHistoricJobLog);
            Assert.AreEqual(exceptionMessage.Substring(0, JobEntity.MaxExceptionMessageLength),
                failedHistoricJobLog.JobExceptionMessage);
        }

        [Test]
        [Deployment]
        public virtual void testThrowExceptionWithoutMessage()
        {
            // given
            var id = runtimeService.StartProcessInstanceByKey("process")
                .Id;

            var jobId = managementService.CreateJobQuery()
                .First()
                .Id;

            // when
            try
            {
                managementService.ExecuteJob(jobId);
                Assert.Fail();
            }
            catch (System.Exception)
            {
                // expected
            }

            // then
            var failedHistoricJobLog = historyService.CreateHistoricJobLogQuery(c=>c.State == JobStateFields.Failed.StateCode)
                .First();

            var failedHistoricJobLogId = failedHistoricJobLog.Id;

            Assert.IsNull(failedHistoricJobLog.JobExceptionMessage);

            var stacktrace = historyService.GetHistoricJobLogExceptionStacktrace(failedHistoricJobLogId);
            Assert.NotNull(stacktrace);
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
            AssertTextPresent(typeof(ThrowExceptionWithoutMessageDelegate).FullName, stacktrace);
        }
    }
}