using System.Collections.Generic;
using System.Linq;
using Engine.Tests.ConCurrency;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;

namespace Engine.Tests.JobExecutor
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class ReuseEntityCacheTest
    {
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
            DefaultSetting = EngineConfig.DbEntityCacheReuseEnabled;
            EngineConfig.SetDbEntityCacheReuseEnabled(true);
            JobExecutor = (ControllableJobExecutor)EngineConfig.JobExecutor;
            ExecutionThreadControl = JobExecutor.ExecutionThreadControl;
            AcquisitionThreadControl = JobExecutor.AcquisitionThreadControl;
        }

        [TearDown]
        public void TearDownEngineRule()
        {
            _bootstrapRule.Finished();
            _engineRule.Finished();
        }

        [TearDown]
        public virtual void ResetEngineConfiguration()
        {
            EngineConfig.SetDbEntityCacheReuseEnabled(DefaultSetting);
        }

        [TearDown]
        public virtual void ShutdownJobExecutor()
        {
            JobExecutor.Shutdown();
        }

        private readonly bool _instanceFieldsInitialized;

        public ReuseEntityCacheTest()
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


        public const string EntityId1 = "Execution1";
        public const string EntityId2 = "Execution2";

        protected internal ProcessEngineBootstrapRule _bootstrapRule;

        private class ProcessEngineBootstrapRuleAnonymousInnerClass : ProcessEngineBootstrapRule
        {
            public override ProcessEngineConfiguration ConfigureEngine(ProcessEngineConfigurationImpl configuration)
            {
                return configuration.SetJobExecutor(new ControllableJobExecutor());
            }
        }

        protected internal ProcessEngineRule _engineRule;
        

        protected internal bool DefaultSetting;

        protected internal ControllableJobExecutor JobExecutor;

        protected internal static ConcurrencyTestCase.ThreadControl ExecutionThreadControl;
        protected internal ConcurrencyTestCase.ThreadControl AcquisitionThreadControl;

        //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
        protected internal static readonly IBpmnModelInstance Process =
            ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process")
                .StartEvent()
                .ServiceTask()
                .CamundaClass(typeof(CreateEntitiesDelegate).FullName)
                .CamundaAsyncBefore()
                .CamundaExclusive(true)
                .ServiceTask()
                .CamundaClass(typeof(UpdateEntitiesDelegate).FullName)
                .CamundaAsyncBefore()
                .CamundaExclusive(true)
                .ServiceTask()
                .CamundaClass(typeof(RemoveEntitiesDelegate).FullName)
                .CamundaAsyncBefore()
                .CamundaExclusive(true)
                .EndEvent()
                .Done();

        // Todo: 优先级降低，暂停
        [Test]
        public virtual void TestFlushOrderWithEntityCacheReuse()
        {
            // given
            var deployment =
                _engineRule.RepositoryService.CreateDeployment().AddModelInstance("foo.bpmn", Process).Deploy();
            _engineRule.ManageDeployment(deployment);

            _engineRule.RuntimeService.StartProcessInstanceByKey("process");

            // when
            JobExecutor.Start();

            // the job is acquired
            AcquisitionThreadControl.WaitForSync();

            // and job acquisition finishes successfully
            AcquisitionThreadControl.MakeContinueAndWaitForSync();
            AcquisitionThreadControl.MakeContinue();

            // and the first delegate is completed
            ExecutionThreadControl.WaitForSync();
            ExecutionThreadControl.MakeContinueAndWaitForSync();

            // and the second delegate is completed
            ExecutionThreadControl.MakeContinueAndWaitForSync();

            // and the third delegate is completed
            ExecutionThreadControl.MakeContinue();

            AcquisitionThreadControl.WaitForSync();

            // then the job has been successfully executed
            Assert.AreEqual(0, _engineRule.ManagementService.CreateJobQuery().Count());
        }

        protected internal virtual ProcessEngineConfigurationImpl EngineConfig
        {
            get { return (ProcessEngineConfigurationImpl)_engineRule.ProcessEngine.ProcessEngineConfiguration; }
        }

        public class CreateEntitiesDelegate : IJavaDelegate
        {
            //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
            //ORIGINAL LINE: @Override public void Execute(org.Camunda.bpm.Engine.Delegate.IDelegateExecution execution) throws Exception
            public void Execute(IBaseDelegateExecution execution)
            {
                var execution1 = new ExecutionEntity();
                execution1.Id = EntityId1;
                execution1.SetExecutions(new List<IActivityExecution>());

                var execution2 = new ExecutionEntity();
                execution2.Id = EntityId2;
                execution2.SetExecutions(new List<IActivityExecution>());
                execution2.SetParent(execution1);

                var executionManager = Context.CommandContext.ExecutionManager;
                executionManager.InsertExecution(execution1);
                executionManager.InsertExecution(execution2);

                ExecutionThreadControl.Sync();
            }
        }

        public class UpdateEntitiesDelegate : IJavaDelegate
        {
            public void Execute(IBaseDelegateExecution execution)
            {
                var executionManager = Context.CommandContext.ExecutionManager;
                var execution1 = executionManager.FindExecutionById(EntityId1);
                var execution2 = executionManager.FindExecutionById(EntityId2);

                // revert the references
                execution2.SetParent(null);
                execution1.SetParent(execution2);

                ExecutionThreadControl.Sync();
            }
        }

        public class RemoveEntitiesDelegate : IJavaDelegate
        {
            public void Execute(IBaseDelegateExecution execution)
            {
                var executionManager = Context.CommandContext.ExecutionManager;
                var execution1 = executionManager.FindExecutionById(EntityId1);
                var execution2 = executionManager.FindExecutionById(EntityId2);

                executionManager.DeleteExecution(execution1);
                executionManager.DeleteExecution(execution2);

                ExecutionThreadControl.Sync();
            }
        }
    }
}