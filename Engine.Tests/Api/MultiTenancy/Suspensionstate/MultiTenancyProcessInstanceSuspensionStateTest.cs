

//using System.Collections.Generic;
//using System.Linq;
//using ESS.FW.Bpm.Engine.Externaltask;
//using ESS.FW.Bpm.Engine.Repository;
//using ESS.FW.Bpm.Engine.Runtime;
//using ESS.FW.Bpm.Engine.Task;
//using ESS.FW.Bpm.Engine.Tests.Util;
//using ESS.FW.Bpm.Model.Bpmn;
//using NUnit.Framework;

//namespace ESS.FW.Bpm.Engine.Tests.Api.MultiTenancy.Suspensionstate
//{
    

//	public class MultiTenancyProcessInstanceSuspensionStateTest
//	{
//		private bool InstanceFieldsInitialized = false;

//		public MultiTenancyProcessInstanceSuspensionStateTest()
//		{
//			if (!InstanceFieldsInitialized)
//			{
//				InitializeInstanceFields();
//				InstanceFieldsInitialized = true;
//			}
//		}

//		private void InitializeInstanceFields()
//		{
//			testRule = new ProcessEngineTestRule(engineRule);
//			//ruleChain = RuleChain.outerRule(engineRule).around(testRule);
//		}


//	  protected internal const string TENANT_ONE = "tenant1";
//	  protected internal const string TENANT_TWO = "tenant2";

//	  protected internal const string PROCESS_DEFINITION_KEY = "testProcess";

//	  protected internal static readonly IBpmnModelInstance PROCESS = Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_DEFINITION_KEY)
//            .StartEvent().ParallelGateway("fork").UserTask()
//         //   .MoveToLastGateway().SendTask().CamundaType("external")
//         //   .CamundaTopic("test").BoundaryEvent()
//          //  .TimerWithDuration("PT1M")
//            .Done();

//	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();

//	  protected internal ProcessEngineTestRule testRule;

//      [SetUp]
//	  public virtual void setUp()
//	  {

//		testRule.DeployForTenant(TENANT_ONE, PROCESS);
//		testRule.DeployForTenant(TENANT_TWO, PROCESS);
//		testRule.Deploy(PROCESS);

//		engineRule.RuntimeService.CreateProcessInstanceByKey(PROCESS_DEFINITION_KEY).SetProcessDefinitionTenantId(TENANT_ONE).Execute();
//		engineRule.RuntimeService.CreateProcessInstanceByKey(PROCESS_DEFINITION_KEY).SetProcessDefinitionTenantId(TENANT_TWO).Execute();
//		engineRule.RuntimeService.CreateProcessInstanceByKey(PROCESS_DEFINITION_KEY).ProcessDefinitionWithoutTenantId().Execute();
//	  }

//        [Test]
//        public virtual void suspendAndActivateProcessInstancesForAllTenants()
//	  {
//		// given activated process instances
//		 IQueryable<IProcessInstance> query = engineRule.RuntimeService.CreateProcessInstanceQuery();
//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(3L));
//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(0L));

//		// first suspend
//		engineRule.RuntimeService.UpdateProcessInstanceSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).Suspend();

//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(0L));
//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(3L));

//		// then activate
//		engineRule.RuntimeService.UpdateProcessInstanceSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).Activate();

//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(3L));
//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(0L));
//	  }

//        [Test]
//        public virtual void suspendProcessInstanceForTenant()
//	  {
//		// given activated process instances
//		 IQueryable<IProcessInstance> query = engineRule.RuntimeService.CreateProcessInstanceQuery();
//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(3L));
//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(0L));

//		engineRule.RuntimeService.UpdateProcessInstanceSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY)
//                .ProcessDefinitionTenantId(TENANT_ONE)
//                .Suspend();

//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(2L));
//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(1L));
//		Assert.That(query.Where(c => c.SuspensionState == SuspensionStateFields.Suspended.StateCode &&c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
//	  }

//        [Test]
//        public virtual void suspendProcessInstanceForNonTenant()
//	  {
//		// given activated process instances
//		 IQueryable<IProcessInstance> query = engineRule.RuntimeService.CreateProcessInstanceQuery();
//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(3L));
//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(0L));

//		engineRule.RuntimeService.UpdateProcessInstanceSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).ProcessDefinitionWithoutTenantId().Suspend();

//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(2L));
//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(1L));
//		Assert.That(query.Where(c => c.SuspensionState == SuspensionStateFields.Suspended.StateCode &&c.TenantId == null).Count(), Is.EqualTo(1L));
//	  }

