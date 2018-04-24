using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.History.Impl;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;

namespace Engine.Tests.Api.MultiTenancy.TenantCheck
{

	/// <summary>
	/// 
	/// </summary>
	public class MultiTenancyProcessDefinitionCmdsTenantCheckTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyProcessDefinitionCmdsTenantCheckTest()
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

	  protected internal const string BPMN_PROCESS_MODEL = "resources/api/multitenancy/testProcess.bpmn";
	  protected internal const string BPMN_PROCESS_DIAGRAM = "resources/api/multitenancy/testProcess.png";

	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();

	  protected internal ProcessEngineTestRule testRule;

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(engineRule).around(testRule);
	  //public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.Rules.ExpectedException thrown= org.junit.Rules.ExpectedException.None();
	  //public ExpectedException thrown = ExpectedException.None();

	  protected internal IRepositoryService repositoryService;
	  protected internal IIdentityService identityService;
	  protected internal ProcessEngineConfigurationImpl processEngineConfiguration;

	  protected internal string processDefinitionId;

    [SetUp]
	  public virtual void setUp()
	  {
		processEngineConfiguration = engineRule.ProcessEngineConfiguration;
		repositoryService = engineRule.RepositoryService;
		identityService = engineRule.IdentityService;

		testRule.DeployForTenant(TENANT_ONE, BPMN_PROCESS_MODEL, BPMN_PROCESS_DIAGRAM);

		processDefinitionId = repositoryService.CreateProcessDefinitionQuery().First().Id;
	  }


        [Test]
        public virtual void failToGetProcessModelNoAuthenticatedTenants()
	  {
		identityService.SetAuthentication("user", null, null);

		// declare expected exception
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot get the process definition");

		repositoryService.GetProcessModel(processDefinitionId);
	  }


        [Test]
        public virtual void getProcessModelWithAuthenticatedTenant()
	  {
		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		System.IO.Stream inputStream = repositoryService.GetProcessModel(processDefinitionId);

		Assert.That(inputStream!=null);
	  }


        [Test]
        public virtual void getProcessModelDisabledTenantCheck()
	  {
		processEngineConfiguration.SetTenantCheckEnabled(false);
		identityService.SetAuthentication("user", null, null);

		System.IO.Stream inputStream = repositoryService.GetProcessModel(processDefinitionId);

		Assert.That(inputStream!=null);
	  }


        [Test]
        public virtual void failToGetProcessDiagramNoAuthenticatedTenants()
	  {
		identityService.SetAuthentication("user", null, null);

		// declare expected exception
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot get the process definition");

		repositoryService.GetProcessDiagram(processDefinitionId);
	  }


        [Test]
        public virtual void getProcessDiagramWithAuthenticatedTenant()
	  {
		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		System.IO.Stream inputStream = repositoryService.GetProcessDiagram(processDefinitionId);

		Assert.That(inputStream!=null);
	  }


        [Test]
        public virtual void getProcessDiagramDisabledTenantCheck()
	  {
		processEngineConfiguration.SetTenantCheckEnabled(false);
		identityService.SetAuthentication("user", null, null);

		System.IO.Stream inputStream = repositoryService.GetProcessDiagram(processDefinitionId);

		Assert.That(inputStream!=null);
	  }


        [Test]
        public virtual void failToGetProcessDiagramLayoutNoAuthenticatedTenants()
	  {
		identityService.SetAuthentication("user", null, null);

		// declare expected exception
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot get the process definition");

		repositoryService.GetProcessDiagramLayout(processDefinitionId);
	  }


        [Test]
        public virtual void getProcessDiagramLayoutWithAuthenticatedTenant()
	  {
		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		DiagramLayout diagramLayout = repositoryService.GetProcessDiagramLayout(processDefinitionId);

		Assert.That(diagramLayout!=null);
	  }


        [Test]
        public virtual void getProcessDiagramLayoutDisabledTenantCheck()
	  {
		processEngineConfiguration.SetTenantCheckEnabled(false);
		identityService.SetAuthentication("user", null, null);

		DiagramLayout diagramLayout = repositoryService.GetProcessDiagramLayout(processDefinitionId);

		Assert.That(diagramLayout!=null);
	  }


        [Test]
        public virtual void failToGetProcessDefinitionNoAuthenticatedTenants()
	  {
		identityService.SetAuthentication("user", null, null);

		// declare expected exception
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot get the process definition");

		repositoryService.GetProcessDefinition(processDefinitionId);
	  }


