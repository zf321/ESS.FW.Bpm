using System;
using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Parser;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.Async
{

    [TestFixture]
    public class FoxJobRetryCmdTest : PluggableProcessEngineTestCase
    {

        [Test]
        [Deployment(new string[] { "resources/bpmn/async/FoxJobRetryCmdTest.TestFailedServiceTask.bpmn20.xml" })]
        public virtual void testFailedServiceTask()
        {
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("failedServiceTask");

            AssertJobRetriesForActivity(pi, "failingServiceTask");
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/async/FoxJobRetryCmdTest.TestFailedUserTask.bpmn20.xml" })]
        public virtual void testFailedUserTask()
        {
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("failedUserTask");

            AssertJobRetriesForActivity(pi, "failingUserTask");
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/async/FoxJobRetryCmdTest.TestFailedBusinessRuleTask.bpmn20.xml" })]
        public virtual void testFailedBusinessRuleTask()
        {
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("failedBusinessRuleTask");

            AssertJobRetriesForActivity(pi, "failingBusinessRuleTask");
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/async/FoxJobRetryCmdTest.TestFailedCallActivity.bpmn20.xml" })]
        public virtual void testFailedCallActivity()
        {
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("failedCallActivity");

            AssertJobRetriesForActivity(pi, "failingCallActivity");
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/async/FoxJobRetryCmdTest.TestFailedScriptTask.bpmn20.xml" })]
        public virtual void testFailedScriptTask()
        {
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("failedScriptTask");

            AssertJobRetriesForActivity(pi, "failingScriptTask");
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/async/FoxJobRetryCmdTest.TestFailedSendTask.bpmn20.xml" })]
        public virtual void testFailedSendTask()
        {
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("failedSendTask");

            AssertJobRetriesForActivity(pi, "failingSendTask");
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/async/FoxJobRetryCmdTest.TestFailedSubProcess.bpmn20.xml" })]
        public virtual void testFailedSubProcess()
        {
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("failedSubProcess");

            AssertJobRetriesForActivity(pi, "failingSubProcess");
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/async/FoxJobRetryCmdTest.TestFailedTask.bpmn20.xml" })]
        public virtual void testFailedTask()
        {
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("failedTask");

            AssertJobRetriesForActivity(pi, "failingTask");
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/async/FoxJobRetryCmdTest.TestFailedTransaction.bpmn20.xml" })]
        public virtual void testFailedTransaction()
        {
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("failedTask");

            AssertJobRetriesForActivity(pi, "failingTransaction");
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/async/FoxJobRetryCmdTest.TestFailedReceiveTask.bpmn20.xml" })]
        public virtual void testFailedReceiveTask()
        {
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("failedReceiveTask");

            AssertJobRetriesForActivity(pi, "failingReceiveTask");
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/async/FoxJobRetryCmdTest.TestFailedBoundaryTimerEvent.bpmn20.xml" })]
        public virtual void testFailedBoundaryTimerEvent()
        {
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("failedBoundaryTimerEvent");

            AssertJobRetriesForActivity(pi, "userTask");
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/async/FoxJobRetryCmdTest.TestFailedIntermediateCatchingTimerEvent.bpmn20.xml" })]
        public virtual void testFailedIntermediateCatchingTimerEvent()
        {
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("failedIntermediateCatchingTimerEvent");

            AssertJobRetriesForActivity(pi, "failingTimerEvent");
        }

        [Test]
        [Deployment]
        public virtual void testFailingMultiInstanceBody()
        {
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("failingMultiInstance");

            // multi-instance body of task
            AssertJobRetriesForActivity(pi, "task" + BpmnParse.MultiInstanceBodyIdSuffix);
        }

        [Test]
        [Deployment]
        public virtual void testFailingMultiInstanceInnerActivity()
        {
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("failingMultiInstance");

            // inner activity of multi-instance body
            AssertJobRetriesForActivity(pi, "task");
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/async/FoxJobRetryCmdTest.TestBrokenFoxJobRetryValue.bpmn20.xml" })]
        public virtual void testBrokenFoxJobRetryValue()
        {
            IJob job = managementService.CreateJobQuery().First();
            Assert.NotNull(job);
            Assert.AreEqual(3, job.Retries);

            WaitForExecutedJobWithRetriesLeft(0, job.Id);
            job = refreshJob(job.Id);
            Assert.AreEqual(0, job.Retries);
            Assert.AreEqual(1, managementService.CreateJobQuery(c=>c.Retries == 0).Count());
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/async/FoxJobRetryCmdTest.TestFailedStartTimerEvent.bpmn20.xml" })]
        public virtual void testFailedTimerStartEvent()
        {
            // After process start, there should be timer created
            IQueryable<IJob> jobQuery = managementService.CreateJobQuery();
            Assert.AreEqual(1, jobQuery.Count());

            IJob job = managementService.CreateJobQuery().First();
            Assert.NotNull(job);
            string jobId = job.Id;

            WaitForExecutedJobWithRetriesLeft(4, jobId);
            stillOneJobWithExceptionAndRetriesLeft(jobId);

            job = refreshJob(jobId);
            Assert.NotNull(job);

            Assert.AreEqual(4, job.Retries);

            WaitForExecutedJobWithRetriesLeft(3, jobId);

            job = refreshJob(jobId);
            Assert.AreEqual(3, job.Retries);
            stillOneJobWithExceptionAndRetriesLeft(jobId);

            WaitForExecutedJobWithRetriesLeft(2, jobId);

            job = refreshJob(jobId);
            Assert.AreEqual(2, job.Retries);
            stillOneJobWithExceptionAndRetriesLeft(jobId);

            WaitForExecutedJobWithRetriesLeft(1, jobId);

            job = refreshJob(jobId);
            Assert.AreEqual(1, job.Retries);
            stillOneJobWithExceptionAndRetriesLeft(jobId);

            WaitForExecutedJobWithRetriesLeft(0, jobId);

            job = refreshJob(jobId);
            Assert.AreEqual(0, job.Retries);
            Assert.AreEqual(1, managementService.CreateJobQuery()/*.SetWithException()*/.Count());
            Assert.AreEqual(0, managementService.CreateJobQuery()/*.JobId(jobId).WithRetriesLeft()*/.Count());
            Assert.AreEqual(1, managementService.CreateJobQuery(c=>c.Retries == 0).Count());
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/async/FoxJobRetryCmdTest.TestFailedIntermediateThrowingSignalEvent.bpmn20.xml", "resources/bpmn/async/FoxJobRetryCmdTest.FailingSignalStart.bpmn20.xml" })]
        public virtual void FAILING_testFailedIntermediateThrowingSignalEvent()
        {
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("failedIntermediateThrowingSignalEvent");

            AssertJobRetriesForActivity(pi, "failingSignalEvent");
        }

        [Test]
        [Deployment]
        public virtual void testRetryOnTimerStartEventInEventSubProcess()
        {
            var id = runtimeService.StartProcessInstanceByKey("process").Id;

            IJob job = managementService.CreateJobQuery().First();

            Assert.AreEqual(3, job.Retries);

            try
            {
                managementService.ExecuteJob(job.Id);
                Assert.Fail();
            }
            catch (System.Exception)
            {
                // expected
            }

            job = managementService.CreateJobQuery().First();

            Assert.AreEqual(4, job.Retries);
        }

        //[Test]
        //public virtual void testRetryOnServiceTaskLikeMessageThrowEvent()
        //{
        //    // given
        //    IBpmnModelInstance bpmnModelInstance = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process")
        //        .StartEvent()
        //        .IntermediateThrowEvent()
        //        .CamundaAsyncBefore()
        //        .CamundaFailedJobRetryTimeCycle("R10/PT5S")
        //        .MessageEventDefinition("messageDefinition")
        //        .Message("message")
        //        .MessageEventDefinitionDone()
        //        .EndEvent()
        //        .Done()
        //        ;

        //    IMessageEventDefinition messageDefinition = bpmnModelInstance.GetModelElementById("messageDefinition");
        //    //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
        //    messageDefinition.CamundaClass = typeof(FailingDelegate).FullName;

        //    Deployment(bpmnModelInstance);

        //    runtimeService.StartProcessInstanceByKey("process");

        //    IJob job = managementService.CreateJobQuery().First();

        //    // when job fails
        //    try
        //    {
        //        managementService.ExecuteJob(job.Id);
        //    }
        //    catch (Exception)
        //    {
        //        // ignore
        //    }

        //    // then
        //    job = managementService.CreateJobQuery().First();
        //    Assert.AreEqual(9, job.Retries);
        //}

        [Test]
        [Deployment(new string[] { "resources/bpmn/async/FoxJobRetryCmdTest.TestFailedServiceTask.bpmn20.xml" })]
        public virtual void FAILING_testFailedRetryWithTimeShift()
        {
            // set date to hour before time shift (2015-10-25T03:00:00 CEST =>
            // 2015-10-25T02:00:00 CET)
            DateTime tenMinutesBeforeTimeShift = createDateFromLocalString("2015-10-25T02:50:00 CEST");
            DateTime fiveMinutesBeforeTimeShift = createDateFromLocalString("2015-10-25T02:55:00 CEST");
            DateTime twoMinutesBeforeTimeShift = createDateFromLocalString("2015-10-25T02:58:00 CEST");
            ClockUtil.CurrentTime = tenMinutesBeforeTimeShift;

            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("failedServiceTask");
            Assert.NotNull(pi);

            // a job is acquirable
            IList<JobEntity> acquirableJobs = findAndLockAcquirableJobs();
            Assert.AreEqual(1, acquirableJobs.Count);

            // execute job
            WaitForExecutedJobWithRetriesLeft(4);

            // the job lock time is after the current time but before the time shift
            JobEntity job = (JobEntity)fetchJob(pi.ProcessInstanceId);
            Assert.True(tenMinutesBeforeTimeShift < job.LockExpirationTime);
            Assert.AreEqual(fiveMinutesBeforeTimeShift, job.LockExpirationTime);
            Assert.True(twoMinutesBeforeTimeShift > job.LockExpirationTime);

            // the job is not acquirable
            acquirableJobs = findAndLockAcquirableJobs();
            Assert.AreEqual(0, acquirableJobs.Count);

            // set clock to two minutes before time shift
            ClockUtil.CurrentTime = twoMinutesBeforeTimeShift;

            // the job is now acquirable
            acquirableJobs = findAndLockAcquirableJobs();
            Assert.AreEqual(1, acquirableJobs.Count);

            // execute job
            WaitForExecutedJobWithRetriesLeft(3);

            // the job lock time is after the current time
            job = (JobEntity)refreshJob(job.Id);
            Assert.True(twoMinutesBeforeTimeShift < job.LockExpirationTime);

            // the job is not acquirable
            acquirableJobs = findAndLockAcquirableJobs();
            Assert.AreEqual(0,acquirableJobs.Count, "IJob shouldn't be acquirable");
            //Assert.AreEqual("IJob shouldn't be acquirable", 0, acquirableJobs.Count);

            ClockUtil.Reset();
        }

        protected internal virtual void AssertJobRetriesForActivity(IProcessInstance pi, string activityId)
        {
            Assert.NotNull(pi);

            WaitForExecutedJobWithRetriesLeft(4);
            stillOneJobWithExceptionAndRetriesLeft();

            IJob job = fetchJob(pi.ProcessInstanceId);
            Assert.NotNull(job);
            Assert.AreEqual(pi.ProcessInstanceId, job.ProcessInstanceId);

            Assert.AreEqual(4, job.Retries);

            ExecutionEntity execution = fetchExecutionEntity(pi.ProcessInstanceId, activityId);
            Assert.NotNull(execution);

            WaitForExecutedJobWithRetriesLeft(3);

            job = refreshJob(job.Id);
            Assert.AreEqual(3, job.Retries);
            stillOneJobWithExceptionAndRetriesLeft();

            execution = refreshExecutionEntity(execution.Id);
            Assert.AreEqual(activityId, execution.ActivityId);

            WaitForExecutedJobWithRetriesLeft(2);

            job = refreshJob(job.Id);
            Assert.AreEqual(2, job.Retries);
            stillOneJobWithExceptionAndRetriesLeft();

            execution = refreshExecutionEntity(execution.Id);
            Assert.AreEqual(activityId, execution.ActivityId);

            WaitForExecutedJobWithRetriesLeft(1);

            job = refreshJob(job.Id);
            Assert.AreEqual(1, job.Retries);
            stillOneJobWithExceptionAndRetriesLeft();

            execution = refreshExecutionEntity(execution.Id);
            Assert.AreEqual(activityId, execution.ActivityId);

            WaitForExecutedJobWithRetriesLeft(0);

            job = refreshJob(job.Id);
            Assert.AreEqual(0, job.Retries);
            //Assert.AreEqual(1, managementService.CreateJobQuery()/*.SetWithException()*/.Count());
            Assert.AreEqual(0, managementService.CreateJobQuery(c=>c.Retries > 0).Count());
            Assert.AreEqual(1, managementService.CreateJobQuery(c=>c.Retries == 0).Count());

            execution = refreshExecutionEntity(execution.Id);
            Assert.AreEqual(activityId, execution.ActivityId);
        }

        protected internal virtual void WaitForExecutedJobWithRetriesLeft(int retriesLeft, string jobId)
        {
            IQueryable<IJob> jobQuery = managementService.CreateJobQuery();

            if (!string.ReferenceEquals(jobId, null))
            {
                //jobQuery.JobId(jobId);
            }

            IJob job = jobQuery.First();

            try
            {
                managementService.ExecuteJob(job.Id);
            }
            catch (System.Exception)
            {
            }

            // update job
            job = jobQuery.First();

            if (job.Retries != retriesLeft)
            {
                WaitForExecutedJobWithRetriesLeft(retriesLeft, jobId);
            }
        }

        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: protected void WaitForExecutedJobWithRetriesLeft(final int retriesLeft)
        protected internal virtual void WaitForExecutedJobWithRetriesLeft(int retriesLeft)
        {
            WaitForExecutedJobWithRetriesLeft(retriesLeft, null);
        }

        protected internal virtual ExecutionEntity refreshExecutionEntity(string executionId)
        {
            return (ExecutionEntity)runtimeService.CreateExecutionQuery(c=>c.Id == executionId).First();
        }

        protected internal virtual ExecutionEntity fetchExecutionEntity(string ProcessInstanceId, string activityId)
        {
            return (ExecutionEntity)runtimeService.CreateExecutionQuery(c=>c.ProcessInstanceId == ProcessInstanceId && c.ActivityId == activityId).First();
        }

        protected internal virtual IJob refreshJob(string jobId)
        {
            return managementService.CreateJobQuery(c=>c.Id == jobId).First();
        }

        protected internal virtual IJob fetchJob(string ProcessInstanceId)
        {
            return managementService.CreateJobQuery(c=>c.ProcessInstanceId == ProcessInstanceId).First();
        }

        protected internal virtual void stillOneJobWithExceptionAndRetriesLeft(string jobId)
        {
            Assert.AreEqual(1, managementService.CreateJobQuery(c=>c.Id == jobId)/*.SetWithException()*/.Count());
            Assert.AreEqual(1, managementService.CreateJobQuery()/*.JobId(jobId).WithRetriesLeft()*/.Count());
        }

        protected internal virtual void stillOneJobWithExceptionAndRetriesLeft()
        {
            Assert.AreEqual(1, managementService.CreateJobQuery()/*.SetWithException()*/.Count());
            Assert.AreEqual(1, managementService.CreateJobQuery(c=>c.Retries > 0).Count());
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: protected java.Util.Date createDateFromLocalString(String dateString) throws java.Text.ParseException
        protected internal virtual DateTime createDateFromLocalString(string dateString)
        {
            // Format: 2015-10-25T02:50:00 CEST
            //DateFormat dateFormat = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss z", Locale.US);
            //return dateFormat.parse(dateString);
            return Convert.ToDateTime(DateTime.Parse(dateString).ToString("yyyy-MM-dd'T'HH:mm:ss z"));
        }

        protected internal virtual IList<JobEntity> findAndLockAcquirableJobs()
        {
            return processEngineConfiguration.CommandExecutorTxRequired.Execute(new CommandAnonymousInnerClass(this));
        }

        private class CommandAnonymousInnerClass : ICommand<IList<JobEntity>>
        {
            private readonly FoxJobRetryCmdTest outerInstance;

            public CommandAnonymousInnerClass(FoxJobRetryCmdTest outerInstance)
            {
                this.outerInstance = outerInstance;
            }

            public IList<JobEntity> Execute(CommandContext commandContext)
            {
                IList<JobEntity> jobs = commandContext.JobManager.FindNextJobsToExecute(new Page(0, 100));
                foreach (JobEntity job in jobs)
                {
                    job.LockOwner = "test";
                }
                return jobs;
            }

            //public override IList<JobEntity> execute(CommandContext commandContext)
            //{
            //    IList<JobEntity> jobs = commandContext.JobManager.FindNextJobsToExecute(new Page(0, 100));
            //    foreach (JobEntity job in jobs)
            //    {
            //        job.LockOwner = "test";
            //    }
            //    return jobs;
            //}
        }

    }

}