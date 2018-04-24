using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;

namespace Engine.Tests.Api.MultiTenancy
{
    
	public class MultiTenancyBusinessRuleTaskTest : PluggableProcessEngineTestCase
	{

	  protected internal const string TENANT_ONE = "tenant1";
	  protected internal const string TENANT_TWO = "tenant2";

	  protected internal const string DMN_FILE = "resources/api/multitenancy/simpleDecisionTable.Dmn";
	  protected internal const string DMN_FILE_VERSION_TWO = "resources/api/multitenancy/simpleDecisionTable_v2.Dmn";

	  protected internal const string RESULT_OF_VERSION_ONE = "A";
	  protected internal const string RESULT_OF_VERSION_TWO = "C";

	   [Test]   public virtual void testEvaluateDecisionWithDeploymentBinding()
	  {

		IBpmnModelInstance process = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process").StartEvent().BusinessRuleTask().CamundaDecisionRef("decision").CamundaDecisionRefBinding("deployment").CamundaMapDecisionResult("singleEntry").CamundaResultVariable("decisionVar").CamundaAsyncAfter().EndEvent().Done();

		DeploymentForTenant(TENANT_ONE, DMN_FILE, process);
		DeploymentForTenant(TENANT_TWO, DMN_FILE_VERSION_TWO, process);

		IProcessInstance processInstanceOne = runtimeService.CreateProcessInstanceByKey("process").SetVariable("status", "gold").SetProcessDefinitionTenantId(TENANT_ONE).Execute();

		IProcessInstance processInstanceTwo = runtimeService.CreateProcessInstanceByKey("process").SetVariable("status", "gold").SetProcessDefinitionTenantId(TENANT_TWO).Execute();

		Assert.That((string)runtimeService.GetVariable(processInstanceOne.Id, "decisionVar"), Is.EqualTo(RESULT_OF_VERSION_ONE));
		Assert.That((string)runtimeService.GetVariable(processInstanceTwo.Id, "decisionVar"), Is.EqualTo(RESULT_OF_VERSION_TWO));
	  }

	   [Test]   public virtual void testEvaluateDecisionWithLatestBindingSameVersion()
	  {

		IBpmnModelInstance process = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process").StartEvent().BusinessRuleTask().CamundaDecisionRef("decision").CamundaDecisionRefBinding("latest").CamundaMapDecisionResult("singleEntry").CamundaResultVariable("decisionVar").CamundaAsyncAfter().EndEvent().Done();

		DeploymentForTenant(TENANT_ONE, DMN_FILE, process);
		DeploymentForTenant(TENANT_TWO, DMN_FILE_VERSION_TWO, process);

		IProcessInstance processInstanceOne = runtimeService.CreateProcessInstanceByKey("process").SetVariable("status", "gold").SetProcessDefinitionTenantId(TENANT_ONE).Execute();

		IProcessInstance processInstanceTwo = runtimeService.CreateProcessInstanceByKey("process").SetVariable("status", "gold").SetProcessDefinitionTenantId(TENANT_TWO).Execute();

		Assert.That((string)runtimeService.GetVariable(processInstanceOne.Id, "decisionVar"), Is.EqualTo(RESULT_OF_VERSION_ONE));
		Assert.That((string)runtimeService.GetVariable(processInstanceTwo.Id, "decisionVar"), Is.EqualTo(RESULT_OF_VERSION_TWO));
	  }

	   [Test]   public virtual void testEvaluateDecisionWithLatestBindingDifferentVersions()
	  {

		IBpmnModelInstance process = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process").StartEvent().BusinessRuleTask().CamundaDecisionRef("decision").CamundaDecisionRefBinding("latest").CamundaMapDecisionResult("singleEntry").CamundaResultVariable("decisionVar").CamundaAsyncAfter().EndEvent().Done();

		DeploymentForTenant(TENANT_ONE, DMN_FILE, process);

		DeploymentForTenant(TENANT_TWO, DMN_FILE, process);
		DeploymentForTenant(TENANT_TWO, DMN_FILE_VERSION_TWO);

		IProcessInstance processInstanceOne = runtimeService.CreateProcessInstanceByKey("process").SetVariable("status", "gold").SetProcessDefinitionTenantId(TENANT_ONE).Execute();

		IProcessInstance processInstanceTwo = runtimeService.CreateProcessInstanceByKey("process").SetVariable("status", "gold").SetProcessDefinitionTenantId(TENANT_TWO).Execute();

		Assert.That((string)runtimeService.GetVariable(processInstanceOne.Id, "decisionVar"), Is.EqualTo(RESULT_OF_VERSION_ONE));
		Assert.That((string)runtimeService.GetVariable(processInstanceTwo.Id, "decisionVar"), Is.EqualTo(RESULT_OF_VERSION_TWO));
	  }