//        [Test]
//        public virtual void activateProcessInstanceForTenant()
//	  {
//		// given suspended process instances
//		engineRule.RuntimeService.UpdateProcessInstanceSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).Suspend();

//		 IQueryable<IProcessInstance> query = engineRule.RuntimeService.CreateProcessInstanceQuery();
//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(0L));
//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(3L));

//		engineRule.RuntimeService.UpdateProcessInstanceSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).ProcessDefinitionTenantId(TENANT_ONE).Activate();

//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(2L));
//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(1L));
//		Assert.That(query.Where(c => c.SuspensionState == SuspensionStateFields.Active.StateCode &&c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
//	  }

//        [Test]
//        public virtual void activateProcessInstanceForNonTenant()
//	  {
//		// given suspended process instances
//		engineRule.RuntimeService.UpdateProcessInstanceSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).Suspend();

//		 IQueryable<IProcessInstance> query = engineRule.RuntimeService.CreateProcessInstanceQuery();
//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(0L));
//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(3L));

//		engineRule.RuntimeService.UpdateProcessInstanceSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).ProcessDefinitionWithoutTenantId().Activate();

//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(2L));
//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(1L));
//		Assert.That(query.Where(c => c.SuspensionState == SuspensionStateFields.Active.StateCode &&c.TenantId == null).Count(), Is.EqualTo(1L));
//	  }

//        [Test]
//        public virtual void suspendAndActivateProcessInstancesIncludingUserTasksForAllTenants()
//	  {
//		// given activated user tasks
//		IQueryable<ITask> query = engineRule.TaskService.CreateTaskQuery();
//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(3L));
//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(0L));

//		// first suspend
//		engineRule.RuntimeService.UpdateProcessInstanceSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).Suspend();

//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(0L));
//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(3L));

//		// then activate
//		engineRule.RuntimeService.UpdateProcessInstanceSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).Activate();

//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(3L));
//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(0L));
//	  }

//        [Test]
//        public virtual void suspendProcessInstanceIncludingUserTaskForTenant()
//	  {
//		// given activated user tasks
//		IQueryable<ITask> query = engineRule.TaskService.CreateTaskQuery();
//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(3L));
//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(0L));

//		engineRule.RuntimeService.UpdateProcessInstanceSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).ProcessDefinitionTenantId(TENANT_ONE).Suspend();

//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(2L));
//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(1L));
//		Assert.That(query.Where(c => c.SuspensionState == SuspensionStateFields.Suspended.StateCode &&c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
//	  }

//        [Test]
//        public virtual void suspendProcessInstanceIncludingUserTaskForNonTenant()
//	  {
//		// given activated user tasks
//		IQueryable<ITask> query = engineRule.TaskService.CreateTaskQuery();
//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(3L));
//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(0L));

//		engineRule.RuntimeService.UpdateProcessInstanceSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).ProcessDefinitionWithoutTenantId().Suspend();

//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(2L));
//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(1L));
//		Assert.That(query.Where(c => c.SuspensionState == SuspensionStateFields.Suspended.StateCode &&c.TenantId == null).Count(), Is.EqualTo(1L));
//	  }

//        [Test]
//        public virtual void activateProcessInstanceIncludingUserTaskForTenant()
//	  {
//		// given suspended user tasks
//		engineRule.RuntimeService.UpdateProcessInstanceSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).Suspend();

//		IQueryable<ITask> query = engineRule.TaskService.CreateTaskQuery();
//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(0L));
//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(3L));

//		engineRule.RuntimeService.UpdateProcessInstanceSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).ProcessDefinitionTenantId(TENANT_ONE).Activate();

//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(2L));
//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(1L));
//		Assert.That(query.Where(c => c.SuspensionState == SuspensionStateFields.Active.StateCode &&c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
//	  }

//        [Test]
//        public virtual void activateProcessInstanceIncludingUserTaskForNonTenant()
//	  {
//		// given suspended user tasks
//		engineRule.RuntimeService.UpdateProcessInstanceSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).Suspend();

//		IQueryable<ITask> query = engineRule.TaskService.CreateTaskQuery();
//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(0L));
//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(3L));

//		engineRule.RuntimeService.UpdateProcessInstanceSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).ProcessDefinitionWithoutTenantId().Activate();

//		//Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(2L));
//		//Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(1L));
//		//Assert.That(query.Where(c => c.SuspensionState == SuspensionStateFields.Active.StateCode &&c.TenantId == null).Count(), Is.EqualTo(1L));
//	  }

//        [Test]
//        public virtual void suspendAndActivateProcessInstancesIncludingExternalTasksForAllTenants()
//	  {
//		// given activated external tasks
//		IQueryable<Externaltask.IExternalTask> query = engineRule.ExternalTaskService.CreateExternalTaskQuery();
//		//Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(3L));
//		//Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(0L));

