using System.Linq;
using Engine.Tests.ConCurrency;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using NUnit.Framework;

namespace Engine.Tests.JobExecutor
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class JobAcquisitionTest
    {
        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(bootstrapRule).around(engineRule);
        ////public RuleChain ruleChain;

        [SetUp]
        public void SetUpEngineRule()
        {
            if (EngineRule.ProcessEngine == null)
                EngineRule.InitializeProcessEngine();

            EngineRule.InitializeServices();

            EngineRule.Starting();
        }

        [SetUp]
        public virtual void SetUp()
        {
            // two job executors with the default settings
            JobExecutor1 = (EngineRule.ProcessEngine.ProcessEngineConfiguration as ProcessEngineConfigurationImpl)?.JobExecutor as ControllableJobExecutor;
            
                
            JobExecutor1.MaxJobsPerAcquisition = DefaultNumJobsToAcquire;
            AcquisitionThread1 = JobExecutor1.AcquisitionThreadControl;

            JobExecutor2 = new ControllableJobExecutor((ProcessEngineImpl) EngineRule.ProcessEngine);
            JobExecutor2.MaxJobsPerAcquisition = DefaultNumJobsToAcquire;
            AcquisitionThread2 = JobExecutor2.AcquisitionThreadControl;
        }

        [TearDown]
        public virtual void TearDown()
        {
            JobExecutor1.Shutdown();
            JobExecutor2.Shutdown();
        }

        private readonly bool _instanceFieldsInitialized;

        public JobAcquisitionTest()
        {
            if (!_instanceFieldsInitialized)
            {
                InitializeInstanceFields();
                _instanceFieldsInitialized = true;
            }
        }

        private void InitializeInstanceFields()
        {
            BootstrapRule = new ProcessEngineBootstrapRuleAnonymousInnerClass();
            EngineRule = new ProvidedProcessEngineRule(BootstrapRule);
            ////ruleChain = RuleChain.outerRule(BootstrapRule).around(EngineRule);
        }


        protected internal const int DefaultNumJobsToAcquire = 3;

        protected internal ControllableJobExecutor JobExecutor1;
        protected internal ControllableJobExecutor JobExecutor2;

        protected internal ConcurrencyTestCase.ThreadControl AcquisitionThread1;
        protected internal ConcurrencyTestCase.ThreadControl AcquisitionThread2;

        protected internal ProcessEngineBootstrapRule BootstrapRule =
            new ProcessEngineBootstrapRuleAnonymousInnerClass();

        private class ProcessEngineBootstrapRuleAnonymousInnerClass : ProcessEngineBootstrapRule
        {
            public override ProcessEngineConfiguration ConfigureEngine(ProcessEngineConfigurationImpl configuration)
            {
                return configuration.SetJobExecutor(new ControllableJobExecutor());
            }
        }

        protected internal ProcessEngineRule EngineRule;

        [Test][Deployment("resources/jobexecutor/simpleAsyncProcess.bpmn20.xml") ]
        public virtual void TestJobLockingFailure()
        {
            var numberOfInstances = 3;

            // when starting a number of process instances
            for (var i = 0; i < numberOfInstances; i++)
            {
                var id = EngineRule.RuntimeService.StartProcessInstanceByKey("simpleAsyncProcess").Id;
            }

            // when starting job execution, both acquisition threads Wait before acquiring something
            JobExecutor1.Start();
            AcquisitionThread1.WaitForSync();
            JobExecutor2.Start();
            AcquisitionThread2.WaitForSync();

            // when having both threads acquire jobs
            // then both Wait before committing the acquiring transaction (AcquireJobsCmd)
            AcquisitionThread1.MakeContinueAndWaitForSync();
            AcquisitionThread2.MakeContinueAndWaitForSync();

            // when continuing acquisition thread 1
            AcquisitionThread1.MakeContinueAndWaitForSync();

            // then it has not performed Waiting since it was able to acquire and execute all jobs
            Assert.AreEqual(0, EngineRule.ManagementService.CreateJobQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).Count());
            var jobExecutor1WaitEvents = ((RecordingAcquireJobsRunnable) JobExecutor1.AcquireJobsRunnable).WaitEvents;
            Assert.AreEqual(1, jobExecutor1WaitEvents.Count);
            Assert.AreEqual(0, jobExecutor1WaitEvents[0].TimeBetweenAcquisitions);

            // when continuing acquisition thread 2
            AcquisitionThread2.MakeContinueAndWaitForSync();

            // then its acquisition cycle fails with OLEs
            // but the acquisition thread immediately tries again
            var jobExecutor2WaitEvents = ((RecordingAcquireJobsRunnable) JobExecutor2.AcquireJobsRunnable).WaitEvents;
            Assert.AreEqual(1, jobExecutor2WaitEvents.Count);
            Assert.AreEqual(0, jobExecutor2WaitEvents[0].TimeBetweenAcquisitions);
        }
    }
}