	   [Test]   public virtual void testEvaluateDecisionWithVersionBinding()
	  {

		IBpmnModelInstance process = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process").StartEvent().BusinessRuleTask().CamundaDecisionRef("decision").CamundaDecisionRefBinding("version").CamundaDecisionRefVersion("1").CamundaMapDecisionResult("singleEntry").CamundaResultVariable("decisionVar").CamundaAsyncAfter().EndEvent().Done();

		DeploymentForTenant(TENANT_ONE, DMN_FILE, process);
		DeploymentForTenant(TENANT_ONE, DMN_FILE_VERSION_TWO);

		DeploymentForTenant(TENANT_TWO, DMN_FILE_VERSION_TWO, process);
		DeploymentForTenant(TENANT_TWO, DMN_FILE);

		IProcessInstance processInstanceOne = runtimeService.CreateProcessInstanceByKey("process").SetVariable("status", "gold").SetProcessDefinitionTenantId(TENANT_ONE).Execute();

		IProcessInstance processInstanceTwo = runtimeService.CreateProcessInstanceByKey("process").SetVariable("status", "gold").SetProcessDefinitionTenantId(TENANT_TWO).Execute();

		Assert.That((string)runtimeService.GetVariable(processInstanceOne.Id, "decisionVar"), Is.EqualTo(RESULT_OF_VERSION_ONE));
		Assert.That((string)runtimeService.GetVariable(processInstanceTwo.Id, "decisionVar"), Is.EqualTo(RESULT_OF_VERSION_TWO));
	  }

	   [Test]   public virtual void testFailEvaluateDecisionFromOtherTenantWithDeploymentBinding()
	  {

		IBpmnModelInstance process = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process").StartEvent().BusinessRuleTask().CamundaDecisionRef("decision").CamundaDecisionRefBinding("deployment").CamundaAsyncAfter().EndEvent().Done();

		DeploymentForTenant(TENANT_ONE, process);
		DeploymentForTenant(TENANT_TWO, DMN_FILE);

		try
		{
		  runtimeService.CreateProcessInstanceByKey("process").SetProcessDefinitionTenantId(TENANT_ONE).Execute();

		  Assert.Fail("expected exception");
		}
		catch (ProcessEngineException e)
		{
		  Assert.That(e.Message, Does.Contain("no decision definition deployed with key = 'decision'"));
		}
	  }

	   [Test]   public virtual void testFailEvaluateDecisionFromOtherTenantWithLatestBinding()
	  {

		IBpmnModelInstance process = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process").StartEvent().BusinessRuleTask().CamundaDecisionRef("decision").CamundaDecisionRefBinding("latest").CamundaAsyncAfter().EndEvent().Done();

		DeploymentForTenant(TENANT_ONE, process);
		DeploymentForTenant(TENANT_TWO, DMN_FILE);

		try
		{
		  runtimeService.CreateProcessInstanceByKey("process").SetProcessDefinitionTenantId(TENANT_ONE).Execute();

		  Assert.Fail("expected exception");
		}
		catch (ProcessEngineException e)
		{
		  Assert.That(e.Message, Does.Contain("no decision definition deployed with key 'decision'"));
		}
	  }

	   [Test]   public virtual void testFailEvaluateDecisionFromOtherTenantWithVersionBinding()
	  {

		IBpmnModelInstance process = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process").StartEvent().BusinessRuleTask().CamundaDecisionRef("decision").CamundaDecisionRefBinding("version").CamundaDecisionRefVersion("2").CamundaAsyncAfter().EndEvent().Done();

		DeploymentForTenant(TENANT_ONE, DMN_FILE, process);

		DeploymentForTenant(TENANT_TWO, DMN_FILE);
		DeploymentForTenant(TENANT_TWO, DMN_FILE);

		try
		{
		  runtimeService.CreateProcessInstanceByKey("process").SetProcessDefinitionTenantId(TENANT_ONE).Execute();

		  Assert.Fail("expected exception");
		}
		catch (ProcessEngineException e)
		{
		  Assert.That(e.Message, Does.Contain("no decision definition deployed with key = 'decision', version = '2' and tenant-id 'tenant1'"));
		}
	  }

