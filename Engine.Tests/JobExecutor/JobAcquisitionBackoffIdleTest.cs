using System;
using System.Linq;
using Engine.Tests.ConCurrency;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.JobExecutor
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class JobAcquisitionBackoffIdleTest
    {
        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(bootstrapRule).around(engineRule);
        ////public RuleChain ruleChain;

        #region members


        private  const int BaseIdleWaitTime = 5000;
        private  const int MaxIdleWaitTime = 60000;

        private  ControllableJobExecutor _jobExecutor;
        private  ConcurrencyTestCase.ThreadControl _acquisitionThread;

        protected internal ProcessEngineBootstrapRule _bootstrapRule;
        protected internal ProvidedProcessEngineRule _engineRule;

        private readonly bool _instanceFieldsInitialized;

        #endregion


        public JobAcquisitionBackoffIdleTest()
        {
            if (!_instanceFieldsInitialized)
            {
                InitializeInstanceFields();
                _instanceFieldsInitialized = true;
            }
        }

        private void InitializeInstanceFields()
        {
            ProcessEngineBootstrapRuleAnonymousInnerClass.OuterInstance = this;
            _bootstrapRule = new ProcessEngineBootstrapRuleAnonymousInnerClass();
            _engineRule = new ProvidedProcessEngineRule(_bootstrapRule);
            ////ruleChain = RuleChain.outerRule(BootstrapRule).around(EngineRule);
        }

        [SetUp]
        public void SetUpEngineRule()
        {
            if (_engineRule.ProcessEngine == null)
                _engineRule.InitializeProcessEngine();

            _engineRule.InitializeServices();

            _engineRule.Starting();
        }

        [TearDown]
        public virtual void ShutdownJobExecutor()
        {
            _jobExecutor.Shutdown();
        }

        



        private class ProcessEngineBootstrapRuleAnonymousInnerClass : ProcessEngineBootstrapRule
        {

            //private readonly JobAcquisitionBackoffIdleTest _outerInstance;

            //public ProcessEngineBootstrapRuleAnonymousInnerClass(JobAcquisitionBackoffIdleTest outerInstance)
            //{
            //    _outerInstance = outerInstance;
            //}

            public static  JobAcquisitionBackoffIdleTest OuterInstance { private get; set; }

            public override ProcessEngineConfiguration ConfigureEngine(ProcessEngineConfigurationImpl configuration)
            {
                OuterInstance._jobExecutor = new ControllableJobExecutor(true)
                {
                    MaxJobsPerAcquisition = 1,
                    WaitTimeInMillis = BaseIdleWaitTime,
                    MaxWait = MaxIdleWaitTime
                };
                OuterInstance._acquisitionThread = OuterInstance._jobExecutor.AcquisitionThreadControl;

                return configuration.SetJobExecutor(OuterInstance._jobExecutor);
            }
        }

       

        protected internal virtual void CycleJobAcquisitionToMaxIdleTime()
        {
            // cycle of job acquisition
            // => 0 jobs are acquired
            // => acquisition should become idle
            TriggerReconfigurationAndNextCycle();
            AssertJobExecutorWaitEvent(BaseIdleWaitTime);

            // another cycle of job acquisition
            // => 0 jobs are acquired
            // => acquisition should increase idle time
            TriggerReconfigurationAndNextCycle();
            AssertJobExecutorWaitEvent(BaseIdleWaitTime * 2);

            // another cycle of job acquisition
            // => 0 jobs are acquired
            // => acquisition should increase idle time exponentially
            TriggerReconfigurationAndNextCycle();
            AssertJobExecutorWaitEvent(BaseIdleWaitTime * 4);

            // another cycle of job acquisition
            // => 0 jobs are acquired
            // => acquisition should increase idle time exponentially
            TriggerReconfigurationAndNextCycle();
            AssertJobExecutorWaitEvent(BaseIdleWaitTime * 8);

            // another cycle of job acquisition
            // => 0 jobs are acquired
            // => acquisition should increase to max idle time
            TriggerReconfigurationAndNextCycle();
            AssertJobExecutorWaitEvent(MaxIdleWaitTime);
        }

        /// <summary>
        ///     CAM-5073
        /// </summary>

        [Test]
        [Deployment("resources/jobexecutor/simpleAsyncProcess.bpmn20.xml")]
        public virtual void TestIdlingAfterConcurrentJobAddedNotification()
        {
            // start job acquisition - Waiting before acquiring jobs
            _jobExecutor.Start();
            _acquisitionThread.WaitForSync();

            // acquire jobs
            _acquisitionThread.MakeContinueAndWaitForSync();

            // issue a message added notification
            _engineRule.RuntimeService.StartProcessInstanceByKey("simpleAsyncProcess");

            // complete job acquisition - trigger re-configuration
            // => due to the hint, the job executor should not become idle
            _acquisitionThread.MakeContinueAndWaitForSync();
            AssertJobExecutorWaitEvent(0);

            // another cycle of job acquisition
            // => acquires and executes the new job
            // => acquisition does not become idle because enough jobs could be acquired
            TriggerReconfigurationAndNextCycle();
            AssertJobExecutorWaitEvent(0);

            CycleJobAcquisitionToMaxIdleTime();
        }

        protected internal virtual void InitAcquisitionAndIdleToMaxTime()
        {
            // start job acquisition - Waiting before acquiring jobs
            _jobExecutor.Start();
            _acquisitionThread.WaitForSync();

            //cycle acquistion till max idle time is reached
            CycleJobAcquisitionToMaxIdleTime();
        }

        protected internal virtual void CycleAcquisitionAndAssertAfterJobExecution(IQueryable<IJob> jobQuery)
        {
            // another cycle of job acquisition after acuqisition idle was reseted
            // => 1 jobs are acquired
            TriggerReconfigurationAndNextCycle();
            AssertJobExecutorWaitEvent(0);

            // we have no timer to fire
            Assert.AreEqual(0, jobQuery.Count());

            // and we are in the second state
            Assert.AreEqual(1L, _engineRule.TaskService.CreateTaskQuery().Count());
            var task = _engineRule.TaskService.CreateTaskQuery()/*.OrderByTaskName()*//*.Desc()*/.First();
            Assert.AreEqual("Next Task", task.Name);
            // complete the task and end the execution
            _engineRule.TaskService.Complete(task.Id);
        }

        public interface IJobCreationInCycle
        {
            IProcessInstance CreateJobAndContinueCycle();
        }

        public virtual void TestIdlingWithHint(IJobCreationInCycle jobCreationInCycle)
        {
            InitAcquisitionAndIdleToMaxTime();

            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final java.util.Date startTime = new java.util.Date();
            var startTime = DateTime.Now;
            var procInstance = jobCreationInCycle.CreateJobAndContinueCycle();

            // After process start, there should be 1 timer created
            var task1 = _engineRule.TaskService.CreateTaskQuery(null).First();
            Assert.AreEqual("Timer Task", task1.Name);
            //and one job
            var jobQuery = _engineRule.ManagementService.CreateJobQuery(c => c.ProcessInstanceId == procInstance.Id);
            var job = jobQuery.First();
            Assert.NotNull(job);

            // the hint of the added job resets the idle time
            // => 0 jobs are acquired so we had to Wait BASE IDLE TIME
            //after this time we can acquire the timer
            TriggerReconfigurationAndNextCycle();
            AssertJobExecutorWaitEvent(BaseIdleWaitTime);

            //time is increased so timer is found
            ClockUtil.CurrentTime = new DateTime(startTime.Ticks + BaseIdleWaitTime);
            //now we are able to acquire the job
            //CycleAcquisitionAndAssertAfterJobExecution(jobQuery);
        }

        [Test]
        [Ignore("不支持groovy语法")]
        [Deployment("resources/jobexecutor/JobAcquisitionBackoffIdleTest.TestShortTimerOnUserTaskWithExpression.bpmn20.xml")]
        public virtual void TestIdlingWithHintOnSuspend()
        {
            TestIdlingWithHint(new JobCreationInCycleAnonymousInnerClass(this));
        }

        private class JobCreationInCycleAnonymousInnerClass : IJobCreationInCycle
        {
            private readonly JobAcquisitionBackoffIdleTest _outerInstance;

            public JobCreationInCycleAnonymousInnerClass(JobAcquisitionBackoffIdleTest outerInstance)
            {
                _outerInstance = outerInstance;
            }

            public virtual IProcessInstance CreateJobAndContinueCycle()
            {
                //continue sync before acquire
                _outerInstance._acquisitionThread.MakeContinueAndWaitForSync();
                //continue sync after acquire
                _outerInstance._acquisitionThread.MakeContinueAndWaitForSync();

                //process is started with timer boundary event which should start after 3 seconds
                var procInstance = _outerInstance._engineRule.RuntimeService.StartProcessInstanceByKey("timer-example");
                //release suspend sync
                _outerInstance._acquisitionThread.MakeContinueAndWaitForSync();
                //Assert max idle time and clear events
                _outerInstance.AssertJobExecutorWaitEvent(MaxIdleWaitTime);

                //trigger continue and Assert that new acquisition cycle was triggered right after the hint
                _outerInstance.TriggerReconfigurationAndNextCycle();
                _outerInstance.AssertJobExecutorWaitEvent(0);
                return procInstance;
            }
        }

        [Test]
        [Ignore("不支持groovy语法")]
        [Deployment("resources/jobexecutor/JobAcquisitionBackoffIdleTest.TestShortTimerOnUserTaskWithExpression.bpmn20.xml")]
        public virtual void TestIdlingWithHintOnAquisition()
        {
            TestIdlingWithHint(new JobCreationInCycleAnonymousInnerClass2(this));
        }

        private class JobCreationInCycleAnonymousInnerClass2 : IJobCreationInCycle
        {
            private readonly JobAcquisitionBackoffIdleTest _outerInstance;

            public JobCreationInCycleAnonymousInnerClass2(JobAcquisitionBackoffIdleTest outerInstance)
            {
                _outerInstance = outerInstance;
            }

            public virtual IProcessInstance CreateJobAndContinueCycle()
            {
                //continue sync before acquire
                _outerInstance._acquisitionThread.MakeContinueAndWaitForSync();

                //process is started with timer boundary event which should start after 3 seconds
                var procInstance = _outerInstance._engineRule.RuntimeService.StartProcessInstanceByKey("timer-example");

                //continue sync after acquire
                _outerInstance._acquisitionThread.MakeContinueAndWaitForSync();
                //release suspend sync
                _outerInstance._acquisitionThread.MakeContinueAndWaitForSync();
                //Assert max idle time and clear events
                _outerInstance.AssertJobExecutorWaitEvent(0);
                return procInstance;
            }
        }


        [Test]
        [Ignore("不支持groovy语法")]
        [Deployment("resources/jobexecutor/JobAcquisitionBackoffIdleTest.TestShortTimerOnUserTaskWithExpression.bpmn20.xml")]
        public virtual void TestIdlingWithHintBeforeAquisition()
        {
            TestIdlingWithHint(new JobCreationInCycleAnonymousInnerClass3(this));
        }

        private class JobCreationInCycleAnonymousInnerClass3 : IJobCreationInCycle
        {
            private readonly JobAcquisitionBackoffIdleTest _outerInstance;

            public JobCreationInCycleAnonymousInnerClass3(JobAcquisitionBackoffIdleTest outerInstance)
            {
                _outerInstance = outerInstance;
            }

            public virtual IProcessInstance CreateJobAndContinueCycle()
            {
                //process is started with timer boundary event which should start after 3 seconds
                var procInstance = _outerInstance._engineRule.RuntimeService.StartProcessInstanceByKey("timer-example");

                //continue sync before acquire
                _outerInstance._acquisitionThread.MakeContinueAndWaitForSync();
                //continue sync after acquire
                _outerInstance._acquisitionThread.MakeContinueAndWaitForSync();
                //release suspend sync
                _outerInstance._acquisitionThread.MakeContinueAndWaitForSync();
                //Assert max idle time and clear events
                _outerInstance.AssertJobExecutorWaitEvent(0);
                return procInstance;
            }
        }

        protected internal virtual void TriggerReconfigurationAndNextCycle()
        {
            _acquisitionThread.MakeContinueAndWaitForSync();
            _acquisitionThread.MakeContinueAndWaitForSync();
            _acquisitionThread.MakeContinueAndWaitForSync();
        }

        protected internal virtual void AssertJobExecutorWaitEvent(long expectedTimeout)
        {
            var WaitEvents = ((RecordingAcquireJobsRunnable)_jobExecutor.AcquireJobsRunnable).WaitEvents;
            Assert.AreEqual(1, WaitEvents.Count);
            Assert.AreEqual(expectedTimeout, WaitEvents[0].TimeBetweenAcquisitions);

            // discard Wait event if successfully Asserted
            WaitEvents.Clear();
        }
    }
}