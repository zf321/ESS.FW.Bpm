using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Management;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Task;
using ESS.FW.Bpm.Engine.Variable;
using ESS.FW.Bpm.Engine.Variable.Value;
using NUnit.Framework;


namespace Engine.Tests.Api.MultiTenancy
{
    

    public class MultiTenancyExecutionPropagationTest : PluggableProcessEngineTestCase
	{

	  protected internal const string CMMN_FILE = "resources/api/cmmn/oneTaskCase.cmmn";
	  protected internal const string SET_VARIABLE_CMMN_FILE = "resources/api/multitenancy/HumanTaskSetVariableExecutionListener.cmmn";

	  protected internal const string PROCESS_DEFINITION_KEY = "testProcess";
	  protected internal const string TENANT_ID = "tenant1";

	   [Test]   public virtual void testPropagateTenantIdToProcessDefinition()
	  {

		DeploymentForTenant(TENANT_ID, ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_DEFINITION_KEY).Done());

		IProcessDefinition processDefinition = repositoryService.CreateProcessDefinitionQuery().First();

		Assert.NotNull(processDefinition);
		// inherit the tenant id from deployment
		Assert.AreEqual(TENANT_ID, processDefinition.TenantId);
	  }

	   [Test]   public virtual void testPropagateTenantIdToProcessInstance()
	  {
		DeploymentForTenant(TENANT_ID, ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_DEFINITION_KEY).StartEvent().UserTask().EndEvent().Done());

		startProcessInstance(PROCESS_DEFINITION_KEY);