	   [Test]   public virtual void testEvaluateDecisionTenantIdConstant()
	  {

		IBpmnModelInstance process = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process").StartEvent().BusinessRuleTask().CamundaDecisionRef("decision").CamundaDecisionRefBinding("latest").CamundaDecisionRefTenantId(TENANT_ONE).CamundaMapDecisionResult("singleEntry").CamundaResultVariable("decisionVar").CamundaAsyncAfter().EndEvent().Done();

		DeploymentForTenant(TENANT_ONE, DMN_FILE);
		DeploymentForTenant(TENANT_TWO, DMN_FILE_VERSION_TWO);
		Deployment(process);

		IProcessInstance processInstanceOne = runtimeService.CreateProcessInstanceByKey("process").SetVariable("status", "gold").Execute();

		Assert.That((string)runtimeService.GetVariable(processInstanceOne.Id, "decisionVar"), Is.EqualTo(RESULT_OF_VERSION_ONE));
	  }

	   [Test]   public virtual void testEvaluateDecisionWithoutTenantIdConstant()
	  {

		IBpmnModelInstance process = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process").StartEvent().BusinessRuleTask().CamundaDecisionRef("decision").CamundaDecisionRefBinding("latest").CamundaDecisionRefTenantId("${null}").CamundaMapDecisionResult("singleEntry").CamundaResultVariable("decisionVar").CamundaAsyncAfter().EndEvent().Done();

		Deployment(DMN_FILE);
		DeploymentForTenant(TENANT_ONE, process);
		DeploymentForTenant(TENANT_TWO, DMN_FILE_VERSION_TWO);

		IProcessInstance processInstanceOne = runtimeService.CreateProcessInstanceByKey("process").SetVariable("status", "gold").Execute();

		Assert.That((string)runtimeService.GetVariable(processInstanceOne.Id, "decisionVar"), Is.EqualTo(RESULT_OF_VERSION_ONE));
	  }

	   [Test]   public virtual void testEvaluateDecisionTenantIdExpression()
	  {

		IBpmnModelInstance process = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process").StartEvent().BusinessRuleTask().CamundaDecisionRef("decision").CamundaDecisionRefBinding("latest").CamundaDecisionRefTenantId("${'" + TENANT_ONE + "'}").CamundaMapDecisionResult("singleEntry").CamundaResultVariable("decisionVar").CamundaAsyncAfter().EndEvent().Done();

		DeploymentForTenant(TENANT_ONE, DMN_FILE);
		DeploymentForTenant(TENANT_TWO, DMN_FILE_VERSION_TWO);
		Deployment(process);

		IProcessInstance processInstanceOne = runtimeService.CreateProcessInstanceByKey("process").SetVariable("status", "gold").Execute();

		Assert.That((string)runtimeService.GetVariable(processInstanceOne.Id, "decisionVar"), Is.EqualTo(RESULT_OF_VERSION_ONE));
	  }

	   [Test]   public virtual void testEvaluateDecisionTenantIdCompositeExpression()
	  {
		// given
		IBpmnModelInstance process = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process").StartEvent().BusinessRuleTask().CamundaDecisionRef("decision").CamundaDecisionRefBinding("latest").CamundaDecisionRefTenantId("tenant${'1'}").CamundaMapDecisionResult("singleEntry").CamundaResultVariable("decisionVar").CamundaAsyncAfter().EndEvent().Done();
		DeploymentForTenant(TENANT_ONE, DMN_FILE);
		DeploymentForTenant(TENANT_TWO, DMN_FILE_VERSION_TWO);
		Deployment(process);

		// when
		IProcessInstance processInstanceOne = runtimeService.CreateProcessInstanceByKey("process").SetVariable("status", "gold").Execute();

		// then
		Assert.That((string)runtimeService.GetVariable(processInstanceOne.Id, "decisionVar"), Is.EqualTo(RESULT_OF_VERSION_ONE));
	  }

	}

}