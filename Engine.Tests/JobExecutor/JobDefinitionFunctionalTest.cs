using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Cmd;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;

namespace Engine.Tests.JobExecutor
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class JobDefinitionFunctionalTest
    {
        #region members

        private ProvidedProcessEngineRule _engineRule;
        private ProcessEngineTestRule _testRule;

        private IRuntimeService _runtimeService;
        private IManagementService _managementService;
        private ProcessEngineConfigurationImpl _processEngineConfiguration;

        #endregion

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
        public virtual void InitServices()
        {
            _runtimeService = _engineRule.RuntimeService;
            _managementService = _engineRule.ManagementService;
            _processEngineConfiguration = _engineRule.ProcessEngineConfiguration;
        }

        [TearDown]
        public void TearDownEngineRule()
        {
            //_bootstrapRule.Finished();
            //_engineRule.Finished();
        }

        [TearDown]
        public void TearDownTestRule()
        {
            _testRule.Finished();
        }

        private readonly bool _instanceFieldsInitialized;

        public JobDefinitionFunctionalTest()
        {
            if (!_instanceFieldsInitialized)
            {
                InitializeInstanceFields();
                _instanceFieldsInitialized = true;
            }
        }

        private void InitializeInstanceFields()
        {
            _engineRule = new ProvidedProcessEngineRule();
            _testRule = new ProcessEngineTestRule(_engineRule);
        }

        protected internal static readonly IBpmnModelInstance _simpleAsyncProcess =
            ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("simpleAsyncProcess")
                .StartEvent()
                .ServiceTask()
                .CamundaExpression("${true}")
                .CamundaAsyncBefore()
                .EndEvent()
                .Done();

        [Test]
        public virtual void TestCreateJobInstanceSuspended()
        {
            _testRule.Deploy(_simpleAsyncProcess);

            // given suspended job definition:
            _managementService.SuspendJobDefinitionByProcessDefinitionKey("simpleAsyncProcess");

            // if I start a new instance
            _runtimeService.StartProcessInstanceByKey("simpleAsyncProcess");

            // then the new job instance is created as suspended:
            Assert.NotNull(_managementService.CreateJobQuery(c => c.SuspensionState == SuspensionStateFields.Suspended.StateCode).FirstOrDefault());
            Assert.IsNull(_managementService.CreateJobQuery(c => c.SuspensionState == SuspensionStateFields.Active.StateCode).FirstOrDefault());
        }

        [Test]
        public virtual void TestCreateJobInstanceActive()
        {
            _testRule.Deploy(_simpleAsyncProcess);

            // given that the job definition is not suspended:

            // if I start a new instance
            _runtimeService.StartProcessInstanceByKey("simpleAsyncProcess");

            // then the new job instance is created as active:
            Assert.IsNull(_managementService.CreateJobQuery(c => c.SuspensionState == SuspensionStateFields.Suspended.StateCode).FirstOrDefault());
            Assert.NotNull(_managementService.CreateJobQuery(c => c.SuspensionState == SuspensionStateFields.Active.StateCode).FirstOrDefault());
        }

        [Test]
        public virtual void TestJobExecutorOnlyAcquiresActiveJobs()
        {
            _testRule.Deploy(_simpleAsyncProcess);

            // given suspended job definition:
            _managementService.SuspendJobDefinitionByProcessDefinitionKey("simpleAsyncProcess");

            // if I start a new instance
            _runtimeService.StartProcessInstanceByKey("simpleAsyncProcess");

            // then the new job executor will not acquire the job:
            var acquiredJobs = AcquireJobs();
            Assert.AreEqual(0, acquiredJobs.Size());

            // -------------------------

            // given a active job definition:
            _managementService.ActivateJobDefinitionByProcessDefinitionKey("simpleAsyncProcess", true);

            // then the new job executor will not acquire the job:
            acquiredJobs = AcquireJobs();
            Assert.AreEqual(1, acquiredJobs.Size());
        }

        [Test]
        public virtual void TestExclusiveJobs()
        {
            var deploy = _testRule.Deploy(ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("testProcess")
                .StartEvent()
                .ServiceTask("task1")
                .CamundaExpression("${true}")
                .CamundaAsyncBefore()
                .ServiceTask("task2")
                .CamundaExpression("${true}")
                .CamundaAsyncBefore()
                .EndEvent()
                .Done());

            var processDefinition = _runtimeService.GetManager<IProcessDefinitionManager>().First(c => c.DeploymentId == deploy.Id);
            var jobDefinition = _managementService.CreateJobDefinitionQuery(c => c.ActivityId == "task2").FirstOrDefault();

            // given that the second task is suspended
            _managementService.SuspendJobDefinitionById(jobDefinition.Id);

            // if I start a process instance
            _runtimeService.StartProcessInstanceByKey("testProcess");

            _testRule.WaitForJobExecutorToProcessAllJobs(10000);

            // then the second task is not Executed
            //因为其他流程数据没有删除，所以要根据ProcessDefinition来进行查询
            //Assert.AreEqual(1, _runtimeService.CreateProcessInstanceQuery().Count());
            Assert.AreEqual(1, _runtimeService.GetManager<IExecutionManager>().FindProcessInstanceCountByProcessDefinitionId(processDefinition?.Id));
            // there is a suspended job instance
            var job = _managementService.CreateJobQuery().First();
            Assert.AreEqual(job.JobDefinitionId, jobDefinition.Id);
            Assert.True(job.Suspended);

            // if I unsuspend the job definition, the job is Executed:
            _managementService.ActivateJobDefinitionById(jobDefinition.Id, true);

            _testRule.WaitForJobExecutorToProcessAllJobs(10000);

            //因为其他流程数据没有删除，所以要根据ProcessDefinition来进行查询
            //Assert.AreEqual(0, _runtimeService.CreateProcessInstanceQuery().Count());
            Assert.AreEqual(0, _runtimeService.GetManager<IExecutionManager>().FindProcessInstanceCountByProcessDefinitionId(processDefinition?.Id));
        }

        protected internal virtual AcquiredJobs AcquireJobs()
        {
            var jobExecutor = _processEngineConfiguration.JobExecutor;

            return _processEngineConfiguration.CommandExecutorTxRequired.Execute(new AcquireJobsCmd(jobExecutor));
        }
    }
}