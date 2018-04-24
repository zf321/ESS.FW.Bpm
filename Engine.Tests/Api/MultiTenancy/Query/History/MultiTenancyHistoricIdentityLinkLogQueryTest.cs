//using System.Collections.Generic;
//using System.Linq;
//using ESS.FW.Bpm.Engine.History;
//using ESS.FW.Bpm.Engine.Repository;
//using ESS.FW.Bpm.Engine.Runtime;
//using ESS.FW.Bpm.Engine.Task;
//using ESS.FW.Bpm.Engine.Tests.Util;
//using ESS.FW.Bpm.Model.Bpmn;
//using NUnit.Framework;

//namespace ESS.FW.Bpm.Engine.Tests.Api.MultiTenancy.Query.History
//{
    
//	/// 
//	/// <summary>
//	/// 
//	/// 
//	/// </summary>

//	public class MultiTenancyHistoricIdentityLinkLogQueryTest
//	{
//		private bool InstanceFieldsInitialized = false;

//		public MultiTenancyHistoricIdentityLinkLogQueryTest()
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


//	  private const string GROUP_1 = "Group1";
//	  private const string USER_1 = "User1";

//	  private static string PROCESS_DEFINITION_KEY = "oneTaskProcess";

//	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();

//	  protected internal ProcessEngineTestRule testRule;

//	  protected internal IHistoryService historyService;
//	  protected internal IRuntimeService runtimeService;
//	  protected internal IRepositoryService repositoryService;
//	  protected internal ITaskService taskService;



//	  protected internal const string A_USER_ID = "aUserId";

//	  protected internal const string TENANT_1 = "tenant1";
//	  protected internal const string TENANT_2 = "tenant2";
//	  protected internal const string TENANT_3 = "tenant3";

//        [SetUp]
//	  public virtual void init()
//	  {
//		taskService = engineRule.TaskService;
//		repositoryService = engineRule.RepositoryService;
//		historyService = engineRule.HistoryService;
//		runtimeService = engineRule.RuntimeService;

//		// create sample identity link
//		IBpmnModelInstance oneTaskProcess = Model.Bpmn.Bpmn.CreateExecutableProcess("testProcess").StartEvent().UserTask("task").CamundaCandidateUsers(A_USER_ID).EndEvent().Done();

//		// deploy tenants
//		testRule.DeployForTenant(TENANT_1, oneTaskProcess);
//		testRule.DeployForTenant(TENANT_2, oneTaskProcess);
//		testRule.DeployForTenant(TENANT_3, oneTaskProcess);
//	  }


//        [SetUp]
//        public virtual void addandDeleteHistoricIdentityLinkForSingleTenant()
//	  {

//		startProcessInstanceForTenant(TENANT_1);

//		IHistoricIdentityLinkLog historicIdentityLink = historyService.CreateHistoricIdentityLinkLogQuery().First();

//		taskService.DeleteCandidateUser(historicIdentityLink.TaskId, A_USER_ID);

//		IQueryable<IHistoricIdentityLinkLog> query = historyService.CreateHistoricIdentityLinkLogQuery();
//		Assert.AreEqual(query.TenantIdIn(TENANT_1).Count(), 2);

//	  }


//        [SetUp]
//        public virtual void historicIdentityLinkForMultipleTenant()
//	  {
//		startProcessInstanceForTenant(TENANT_1);

//		// Query test
//		IHistoricIdentityLinkLog historicIdentityLink = historyService.CreateHistoricIdentityLinkLogQuery().First();

//		Assert.AreEqual(historicIdentityLink.TenantId, TENANT_1);

//		// start process instance for another tenant
//		startProcessInstanceForTenant(TENANT_2);

//		// Query test
//		IList<IHistoricIdentityLinkLog> historicIdentityLinks = historyService.CreateHistoricIdentityLinkLogQuery().ToList();

//		Assert.AreEqual(historicIdentityLinks.Count, 2);

//		IQueryable<IHistoricIdentityLinkLog> query = historyService.CreateHistoricIdentityLinkLogQuery();
//		Assert.AreEqual(query.TenantIdIn(TENANT_1).Count(), 1);

//		query = historyService.CreateHistoricIdentityLinkLogQuery();
//		Assert.AreEqual(query.TenantIdIn(TENANT_2).Count(), 1);
//	  }


//        [SetUp]
//        public virtual void addAndRemoveHistoricIdentityLinksForProcessDefinitionWithTenantId()
//	  {
//		string resourceName = "resources/api/runtime/oneTaskProcess.bpmn20.xml";
//		testRule.DeployForTenant(TENANT_1, resourceName);
//		testRule.DeployForTenant(TENANT_2, resourceName);