        [Test]
        public virtual void getProcessDefinitionWithAuthenticatedTenant()
	  {
		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		IProcessDefinition definition = repositoryService.GetProcessDefinition(processDefinitionId);

		Assert.That(definition.TenantId, Is.EqualTo(TENANT_ONE));
	  }


        [Test]
        public virtual void getProcessDefinitionDisabledTenantCheck()
	  {
		processEngineConfiguration.SetTenantCheckEnabled(false);
		identityService.SetAuthentication("user", null, null);

		IProcessDefinition definition = repositoryService.GetProcessDefinition(processDefinitionId);

		Assert.That(definition.TenantId, Is.EqualTo(TENANT_ONE));
	  }


        [Test]
        public virtual void failToGetBpmnModelInstanceNoAuthenticatedTenants()
	  {
		identityService.SetAuthentication("user", null, null);

		// declare expected exception
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot get the process definition");

		repositoryService.GetBpmnModelInstance(processDefinitionId);
	  }


        [Test]
        public virtual void getBpmnModelInstanceWithAuthenticatedTenant()
	  {
		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		IBpmnModelInstance modelInstance = repositoryService.GetBpmnModelInstance(processDefinitionId);

		Assert.That(modelInstance!=null);
	  }


        [Test]
        public virtual void getBpmnModelInstanceDisabledTenantCheck()
	  {
		processEngineConfiguration.SetTenantCheckEnabled(false);
		identityService.SetAuthentication("user", null, null);

		IBpmnModelInstance modelInstance = repositoryService.GetBpmnModelInstance(processDefinitionId);

		Assert.That(modelInstance!=null);
	  }


        [Test]
        public virtual void failToDeleteProcessDefinitionNoAuthenticatedTenant()
	  {
		//given deployment with a process definition
	      IList<IProcessDefinition> processDefinitions = repositoryService.CreateProcessDefinitionQuery()
	          .ToList();
		//and user with no tenant authentication
		identityService.SetAuthentication("user", null, null);

		// declare expected exception
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot Delete the process definition");

		//deletion should end in exception, since tenant authorization is missing
		repositoryService.DeleteProcessDefinition(processDefinitions[0].Id);
	  }


