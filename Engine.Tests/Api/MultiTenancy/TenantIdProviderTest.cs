using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Impl.Cfg.Multitenancy;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Variable;
using ESS.FW.Bpm.Engine.Variable.Value;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;

namespace Engine.Tests.Api.MultiTenancy
{


    /// <summary>
    /// 
    /// 
    /// </summary>
    [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull)]
    public class TenantIdProviderTest
	{
		private bool InstanceFieldsInitialized = false;

		public TenantIdProviderTest()
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


	  protected internal const string CONFIGURATION_RESOURCE = "resources/api/multitenancy/TenantIdProviderTest.Camunda.cfg.xml";

	  protected internal const string PROCESS_DEFINITION_KEY = "testProcess";
	  protected internal const string DECISION_DEFINITION_KEY = "decision";
	  protected internal const string CASE_DEFINITION_KEY = "caseTaskCase";

	  protected internal const string DMN_FILE = "resources/api/multitenancy/simpleDecisionTable.Dmn";

	  protected internal const string CMMN_FILE = "resources/api/multitenancy/CaseWithCaseTask.cmmn";
	  protected internal const string CMMN_FILE_WITH_MANUAL_ACTIVATION = "resources/api/multitenancy/CaseWithCaseTaskWithManualActivation.cmmn";
	  protected internal const string CMMN_VARIABLE_FILE = "resources/api/multitenancy/CaseWithCaseTaskVariables.cmmn";
	  protected internal const string CMMN_SUBPROCESS_FILE = "resources/api/cmmn/oneTaskCase.cmmn";

	  protected internal const string TENANT_ID = "tenant1";


	  public static ProcessEngineBootstrapRule bootstrapRule = new ProcessEngineBootstrapRule(CONFIGURATION_RESOURCE);
	  public ProcessEngineRule engineRule = new ProvidedProcessEngineRule(bootstrapRule);

	  protected internal ProcessEngineTestRule testRule;

      [TearDown]
	  public virtual void tearDown()
	  {
		TestTenantIdProvider.reset();
	  }


        [Test]
        public virtual void providerCalledForProcessDefinitionWithoutTenantId()
	  {

		ContextLoggingTenantIdProvider tenantIdProvider = new ContextLoggingTenantIdProvider();
		TestTenantIdProvider.@delegate = tenantIdProvider;

		// given a deployment without tenant id
		testRule.Deploy(ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_DEFINITION_KEY).StartEvent().Done());

		// if a process instance is started
		engineRule.RuntimeService.StartProcessInstanceByKey(PROCESS_DEFINITION_KEY);

		// then the tenant id provider is invoked
		Assert.That(tenantIdProvider.parameters.Count, Is.EqualTo(1));
	  }


        [Test]
        public virtual void providerNotCalledForProcessDefinitionWithTenantId()
	  {

		ContextLoggingTenantIdProvider tenantIdProvider = new ContextLoggingTenantIdProvider();
		TestTenantIdProvider.@delegate = tenantIdProvider;

		// given a deployment with a tenant id
		testRule.DeployForTenant(TENANT_ID, ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_DEFINITION_KEY).StartEvent().Done());

		// if a process instance is started
		engineRule.RuntimeService.StartProcessInstanceByKey(PROCESS_DEFINITION_KEY);

		// then the tenant id provider is not invoked
		Assert.That(tenantIdProvider.parameters.Count, Is.EqualTo(0));
	  }




        [Test]
        public virtual void providerCalledForStartedProcessInstanceByStartFormWithoutTenantId()
	  {
		ContextLoggingTenantIdProvider tenantIdProvider = new ContextLoggingTenantIdProvider();
		TestTenantIdProvider.@delegate = tenantIdProvider;

		// given a deployment without a tenant id
		testRule.Deploy(ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_DEFINITION_KEY).StartEvent().Done(), "resources/api/form/util/request.Form");

		// when a process instance is started with a start form
		string processDefinitionId = engineRule.RepositoryService.CreateProcessDefinitionQuery().First().Id;

		IDictionary<string, object> properties = new Dictionary<string, object>();
		properties["employeeName"] = "demo";

		IProcessInstance procInstance = engineRule.FormService.SubmitStartForm(processDefinitionId, properties);
		Assert.NotNull(procInstance);

		// then the tenant id provider is invoked
		Assert.That(tenantIdProvider.parameters.Count, Is.EqualTo(1));
	  }



        [Test]
        public virtual void providerNotCalledForStartedProcessInstanceByStartFormWithTenantId()
	  {
		ContextLoggingTenantIdProvider tenantIdProvider = new ContextLoggingTenantIdProvider();
		TestTenantIdProvider.@delegate = tenantIdProvider;

		// given a deployment with a tenant id
		testRule.DeployForTenant(TENANT_ID, ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_DEFINITION_KEY).StartEvent().Done(), "resources/api/form/util/request.Form");

		// when a process instance is started with a start form
		string processDefinitionId = engineRule.RepositoryService.CreateProcessDefinitionQuery().First().Id;

		IDictionary<string, object> properties = new Dictionary<string, object>();
		properties["employeeName"] = "demo";

		IProcessInstance procInstance = engineRule.FormService.SubmitStartForm(processDefinitionId, properties);
		Assert.NotNull(procInstance);

		// then the tenant id provider is not invoked
		Assert.That(tenantIdProvider.parameters.Count, Is.EqualTo(0));
	  }


        [Test]
        public virtual void providerCalledForStartedProcessInstanceByModificationWithoutTenantId()
	  {
		// given a deployment without a tenant id
		ContextLoggingTenantIdProvider tenantIdProvider = new ContextLoggingTenantIdProvider();
		TestTenantIdProvider.@delegate = tenantIdProvider;
		testRule.Deploy(ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_DEFINITION_KEY).StartEvent().UserTask("task").EndEvent().Done(), "resources/api/form/util/request.Form");

		// when a process instance is created and the instance is set to a starting point
		string ProcessInstanceId = engineRule.RuntimeService.CreateProcessInstanceByKey(PROCESS_DEFINITION_KEY).StartBeforeActivity("task").Execute().ProcessInstanceId;

		//then provider is called
		Assert.NotNull(engineRule.RuntimeService.GetActivityInstance(ProcessInstanceId));
		Assert.That(tenantIdProvider.parameters.Count, Is.EqualTo(1));
	  }


        [Test]
        public virtual void providerNotCalledForStartedProcessInstanceByModificationWithTenantId()
	  {
		// given a deployment with a tenant id
		ContextLoggingTenantIdProvider tenantIdProvider = new ContextLoggingTenantIdProvider();
		TestTenantIdProvider.@delegate = tenantIdProvider;
		testRule.DeployForTenant(TENANT_ID, ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_DEFINITION_KEY).StartEvent().UserTask("task").EndEvent().Done(), "resources/api/form/util/request.Form");

		// when a process instance is created and the instance is set to a starting point
		string ProcessInstanceId = engineRule.RuntimeService.CreateProcessInstanceByKey(PROCESS_DEFINITION_KEY).StartBeforeActivity("task").Execute().ProcessInstanceId;

		//then provider should not be called
		Assert.NotNull(engineRule.RuntimeService.GetActivityInstance(ProcessInstanceId));
		Assert.That(tenantIdProvider.parameters.Count, Is.EqualTo(0));
	  }


        [Test]
        public virtual void providerCalledWithVariables()
	  {

		ContextLoggingTenantIdProvider tenantIdProvider = new ContextLoggingTenantIdProvider();
		TestTenantIdProvider.@delegate = tenantIdProvider;

		testRule.Deploy(ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_DEFINITION_KEY).StartEvent().Done());

		// if a process instance is started
		engineRule.RuntimeService.StartProcessInstanceByKey(PROCESS_DEFINITION_KEY, ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables().PutValue("varName", true));

		// then the tenant id provider is passed in the variable
		Assert.That(tenantIdProvider.parameters.Count, Is.EqualTo(1));
		Assert.That((bool) tenantIdProvider.parameters[0].Variables.ContainsKey("varName"));
	  }


        [Test]
        public virtual void providerCalledWithProcessDefinition()
	  {

		ContextLoggingTenantIdProvider tenantIdProvider = new ContextLoggingTenantIdProvider();
		TestTenantIdProvider.@delegate = tenantIdProvider;

		testRule.Deploy(ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_DEFINITION_KEY).StartEvent().Done());
		IProcessDefinition deployedProcessDefinition = engineRule.RepositoryService.CreateProcessDefinitionQuery().First();

		// if a process instance is started
		engineRule.RuntimeService.StartProcessInstanceByKey(PROCESS_DEFINITION_KEY);

		// then the tenant id provider is passed in the process definition
		IProcessDefinition passedProcessDefinition = tenantIdProvider.parameters[0].ProcessDefinition;
		Assert.That(passedProcessDefinition!=null);
		Assert.That(passedProcessDefinition.Id, Is.EqualTo(deployedProcessDefinition.Id));
	  }


        [Test]
        public virtual void setsTenantId()
	  {

		string tenantId = TENANT_ID;
		StaticTenantIdTestProvider tenantIdProvider = new StaticTenantIdTestProvider(tenantId);
		TestTenantIdProvider.@delegate = tenantIdProvider;

		testRule.Deploy(ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_DEFINITION_KEY).StartEvent().UserTask().Done());

		// if a process instance is started
		engineRule.RuntimeService.StartProcessInstanceByKey(PROCESS_DEFINITION_KEY);

		// then the tenant id provider can set the tenant id to a value
		IProcessInstance processInstance = engineRule.RuntimeService.CreateProcessInstanceQuery().First();
		Assert.That(processInstance.TenantId, Is.EqualTo(tenantId));
	  }


        [Test]
        public virtual void setNullTenantId()
	  {

		string tenantId = null;
		StaticTenantIdTestProvider tenantIdProvider = new StaticTenantIdTestProvider(tenantId);
		TestTenantIdProvider.@delegate = tenantIdProvider;

		testRule.Deploy(ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_DEFINITION_KEY).StartEvent().UserTask().Done());

		// if a process instance is started
		engineRule.RuntimeService.StartProcessInstanceByKey(PROCESS_DEFINITION_KEY);

		// then the tenant id provider can set the tenant id to null
		IProcessInstance processInstance = engineRule.RuntimeService.CreateProcessInstanceQuery().First();
		Assert.That(processInstance.TenantId, Is.EqualTo(null));
	  }


        [Test]
        public virtual void providerCalledForProcessDefinitionWithoutTenantId_SubProcessInstance()
	  {

		ContextLoggingTenantIdProvider tenantIdProvider = new ContextLoggingTenantIdProvider();
		TestTenantIdProvider.@delegate = tenantIdProvider;

		// given a deployment without tenant id
		testRule.Deploy(ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_DEFINITION_KEY).StartEvent().Done(), ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("superProcess").StartEvent().CallActivity().CalledElement(PROCESS_DEFINITION_KEY).Done());

		// if a process instance is started
		engineRule.RuntimeService.StartProcessInstanceByKey("superProcess");

		// then the tenant id provider is invoked twice
		Assert.That(tenantIdProvider.parameters.Count, Is.EqualTo(2));
	  }


        [Test]
        public virtual void providerNotCalledForProcessDefinitionWithTenantId_SubProcessInstance()
	  {

		ContextLoggingTenantIdProvider tenantIdProvider = new ContextLoggingTenantIdProvider();
		TestTenantIdProvider.@delegate = tenantIdProvider;

		// given a deployment with a tenant id
		testRule.DeployForTenant(TENANT_ID, ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_DEFINITION_KEY).StartEvent().Done(), ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("superProcess").StartEvent().CallActivity().CalledElement(PROCESS_DEFINITION_KEY).Done());

		// if a process instance is started
		engineRule.RuntimeService.StartProcessInstanceByKey("superProcess");

		// then the tenant id provider is not invoked
		Assert.That(tenantIdProvider.parameters.Count, Is.EqualTo(0));
	  }


        [Test]
        public virtual void providerCalledWithVariables_SubProcessInstance()
	  {

		ContextLoggingTenantIdProvider tenantIdProvider = new ContextLoggingTenantIdProvider();
		TestTenantIdProvider.@delegate = tenantIdProvider;

		testRule.Deploy(ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_DEFINITION_KEY).StartEvent().Done(), ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("superProcess").StartEvent().CallActivity().CalledElement(PROCESS_DEFINITION_KEY).CamundaIn("varName", "varName").Done());

		// if a process instance is started
		engineRule.RuntimeService.StartProcessInstanceByKey("superProcess", ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables().PutValue("varName", true));

		// then the tenant id provider is passed in the variable
		Assert.That(tenantIdProvider.parameters[1].Variables.Count==1);
        Assert.That((bool) tenantIdProvider.parameters[1].Variables.ContainsKey("varName"));
	  }


        [Test]
        public virtual void providerCalledWithProcessDefinition_SubProcessInstance()
	  {

		ContextLoggingTenantIdProvider tenantIdProvider = new ContextLoggingTenantIdProvider();
		TestTenantIdProvider.@delegate = tenantIdProvider;

		testRule.Deploy(ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_DEFINITION_KEY).StartEvent().Done(), ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("superProcess").StartEvent().CallActivity().CalledElement(PROCESS_DEFINITION_KEY).Done());
		IProcessDefinition deployedProcessDefinition = engineRule.RepositoryService.CreateProcessDefinitionQuery(c=>c.Key == PROCESS_DEFINITION_KEY).First();

		// if a process instance is started
		engineRule.RuntimeService.StartProcessInstanceByKey("superProcess");

		// then the tenant id provider is passed in the process definition
		IProcessDefinition passedProcessDefinition = tenantIdProvider.parameters[1].ProcessDefinition;
		Assert.That(passedProcessDefinition!=null);
		Assert.That(passedProcessDefinition.Id, Is.EqualTo(deployedProcessDefinition.Id));
	  }


        [Test]
        public virtual void providerCalledWithSuperProcessInstance()
	  {

		ContextLoggingTenantIdProvider tenantIdProvider = new ContextLoggingTenantIdProvider();
		TestTenantIdProvider.@delegate = tenantIdProvider;

		testRule.Deploy(ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_DEFINITION_KEY).StartEvent().Done(), ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("superProcess").StartEvent().CallActivity().CalledElement(PROCESS_DEFINITION_KEY).Done());
		IProcessDefinition superProcessDefinition = engineRule.RepositoryService.CreateProcessDefinitionQuery(c=>c.Key == "superProcess").First();


		// if a process instance is started
		engineRule.RuntimeService.StartProcessInstanceByKey("superProcess");

		// then the tenant id provider is passed in the process definition
		IDelegateExecution superExecution = tenantIdProvider.parameters[1].SuperExecution;
		Assert.That(superExecution!=null);
		Assert.That(superExecution.ProcessDefinitionId, Is.EqualTo(superProcessDefinition.Id));
	  }


        [Test]
        public virtual void setsTenantId_SubProcessInstance()
	  {

		string tenantId = TENANT_ID;
		SetValueOnSubProcessInstanceTenantIdProvider tenantIdProvider = new SetValueOnSubProcessInstanceTenantIdProvider(tenantId);
		TestTenantIdProvider.@delegate = tenantIdProvider;

		testRule.Deploy(ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_DEFINITION_KEY).StartEvent().UserTask().Done(), ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("superProcess").StartEvent().CallActivity().CalledElement(PROCESS_DEFINITION_KEY).Done());

		// if a process instance is started
		engineRule.RuntimeService.StartProcessInstanceByKey("superProcess");

		// then the tenant id provider can set the tenant id to a value
		IProcessInstance subProcessInstance = engineRule.RuntimeService.CreateProcessInstanceQuery(c=>c.ProcessDefinitionId == PROCESS_DEFINITION_KEY).First();
		Assert.That(subProcessInstance.TenantId, Is.EqualTo(tenantId));

		// and the super process instance is not assigned a tenant id
		IProcessInstance superProcessInstance = engineRule.RuntimeService.CreateProcessInstanceQuery(c=>c.ProcessDefinitionId == "superProcess").First();
		Assert.That(superProcessInstance.TenantId, Is.EqualTo(null));
	  }


        [Test]
        public virtual void setNullTenantId_SubProcessInstance()
	  {

		string tenantId = null;
		SetValueOnSubProcessInstanceTenantIdProvider tenantIdProvider = new SetValueOnSubProcessInstanceTenantIdProvider(tenantId);
		TestTenantIdProvider.@delegate = tenantIdProvider;

		testRule.Deploy(ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_DEFINITION_KEY).StartEvent().UserTask().Done(), ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("superProcess").StartEvent().CallActivity().CalledElement(PROCESS_DEFINITION_KEY).Done());

		// if a process instance is started
		engineRule.RuntimeService.StartProcessInstanceByKey("superProcess");

		// then the tenant id provider can set the tenant id to null
		IProcessInstance processInstance = engineRule.RuntimeService.CreateProcessInstanceQuery(c=>c.ProcessDefinitionId == PROCESS_DEFINITION_KEY).First();
		Assert.That(processInstance.TenantId, Is.EqualTo(null));
	  }


        [Test]
        public virtual void tenantIdInheritedFromSuperProcessInstance()
	  {

		string tenantId = TENANT_ID;
		SetValueOnRootProcessInstanceTenantIdProvider tenantIdProvider = new SetValueOnRootProcessInstanceTenantIdProvider(tenantId);
		TestTenantIdProvider.@delegate = tenantIdProvider;

		testRule.Deploy(ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_DEFINITION_KEY).StartEvent().UserTask().Done(), ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("superProcess").StartEvent().CallActivity().CalledElement(PROCESS_DEFINITION_KEY).Done());

		// if a process instance is started
		engineRule.RuntimeService.StartProcessInstanceByKey("superProcess");

		// then the tenant id is inherited to the sub process instance even tough it is not set by the provider
		IProcessInstance processInstance = engineRule.RuntimeService.CreateProcessInstanceQuery(c=>c.ProcessDefinitionId == PROCESS_DEFINITION_KEY).First();
		Assert.That(processInstance.TenantId, Is.EqualTo(tenantId));
	  }


        [Test]
        public virtual void providerCalledForProcessDefinitionWithoutTenantId_ProcessTask()
	  {

		ContextLoggingTenantIdProvider tenantIdProvider = new ContextLoggingTenantIdProvider();
		TestTenantIdProvider.@delegate = tenantIdProvider;

		testRule.Deploy(ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_DEFINITION_KEY).StartEvent().UserTask().Done(), "resources/api/multitenancy/CaseWithProcessTask.cmmn");

		// if the case is started
		engineRule.CaseService.CreateCaseInstanceByKey("testCase");
		ICaseExecution caseExecution = engineRule.CaseService.CreateCaseExecutionQuery(c=>c.ActivityId == "PI_ProcessTask_1").First();


		// then the tenant id provider is invoked once for the process instance
		Assert.That(tenantIdProvider.parameters.Count, Is.EqualTo(1));
	  }


        [Test]
        public virtual void providerNotCalledForProcessDefinitionWithTenantId_ProcessTask()
	  {

		ContextLoggingTenantIdProvider tenantIdProvider = new ContextLoggingTenantIdProvider();
		TestTenantIdProvider.@delegate = tenantIdProvider;

		testRule.DeployForTenant(TENANT_ID, ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_DEFINITION_KEY).StartEvent().UserTask().Done(), "resources/api/multitenancy/CaseWithProcessTask.cmmn");

		// if the case is started
		engineRule.CaseService.CreateCaseInstanceByKey("testCase");
		ICaseExecution caseExecution = engineRule.CaseService.CreateCaseExecutionQuery(c=>c.ActivityId == "PI_ProcessTask_1").First();

		// then the tenant id provider is not invoked
		Assert.That(tenantIdProvider.parameters.Count, Is.EqualTo(0));
	  }


        [Test]
        public virtual void providerCalledWithVariables_ProcessTask()
	  {

		ContextLoggingTenantIdProvider tenantIdProvider = new ContextLoggingTenantIdProvider();
		TestTenantIdProvider.@delegate = tenantIdProvider;

		testRule.Deploy(ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_DEFINITION_KEY).StartEvent().UserTask().Done(), "resources/api/multitenancy/CaseWithProcessTask.cmmn");

		// if the case is started
		engineRule.CaseService.CreateCaseInstanceByKey("testCase", ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables().PutValue("varName", true));
		ICaseExecution caseExecution = engineRule.CaseService.CreateCaseExecutionQuery(c=>c.ActivityId == "PI_ProcessTask_1").First();

		// then the tenant id provider is passed in the variable
		Assert.That(tenantIdProvider.parameters.Count, Is.EqualTo(1));

		IVariableMap variables = tenantIdProvider.parameters[0].Variables;
		Assert.That(variables.Count, Is.EqualTo(1));
		Assert.That(variables.ContainsKey("varName"));
	  }


        [Test]
        public virtual void providerCalledWithProcessDefinition_ProcessTask()
	  {

		ContextLoggingTenantIdProvider tenantIdProvider = new ContextLoggingTenantIdProvider();
		TestTenantIdProvider.@delegate = tenantIdProvider;

		testRule.Deploy(ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_DEFINITION_KEY).StartEvent().UserTask().Done(), "resources/api/multitenancy/CaseWithProcessTask.cmmn");

		// if the case is started
		engineRule.CaseService.CreateCaseInstanceByKey("testCase");
		ICaseExecution caseExecution = engineRule.CaseService.CreateCaseExecutionQuery(c=>c.ActivityId == "PI_ProcessTask_1").First();

		// then the tenant id provider is passed in the process definition
		Assert.That(tenantIdProvider.parameters.Count, Is.EqualTo(1));
		Assert.That(tenantIdProvider.parameters[0].ProcessDefinition!=null);
	  }


        [Test]
        public virtual void providerCalledWithSuperCaseExecution()
	  {

		ContextLoggingTenantIdProvider tenantIdProvider = new ContextLoggingTenantIdProvider();
		TestTenantIdProvider.@delegate = tenantIdProvider;

		testRule.Deploy(ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_DEFINITION_KEY).StartEvent().UserTask().Done(), "resources/api/multitenancy/CaseWithProcessTask.cmmn");

		// if the case is started
		engineRule.CaseService.CreateCaseInstanceByKey("testCase");
		ICaseExecution caseExecution = engineRule.CaseService.CreateCaseExecutionQuery(c=>c.ActivityId == "PI_ProcessTask_1").First();

		// then the tenant id provider is handed in the super case execution
		Assert.That(tenantIdProvider.parameters.Count, Is.EqualTo(1));
		Assert.That(tenantIdProvider.parameters[0].SuperCaseExecution!=null);
	  }


        [Test]
        public virtual void providerCalledForDecisionDefinitionWithoutTenantId()
	  {

		ContextLoggingTenantIdProvider tenantIdProvider = new ContextLoggingTenantIdProvider();
		TestTenantIdProvider.@delegate = tenantIdProvider;

		// given a deployment without tenant id
		testRule.Deploy(DMN_FILE);

		// if a decision definition is evaluated
		engineRule.DecisionService.EvaluateDecisionTableByKey(DECISION_DEFINITION_KEY).SetVariables(createVariables()).Evaluate();

		// then the tenant id provider is invoked
		Assert.That(tenantIdProvider.dmnParameters.Count, Is.EqualTo(1));
	  }


        [Test]
        public virtual void providerNotCalledForDecisionDefinitionWithTenantId()
	  {

		ContextLoggingTenantIdProvider tenantIdProvider = new ContextLoggingTenantIdProvider();
		TestTenantIdProvider.@delegate = tenantIdProvider;

		// given a deployment with a tenant id
		testRule.DeployForTenant(TENANT_ID, DMN_FILE);

		// if a decision definition is evaluated
		engineRule.DecisionService.EvaluateDecisionTableByKey(DECISION_DEFINITION_KEY).SetVariables(createVariables()).Evaluate();

		// then the tenant id provider is not invoked
		Assert.That(tenantIdProvider.dmnParameters.Count, Is.EqualTo(0));
	  }


        [Test]
        public virtual void providerCalledWithDecisionDefinition()
	  {

		ContextLoggingTenantIdProvider tenantIdProvider = new ContextLoggingTenantIdProvider();
		TestTenantIdProvider.@delegate = tenantIdProvider;

		testRule.Deploy(DMN_FILE);
		IDecisionDefinition deployedDecisionDefinition = engineRule.RepositoryService.CreateDecisionDefinitionQuery().First();

		// if a decision definition is evaluated
		engineRule.DecisionService.EvaluateDecisionTableByKey(DECISION_DEFINITION_KEY).SetVariables(createVariables()).Evaluate();

		// then the tenant id provider is passed in the decision definition
		IDecisionDefinition passedDecisionDefinition = tenantIdProvider.dmnParameters[0].DecisionDefinition;
		Assert.That(passedDecisionDefinition!=null);
		Assert.That(passedDecisionDefinition.Id, Is.EqualTo(deployedDecisionDefinition.Id));
	  }


        [Test]
        public virtual void setsTenantIdForHistoricDecisionInstance()
	  {

		string tenantId = TENANT_ID;
		StaticTenantIdTestProvider tenantIdProvider = new StaticTenantIdTestProvider(tenantId);
		TestTenantIdProvider.@delegate = tenantIdProvider;

		testRule.Deploy(DMN_FILE);

		// if a decision definition is evaluated
		engineRule.DecisionService.EvaluateDecisionTableByKey(DECISION_DEFINITION_KEY).SetVariables(createVariables()).Evaluate();

		// then the tenant id provider can set the tenant id to a value
		IHistoricDecisionInstance historicDecisionInstance = engineRule.HistoryService.CreateHistoricDecisionInstanceQuery().First();
		Assert.That(historicDecisionInstance.TenantId, Is.EqualTo(tenantId));
	  }


        [Test]
        public virtual void setNullTenantIdForHistoricDecisionInstance()
	  {

		string tenantId = null;
		StaticTenantIdTestProvider tenantIdProvider = new StaticTenantIdTestProvider(tenantId);
		TestTenantIdProvider.@delegate = tenantIdProvider;

		testRule.Deploy(DMN_FILE);

		// if a decision definition is evaluated
		engineRule.DecisionService.EvaluateDecisionTableByKey(DECISION_DEFINITION_KEY).SetVariables(createVariables()).Evaluate();

		// then the tenant id provider can set the tenant id to null
		IHistoricDecisionInstance historicDecisionInstance = engineRule.HistoryService.CreateHistoricDecisionInstanceQuery().First();
		Assert.That(historicDecisionInstance.TenantId, Is.EqualTo(null));
	  }


        [Test]
        public virtual void providerCalledForHistoricDecisionDefinitionWithoutTenantId_BusinessRuleTask()
	  {

		ContextLoggingTenantIdProvider tenantIdProvider = new ContextLoggingTenantIdProvider();
		TestTenantIdProvider.@delegate = tenantIdProvider;

		IBpmnModelInstance process = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_DEFINITION_KEY).StartEvent().BusinessRuleTask().CamundaDecisionRef(DECISION_DEFINITION_KEY).EndEvent().Done();

		// given a deployment without tenant id
		testRule.Deploy(process, DMN_FILE);

		// if a decision definition is evaluated
		engineRule.RuntimeService.StartProcessInstanceByKey(PROCESS_DEFINITION_KEY, createVariables());

		// then the tenant id provider is invoked
		Assert.That(tenantIdProvider.dmnParameters.Count, Is.EqualTo(1));
	  }


        [Test]
        public virtual void providerNotCalledForHistoricDecisionDefinitionWithTenantId_BusinessRuleTask()
	  {

		ContextLoggingTenantIdProvider tenantIdProvider = new ContextLoggingTenantIdProvider();
		TestTenantIdProvider.@delegate = tenantIdProvider;

		// given a deployment with a tenant id
		testRule.DeployForTenant(TENANT_ID, ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_DEFINITION_KEY).StartEvent().BusinessRuleTask().CamundaDecisionRef(DECISION_DEFINITION_KEY).EndEvent().Done(), DMN_FILE);

		// if a process instance is started
		engineRule.RuntimeService.StartProcessInstanceByKey(PROCESS_DEFINITION_KEY, createVariables());

		// then the tenant id providers are not invoked
		Assert.That(tenantIdProvider.dmnParameters.Count, Is.EqualTo(0));
	  }


        [Test]
        public virtual void providerCalledWithExecution_BusinessRuleTasks()
	  {

		ContextLoggingTenantIdProvider tenantIdProvider = new ContextLoggingTenantIdProvider();
		TestTenantIdProvider.@delegate = tenantIdProvider;

		IBpmnModelInstance process = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_DEFINITION_KEY).StartEvent().BusinessRuleTask().CamundaDecisionRef(DECISION_DEFINITION_KEY).CamundaAsyncAfter().EndEvent().Done();

		testRule.Deploy(process, DMN_FILE);

		// if a process instance is started
		engineRule.RuntimeService.StartProcessInstanceByKey(PROCESS_DEFINITION_KEY, createVariables());
		IExecution execution = engineRule.RuntimeService.CreateExecutionQuery(c=>c.ProcessInstanceId == PROCESS_DEFINITION_KEY).First();

		// then the tenant id provider is invoked
		Assert.That(tenantIdProvider.dmnParameters.Count, Is.EqualTo(1));
		ExecutionEntity passedExecution = (ExecutionEntity) tenantIdProvider.dmnParameters[0].Execution;
		Assert.That(passedExecution!=null);
		Assert.That(passedExecution.ParentExecution.Id, Is.EqualTo(execution.Id));
	  }


        [Test]
        public virtual void setsTenantIdForHistoricDecisionInstance_BusinessRuleTask()
	  {

		string tenantId = TENANT_ID;
		SetValueOnHistoricDecisionInstanceTenantIdProvider tenantIdProvider = new SetValueOnHistoricDecisionInstanceTenantIdProvider(tenantId);
		TestTenantIdProvider.@delegate = tenantIdProvider;

		IBpmnModelInstance process = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_DEFINITION_KEY).StartEvent().BusinessRuleTask().CamundaDecisionRef(DECISION_DEFINITION_KEY).CamundaAsyncAfter().EndEvent().Done();

		testRule.Deploy(process, DMN_FILE);

		// if a process instance is started
		engineRule.RuntimeService.CreateProcessInstanceByKey(PROCESS_DEFINITION_KEY)
                .SetVariables(createVariables() as IDictionary<string,ITypedValue>).Execute();

		// then the tenant id provider can set the tenant id to a value
		IHistoricDecisionInstance historicDecisionInstance = engineRule.HistoryService.CreateHistoricDecisionInstanceQuery(c=>c.DecisionDefinitionKey== DECISION_DEFINITION_KEY).First();
		Assert.That(historicDecisionInstance.TenantId, Is.EqualTo(tenantId));
	  }


        [Test]
        public virtual void setNullTenantIdForHistoricDecisionInstance_BusinessRuleTask()
	  {

		string tenantId = null;
		SetValueOnHistoricDecisionInstanceTenantIdProvider tenantIdProvider = new SetValueOnHistoricDecisionInstanceTenantIdProvider(tenantId);
		TestTenantIdProvider.@delegate = tenantIdProvider;

		IBpmnModelInstance process = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_DEFINITION_KEY).StartEvent().BusinessRuleTask().CamundaDecisionRef(DECISION_DEFINITION_KEY).CamundaAsyncAfter().EndEvent().Done();

		testRule.Deploy(process, DMN_FILE);

		// if a process instance is started
		engineRule.RuntimeService.CreateProcessInstanceByKey(PROCESS_DEFINITION_KEY).SetVariables(createVariables() as IDictionary<string, ITypedValue> ).Execute();

		// then the tenant id provider can set the tenant id to a value
		IHistoricDecisionInstance historicDecisionInstance = engineRule.HistoryService.CreateHistoricDecisionInstanceQuery(c=>c.DecisionDefinitionKey== DECISION_DEFINITION_KEY).First();
		Assert.That(historicDecisionInstance.TenantId, Is.EqualTo(null));
	  }

	  protected internal virtual IVariableMap createVariables()
	  {
		return ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables().PutValue("status", "gold");
	  }


        [Test]
        public virtual void providerCalledForCaseDefinitionWithoutTenantId()
	  {

		ContextLoggingTenantIdProvider tenantIdProvider = new ContextLoggingTenantIdProvider();
		TestTenantIdProvider.@delegate = tenantIdProvider;

		// given a deployment without tenant id
		testRule.Deploy(CMMN_FILE_WITH_MANUAL_ACTIVATION);

		// if a case instance is created
		engineRule.CaseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY).Create();

		// then the tenant id provider is invoked
		Assert.That(tenantIdProvider.caseParameters.Count, Is.EqualTo(1));
	  }


        [Test]
        public virtual void providerNotCalledForCaseInstanceWithTenantId()
	  {

		ContextLoggingTenantIdProvider tenantIdProvider = new ContextLoggingTenantIdProvider();
		TestTenantIdProvider.@delegate = tenantIdProvider;

		// given a deployment with a tenant id
		testRule.DeployForTenant(TENANT_ID, CMMN_FILE_WITH_MANUAL_ACTIVATION);

		// if a case instance is created
		engineRule.CaseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY).Create();

		// then the tenant id provider is not invoked
		Assert.That(tenantIdProvider.caseParameters.Count, Is.EqualTo(0));
	  }


        [Test]
        public virtual void providerCalledForCaseInstanceWithVariables()
	  {

		ContextLoggingTenantIdProvider tenantIdProvider = new ContextLoggingTenantIdProvider();
		TestTenantIdProvider.@delegate = tenantIdProvider;

		testRule.Deploy(CMMN_FILE_WITH_MANUAL_ACTIVATION);

		// if a case instance is created
		engineRule.CaseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY).SetVariables(ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables().PutValue("varName", true)).Create();

		// then the tenant id provider is passed in the variable
		Assert.That(tenantIdProvider.caseParameters.Count, Is.EqualTo(1));
		Assert.That((bool?) tenantIdProvider.caseParameters[0].Variables.ContainsKey("varName"), Is.EqualTo(true));
	  }


        [Test]
        public virtual void providerCalledWithCaseDefinition()
	  {

		ContextLoggingTenantIdProvider tenantIdProvider = new ContextLoggingTenantIdProvider();
		TestTenantIdProvider.@delegate = tenantIdProvider;

		testRule.Deploy(CMMN_FILE_WITH_MANUAL_ACTIVATION);
		ICaseDefinition deployedCaseDefinition = engineRule.RepositoryService.CreateCaseDefinitionQuery().First();

		// if a case instance is created
		engineRule.CaseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY).Create();

		// then the tenant id provider is passed in the case definition
		ICaseDefinition passedCaseDefinition = tenantIdProvider.caseParameters[0].CaseDefinition;
		Assert.That(passedCaseDefinition!=null);
		Assert.That(passedCaseDefinition.Id, Is.EqualTo(deployedCaseDefinition.Id));
	  }


        [Test]
        public virtual void setsTenantIdForCaseInstance()
	  {

		string tenantId = TENANT_ID;
		StaticTenantIdTestProvider tenantIdProvider = new StaticTenantIdTestProvider(tenantId);
		TestTenantIdProvider.@delegate = tenantIdProvider;

		testRule.Deploy(CMMN_FILE_WITH_MANUAL_ACTIVATION);

		// if a case instance is created
		engineRule.CaseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY).Create();

		// then the tenant id provider can set the tenant id to a value
		ICaseInstance caseInstance = engineRule.CaseService.CreateCaseInstanceQuery().First();
		Assert.That(caseInstance.TenantId, Is.EqualTo(tenantId));
	  }


        [Test]
        public virtual void setNullTenantIdForCaseInstance()
	  {

		string tenantId = null;
		StaticTenantIdTestProvider tenantIdProvider = new StaticTenantIdTestProvider(tenantId);
		TestTenantIdProvider.@delegate = tenantIdProvider;

		testRule.Deploy(CMMN_FILE_WITH_MANUAL_ACTIVATION);

		// if a case instance is created
		engineRule.CaseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY).Create();

		// then the tenant id provider can set the tenant id to null
		ICaseInstance caseInstance = engineRule.CaseService.CreateCaseInstanceQuery().First();
		Assert.That(caseInstance.TenantId, Is.EqualTo(null));
	  }


        [Test]
        public virtual void providerCalledForCaseDefinitionWithoutTenantId_SubCaseInstance()
	  {

		ContextLoggingTenantIdProvider tenantIdProvider = new ContextLoggingTenantIdProvider();
		TestTenantIdProvider.@delegate = tenantIdProvider;

		// given a deployment without tenant id
		testRule.Deploy(CMMN_SUBPROCESS_FILE,CMMN_FILE);

		// if a case instance is created
		engineRule.CaseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY).Create();

		// then the tenant id provider is invoked twice
		Assert.That(tenantIdProvider.caseParameters.Count, Is.EqualTo(2));
	  }


        [Test]
        public virtual void providerNotCalledForCaseDefinitionWithTenantId_SubCaseInstance()
	  {

		ContextLoggingTenantIdProvider tenantIdProvider = new ContextLoggingTenantIdProvider();
		TestTenantIdProvider.@delegate = tenantIdProvider;

		// given a deployment with a tenant id
		testRule.DeployForTenant(TENANT_ID, CMMN_SUBPROCESS_FILE, CMMN_FILE);

		// if a case instance is created
		engineRule.CaseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY).Create();

		// then the tenant id provider is not invoked
		Assert.That(tenantIdProvider.caseParameters.Count, Is.EqualTo(0));
	  }


        [Test]
        public virtual void providerCalledWithVariables_SubCaseInstance()
	  {

		ContextLoggingTenantIdProvider tenantIdProvider = new ContextLoggingTenantIdProvider();
		TestTenantIdProvider.@delegate = tenantIdProvider;

		testRule.Deploy(CMMN_SUBPROCESS_FILE, CMMN_VARIABLE_FILE);

		// if a case instance is created
		engineRule.CaseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY).SetVariables(ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables().PutValue("varName", true)).Create();

		// then the tenant id provider is passed in the variable
		Assert.That(tenantIdProvider.caseParameters[1].Variables.Count, Is.EqualTo(1));
		Assert.That((bool) tenantIdProvider.caseParameters[1].Variables.ContainsKey("varName"));
	  }


        [Test]
        public virtual void providerCalledWithCaseDefinition_SubCaseInstance()
	  {

		ContextLoggingTenantIdProvider tenantIdProvider = new ContextLoggingTenantIdProvider();
		TestTenantIdProvider.@delegate = tenantIdProvider;

		testRule.Deploy(CMMN_SUBPROCESS_FILE, CMMN_FILE);
		ICaseDefinition deployedCaseDefinition = engineRule.RepositoryService.CreateCaseDefinitionQuery().First();//.CaseDefinitionKey("oneTaskCase").First();

		// if a case instance is created
		engineRule.CaseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY).Create();

		// then the tenant id provider is passed in the case definition
		ICaseDefinition passedCaseDefinition = tenantIdProvider.caseParameters[1].CaseDefinition;
		Assert.That(passedCaseDefinition!=null);
		Assert.That(passedCaseDefinition.Id, Is.EqualTo(deployedCaseDefinition.Id));
	  }


        [Test]
        public virtual void providerCalledWithSuperCaseInstance()
	  {

		ContextLoggingTenantIdProvider tenantIdProvider = new ContextLoggingTenantIdProvider();
		TestTenantIdProvider.@delegate = tenantIdProvider;

		testRule.Deploy(CMMN_SUBPROCESS_FILE, CMMN_FILE_WITH_MANUAL_ACTIVATION);
		ICaseDefinition superCaseDefinition = engineRule.RepositoryService.CreateCaseDefinitionQuery()
                //.CaseDefinitionKey(CASE_DEFINITION_KEY)
                .First();


		// if a case instance is created
		engineRule.CaseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY).Create();
		startCaseTask();

		// then the tenant id provider is passed in the case definition
		IDelegateCaseExecution superCaseExecution = tenantIdProvider.caseParameters[1].SuperCaseExecution;
		Assert.That(superCaseExecution!=null);
		Assert.That(superCaseExecution.CaseDefinitionId, Is.EqualTo(superCaseDefinition.Id));
	  }


        [Test]
        public virtual void setsTenantId_SubCaseInstance()
	  {

		string tenantId = TENANT_ID;
		SetValueOnSubCaseInstanceTenantIdProvider tenantIdProvider = new SetValueOnSubCaseInstanceTenantIdProvider(tenantId);
		TestTenantIdProvider.@delegate = tenantIdProvider;

		testRule.Deploy(CMMN_SUBPROCESS_FILE, CMMN_FILE);

		// if a case instance is created
		engineRule.CaseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY).Create();

		// then the tenant id provider can set the tenant id to a value
		ICaseInstance subCaseInstance = engineRule.CaseService.CreateCaseInstanceQuery()
               // .CaseDefinitionKey("oneTaskCase")
                .First();
		Assert.That(subCaseInstance.TenantId, Is.EqualTo(tenantId));

		// and the super case instance is not assigned a tenant id
		ICaseInstance superCaseInstance = engineRule.CaseService.CreateCaseInstanceQuery()
                //.CaseDefinitionKey(CASE_DEFINITION_KEY)
                .First();
		Assert.That(superCaseInstance.TenantId, Is.EqualTo(null));
	  }


        [Test]
        public virtual void setNullTenantId_SubCaseInstance()
	  {

		string tenantId = null;
		SetValueOnSubCaseInstanceTenantIdProvider tenantIdProvider = new SetValueOnSubCaseInstanceTenantIdProvider(tenantId);
		TestTenantIdProvider.@delegate = tenantIdProvider;

		testRule.Deploy(CMMN_SUBPROCESS_FILE, CMMN_FILE);

		// if a case instance is created
		engineRule.CaseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY).Create();

		// then the tenant id provider can set the tenant id to null
		ICaseInstance caseInstance = engineRule.CaseService.CreateCaseInstanceQuery().First();//.CaseDefinitionKey("oneTaskCase").First();
		Assert.That(caseInstance.TenantId, Is.EqualTo(null));
	  }


        [Test]
        public virtual void tenantIdInheritedFromSuperCaseInstance()
	  {

		string tenantId = TENANT_ID;
		SetValueOnRootCaseInstanceTenantIdProvider tenantIdProvider = new SetValueOnRootCaseInstanceTenantIdProvider(tenantId);
		TestTenantIdProvider.@delegate = tenantIdProvider;

		testRule.Deploy(CMMN_SUBPROCESS_FILE, CMMN_FILE);

		// if a case instance is created
		engineRule.CaseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY).Create();

		// then the tenant id is inherited to the sub case instance even tough it is not set by the provider
		ICaseInstance caseInstance = engineRule.CaseService.CreateCaseInstanceQuery().First();//.CaseDefinitionKey("oneTaskCase").First();
		Assert.That(caseInstance.TenantId, Is.EqualTo(tenantId));
	  }


        [Test]
        public virtual void providerCalledForCaseInstanceWithSuperCaseExecution()
	  {

		ContextLoggingTenantIdProvider tenantIdProvider = new ContextLoggingTenantIdProvider();
		TestTenantIdProvider.@delegate = tenantIdProvider;

		testRule.Deploy(CMMN_SUBPROCESS_FILE, CMMN_FILE);

		// if the case is started
		engineRule.CaseService.WithCaseDefinitionByKey(CASE_DEFINITION_KEY).Create();

		// then the tenant id provider is handed in the super case execution
		Assert.That(tenantIdProvider.caseParameters.Count, Is.EqualTo(2));
		Assert.That(tenantIdProvider.caseParameters[1].SuperCaseExecution!=null);
	  }

	  protected internal virtual void startCaseTask()
	  {
		ICaseExecution caseExecution = engineRule.CaseService.CreateCaseExecutionQuery(c=>c.ActivityId == "PI_CaseTask_1").First();
		engineRule.CaseService.WithCaseExecution(caseExecution.Id).ManualStart();
	  }

	  // helpers //////////////////////////////////////////

	  public class TestTenantIdProvider : ITenantIdProvider
	  {

		protected internal static ITenantIdProvider @delegate;

		public string ProvideTenantIdForProcessInstance(TenantIdProviderProcessInstanceContext ctx)
		{
		  if (@delegate != null)
		  {
			return @delegate.ProvideTenantIdForProcessInstance(ctx);
		  }
		  else
		  {
			return null;
		  }
		}

		public static void reset()
		{
		  @delegate = null;
		}

		public virtual string ProvideTenantIdForHistoricDecisionInstance(TenantIdProviderHistoricDecisionInstanceContext ctx)
		{
		  if (@delegate != null)
		  {
			return @delegate.ProvideTenantIdForHistoricDecisionInstance(ctx);
		  }
		  else
		  {
			return null;
		  }
		}

		public virtual string ProvideTenantIdForCaseInstance(TenantIdProviderCaseInstanceContext ctx)
		{
		  if (@delegate != null)
		  {
			return @delegate.ProvideTenantIdForCaseInstance(ctx);
		  }
		  else
		  {
			return null;
		  }
		}
	  }

	  public class ContextLoggingTenantIdProvider : ITenantIdProvider
	  {

		protected internal IList<TenantIdProviderProcessInstanceContext> parameters = new List<TenantIdProviderProcessInstanceContext>();
		protected internal IList<TenantIdProviderHistoricDecisionInstanceContext> dmnParameters = new List<TenantIdProviderHistoricDecisionInstanceContext>();
		protected internal IList<TenantIdProviderCaseInstanceContext> caseParameters = new List<TenantIdProviderCaseInstanceContext>();

		public string ProvideTenantIdForProcessInstance(TenantIdProviderProcessInstanceContext ctx)
		{
		  parameters.Add(ctx);
		  return null;
		}

		public string ProvideTenantIdForHistoricDecisionInstance(TenantIdProviderHistoricDecisionInstanceContext ctx)
		{
		  dmnParameters.Add(ctx);
		  return null;
		}

		public string ProvideTenantIdForCaseInstance(TenantIdProviderCaseInstanceContext ctx)
		{
		  caseParameters.Add(ctx);
		  return null;
		}

	  }

	  //only sets tenant ids on sub process instances
	  public class SetValueOnSubProcessInstanceTenantIdProvider : ITenantIdProvider
	  {

		internal readonly string TenantIdToSet;

		public SetValueOnSubProcessInstanceTenantIdProvider(string tenantIdToSet)
		{
		  this.TenantIdToSet = tenantIdToSet;
		}

		public string ProvideTenantIdForProcessInstance(TenantIdProviderProcessInstanceContext ctx)
		{
		  return ctx.SuperExecution != null ? TenantIdToSet : null;
		}

		public string ProvideTenantIdForHistoricDecisionInstance(TenantIdProviderHistoricDecisionInstanceContext ctx)
		{
		  return null;
		}

		public string ProvideTenantIdForCaseInstance(TenantIdProviderCaseInstanceContext ctx)
		{
		  return null;
		}

	  }

	  // only sets tenant ids on root process instances
	  public class SetValueOnRootProcessInstanceTenantIdProvider : ITenantIdProvider
	  {

		internal readonly string TenantIdToSet;

		public SetValueOnRootProcessInstanceTenantIdProvider(string tenantIdToSet)
		{
		  this.TenantIdToSet = tenantIdToSet;
		}

		public virtual string ProvideTenantIdForProcessInstance(TenantIdProviderProcessInstanceContext ctx)
		{
		  return ctx.SuperExecution == null ? TenantIdToSet : null;
		}

		public virtual string ProvideTenantIdForHistoricDecisionInstance(TenantIdProviderHistoricDecisionInstanceContext ctx)
		{
		  return null;
		}

		public virtual string ProvideTenantIdForCaseInstance(TenantIdProviderCaseInstanceContext ctx)
		{
		  return null;
		}
	  }

	  //only sets tenant ids on historic decision instances when an execution exists
	  public class SetValueOnHistoricDecisionInstanceTenantIdProvider : ITenantIdProvider
	  {

		internal readonly string TenantIdToSet;

		public SetValueOnHistoricDecisionInstanceTenantIdProvider(string tenantIdToSet)
		{
		  this.TenantIdToSet = tenantIdToSet;
		}

		public virtual string ProvideTenantIdForProcessInstance(TenantIdProviderProcessInstanceContext ctx)
		{
		  return null;
		}

		public virtual string ProvideTenantIdForHistoricDecisionInstance(TenantIdProviderHistoricDecisionInstanceContext ctx)
		{
		  return ctx.CaseExecution != null ? TenantIdToSet : null;
		}

		public virtual string ProvideTenantIdForCaseInstance(TenantIdProviderCaseInstanceContext ctx)
		{
		  return null;
		}
	  }

	  //only sets tenant ids on sub case instances
	  public class SetValueOnSubCaseInstanceTenantIdProvider : ITenantIdProvider
	  {

		internal readonly string TenantIdToSet;

		public SetValueOnSubCaseInstanceTenantIdProvider(string tenantIdToSet)
		{
		  this.TenantIdToSet = tenantIdToSet;
		}

		public virtual string ProvideTenantIdForProcessInstance(TenantIdProviderProcessInstanceContext ctx)
		{
		  return null;
		}

		public virtual string ProvideTenantIdForHistoricDecisionInstance(TenantIdProviderHistoricDecisionInstanceContext ctx)
		{
		  return null;
		}

		public virtual string ProvideTenantIdForCaseInstance(TenantIdProviderCaseInstanceContext ctx)
		{
		  return ctx.SuperCaseExecution != null ? TenantIdToSet : null;
		}
	  }

	  // only sets tenant ids on root case instances
	  public class SetValueOnRootCaseInstanceTenantIdProvider : ITenantIdProvider
	  {

		internal readonly string TenantIdToSet;

		public SetValueOnRootCaseInstanceTenantIdProvider(string tenantIdToSet)
		{
		  this.TenantIdToSet = tenantIdToSet;
		}

		public string ProvideTenantIdForProcessInstance(TenantIdProviderProcessInstanceContext ctx)
		{
		  return null;
		}

		public string ProvideTenantIdForHistoricDecisionInstance(TenantIdProviderHistoricDecisionInstanceContext ctx)
		{
		  return null;
		}

		public string ProvideTenantIdForCaseInstance(TenantIdProviderCaseInstanceContext ctx)
		{
		  return ctx.SuperCaseExecution == null ? TenantIdToSet : null;
		}
	  }

	}

}