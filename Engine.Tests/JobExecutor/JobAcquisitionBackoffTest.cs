using System.Linq;
using Engine.Tests.ConCurrency;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using NUnit.Framework;

namespace Engine.Tests.JobExecutor
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class JobAcquisitionBackoffTest
    {

        #region members


        private const int BaseBackoffTime = 1000;
        private const int MaxBackoffTime = 5000;
        private const int BackoffFactor = 2;
        private const int BackoffDecreaseThreshold = 2;
        private const int DefaultNumJobsToAcquire = 3;

        private readonly bool _instanceFieldsInitialized;

        protected internal ProcessEngineBootstrapRule _bootstrapRule;
        protected internal ProcessEngineRule _engineRule;

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(bootstrapRule).around(engineRule);
        ////public RuleChain ruleChain;

        protected internal ControllableJobExecutor _jobExecutor1;
        protected internal ControllableJobExecutor _jobExecutor2;

        protected internal ConcurrencyTestCase.ThreadControl _acquisitionThread1;
        protected internal ConcurrencyTestCase.ThreadControl _acquisitionThread2;


        #endregion

        public JobAcquisitionBackoffTest()
        {
            if (!_instanceFieldsInitialized)
            {
                InitializeInstanceFields();
                _instanceFieldsInitialized = true;
            }
        }

        private void InitializeInstanceFields()
        {
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

        [SetUp]
        public virtual void SetUp()
        {
            _jobExecutor1 = (_engineRule.ProcessEngine.ProcessEngineConfiguration as ProcessEngineConfigurationImpl)
                ?.JobExecutor as ControllableJobExecutor;

            //(ControllableJobExecutor)
            //((ProcessEngineConfigurationImpl)EngineRule.ProcessEngine.ProcessEngineConfiguration).JobExecutor;

            _jobExecutor1.MaxJobsPerAcquisition = DefaultNumJobsToAcquire;
            _jobExecutor1.BackoffTimeInMillis = BaseBackoffTime;
            _jobExecutor1.MaxBackoff = MaxBackoffTime;
            _jobExecutor1.BackoffDecreaseThreshold = BackoffDecreaseThreshold;
            _acquisitionThread1 = _jobExecutor1.AcquisitionThreadControl;

            _jobExecutor2 = new ControllableJobExecutor((ProcessEngineImpl)_engineRule.ProcessEngine);
            _jobExecutor2.MaxJobsPerAcquisition = DefaultNumJobsToAcquire;
            _jobExecutor2.BackoffTimeInMillis = BaseBackoffTime;
            _jobExecutor2.MaxBackoff = MaxBackoffTime;
            _jobExecutor2.BackoffDecreaseThreshold = BackoffDecreaseThreshold;
            _acquisitionThread2 = _jobExecutor2.AcquisitionThreadControl;

        }

        [TearDown]
        public void TearDownEngineRule()
        {
            _engineRule.Finished();
            //_bootstrapRule.Finished();
        }

        [TearDown]
        public virtual void TearDown()
        {
            _jobExecutor1.Shutdown();
            _jobExecutor2.Shutdown();
        }

        private class ProcessEngineBootstrapRuleAnonymousInnerClass : ProcessEngineBootstrapRule
        {
            public override ProcessEngineConfiguration ConfigureEngine(ProcessEngineConfigurationImpl configuration)
            {
                return configuration.SetJobExecutor(new ControllableJobExecutor());
            }
        }


        [Test]
        [Deployment("resources/jobexecutor/simpleAsyncProcess.bpmn20.xml")]
        public virtual void TestBackoffOnOptimisticLocking()
        {
            // when starting a number of process instances process instance
            for (var i = 0; i < 9; i++)
            {
                _engineRule.RuntimeService.StartProcessInstanceByKey("simpleAsyncProcess"); //.Id;
            }

            // ensure that both acquisition threads acquire the same jobs thereby provoking an optimistic locking exception
            JobAcquisitionTestHelper.SuspendInstances(_engineRule.ProcessEngine, 6);

            // when starting job execution, both acquisition threads Wait before acquiring something
            _jobExecutor1.Start();
            _acquisitionThread1.WaitForSync();
            _jobExecutor2.Start();
            _acquisitionThread2.WaitForSync();

            // when having both threads acquire jobs
            // then both Wait before committing the acquiring transaction (AcquireJobsCmd)
            _acquisitionThread1.MakeContinueAndWaitForSync();
            _acquisitionThread2.MakeContinueAndWaitForSync();

            // when continuing acquisition thread 1
            _acquisitionThread1.MakeContinueAndWaitForSync();

            // then it has not performed Waiting since it was able to acquire and execute all jobs
            var jobExecutor1WaitEvents = _jobExecutor1.AcquireJobsRunnable.WaitEvents;
            Assert.AreEqual(1, jobExecutor1WaitEvents.Count);
            Assert.AreEqual(0, jobExecutor1WaitEvents.ElementAt(0).TimeBetweenAcquisitions);

            // when continuing acquisition thread 2, acquisition fails with an OLE
            _acquisitionThread2.MakeContinueAndWaitForSync();

            // and has performed backoff
            var jobExecutor2WaitEvents = _jobExecutor2.AcquireJobsRunnable.WaitEvents;
            Assert.AreEqual(1, jobExecutor2WaitEvents.Count);
            var waitEvent = jobExecutor2WaitEvents.ElementAt(0);
            // we don't know the exact Wait time,
            // since there is random jitter applied
            JobAcquisitionTestHelper.AssertInBetween(BaseBackoffTime, BaseBackoffTime + BaseBackoffTime / 2,  waitEvent.TimeBetweenAcquisitions);

            // when performing another cycle of acquisition
            JobAcquisitionTestHelper.ActivateInstances(_engineRule.ProcessEngine, 6);
            _acquisitionThread1.MakeContinueAndWaitForSync();
            _acquisitionThread2.MakeContinueAndWaitForSync();

            // and thread 1 again acquires all jobs successfully
            _acquisitionThread1.MakeContinueAndWaitForSync();

            // while thread 2 again fails with OLE
            _acquisitionThread2.MakeContinueAndWaitForSync();

            // then thread 1 has tried to acquired 3 jobs again
            var jobExecutor1AcquisitionEvents = _jobExecutor1.AcquireJobsRunnable.AcquisitionEvents;
            var secondAcquisitionAttempt = jobExecutor1AcquisitionEvents.ElementAt(1);
            Assert.AreEqual(3, secondAcquisitionAttempt.NumJobsToAcquire);

            // and not Waited
            jobExecutor1WaitEvents = _jobExecutor1.AcquireJobsRunnable.WaitEvents;
            Assert.AreEqual(2, jobExecutor1WaitEvents.Count);
            Assert.AreEqual(0, jobExecutor1WaitEvents.ElementAt(1).TimeBetweenAcquisitions);

            // then thread 2 has tried to acquire 6 jobs this time
            var jobExecutor2AcquisitionEvents = _jobExecutor2.AcquireJobsRunnable.AcquisitionEvents;
            secondAcquisitionAttempt = jobExecutor2AcquisitionEvents.ElementAt(1);
            Assert.AreEqual(6, secondAcquisitionAttempt.NumJobsToAcquire);

            // and again increased its backoff
            jobExecutor2WaitEvents = _jobExecutor2.AcquireJobsRunnable.WaitEvents;
            Assert.AreEqual(2, jobExecutor2WaitEvents.Count);
            var secondWaitEvent = jobExecutor2WaitEvents.ElementAt(1);
            long expectedBackoffTime = BaseBackoffTime * BackoffFactor; // 1000 * 2^1
            JobAcquisitionTestHelper.AssertInBetween(expectedBackoffTime, expectedBackoffTime + expectedBackoffTime / 2,  secondWaitEvent.TimeBetweenAcquisitions);
        }

        [Test]
        [Deployment("resources/jobexecutor/simpleAsyncProcess.bpmn20.xml")]
        public virtual void TestBackoffDecrease()
        {
            // when starting a number of process instances process instance
            for (var i = 0; i < 15; i++)
            {
                var id = _engineRule.RuntimeService.StartProcessInstanceByKey("simpleAsyncProcess").Id;
            }

            // ensure that both acquisition threads acquire the same jobs thereby provoking an optimistic locking exception
            JobAcquisitionTestHelper.SuspendInstances(_engineRule.ProcessEngine, 12);

            // when starting job execution, both acquisition threads Wait before acquiring something
            _jobExecutor1.Start();
            _acquisitionThread1.WaitForSync();
            _jobExecutor2.Start();
            _acquisitionThread2.WaitForSync();

            // when continuing acquisition thread 1
            // then it is able to acquire and execute all jobs
            _acquisitionThread1.MakeContinueAndWaitForSync();

            // when continuing acquisition thread 2
            // acquisition fails with an OLE
            _acquisitionThread2.MakeContinueAndWaitForSync();

            _jobExecutor1.Shutdown();
            _acquisitionThread1.WaitUntilDone();
            _acquisitionThread2.MakeContinueAndWaitForSync();

            // such that acquisition thread 2 performs backoff
            var jobExecutor2WaitEvents = ((RecordingAcquireJobsRunnable)_jobExecutor2.AcquireJobsRunnable).WaitEvents;
            Assert.AreEqual(1, jobExecutor2WaitEvents.Count);

            // when in the next cycles acquisition thread2 successfully acquires jobs without OLE for n times
            JobAcquisitionTestHelper.ActivateInstances(_engineRule.ProcessEngine, 12);

            for (var i = 0; i < BackoffDecreaseThreshold; i++)
            {
                // backoff has not decreased yet
                Assert.True(jobExecutor2WaitEvents[i].TimeBetweenAcquisitions > 0);

                _acquisitionThread2.MakeContinueAndWaitForSync(); // acquire
                _acquisitionThread2.MakeContinueAndWaitForSync(); // continue after acquisition with next cycle
            }

            // it decreases its backoff again
            var lastBackoff = jobExecutor2WaitEvents[BackoffDecreaseThreshold].TimeBetweenAcquisitions;
            Assert.AreEqual(0, lastBackoff);
        }
    }
}