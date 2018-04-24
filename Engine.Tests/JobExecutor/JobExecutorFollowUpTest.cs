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
    ///     Test cases for handling of new jobs created while a job is executed
    /// </summary>
    [TestFixture]
    public class JobExecutorFollowUpTest
    {
        #region members
        private readonly IBpmnModelInstance _twoTasksProcess = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process").StartEvent().ServiceTask("serviceTask1").CamundaAsyncBefore().Func(m => m.CamundaClass = (typeof(SyncDelegate).AssemblyQualifiedName)).ServiceTask("serviceTask2").CamundaAsyncBefore().Func(m => m.CamundaClass = typeof(SyncDelegate).AssemblyQualifiedName).EndEvent().Done();
        private readonly IBpmnModelInstance _callActivityProcess = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("callActivityProcess").StartEvent().CallActivity("callActivity").CamundaAsyncBefore().Func(m => m.CalledElement = "oneTaskProcess").EndEvent().Done();
        private readonly IBpmnModelInstance _oneTaskProcess = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("oneTaskProcess").StartEvent().UserTask("serviceTask").CamundaAsyncBefore().EndEvent().Done();

        protected static  ConcurrencyTestCase.ThreadControl executionThread;
        private readonly bool _instanceFieldsInitialized;
        private ConcurrencyTestCase.ThreadControl _acquisitionThread;

        private ProcessEngineBootstrapRule _bootstrapRule;

        private ProcessEngineRule _engineRule;
        
        private ControllableJobExecutor _jobExecutor;
        private ProcessEngineTestRule _testRule;
        #endregion


        public JobExecutorFollowUpTest()
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
            _testRule = new ProcessEngineTestRule(_engineRule);
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
        public void SetUpTestRule()
        {
            _testRule.Starting();
        }

        [SetUp]
        public virtual void SetUp()
        {
            _jobExecutor =
                (ControllableJobExecutor)
                ((ProcessEngineConfigurationImpl)_engineRule.ProcessEngine.ProcessEngineConfiguration).JobExecutor;
            _jobExecutor.MaxJobsPerAcquisition = 2;
            _acquisitionThread = _jobExecutor.AcquisitionThreadControl;
            executionThread = _jobExecutor.ExecutionThreadControl;
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










        protected internal  ControllableJobExecutor BuildControllableJobExecutor()
        {
            var jobExecutor = new ControllableJobExecutor();
            jobExecutor.MaxJobsPerAcquisition = 2;
            jobExecutor.ProceedAndWaitOnShutdown(false);
            return jobExecutor;
        }


        // Todo: 优先级降低，暂停
        [Test]
        public virtual void TestExecuteExclusiveFollowUpJobInSameProcessInstance()
        {
            _testRule.Deploy(_twoTasksProcess);

            // given
            // a process instance with a single job
            var processInstance = _engineRule.RuntimeService.StartProcessInstanceByKey("process");

            _jobExecutor.Start();

            // and first job acquisition that acquires the job
            _acquisitionThread.WaitForSync();
            _acquisitionThread.MakeContinueAndWaitForSync();
            // and first job execution
            _acquisitionThread.MakeContinue();

            // Waiting inside delegate
            executionThread.WaitForSync();

            // completing delegate
            executionThread.MakeContinueAndWaitForSync();

            // then
            // the follow-up job should be executed right away
            // i.E., there is a transition instance for the second service task
            var activityInstance = _engineRule.RuntimeService.GetActivityInstance(processInstance.Id);
            Assert.AreEqual(1, activityInstance.GetTransitionInstances("serviceTask2").Length);

            // and the corresponding job is locked
            var followUpJob = (JobEntity)_engineRule.ManagementService.CreateJobQuery().First();
            Assert.NotNull(followUpJob);
            Assert.NotNull(followUpJob.LockOwner);
            Assert.NotNull(followUpJob.LockExpirationTime);

            // and the job can be completed successfully such that the process instance ends
            executionThread.MakeContinue();
            _acquisitionThread.WaitForSync();

            // and the process instance has finished
            _testRule.AssertProcessEnded(processInstance.Id);
        }

        // Todo: 优先级降低，暂停
        [Test]
        public virtual void TestExecuteExclusiveFollowUpJobInDifferentProcessInstance()
        {
            _testRule.Deploy(_callActivityProcess, _oneTaskProcess);

            // given
            // a process instance with a single job
            var processInstance = _engineRule.RuntimeService.StartProcessInstanceByKey("callActivityProcess");

            _jobExecutor.Start();

            // and first job acquisition that acquires the job
            _acquisitionThread.WaitForSync();
            _acquisitionThread.MakeContinueAndWaitForSync();
            // and job is executed
            _acquisitionThread.MakeContinueAndWaitForSync();

            // then
            // the called instance has been created

            //var calledInstance =EngineRule.RuntimeService.CreateProcessInstanceQuery()
            //        //SuperProcessInstanceId(processInstance.Id)
            //        .First();
            var calledInstance = _engineRule.RuntimeService.GetManager<IExecutionManager>()
                .FindProcessInstancesBySuperProcessInstanceId(processInstance.Id,null).SingleOrDefault();

            Assert.NotNull(calledInstance);

            // and there is a transition instance for the service task
            var activityInstance = _engineRule.RuntimeService.GetActivityInstance(calledInstance.Id);
            Assert.AreEqual(1, activityInstance.GetTransitionInstances("serviceTask").Length);

            // but the corresponding job is not locked
            var followUpJob = (JobEntity)_engineRule.ManagementService.CreateJobQuery().First();
            Assert.NotNull(followUpJob);
            Assert.IsNull(followUpJob.LockOwner);
            Assert.IsNull(followUpJob.LockExpirationTime);
        }

        private class ProcessEngineBootstrapRuleAnonymousInnerClass : ProcessEngineBootstrapRule
        {
            public static JobExecutorFollowUpTest OuterInstance { private get; set; }
            public override ProcessEngineConfiguration ConfigureEngine(ProcessEngineConfigurationImpl configuration)
            {
                return configuration.SetJobExecutor(OuterInstance.BuildControllableJobExecutor());
            }
        }


        public class SyncDelegate : IJavaDelegate
        {
            public void Execute(IBaseDelegateExecution execution)
            {
                executionThread.Sync();
            }
        }
    }
}