//		// first suspend
//		engineRule.RuntimeService.UpdateProcessInstanceSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).Suspend();

//		//Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(0L));
//		//Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(3L));

//		// then activate
//		engineRule.RuntimeService.UpdateProcessInstanceSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).Activate();

//		//Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(3L));
//		//Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(0L));
//	  }

//        [Test]
//        public virtual void suspendProcessInstanceIncludingExternalTaskForTenant()
//	  {
//		// given activated external tasks
//		IQueryable<Externaltask.IExternalTask> query = engineRule.ExternalTaskService.CreateExternalTaskQuery();
//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(3L));
//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(0L));

//		engineRule.RuntimeService.UpdateProcessInstanceSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).ProcessDefinitionTenantId(TENANT_ONE).Suspend();

//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(2L));
//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(1L));
//		Assert.That(query.Where(c => c.SuspensionState == SuspensionStateFields.Suspended.StateCode &&c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
//	  }

//        [Test]
//        public virtual void suspendProcessInstanceIncludingExternalTaskForNonTenant()
//	  {
//		// given activated external tasks
//		IQueryable<IExternalTask> query = engineRule.ExternalTaskService.CreateExternalTaskQuery();
//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(3L));
//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(0L));

//		engineRule.RuntimeService.UpdateProcessInstanceSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).ProcessDefinitionWithoutTenantId().Suspend();

//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(2L));
//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(1L));
//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).First().TenantId, Is.EqualTo(null));
//	  }

//        [Test]
//        public virtual void activateProcessInstanceIncludingExternalTaskForTenant()
//	  {
//		// given suspended external tasks
//		engineRule.RuntimeService.UpdateProcessInstanceSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).Suspend();

//		IQueryable<IExternalTask> query = engineRule.ExternalTaskService.CreateExternalTaskQuery();
//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(0L));
//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(3L));

//		engineRule.RuntimeService.UpdateProcessInstanceSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).ProcessDefinitionTenantId(TENANT_ONE).Activate();

//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(2L));
//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(1L));
//		Assert.That(query.Where(c => c.SuspensionState == SuspensionStateFields.Active.StateCode &&c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
//	  }

//        [Test]
//        public virtual void activateProcessInstanceIncludingExternalTaskForNonTenant()
//	  {
//		// given suspended external tasks
//		engineRule.RuntimeService.UpdateProcessInstanceSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).Suspend();

//		IQueryable<IExternalTask> query = engineRule.ExternalTaskService.CreateExternalTaskQuery();
//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(0L));
//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(3L));

//		engineRule.RuntimeService.UpdateProcessInstanceSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).ProcessDefinitionWithoutTenantId().Activate();

//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(2L));
//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(1L));
//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).First().TenantId, Is.EqualTo(null));
//	  }

//        [Test]
//        public virtual void suspendAndActivateProcessInstancesIncludingJobsForAllTenants()
//	  {
//		// given activated jobs
//		IQueryable<IJob> query = engineRule.ManagementService.CreateJobQuery();
//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(3L));
//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(0L));

//		// first suspend
//		engineRule.RuntimeService.UpdateProcessInstanceSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).Suspend();

//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(0L));
//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(3L));

//		// then activate
//		engineRule.RuntimeService.UpdateProcessInstanceSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).Activate();

//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(3L));
//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(0L));
//	  }

//        [Test]
//        public virtual void suspendProcessInstanceIncludingJobForTenant()
//	  {
//		// given activated jobs
//		IQueryable<IJob> query = engineRule.ManagementService.CreateJobQuery();
//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(3L));
//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(0L));

//		engineRule.RuntimeService.UpdateProcessInstanceSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).ProcessDefinitionTenantId(TENANT_ONE).Suspend();

//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(2L));
//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(1L));
//		Assert.That(query.Where(c => c.SuspensionState == SuspensionStateFields.Suspended.StateCode &&c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
//	  }

//        [Test]
//        public virtual void suspendProcessInstanceIncludingJobForNonTenant()
//	  {
//		// given activated jobs
//		IQueryable<IJob> query = engineRule.ManagementService.CreateJobQuery();
//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(3L));
//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(0L));

//		engineRule.RuntimeService.UpdateProcessInstanceSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).ProcessDefinitionWithoutTenantId().Suspend();

//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(2L));
//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(1L));
//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).First().TenantId, Is.EqualTo(null));
//	  }

//        [Test]
//        public virtual void activateProcessInstanceIncludingJobForTenant()
//	  {
//		// given suspended job
//		engineRule.RuntimeService.UpdateProcessInstanceSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).Suspend();

