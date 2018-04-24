using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Task;
using ESS.FW.Bpm.Engine.Variable;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;

namespace Engine.Tests.Api.MultiTenancy.TenantCheck
{
    

	/// <summary>
	/// 
	/// </summary>

	public class MultiTenancyHistoricDataCmdsTenantCheckTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyHistoricDataCmdsTenantCheckTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			testRule = new ProcessEngineTestRule(engineRule);
			//ruleChain = RuleChain.outerRule(engineRule).around(testRule);
		}


	  protected internal const string TENANT_ONE = "tenant1";
	  protected internal const string TENANT_TWO = "tenant2";

	  protected internal const string PROCESS_DEFINITION_KEY = "failingProcess";

	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();

	  protected internal ProcessEngineTestRule testRule;

	  protected internal IRepositoryService repositoryService;
	  protected internal IIdentityService identityService;
	  protected internal IRuntimeService runtimeService;
	  protected internal ITaskService taskService;
	  protected internal ICaseService caseService;
	  protected internal IDecisionService decisionService;
	  protected internal IHistoryService historyService;
	  protected internal ProcessEngineConfiguration processEngineConfiguration;

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(engineRule).around(testRule);
	  //public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.Rules.ExpectedException thrown = org.junit.Rules.ExpectedException.None();
	  //public ExpectedException thrown = ExpectedException.None();

	  protected internal static readonly IBpmnModelInstance BPMN_PROCESS = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_DEFINITION_KEY).StartEvent().EndEvent().Done();

	  protected internal static readonly IBpmnModelInstance FAILING_BPMN_PROCESS = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("failingProcess").StartEvent().ServiceTask().CamundaExpression("${failing}").CamundaAsyncBefore().EndEvent().Done();

	  protected internal const string CMMN_PROCESS_WITH_MANUAL_ACTIVATION = "resources/api/cmmn/oneTaskCaseWithManualActivation.cmmn";

	  protected internal const string DMN = "resources/api/multitenancy/simpleDecisionTable.Dmn";

        [SetUp]
	  public virtual void init()
	  {
		repositoryService = engineRule.RepositoryService;
		identityService = engineRule.IdentityService;
		runtimeService = engineRule.RuntimeService;
		taskService = engineRule.TaskService;
		caseService = engineRule.CaseService;
		decisionService = engineRule.DecisionService;
		historyService = engineRule.HistoryService;
		processEngineConfiguration = engineRule.ProcessEngineConfiguration;
	  }

        [Test]
        public virtual void failToDeleteHistoricProcessInstanceNoAuthenticatedTenants()
	  {
		testRule.DeployForTenant(TENANT_ONE, BPMN_PROCESS);
		string ProcessInstanceId = startProcessInstance(null);

		identityService.SetAuthentication("user", null, null);

		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot Delete the historic process instance");

		historyService.DeleteHistoricProcessInstance(ProcessInstanceId);
	  }

        [Test]
        public virtual void deleteHistoricProcessInstanceWithAuthenticatedTenant()
	  {
		testRule.DeployForTenant(TENANT_ONE, BPMN_PROCESS);
		string ProcessInstanceId = startProcessInstance(null);

		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		historyService.DeleteHistoricProcessInstance(ProcessInstanceId);

		identityService.ClearAuthentication();

		IQueryable<IHistoricProcessInstance> query = historyService.CreateHistoricProcessInstanceQuery();

		Assert.That(query.Count(), Is.EqualTo(0L));
	  }

        [Test]
        public virtual void deleteHistoricProcessInstanceWithDisabledTenantCheck()
	  {
		testRule.DeployForTenant(TENANT_ONE, BPMN_PROCESS);
		testRule.DeployForTenant(TENANT_TWO, BPMN_PROCESS);

		string processInstanceIdOne = startProcessInstance(TENANT_ONE);
		string processInstanceIdTwo = startProcessInstance(TENANT_TWO);

		identityService.SetAuthentication("user", null, null);
		processEngineConfiguration.SetTenantCheckEnabled(false);

		historyService.DeleteHistoricProcessInstance(processInstanceIdOne);
		historyService.DeleteHistoricProcessInstance(processInstanceIdTwo);

		IQueryable<IHistoricProcessInstance> query = historyService.CreateHistoricProcessInstanceQuery();
		Assert.That(query.Count(), Is.EqualTo(0L));
	  }

        [Test]
        public virtual void failToDeleteHistoricTaskInstanceNoAuthenticatedTenants()
	  {
		string taskId = createTaskForTenant(TENANT_ONE);

		identityService.SetAuthentication("user", null, null);

		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot Delete the historic task instance");

		historyService.DeleteHistoricTaskInstance(taskId);
	  }

        [Test]
        public virtual void deleteHistoricTaskInstanceWithAuthenticatedTenant()
	  {
		string taskId = createTaskForTenant(TENANT_ONE);

		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		historyService.DeleteHistoricTaskInstance(taskId);

		identityService.ClearAuthentication();

		IQueryable<IHistoricTaskInstance> query = historyService.CreateHistoricTaskInstanceQuery();

		Assert.That(query.Count(), Is.EqualTo(0L));
	  }

        [Test]
        public virtual void deleteHistoricTaskInstanceWithDisabledTenantCheck()
	  {
		string taskIdOne = createTaskForTenant(TENANT_ONE);
		string taskIdTwo = createTaskForTenant(TENANT_TWO);

		identityService.SetAuthentication("user", null, null);
		processEngineConfiguration.SetTenantCheckEnabled(false);

		historyService.DeleteHistoricTaskInstance(taskIdOne);
		historyService.DeleteHistoricTaskInstance(taskIdTwo);

		IQueryable<IHistoricTaskInstance> query = historyService.CreateHistoricTaskInstanceQuery();

		Assert.That(query.Count(), Is.EqualTo(0L));
	  }

        [Test]
        public virtual void failToDeleteHistoricCaseInstanceNoAuthenticatedTenants()
	  {
		testRule.DeployForTenant(TENANT_ONE, CMMN_PROCESS_WITH_MANUAL_ACTIVATION);
		string caseInstanceId = createAndCloseCaseInstance(null);

		identityService.SetAuthentication("user", null, null);

		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot Delete the historic case instance");

		historyService.DeleteHistoricCaseInstance(caseInstanceId);
	  }

        [Test]
        public virtual void deleteHistoricCaseInstanceWithAuthenticatedTenant()
	  {
		testRule.DeployForTenant(TENANT_ONE, CMMN_PROCESS_WITH_MANUAL_ACTIVATION);
		string caseInstanceId = createAndCloseCaseInstance(null);

		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		historyService.DeleteHistoricCaseInstance(caseInstanceId);

		identityService.ClearAuthentication();

		IQueryable<IHistoricCaseInstance> query = historyService.CreateHistoricCaseInstanceQuery();

		Assert.That(query.Count(), Is.EqualTo(0L));
	  }

        [Test]
        public virtual void deleteHistoricCaseInstanceWithDisabledTenantCheck()
	  {
		testRule.DeployForTenant(TENANT_ONE, CMMN_PROCESS_WITH_MANUAL_ACTIVATION);
		testRule.DeployForTenant(TENANT_TWO, CMMN_PROCESS_WITH_MANUAL_ACTIVATION);

		string caseInstanceIdOne = createAndCloseCaseInstance(TENANT_ONE);
		string caseInstanceIdTwo = createAndCloseCaseInstance(TENANT_TWO);

		identityService.SetAuthentication("user", null, null);
		processEngineConfiguration.SetTenantCheckEnabled(false);

		historyService.DeleteHistoricCaseInstance(caseInstanceIdOne);
		historyService.DeleteHistoricCaseInstance(caseInstanceIdTwo);

		IQueryable<IHistoricCaseInstance> query = historyService.CreateHistoricCaseInstanceQuery();
		Assert.That(query.Count(), Is.EqualTo(0L));
	  }

        [Test]
        public virtual void deleteHistoricDecisionInstanceNoAuthenticatedTenants()
	  {
		testRule.DeployForTenant(TENANT_ONE, DMN);

		string decisionDefinitionId = evaluateDecisionTable(null);

		identityService.SetAuthentication("user", null, null);

		historyService.DeleteHistoricDecisionInstanceByDefinitionId(decisionDefinitionId);

		identityService.ClearAuthentication();

		IQueryable<IHistoricDecisionInstance> query = historyService.CreateHistoricDecisionInstanceQuery();

		Assert.That(query.Count(), Is.EqualTo(1L));
	  }

        [Test]
        public virtual void deleteHistoricDecisionInstanceWithAuthenticatedTenant()
	  {
		testRule.DeployForTenant(TENANT_ONE, DMN);
		string decisionDefinitionId = evaluateDecisionTable(null);

		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		historyService.DeleteHistoricDecisionInstanceByDefinitionId(decisionDefinitionId);

		identityService.ClearAuthentication();

		IQueryable<IHistoricDecisionInstance> query = historyService.CreateHistoricDecisionInstanceQuery();

		Assert.That(query.Count(), Is.EqualTo(0L));
	  }

        [Test]
        public virtual void deleteHistoricDecisionInstanceWithDisabledTenantCheck()
	  {
		testRule.DeployForTenant(TENANT_ONE, DMN);
		testRule.DeployForTenant(TENANT_TWO, DMN);

		string decisionDefinitionIdOne = evaluateDecisionTable(TENANT_ONE);
		string decisionDefinitionIdTwo = evaluateDecisionTable(TENANT_TWO);

		identityService.SetAuthentication("user", null, null);
		processEngineConfiguration.SetTenantCheckEnabled(false);

		historyService.DeleteHistoricDecisionInstanceByDefinitionId(decisionDefinitionIdOne);
		historyService.DeleteHistoricDecisionInstanceByDefinitionId(decisionDefinitionIdTwo);

		IQueryable<IHistoricDecisionInstance> query = historyService.CreateHistoricDecisionInstanceQuery();
		Assert.That(query.Count(), Is.EqualTo(0L));
	  }

        [Test]
        public virtual void failToDeleteHistoricDecisionInstanceByInstanceIdNoAuthenticatedTenants()
	  {

		// given
		testRule.DeployForTenant(TENANT_ONE, DMN);
		evaluateDecisionTable(null);

		IQueryable<IHistoricDecisionInstance> query = historyService.CreateHistoricDecisionInstanceQuery();
		IHistoricDecisionInstance historicDecisionInstance = query/*/*.IncludeInputs()*///*/*.IncludeOutputs()*/
                .First();

		// when
		identityService.SetAuthentication("user", null, null);

		// then
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot Delete the historic decision instance");

		historyService.DeleteHistoricDecisionInstanceByInstanceId(historicDecisionInstance.Id);
	  }

        [Test]
        public virtual void deleteHistoricDecisionInstanceByInstanceIdWithAuthenticatedTenant()
	  {

		// given
		testRule.DeployForTenant(TENANT_ONE, DMN);
		evaluateDecisionTable(null);

		IQueryable<IHistoricDecisionInstance> query = historyService.CreateHistoricDecisionInstanceQuery();
		IHistoricDecisionInstance historicDecisionInstance = query/*/*.IncludeInputs()*//*.IncludeOutputs()*/.First();

		// when
		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});
		historyService.DeleteHistoricDecisionInstanceByInstanceId(historicDecisionInstance.Id);

		// then
		identityService.ClearAuthentication();
		Assert.That(query.Count(), Is.EqualTo(0L));
	  }

        [Test]
        public virtual void deleteHistoricDecisionInstanceByInstanceIdWithDisabledTenantCheck()
	  {

		// given
		testRule.DeployForTenant(TENANT_ONE, DMN);
		testRule.DeployForTenant(TENANT_TWO, DMN);

		evaluateDecisionTable(TENANT_ONE);
		evaluateDecisionTable(TENANT_TWO);

		IQueryable<IHistoricDecisionInstance> query = historyService.CreateHistoricDecisionInstanceQuery();
	      IList<IHistoricDecisionInstance> historicDecisionInstances = query
           //     /*.IncludeInputs()*/
	          ///*.IncludeOutputs()*/
	          .ToList();
		Assert.That(historicDecisionInstances.Count, Is.EqualTo(2));

		// when user has no authorization
		identityService.SetAuthentication("user", null, null);
		// and when tenant check is disabled
		processEngineConfiguration.SetTenantCheckEnabled(false);
		// and when all decision instances are deleted
		foreach (IHistoricDecisionInstance @in in historicDecisionInstances)
		{
		  historyService.DeleteHistoricDecisionInstanceByInstanceId(@in.Id);
		}

		// then
		identityService.ClearAuthentication();
		Assert.That(query.Count(), Is.EqualTo(0L));
	  }

        [Test]
        public virtual void failToGetHistoricJobLogExceptionStacktraceNoAuthenticatedTenants()
	  {
		testRule.DeployForTenant(TENANT_ONE, FAILING_BPMN_PROCESS);
		string ProcessInstanceId = startProcessInstance(null);

		string historicJobLogId = historyService.CreateHistoricJobLogQuery(c=>c.ProcessInstanceId == ProcessInstanceId).First().Id;

		identityService.SetAuthentication("user", null, null);

		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot get the historic job log");

		historyService.GetHistoricJobLogExceptionStacktrace(historicJobLogId);
	  }

        [Test]
        public virtual void getHistoricJobLogExceptionStacktraceWithAuthenticatedTenant()
	  {
		testRule.DeployForTenant(TENANT_ONE, FAILING_BPMN_PROCESS);
		string ProcessInstanceId = startProcessInstance(null);

		testRule.ExecuteAvailableJobs();

		IHistoricJobLog log = historyService.CreateHistoricJobLogQuery(c=>c.ProcessInstanceId == ProcessInstanceId && c.FailureLog)/*.ListPage(0, 1)*/.First();

		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		string historicJobLogExceptionStacktrace = historyService.GetHistoricJobLogExceptionStacktrace(log.Id);

		Assert.That(historicJobLogExceptionStacktrace!=null);
	  }

        [Test]
	  public virtual void getHistoricJobLogExceptionStacktraceWithDisabledTenantCheck()
	  {
		testRule.DeployForTenant(TENANT_ONE, FAILING_BPMN_PROCESS);

		string ProcessInstanceId = startProcessInstance(TENANT_ONE);

		testRule.ExecuteAvailableJobs();

		IHistoricJobLog log = historyService.CreateHistoricJobLogQuery(c=>c.ProcessInstanceId == ProcessInstanceId && c.FailureLog)/*.ListPage(0, 1)*/.First();

		identityService.SetAuthentication("user", null, null);
		processEngineConfiguration.SetTenantCheckEnabled(false);

		string historicJobLogExceptionStacktrace = historyService.GetHistoricJobLogExceptionStacktrace(log.Id);

		Assert.That(historicJobLogExceptionStacktrace!=null);
	  }

        [TearDown]
	  public virtual void tearDown()
	  {
		identityService.ClearAuthentication();
		foreach (IHistoricTaskInstance instance in historyService.CreateHistoricTaskInstanceQuery().ToList())
		{
		  historyService.DeleteHistoricTaskInstance(instance.Id);
		}
	  }

	  // helper //////////////////////////////////////////////////////////

	  protected internal virtual string startProcessInstance(string tenantId)
	  {
		if (string.ReferenceEquals(tenantId, null))
		{
		  return runtimeService.StartProcessInstanceByKey(PROCESS_DEFINITION_KEY).Id;
		}
		else
		{
		  return runtimeService.CreateProcessInstanceByKey(PROCESS_DEFINITION_KEY).SetProcessDefinitionTenantId(tenantId).Execute().Id;
		}
	  }

	  protected internal virtual string createAndCloseCaseInstance(string tenantId)
	  {
		string caseInstanceId;

		ICaseInstanceBuilder builder = caseService.WithCaseDefinitionByKey("oneTaskCase");
		if (string.ReferenceEquals(tenantId, null))
		{
		  caseInstanceId = builder.Create().Id;
		}
		else
		{
		  caseInstanceId = builder.CaseDefinitionTenantId(tenantId).Create().Id;
		}

		caseService.CompleteCaseExecution(caseInstanceId);
		caseService.CloseCaseInstance(caseInstanceId);

		return caseInstanceId;
	  }

	  protected internal virtual string evaluateDecisionTable(string tenantId)
	  {
		string decisionDefinitionId;

		if (string.ReferenceEquals(tenantId, null))
		{
		  decisionDefinitionId = repositoryService.CreateDecisionDefinitionQuery().First().Id;
		}
		else
		{
		  decisionDefinitionId = repositoryService.CreateDecisionDefinitionQuery(c=>c.TenantId == tenantId).First().Id;
		}

		IVariableMap variables = ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables().PutValue("status", "bronze");
		decisionService.EvaluateDecisionTableById(decisionDefinitionId, variables);

		return decisionDefinitionId;
	  }

	  protected internal virtual string createTaskForTenant(string tenantId)
	  {
		ITask task = taskService.NewTask();
		task.TenantId = TENANT_ONE;

		taskService.SaveTask(task);
		taskService.Complete(task.Id);

		return task.Id;
	  }

	}

}