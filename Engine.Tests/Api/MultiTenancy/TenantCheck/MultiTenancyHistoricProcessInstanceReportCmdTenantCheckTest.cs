using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Query;
using ESS.FW.Bpm.Engine.Task;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;

namespace Engine.Tests.Api.MultiTenancy.TenantCheck
{
    /// <summary>
    /// 
    /// </summary>
    [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull)]
    public class MultiTenancyHistoricProcessInstanceReportCmdTenantCheckTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyHistoricProcessInstanceReportCmdTenantCheckTest()
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

	  protected internal const string PROCESS_DEFINITION_KEY = "testProcess";

	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();

	  protected internal ProcessEngineTestRule testRule;

	  protected internal IRepositoryService repositoryService;
	  protected internal IIdentityService identityService;
	  protected internal IRuntimeService runtimeService;
	  protected internal ITaskService taskService;
	  protected internal IHistoryService historyService;
	  protected internal ProcessEngineConfiguration processEngineConfiguration;

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(engineRule).around(testRule);
	  //public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.Rules.ExpectedException thrown= org.junit.Rules.ExpectedException.None();
	  //public ExpectedException thrown = ExpectedException.None();

	  protected internal static readonly IBpmnModelInstance BPMN_PROCESS = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_DEFINITION_KEY).StartEvent().UserTask().EndEvent().Done();

        [SetUp]
	  public virtual void init()
	  {
		repositoryService = engineRule.RepositoryService;
		identityService = engineRule.IdentityService;
		runtimeService = engineRule.RuntimeService;
		taskService = engineRule.TaskService;
		historyService = engineRule.HistoryService;
		processEngineConfiguration = engineRule.ProcessEngineConfiguration;
	  }

        [Test]
        public virtual void getDurationReportByMonthNoAuthenticatedTenants()
	  {
		testRule.DeployForTenant(TENANT_ONE, BPMN_PROCESS);

		startAndCompleteProcessInstance(null);

		identityService.SetAuthentication("user", null, null);

		IList<IDurationReportResult> result = historyService.CreateHistoricProcessInstanceReport().Duration(PeriodUnit.Month);

		Assert.That(result.Count, Is.EqualTo(0));
	  }

        [Test]
        public virtual void getDurationReportByMonthWithAuthenticatedTenant()
	  {
		testRule.DeployForTenant(TENANT_ONE, BPMN_PROCESS);

		startAndCompleteProcessInstance(null);

		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		IList<IDurationReportResult> result = historyService.CreateHistoricProcessInstanceReport().Duration(PeriodUnit.Month);

		Assert.That(result.Count, Is.EqualTo(1));
	  }

        [Test]
        public virtual void getDurationReportByMonthDisabledTenantCheck()
	  {
		testRule.DeployForTenant(TENANT_ONE, BPMN_PROCESS);

		startAndCompleteProcessInstance(null);

		identityService.SetAuthentication("user", null, null);
		processEngineConfiguration.SetTenantCheckEnabled(false);

		IList<IDurationReportResult> result = historyService.CreateHistoricProcessInstanceReport().Duration(PeriodUnit.Month);

		Assert.That(result.Count, Is.EqualTo(1));
	  }

        [Test]
        public virtual void getReportByMultipleProcessDefinitionIdByMonthNoAuthenticatedTenants()
	  {
		testRule.DeployForTenant(TENANT_ONE, BPMN_PROCESS);
		testRule.DeployForTenant(TENANT_TWO, BPMN_PROCESS);

		startAndCompleteProcessInstance(TENANT_ONE);
		startAndCompleteProcessInstance(TENANT_TWO);

		string processDefinitionIdOne = repositoryService.CreateProcessDefinitionQuery(c=>c.TenantId == TENANT_ONE).First().Id;
		string processDefinitionIdTwo = repositoryService.CreateProcessDefinitionQuery(c=>c.TenantId == TENANT_TWO).First().Id;

		identityService.SetAuthentication("user", null, null);

		IList<IDurationReportResult> result = historyService.CreateHistoricProcessInstanceReport().ProcessDefinitionIdIn(processDefinitionIdOne, processDefinitionIdTwo).Duration(PeriodUnit.Month);

		Assert.That(result.Count, Is.EqualTo(0));
	  }

        [Test]
        public virtual void getReportByMultipleProcessDefinitionIdByMonthWithAuthenticatedTenant()
	  {
		testRule.DeployForTenant(TENANT_ONE, BPMN_PROCESS);
		testRule.DeployForTenant(TENANT_TWO, BPMN_PROCESS);

		startAndCompleteProcessInstance(TENANT_ONE);
		startAndCompleteProcessInstance(TENANT_TWO);

		string processDefinitionIdOne = repositoryService.CreateProcessDefinitionQuery(c=>c.TenantId == TENANT_ONE).First().Id;
		string processDefinitionIdTwo = repositoryService.CreateProcessDefinitionQuery(c=>c.TenantId == TENANT_TWO).First().Id;

		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		IList<IDurationReportResult> result = historyService.CreateHistoricProcessInstanceReport().ProcessDefinitionIdIn(processDefinitionIdOne, processDefinitionIdTwo).Duration(PeriodUnit.Month);

		Assert.That(result.Count, Is.EqualTo(1));
	  }

        [Test]
        public virtual void getReportByMultipleProcessDefinitionIdByMonthDisabledTenantCheck()
	  {
		testRule.DeployForTenant(TENANT_ONE, BPMN_PROCESS);
		testRule.DeployForTenant(TENANT_TWO, BPMN_PROCESS);

		startAndCompleteProcessInstance(TENANT_ONE);
		startAndCompleteProcessInstance(TENANT_TWO);

		string processDefinitionIdOne = repositoryService.CreateProcessDefinitionQuery(c=>c.TenantId == TENANT_ONE).First().Id;
		string processDefinitionIdTwo = repositoryService.CreateProcessDefinitionQuery(c=>c.TenantId == TENANT_TWO).First().Id;

		identityService.SetAuthentication("user", null, null);
		processEngineConfiguration.SetTenantCheckEnabled(false);

		IList<IDurationReportResult> result = historyService.CreateHistoricProcessInstanceReport().ProcessDefinitionIdIn(processDefinitionIdOne, processDefinitionIdTwo).Duration(PeriodUnit.Month);

		Assert.That(result.Count, Is.EqualTo(2));
	  }

        [Test]
        public virtual void getReportByProcessDefinitionKeyByMonthNoAuthenticatedTenants()
	  {
		testRule.DeployForTenant(TENANT_ONE, BPMN_PROCESS);
		testRule.DeployForTenant(TENANT_TWO, BPMN_PROCESS);

		startAndCompleteProcessInstance(TENANT_ONE);
		startAndCompleteProcessInstance(TENANT_TWO);

		identityService.SetAuthentication("user", null, null);

		IList<IDurationReportResult> result = historyService.CreateHistoricProcessInstanceReport().ProcessDefinitionKeyIn(PROCESS_DEFINITION_KEY).Duration(PeriodUnit.Month);

		Assert.That(result.Count, Is.EqualTo(0));
	  }

        [Test]
        public virtual void getReportByProcessDefinitionKeyByMonthWithAuthenticatedTenant()
	  {
		testRule.DeployForTenant(TENANT_ONE, BPMN_PROCESS);
		testRule.DeployForTenant(TENANT_TWO, BPMN_PROCESS);

		startAndCompleteProcessInstance(TENANT_ONE);
		startAndCompleteProcessInstance(TENANT_TWO);

		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		IList<IDurationReportResult> result = historyService.CreateHistoricProcessInstanceReport().ProcessDefinitionKeyIn(PROCESS_DEFINITION_KEY).Duration(PeriodUnit.Month);

		Assert.That(result.Count, Is.EqualTo(1));
	  }

        [Test]
        public virtual void getReportByProcessDefinitionKeyByMonthDisabledTenantCheck()
	  {
		testRule.DeployForTenant(TENANT_ONE, BPMN_PROCESS);
		testRule.DeployForTenant(TENANT_TWO, BPMN_PROCESS);

		startAndCompleteProcessInstance(TENANT_ONE);
		startAndCompleteProcessInstance(TENANT_TWO);

		identityService.SetAuthentication("user", null, null);
		processEngineConfiguration.SetTenantCheckEnabled(false);

		IList<IDurationReportResult> result = historyService.CreateHistoricProcessInstanceReport().ProcessDefinitionKeyIn(PROCESS_DEFINITION_KEY).Duration(PeriodUnit.Month);

		Assert.That(result.Count, Is.EqualTo(2));
	  }

	  // helper //////////////////////////////////////////////////////////

	  protected internal virtual string startAndCompleteProcessInstance(string tenantId)
	  {
		string ProcessInstanceId = null;
		if (string.ReferenceEquals(tenantId, null))
		{
		  ProcessInstanceId = runtimeService.StartProcessInstanceByKey(PROCESS_DEFINITION_KEY).Id;
		}
		else
		{
		  ProcessInstanceId = runtimeService.CreateProcessInstanceByKey(PROCESS_DEFINITION_KEY).SetProcessDefinitionTenantId(tenantId).Execute().Id;
		}

		//addToCalendar(DateTime.Month, 5);

		ITask task = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==ProcessInstanceId).First();

		taskService.Complete(task.Id);

		return ProcessInstanceId;
	  }

	  protected internal virtual void addToCalendar(int field, int month)
	  {
		DateTime calendar = new DateTime();
		calendar = new DateTime(ClockUtil.CurrentTime.Millisecond);
		//calendar.Add(field, month);
		ClockUtil.CurrentTime = calendar;
	  }

	}

}