		IProcessInstance processInstance = runtimeService.CreateProcessInstanceQuery().First();
		//Assert.That(processInstance, Is.Not.EqualTo(null));
		// inherit the tenant id from process definition
		Assert.That(processInstance.TenantId, Is.EqualTo(TENANT_ID));
	  }

	   [Test]   public virtual void testPropagateTenantIdToConcurrentExecution()
	  {

		//DeploymentForTenant(TENANT_ID, Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_DEFINITION_KEY).StartEvent()
         //   .ParallelGateway("fork").UserTask().ParallelGateway("join").EndEvent().MoveToNode("fork").UserTask().connectTo("join").Done());

		startProcessInstance(PROCESS_DEFINITION_KEY);

		IList<IExecution> executions = runtimeService.CreateExecutionQuery().ToList();
		Assert.That(executions.Count, Is.EqualTo(3));
		Assert.That(executions[0].TenantId, Is.EqualTo(TENANT_ID));
		// inherit the tenant id from process instance
		Assert.That(executions[1].TenantId, Is.EqualTo(TENANT_ID));
		Assert.That(executions[2].TenantId, Is.EqualTo(TENANT_ID));
	  }

	   [Test]   public virtual void testPropagateTenantIdToEmbeddedSubprocess()
	  {

		//DeploymentForTenant(TENANT_ID, Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_DEFINITION_KEY).StartEvent().SubProcess().EmbeddedSubProcess().StartEvent().UserTask().EndEvent().SubProcessDone().EndEvent().Done());

		startProcessInstance(PROCESS_DEFINITION_KEY);

	      IList<IExecution> executions = runtimeService.CreateExecutionQuery()
	          .ToList();
		Assert.That(executions.Count, Is.EqualTo(2));
		Assert.That(executions[0].TenantId, Is.EqualTo(TENANT_ID));
		// inherit the tenant id from parent execution (e.g. process instance)
		Assert.That(executions[1].TenantId, Is.EqualTo(TENANT_ID));
	  }

	   [Test]   public virtual void testPropagateTenantIdToTask()
	  {

		DeploymentForTenant(TENANT_ID, ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_DEFINITION_KEY).StartEvent().UserTask().EndEvent().Done());

		startProcessInstance(PROCESS_DEFINITION_KEY);

		ITask task = taskService.CreateTaskQuery().First();
		Assert.That(task, Is.Not.EqualTo(null));
		// inherit the tenant id from execution
		Assert.That(task.TenantId, Is.EqualTo(TENANT_ID));
	  }

	   [Test]   public virtual void testPropagateTenantIdToVariableInstanceOnStartProcessInstance()
	  {

		DeploymentForTenant(TENANT_ID, ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_DEFINITION_KEY).StartEvent().UserTask().EndEvent().Done());

		IVariableMap variables = ESS.FW.Bpm.Engine.Variable.Variables.PutValue("var", "test");

		IProcessDefinition processDefinition = repositoryService.CreateProcessDefinitionQuery().First();
		runtimeService.StartProcessInstanceById(processDefinition.Id, variables as IDictionary<string,ITypedValue>);

		IVariableInstance variableInstance = runtimeService.CreateVariableInstanceQuery().First();
		//Assert.That(variableInstance, Is.Not.EqualTo(null));
		// inherit the tenant id from process instance
		Assert.That(variableInstance.TenantId, Is.EqualTo(TENANT_ID));
	  }

	   [Test]   public virtual void testPropagateTenantIdToVariableInstanceFromExecution()
	  {

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
		DeploymentForTenant(TENANT_ID, ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_DEFINITION_KEY).StartEvent().ServiceTask().CamundaClass(typeof(SetVariableTask).FullName).CamundaAsyncAfter().EndEvent().Done());

		startProcessInstance(PROCESS_DEFINITION_KEY);

		IVariableInstance variableInstance = runtimeService.CreateVariableInstanceQuery().First();
            Assert.That(variableInstance, Is.Not.EqualTo(null));
            // inherit the tenant id from execution
            Assert.That(variableInstance.TenantId, Is.EqualTo(TENANT_ID));
	  }

	   [Test]   public virtual void testPropagateTenantIdToVariableInstanceFromTask()
	  {

		DeploymentForTenant(TENANT_ID, ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_DEFINITION_KEY).StartEvent().UserTask().CamundaAsyncAfter().EndEvent().Done());

		startProcessInstance(PROCESS_DEFINITION_KEY);

		IVariableMap variables = ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables().PutValue("var", "test");
		ITask task = taskService.CreateTaskQuery().First();
		taskService.SetVariablesLocal(task.Id, variables);

		IVariableInstance variableInstance = runtimeService.CreateVariableInstanceQuery().First();
		//Assert.That(variableInstance, Is.Not.EqualTo(null));
		// inherit the tenant id from task
		Assert.That(variableInstance.TenantId, Is.EqualTo(TENANT_ID));
	  }

	   [Test]   public virtual void testPropagateTenantIdToStartMessageEventSubscription()
	  {

		DeploymentForTenant(TENANT_ID, ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_DEFINITION_KEY).StartEvent().Message("start").EndEvent().Done());

		// the event subscription of the message start is created on deployment
		IEventSubscription eventSubscription = runtimeService.CreateEventSubscriptionQuery().First();
            Assert.That(eventSubscription, Is.Not.EqualTo(null));
            // inherit the tenant id from process definition
            Assert.That(eventSubscription.TenantId, Is.EqualTo(TENANT_ID));
	  }

	   [Test]   public virtual void testPropagateTenantIdToStartSignalEventSubscription()
	  {

		DeploymentForTenant(TENANT_ID, ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_DEFINITION_KEY).StartEvent().Signal("start").EndEvent().Done());

		// the event subscription of the signal start event is created on deployment
		IEventSubscription eventSubscription = runtimeService.CreateEventSubscriptionQuery().First();
            Assert.That(eventSubscription, Is.Not.EqualTo(null));
            // inherit the tenant id from process definition
            Assert.That(eventSubscription.TenantId, Is.EqualTo(TENANT_ID));
	  }

	   [Test]   public virtual void testPropagateTenantIdToIntermediateMessageEventSubscription()
	  {

		DeploymentForTenant(TENANT_ID, ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_DEFINITION_KEY).StartEvent().IntermediateCatchEvent().Message("start").EndEvent().Done());

		startProcessInstance(PROCESS_DEFINITION_KEY);

		IEventSubscription eventSubscription = runtimeService.CreateEventSubscriptionQuery().First();
            Assert.That(eventSubscription, Is.Not.EqualTo(null));
            // inherit the tenant id from process instance
            Assert.That(eventSubscription.TenantId, Is.EqualTo(TENANT_ID));
	  }

	   [Test]   public virtual void testPropagateTenantIdToIntermediateSignalEventSubscription()
	  {

		DeploymentForTenant(TENANT_ID, ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_DEFINITION_KEY).StartEvent().IntermediateCatchEvent().Signal("start").EndEvent().Done());

		startProcessInstance(PROCESS_DEFINITION_KEY);

		IEventSubscription eventSubscription = runtimeService.CreateEventSubscriptionQuery().First();
            Assert.That(eventSubscription, Is.Not.EqualTo(null));
            // inherit the tenant id from process instance
            Assert.That(eventSubscription.TenantId, Is.EqualTo(TENANT_ID));
	  }

	   [Test]   public virtual void testPropagateTenantIdToCompensationEventSubscription()
	  {

		DeploymentForTenant(TENANT_ID, "resources/api/multitenancy/compensationBoundaryEvent.bpmn");

		startProcessInstance(PROCESS_DEFINITION_KEY);

		// the event subscription is created after execute the activity with the attached compensation boundary event
		IEventSubscription eventSubscription = runtimeService.CreateEventSubscriptionQuery().First();
            Assert.That(eventSubscription, Is.Not.EqualTo(null));
            // inherit the tenant id from process instance
            Assert.That(eventSubscription.TenantId, Is.EqualTo(TENANT_ID));
	  }

	   [Test]   public virtual void testPropagateTenantIdToStartTimerJobDefinition()
	  {

		DeploymentForTenant(TENANT_ID, ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_DEFINITION_KEY).StartEvent().TimerWithDuration("PT1M").EndEvent().Done());

		// the job definition is created on deployment
		IJobDefinition jobDefinition = managementService.CreateJobDefinitionQuery().First();
	      Assert.NotNull(jobDefinition);
		// inherit the tenant id from process definition
		Assert.That(jobDefinition.TenantId, Is.EqualTo(TENANT_ID));
	  }

	   [Test]   public virtual void testPropagateTenantIdToIntermediateTimerJob()
	  {

		DeploymentForTenant(TENANT_ID, ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_DEFINITION_KEY).StartEvent().IntermediateCatchEvent().TimerWithDuration("PT1M").EndEvent().Done());

		startProcessInstance(PROCESS_DEFINITION_KEY);

		// the job is created when the timer event is reached
		IJob job = managementService.CreateJobQuery().First();
		Assert.That(job, Is.Not.EqualTo(null));
		// inherit the tenant id from job definition
		Assert.That(job.TenantId, Is.EqualTo(TENANT_ID));
	  }

	   [Test]   public virtual void testPropagateTenantIdToAsyncJob()
	  {

		DeploymentForTenant(TENANT_ID, ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_DEFINITION_KEY).StartEvent().UserTask().CamundaAsyncBefore().EndEvent().Done());

		startProcessInstance(PROCESS_DEFINITION_KEY);

		// the job is created when the asynchronous activity is reached
		IJob job = managementService.CreateJobQuery().First();
		Assert.That(job, Is.Not.EqualTo(null));
		// inherit the tenant id from job definition
		Assert.That(job.TenantId, Is.EqualTo(TENANT_ID));
	  }

	   [Test]   public virtual void testPropagateTenantIdToFailedJobIncident()
	  {

		DeploymentForTenant(TENANT_ID, ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_DEFINITION_KEY).StartEvent().ServiceTask().CamundaExpression("${failing}").CamundaAsyncBefore().EndEvent().Done());

		startProcessInstance(PROCESS_DEFINITION_KEY);

		ExecuteAvailableJobs();

		IIncident incident = runtimeService.CreateIncidentQuery().First();
		//Assert.That(incident, Is.Not.EqualTo(null));
		// inherit the tenant id from execution
		Assert.That(incident.TenantId, Is.EqualTo(TENANT_ID));
	  }

	   [Test]   public virtual void testPropagateTenantIdToFailedStartTimerIncident()
	  {

		DeploymentForTenant(TENANT_ID, ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_DEFINITION_KEY).StartEvent().TimerWithDuration("PT1M").ServiceTask().CamundaExpression("${failing}").EndEvent().Done());

		ExecuteAvailableJobs();

		IIncident incident = runtimeService.CreateIncidentQuery().First();
            Assert.That(incident, Is.Not.EqualTo(null));
            // inherit the tenant id from job
            Assert.That(incident.TenantId, Is.EqualTo(TENANT_ID));
	  }

	   [Test]   public virtual void testPropagateTenantIdToFailedExternalTaskIncident()
	  {

		DeploymentForTenant(TENANT_ID, ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_DEFINITION_KEY).StartEvent().ServiceTask().CamundaType("external").CamundaTopic("test").EndEvent().Done());

		startProcessInstance(PROCESS_DEFINITION_KEY);

		// fetch the external task and mark it as failed which create an incident
		//IList<ILockedExternalTask> tasks = externalTaskService.FetchAndLock(1, "test-worker").Topic("test", 1000).Execute();
		//externalTaskService.HandleFailure(tasks[0].Id, "test-worker", "expected", 0, 0);

		IIncident incident = runtimeService.CreateIncidentQuery().First();
		//Assert.That(incident, Is.Not.EqualTo(null));
		// inherit the tenant id from execution
		Assert.That(incident.TenantId, Is.EqualTo(TENANT_ID));
	  }

	   [Test]   public virtual void testPropagateTenantIdToExternalTask()
	  {

		DeploymentForTenant(TENANT_ID, ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_DEFINITION_KEY).StartEvent().ServiceTask().CamundaType("external").CamundaTopic("test").EndEvent().Done());

		startProcessInstance(PROCESS_DEFINITION_KEY);

		ESS.FW.Bpm.Engine.Externaltask.IExternalTask externalTask = externalTaskService.CreateExternalTaskQuery().First();
		//Assert.That(externalTask, Is.Not.EqualTo(null));
		// inherit the tenant id from execution
		Assert.That(externalTask.TenantId, Is.EqualTo(TENANT_ID));

		//IList<ILockedExternalTask> externalTasks = externalTaskService.FetchAndLock(1, "test").Topic("test", 1000).Execute();
		//Assert.That(externalTasks.Count, Is.EqualTo(1));
		//Assert.That(externalTasks[0].TenantId, Is.EqualTo(TENANT_ID));
	  }

	   [Test]   public virtual void testPropagateTenantIdToVariableInstanceOnCreateCaseInstance()
	  {

		DeploymentForTenant(TENANT_ID, CMMN_FILE);

		IVariableMap variables = ESS.FW.Bpm.Engine.Variable.Variables.PutValue("var", "test");

		ICaseDefinition caseDefinition = repositoryService.CreateCaseDefinitionQuery().First();
		caseService.CreateCaseInstanceById(caseDefinition.Id, variables);

		IVariableInstance variableInstance = runtimeService.CreateVariableInstanceQuery().First();
		//Assert.That(variableInstance, Is.Not.EqualTo(null));
		// inherit the tenant id from case instance
		Assert.That(variableInstance.TenantId, Is.EqualTo(TENANT_ID));
	  }

	   [Test]   public virtual void testPropagateTenantIdToVariableInstanceFromCaseExecution()
	  {

		DeploymentForTenant(TENANT_ID, SET_VARIABLE_CMMN_FILE);

		createCaseInstance();

		IVariableInstance variableInstance = runtimeService.CreateVariableInstanceQuery().First();
		//Assert.That(variableInstance, Is.Not.EqualTo(null));
		// inherit the tenant id from case execution
		Assert.That(variableInstance.TenantId, Is.EqualTo(TENANT_ID));
	  }

	   [Test]   public virtual void testPropagateTenantIdToVariableInstanceFromHumanTask()
	  {

		DeploymentForTenant(TENANT_ID, CMMN_FILE);

		createCaseInstance();

		IVariableMap variables = ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables().PutValue("var", "test");
		ICaseExecution caseExecution = caseService.CreateCaseExecutionQuery(c=>c.ActivityId == "PI_HumanTask_1").First();
		caseService.SetVariables(caseExecution.Id, variables);

		IVariableInstance variableInstance = runtimeService.CreateVariableInstanceQuery().First();
		//Assert.That(variableInstance, Is.Not.EqualTo(null));
		// inherit the tenant id from human task
		Assert.That(variableInstance.TenantId, Is.EqualTo(TENANT_ID));
	  }

	  public class SetVariableTask : IJavaDelegate
	  {
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void execute(org.Camunda.bpm.Engine.Delegate.IDelegateExecution execution) throws Exception
		public void Execute(IBaseDelegateExecution execution)
		{
		  execution.SetVariable("var", "test");
		}
	  }

	  protected internal virtual void startProcessInstance(string processDefinitionKey)
	  {
		IProcessDefinition processDefinition = repositoryService.CreateProcessDefinitionQuery(c=>c.Key ==processDefinitionKey)/*.LatestVersion()*/.First();

		runtimeService.StartProcessInstanceById(processDefinition.Id);
	  }

	  protected internal virtual void createCaseInstance()
	  {
		ICaseDefinition caseDefinition = repositoryService.CreateCaseDefinitionQuery().First();
		caseService.CreateCaseInstanceById(caseDefinition.Id);
	  }

	}

}