//		IProcessDefinition processDefinition1 = repositoryService.CreateProcessDefinitionQuery(c=>c.Key == PROCESS_DEFINITION_KEY).First();
//		IProcessDefinition processDefinition2 = repositoryService.CreateProcessDefinitionQuery(c=>c.Key == PROCESS_DEFINITION_KEY).ToList()[1];

//		Assert.NotNull(processDefinition1);
//		Assert.NotNull(processDefinition2);

//		testTenantsByProcessDefinition(processDefinition1.Id);
//		testTenantsByProcessDefinition(processDefinition2.Id);

//		IList<IHistoricIdentityLinkLog> historicIdentityLinks = historyService.CreateHistoricIdentityLinkLogQuery().ToList();

//		Assert.AreEqual(historicIdentityLinks.Count, 8);

//		// Query test
//		IQueryable<IHistoricIdentityLinkLog> query = historyService.CreateHistoricIdentityLinkLogQuery();
//		Assert.AreEqual(query.TenantIdIn(TENANT_1).Count(), 4);
//		query = historyService.CreateHistoricIdentityLinkLogQuery();
//		Assert.AreEqual(query.TenantIdIn(TENANT_2).Count(), 4);
//	  }


//        [SetUp]
//        public virtual void testTenantsByProcessDefinition(string processDefinitionId)
//	  {

//		repositoryService.AddCandidateStarterGroup(processDefinitionId, GROUP_1);

//		repositoryService.AddCandidateStarterUser(processDefinitionId, USER_1);

//		repositoryService.DeleteCandidateStarterGroup(processDefinitionId, GROUP_1);

//		repositoryService.DeleteCandidateStarterUser(processDefinitionId, USER_1);

//	  }


//        [SetUp]
//        public virtual void identityLinksForProcessDefinitionWithTenantId()
//	  {
//		string resourceName = "resources/api/runtime/oneTaskProcess.bpmn20.xml";
//		testRule.DeployForTenant(TENANT_1, resourceName);
//		testRule.DeployForTenant(TENANT_2, resourceName);

//		IProcessDefinition processDefinition1 = repositoryService.CreateProcessDefinitionQuery(c=>c.Key == PROCESS_DEFINITION_KEY).First();

//		Assert.NotNull(processDefinition1);

//		// Add candidate group with process definition 1
//		repositoryService.AddCandidateStarterGroup(processDefinition1.Id, GROUP_1);

//		// Add candidate user for process definition 2
//		repositoryService.AddCandidateStarterUser(processDefinition1.Id, USER_1);

//		IProcessDefinition processDefinition2 = repositoryService.CreateProcessDefinitionQuery(c=>c.Key == PROCESS_DEFINITION_KEY).ToList()[1];

//		Assert.NotNull(processDefinition2);

//		// Add candidate group with process definition 2
//		repositoryService.AddCandidateStarterGroup(processDefinition2.Id, GROUP_1);

//		// Add candidate user for process definition 2
//		repositoryService.AddCandidateStarterUser(processDefinition2.Id, USER_1);

//		// Identity link test
//		IList<IIdentityLink> identityLinks = repositoryService.GetIdentityLinksForProcessDefinition(processDefinition1.Id);
//		Assert.AreEqual(identityLinks.Count,2);
//		Assert.AreEqual(identityLinks[0].TenantId, TENANT_1);
//		Assert.AreEqual(identityLinks[1].TenantId, TENANT_1);

//		identityLinks = repositoryService.GetIdentityLinksForProcessDefinition(processDefinition2.Id);
//		Assert.AreEqual(identityLinks.Count,2);
//		Assert.AreEqual(identityLinks[0].TenantId, TENANT_2);
//		Assert.AreEqual(identityLinks[1].TenantId, TENANT_2);

//	  }

//        [SetUp]
//	  public virtual void singleQueryForMultipleTenant()
//	  {
//		startProcessInstanceForTenant(TENANT_1);
//		startProcessInstanceForTenant(TENANT_2);
//		startProcessInstanceForTenant(TENANT_3);

//		IQueryable<IHistoricIdentityLinkLog> query = historyService.CreateHistoricIdentityLinkLogQuery();
//		Assert.AreEqual(query.TenantIdIn(TENANT_1, TENANT_2).Count(), 2);

//		query = historyService.CreateHistoricIdentityLinkLogQuery();
//		Assert.AreEqual(query.TenantIdIn(TENANT_2, TENANT_3).Count(), 2);

//		query = historyService.CreateHistoricIdentityLinkLogQuery();
//		Assert.AreEqual(query.TenantIdIn(TENANT_1, TENANT_2, TENANT_3).Count(), 3);

//	  }

//	  protected internal virtual IProcessInstance startProcessInstanceForTenant(string tenant)
//	  {
//		return runtimeService.CreateProcessInstanceByKey("testProcess").SetProcessDefinitionTenantId(tenant).Execute();
//	  }
//	}

//}