//		IQueryable<IJob> query = engineRule.ManagementService.CreateJobQuery();
//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(0L));
//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(3L));

//		engineRule.RuntimeService.UpdateProcessInstanceSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).ProcessDefinitionTenantId(TENANT_ONE).Activate();

//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(2L));
//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(1L));
//		Assert.That(query.Where(c => c.SuspensionState == SuspensionStateFields.Active.StateCode &&c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
//	  }

//        [Test]
//        public virtual void activateProcessInstanceIncludingJobForNonTenant()
//	  {
//		// given suspended jobs
//		engineRule.RuntimeService.UpdateProcessInstanceSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).Suspend();

//		IQueryable<IJob> query = engineRule.ManagementService.CreateJobQuery();
//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(0L));
//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(3L));

//		engineRule.RuntimeService.UpdateProcessInstanceSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).ProcessDefinitionWithoutTenantId().Activate();

//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(2L));
//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(1L));
//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).First().TenantId, Is.EqualTo(null));
//	  }

//        [Test]
//        public virtual void suspendProcessInstanceNoAuthenticatedTenants()
//	  {
//		// given activated process instances
//		 IQueryable<IProcessInstance> query = engineRule.RuntimeService.CreateProcessInstanceQuery();
//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(3L));
//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(0L));

//		engineRule.IdentityService.SetAuthentication("user", null, null);

//		engineRule.RuntimeService.UpdateProcessInstanceSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).Suspend();

//		engineRule.IdentityService.ClearAuthentication();

//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(2L));
//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(1L));
//		Assert.That(query.Where(c => c.SuspensionState == SuspensionStateFields.Suspended.StateCode &&c.TenantId == null).Count(), Is.EqualTo(1L));
//	  }

//        [Test]
//        public virtual void failToSuspendProcessInstanceByProcessDefinitionIdNoAuthenticatedTenants()
//	  {
//		IProcessDefinition processDefinition = engineRule.RepositoryService.CreateProcessDefinitionQuery(c=>c.Key == PROCESS_DEFINITION_KEY).Where(c=>c.TenantId == TENANT_ONE).First();

//		// declare expected exception
//		//thrown.Expect(typeof(ProcessEngineException));
//		//thrown.ExpectMessage("Cannot update the process definition '" + processDefinition.Id + "' because it belongs to no authenticated tenant");

//		engineRule.IdentityService.SetAuthentication("user", null, null);

//		engineRule.RuntimeService.UpdateProcessInstanceSuspensionState().ByProcessDefinitionId(processDefinition.Id).Suspend();
//	  }

//        [Test]
//        public virtual void suspendProcessInstanceWithAuthenticatedTenant()
//	  {
//		// given activated process instances
//		 IQueryable<IProcessInstance> query = engineRule.RuntimeService.CreateProcessInstanceQuery();
//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(3L));
//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(0L));

//		engineRule.IdentityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

//		engineRule.RuntimeService.UpdateProcessInstanceSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).Suspend();

//		engineRule.IdentityService.ClearAuthentication();

//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(1L));
//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(2L));
//		Assert.That(query.Where(c => c.SuspensionState == SuspensionStateFields.Active.StateCode &&c.TenantId == TENANT_TWO).Count(), Is.EqualTo(1L));
//		Assert.That(query.Where(c => c.SuspensionState == SuspensionStateFields.Suspended.StateCode &&c.TenantId == null).Count(), Is.EqualTo(1L));
//		Assert.That(query.Where(c => c.SuspensionState == SuspensionStateFields.Suspended.StateCode &&c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
//	  }

//        [Test]
//        public virtual void suspendProcessInstanceDisabledTenantCheck()
//	  {
//		// given activated process instances
//		 IQueryable<IProcessInstance> query = engineRule.RuntimeService.CreateProcessInstanceQuery();
//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(3L));
//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(0L));

//		engineRule.ProcessEngineConfiguration.SetTenantCheckEnabled(false);
//		engineRule.IdentityService.SetAuthentication("user", null, null);

//		engineRule.RuntimeService.UpdateProcessInstanceSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).Suspend();

//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(0L));
//		Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(3L));
//		Assert.That(query.Where(c => c.SuspensionState == SuspensionStateFields.Suspended.StateCode &&new []{TENANT_ONE, TENANT_TWO }.Contains(c.TenantId)).Count(), Is.EqualTo(2L));
//		Assert.That(query.Where(c => c.SuspensionState == SuspensionStateFields.Suspended.StateCode &&c.TenantId == null).Count(), Is.EqualTo(1L));
//	  }
//	}

//}