        [Test]
        public virtual void testDeleteProcessDefinitionWithAuthenticatedTenant()
	  {
		//given deployment with two process definitions
		IDeployment deployment = testRule.DeployForTenant(TENANT_ONE, "resources/repository/twoProcesses.bpmn20.xml");
		IQueryable<IProcessDefinition> processDefinitionQuery = repositoryService.CreateProcessDefinitionQuery(c=> c.DeploymentId == deployment.Id);
		IList<IProcessDefinition> processDefinitions = processDefinitionQuery.ToList();
		//and user with tenant authentication
		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		//when Delete process definition with authenticated user
		repositoryService.DeleteProcessDefinition(processDefinitions[0].Id);

		//then process definition should be deleted
		identityService.ClearAuthentication();
		Assert.That(processDefinitionQuery.Count(), Is.EqualTo(1L));
		Assert.That(processDefinitionQuery.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
	  }


        [Test]
        public virtual void testDeleteCascadeProcessDefinitionWithAuthenticatedTenant()
	  {
		//given deployment with a process definition and process instance
		IBpmnModelInstance bpmnModel = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process").StartEvent().UserTask().EndEvent().Done();
		testRule.DeployForTenant(TENANT_ONE, bpmnModel);
		IQueryable<IProcessDefinition> processDefinitionQuery = repositoryService.CreateProcessDefinitionQuery();
		//IProcessDefinition processDefinition = processDefinitionQuery.ProcessDefinitionKey("process").First();
		engineRule.RuntimeService.CreateProcessInstanceByKey("process").ExecuteWithVariablesInReturn();

		//and user with tenant authentication
		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		//when the corresponding process definition is cascading deleted from the deployment
		//repositoryService.DeleteProcessDefinition(processDefinition.Id, true);

		//then exist no process instance and one definition
		identityService.ClearAuthentication();
		Assert.AreEqual(0, engineRule.RuntimeService.CreateProcessInstanceQuery().Count());
		if (processEngineConfiguration.HistoryLevel.Id >= HistoryLevelFields.HistoryLevelActivity.Id)
		{
		  Assert.AreEqual(0, engineRule.HistoryService.CreateHistoricActivityInstanceQuery().Count());
		}
		Assert.That(repositoryService.CreateProcessDefinitionQuery().Count(), Is.EqualTo(1L));
		Assert.That(repositoryService.CreateProcessDefinitionQuery(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
	  }


        [Test]
        public virtual void testDeleteProcessDefinitionDisabledTenantCheck()
	  {
		//given deployment with two process definitions
		IDeployment deployment = testRule.DeployForTenant(TENANT_ONE, "resources/repository/twoProcesses.bpmn20.xml");
		//tenant check disabled
		processEngineConfiguration.SetTenantCheckEnabled(false);
		IQueryable<IProcessDefinition> processDefinitionQuery = repositoryService.CreateProcessDefinitionQuery(c=> c.DeploymentId == deployment.Id);
		IList<IProcessDefinition> processDefinitions = processDefinitionQuery.ToList();
		//user with no authentication
		identityService.SetAuthentication("user", null, null);

		//when process definition should be deleted without tenant check
		repositoryService.DeleteProcessDefinition(processDefinitions[0].Id);

		//then process definition is deleted
		identityService.ClearAuthentication();
		Assert.That(processDefinitionQuery.Count(), Is.EqualTo(1L));
		Assert.That(processDefinitionQuery.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
	  }


        [Test]
        public virtual void testDeleteCascadeProcessDefinitionDisabledTenantCheck()
	  {
		//given deployment with a process definition and process instances
		IBpmnModelInstance bpmnModel = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process").StartEvent().UserTask().EndEvent().Done();
		testRule.DeployForTenant(TENANT_ONE, bpmnModel);
		//tenant check disabled
		processEngineConfiguration.SetTenantCheckEnabled(false);
		IQueryable<IProcessDefinition> processDefinitionQuery = repositoryService.CreateProcessDefinitionQuery();
		//IProcessDefinition processDefinition = processDefinitionQuery.ProcessDefinitionKey("process").First();
		engineRule.RuntimeService.CreateProcessInstanceByKey("process").ExecuteWithVariablesInReturn();
		//user with no authentication
		identityService.SetAuthentication("user", null, null);

		//when the corresponding process definition is cascading deleted from the deployment
		//repositoryService.DeleteProcessDefinition(processDefinition.Id, true);

		//then exist no process instance and one definition, because test case deployes per default one definition
		identityService.ClearAuthentication();
		Assert.AreEqual(0, engineRule.RuntimeService.CreateProcessInstanceQuery().Count());
		if (processEngineConfiguration.HistoryLevel.Id >= HistoryLevelFields.HistoryLevelActivity.Id)
		{
		  Assert.AreEqual(0, engineRule.HistoryService.CreateHistoricActivityInstanceQuery().Count());
		}
		Assert.That(repositoryService.CreateProcessDefinitionQuery().Count(), Is.EqualTo(1L));
		Assert.That(repositoryService.CreateProcessDefinitionQuery(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
	  }



        [Test]
        public virtual void updateHistoryTimeToLiveWithAuthenticatedTenant()
	  {
		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		repositoryService.UpdateProcessDefinitionHistoryTimeToLive(processDefinitionId, 6);

		IProcessDefinition definition = repositoryService.GetProcessDefinition(processDefinitionId);

		Assert.That(definition.TenantId, Is.EqualTo(TENANT_ONE));
		//Assert.That(definition.HistoryTimeToLive, Is.EqualTo(6));
	  }


        [Test]
        public virtual void updateHistoryTimeToLiveDisabledTenantCheck()
	  {
		processEngineConfiguration.SetTenantCheckEnabled(false);
		identityService.SetAuthentication("user", null, null);

		repositoryService.UpdateProcessDefinitionHistoryTimeToLive(processDefinitionId, 6);

		IProcessDefinition definition = repositoryService.GetProcessDefinition(processDefinitionId);

		Assert.That(definition.TenantId, Is.EqualTo(TENANT_ONE));
		//Assert.That(definition.HistoryTimeToLive, Is.EqualTo(6));
	  }


        [Test]
        public virtual void updateHistoryTimeToLiveNoAuthenticatedTenants()
	  {
		identityService.SetAuthentication("user", null, null);

		// declare expected exception
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot update the process definition");

		repositoryService.UpdateProcessDefinitionHistoryTimeToLive(processDefinitionId, 6);
	  }

	}

}