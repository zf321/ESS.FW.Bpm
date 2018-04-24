using System.Linq;
using Engine.Tests.ConCurrency;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;

namespace Engine.Tests.JobExecutor
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class JobExecutorShutdownTest
    {
        #region members

        protected internal static readonly IBpmnModelInstance TwoAsyncTasks = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process").StartEvent().ServiceTask("task1").CamundaClass(typeof(SyncDelegate).FullName).CamundaAsyncBefore().CamundaExclusive(true).ServiceTask("task2").CamundaClass(typeof(SyncDelegate).FullName).CamundaAsyncBefore().CamundaExclusive(true).EndEvent().Done();
        protected internal static readonly IBpmnModelInstance SingleAsyncTask = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process").StartEvent().ServiceTask("task1").CamundaClass(typeof(SyncDelegate).FullName).CamundaAsyncBefore().CamundaExclusive(true).EndEvent().Done();
        private readonly bool _instanceFieldsInitialized;
        protected internal ProcessEngineBootstrapRule _bootstrapRule;
        protected internal ProcessEngineRule _engineRule;
        protected internal ControllableJobExecutor _jobExecutor;
        protected internal ConcurrencyTestCase.ThreadControl _acquisitionThread;
        protected internal static ConcurrencyTestCase.ThreadControl ExecutionThread;

        #endregion

        public JobExecutorShutdownTest()
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
        }

        [SetUp]
        public void SetUpEngineRule()
        {
            if (_engineRule.processEngine == null)
                _engineRule.InitializeProcessEngine();
            _engineRule.InitializeServices();
            _engineRule.Starting();
        }

        [SetUp]
        public virtual void SetUp()
        {
            _jobExecutor =
                (ControllableJobExecutor)
                ((ProcessEngineConfigurationImpl)_engineRule.ProcessEngine.ProcessEngineConfiguration).JobExecutor;
            _jobExecutor.MaxJobsPerAcquisition = 2;
            _acquisitionThread = _jobExecutor.AcquisitionThreadControl;
            ExecutionThread = _jobExecutor.ExecutionThreadControl;
        }


        [TearDown]
        public void TearDownEngineRule()
        {
            _bootstrapRule.Finished();
            _engineRule.Finished();

        }

        [TearDown]
        public virtual void ShutdownJobExecutor()
        {
            _jobExecutor.Shutdown();
        }

        private class ProcessEngineBootstrapRuleAnonymousInnerClass : ProcessEngineBootstrapRule
        {
            public static JobExecutorShutdownTest OuterInstance { private get; set; }
            public override ProcessEngineConfiguration ConfigureEngine(ProcessEngineConfigurationImpl configuration)
            {
                return configuration.SetJobExecutor(OuterInstance.BuildControllableJobExecutor());
            }
        }

        protected internal ControllableJobExecutor BuildControllableJobExecutor()
        {
            var jobExecutor = new ControllableJobExecutor();
            jobExecutor.MaxJobsPerAcquisition = 2;
            jobExecutor.ProceedAndWaitOnShutdown(false);
            return jobExecutor;
        }

        // Todo: 优先级降低，暂停
        [Test]
        public virtual void TestConcurrentShutdownAndExclusiveFollowUpJob()
        {
            // given
            var deployment = _engineRule.RepositoryService.CreateDeployment().AddModelInstance("foo.bpmn", TwoAsyncTasks).Deploy();
            _engineRule.ManageDeployment(deployment);

            _engineRule.RuntimeService.StartProcessInstanceByKey("process");

            var firstAsyncJob = _engineRule.ManagementService.CreateJobQuery().First();

            _jobExecutor.Start();

            // Wait before acquisition
            _acquisitionThread.WaitForSync();
            // Wait for no more acquisition syncs
            _acquisitionThread.IgnoreFutureSyncs();
            _acquisitionThread.MakeContinue();

            // when Waiting during execution of first job
            ExecutionThread.WaitForSync();

            // and shutting down the job executor
            _jobExecutor.Shutdown();

            // and continuing job execution
            ExecutionThread.WaitUntilDone();

            // then the current job has completed successfully
            Assert.AreEqual(0, _engineRule.ManagementService.CreateJobQuery(c => c.Id == firstAsyncJob.Id).Count());

            // but the exclusive follow-up job is not executed and is not locked
            var secondAsyncJob = (JobEntity)_engineRule.ManagementService.CreateJobQuery().First();
            Assert.NotNull(secondAsyncJob);
            Assert.IsFalse(secondAsyncJob.Id.Equals(firstAsyncJob.Id));
            Assert.IsNull(secondAsyncJob.LockOwner);
            Assert.IsNull(secondAsyncJob.LockExpirationTime);
        }

        // Todo: 优先级降低，暂停
        [Test]
        public virtual void TestShutdownAndMultipleLockedJobs()
        {
            // given
            var deployment =
                _engineRule.RepositoryService.CreateDeployment().AddModelInstance("foo.bpmn", SingleAsyncTask).Deploy();
            _engineRule.ManageDeployment(deployment);

            // add two jobs by starting two process instances
            _engineRule.RuntimeService.StartProcessInstanceByKey("process");
            _engineRule.RuntimeService.StartProcessInstanceByKey("process");

            _jobExecutor.Start();

            // Wait before acquisition
            _acquisitionThread.WaitForSync();
            // Wait for no more acquisition syncs
            _acquisitionThread.IgnoreFutureSyncs();

            _acquisitionThread.MakeContinue();

            // when Waiting during execution of first job
            ExecutionThread.WaitForSync();

            // jobs must now be locked
            var lockedJobList = _engineRule.ManagementService.CreateJobQuery().ToList();
            Assert.AreEqual(2, lockedJobList.Count);
            foreach (var job in lockedJobList)
            {
                var j = (JobEntity)job;
                Assert.NotNull(j.LockOwner);
            }

            // shut down the job executor while first job is executing
            _jobExecutor.Shutdown();

            // then let first job continue
            ExecutionThread.WaitUntilDone();

            // check that only one job left, which is not executed nor locked
            var jobEntity = (JobEntity)_engineRule.ManagementService.CreateJobQuery().First();
            Assert.NotNull(jobEntity);
            Assert.True(lockedJobList[1].Id.Equals(jobEntity.Id) || lockedJobList[0].Id.Equals(jobEntity.Id));
            Assert.IsNull(jobEntity.LockOwner);
            Assert.IsNull(jobEntity.LockExpirationTime);
        }


        public class SyncDelegate : IJavaDelegate
        {
            //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
            //ORIGINAL LINE: @Override public void Execute(org.Camunda.bpm.Engine.Delegate.IDelegateExecution execution) throws Exception
            public void Execute(IBaseDelegateExecution execution)
            {
                ExecutionThread.Sync();
            }
        }
    }
}