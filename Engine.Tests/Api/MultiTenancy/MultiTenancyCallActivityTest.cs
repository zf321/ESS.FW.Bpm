using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;

namespace Engine.Tests.Api.MultiTenancy
{
    

	public class MultiTenancyCallActivityTest : PluggableProcessEngineTestCase
	{

	  protected internal const string TENANT_ONE = "tenant1";
	  protected internal const string TENANT_TWO = "tenant2";

	  protected internal const string CMMN = "resources/cmmn/deployment/CmmnDeploymentTest.TestSimpleDeployment.cmmn";

	  protected internal static readonly IBpmnModelInstance SUB_PROCESS = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("subProcess").StartEvent().UserTask().EndEvent().Done();

	   [Test]   public virtual void testStartProcessInstanceWithDeploymentBinding()
	  {

		IBpmnModelInstance callingProcess = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("callingProcess").StartEvent().CallActivity().CalledElement("subProcess").CamundaCalledElementBinding("deployment").EndEvent().Done();

		DeploymentForTenant(TENANT_ONE, callingProcess, SUB_PROCESS);
		DeploymentForTenant(TENANT_TWO, callingProcess, SUB_PROCESS);

		runtimeService.CreateProcessInstanceByKey("callingProcess").SetProcessDefinitionTenantId(TENANT_ONE).Execute();
		runtimeService.CreateProcessInstanceByKey("callingProcess").SetProcessDefinitionTenantId(TENANT_TWO).Execute();

		 IQueryable<IProcessInstance> query = runtimeService.CreateProcessInstanceQuery(c=>c.ProcessDefinitionId == "subProcess");
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_TWO).Count(), Is.EqualTo(1L));
	  }

	   [Test]   public virtual void testStartProcessInstanceWithLatestBindingSameVersion()
	  {

		IBpmnModelInstance callingProcess = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("callingProcess").StartEvent().CallActivity().CalledElement("subProcess").CamundaCalledElementBinding("latest").EndEvent().Done();

		DeploymentForTenant(TENANT_ONE, callingProcess, SUB_PROCESS);
		DeploymentForTenant(TENANT_TWO, callingProcess, SUB_PROCESS);

		runtimeService.CreateProcessInstanceByKey("callingProcess").SetProcessDefinitionTenantId(TENANT_ONE).Execute();
		runtimeService.CreateProcessInstanceByKey("callingProcess").SetProcessDefinitionTenantId(TENANT_TWO).Execute();

		 IQueryable<IProcessInstance> query = runtimeService.CreateProcessInstanceQuery(c=>c.ProcessDefinitionId == "subProcess");
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_TWO).Count(), Is.EqualTo(1L));
	  }

	   [Test]   public virtual void testStartProcessInstanceWithLatestBindingDifferentVersion()
	  {

		IBpmnModelInstance callingProcess = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("callingProcess").StartEvent().CallActivity().CalledElement("subProcess").CamundaCalledElementBinding("latest").EndEvent().Done();

		DeploymentForTenant(TENANT_ONE, callingProcess, SUB_PROCESS);

		DeploymentForTenant(TENANT_TWO, callingProcess, SUB_PROCESS);
		DeploymentForTenant(TENANT_TWO, SUB_PROCESS);

		runtimeService.CreateProcessInstanceByKey("callingProcess").SetProcessDefinitionTenantId(TENANT_ONE).Execute();
		runtimeService.CreateProcessInstanceByKey("callingProcess").SetProcessDefinitionTenantId(TENANT_TWO).Execute();

		IProcessDefinition latestSubProcessTenantTwo = repositoryService.CreateProcessDefinitionQuery(c=>c.TenantId == TENANT_TWO)/*.ProcessDefinitionKey("subProcess")/*.LatestVersion()*/.First();

		 IQueryable<IProcessInstance> query = runtimeService.CreateProcessInstanceQuery(c=>c.ProcessDefinitionId == "subProcess");
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_TWO&& c.ProcessDefinitionId==latestSubProcessTenantTwo.Id).Count(), Is.EqualTo(1L));
	  }

	   [Test]   public virtual void testStartProcessInstanceWithVersionBinding()
	  {

		IBpmnModelInstance callingProcess = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("callingProcess").StartEvent().CallActivity().CalledElement("subProcess").CamundaCalledElementBinding("version").CamundaCalledElementVersion("1").EndEvent().Done();

		DeploymentForTenant(TENANT_ONE, callingProcess, SUB_PROCESS);
		DeploymentForTenant(TENANT_TWO, callingProcess, SUB_PROCESS);

		runtimeService.CreateProcessInstanceByKey("callingProcess").SetProcessDefinitionTenantId(TENANT_ONE).Execute();
		runtimeService.CreateProcessInstanceByKey("callingProcess").SetProcessDefinitionTenantId(TENANT_TWO).Execute();

		 IQueryable<IProcessInstance> query = runtimeService.CreateProcessInstanceQuery(c=>c.ProcessDefinitionId == "subProcess");
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_TWO).Count(), Is.EqualTo(1L));
	  }

	   [Test]   public virtual void testFailStartProcessInstanceFromOtherTenantWithDeploymentBinding()
	  {

		IBpmnModelInstance callingProcess = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("callingProcess").StartEvent().CallActivity().CalledElement("subProcess").CamundaCalledElementBinding("deployment").EndEvent().Done();

		DeploymentForTenant(TENANT_ONE, callingProcess);
		DeploymentForTenant(TENANT_TWO, SUB_PROCESS);

		try
		{
		  runtimeService.CreateProcessInstanceByKey("callingProcess").SetProcessDefinitionTenantId(TENANT_ONE).Execute();

		  Assert.Fail("expected exception");
		}
		catch (ProcessEngineException e)
		{
		  Assert.That(e.Message, Does.Contain("no processes deployed with key = 'subProcess'"));
		}
	  }

	   [Test]   public virtual void testFailStartProcessInstanceFromOtherTenantWithLatestBinding()
	  {

		IBpmnModelInstance callingProcess = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("callingProcess").StartEvent().CallActivity().CalledElement("subProcess").CamundaCalledElementBinding("latest").EndEvent().Done();

		DeploymentForTenant(TENANT_ONE, callingProcess);
		DeploymentForTenant(TENANT_TWO, SUB_PROCESS);

		try
		{
		  runtimeService.CreateProcessInstanceByKey("callingProcess").SetProcessDefinitionTenantId(TENANT_ONE).Execute();

		  Assert.Fail("expected exception");
		}
		catch (ProcessEngineException e)
		{
		  Assert.That(e.Message, Does.Contain("no processes deployed with key 'subProcess'"));
		}
	  }

	   [Test]   public virtual void testFailStartProcessInstanceFromOtherTenantWithVersionBinding()
	  {

		IBpmnModelInstance callingProcess = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("callingProcess").StartEvent().CallActivity().CalledElement("subProcess").CamundaCalledElementBinding("version").CamundaCalledElementVersion("2").EndEvent().Done();

		DeploymentForTenant(TENANT_ONE, callingProcess, SUB_PROCESS);

		DeploymentForTenant(TENANT_TWO, SUB_PROCESS);
		DeploymentForTenant(TENANT_TWO, SUB_PROCESS);

		try
		{
		  runtimeService.CreateProcessInstanceByKey("callingProcess").SetProcessDefinitionTenantId(TENANT_ONE).Execute();

		  Assert.Fail("expected exception");
		}
		catch (ProcessEngineException e)
		{
		  Assert.That(e.Message, Does.Contain("no processes deployed with key = 'subProcess'"));
		}
	  }

	   [Test]   public virtual void testStartCaseInstanceWithDeploymentBinding()
	  {

		IBpmnModelInstance callingProcess = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("callingProcess").StartEvent().CallActivity().CamundaCaseRef("Case_1").CamundaCaseBinding("deployment").EndEvent().Done();

		DeploymentForTenant(TENANT_ONE, CMMN, callingProcess);
		DeploymentForTenant(TENANT_TWO, CMMN, callingProcess);

		runtimeService.CreateProcessInstanceByKey("callingProcess").SetProcessDefinitionTenantId(TENANT_ONE).Execute();
		runtimeService.CreateProcessInstanceByKey("callingProcess").SetProcessDefinitionTenantId(TENANT_TWO).Execute();

		//IQueryable<ICaseDefinition> query = caseService.CreateCaseInstanceQuery()/*.CaseDefinitionKey("Case_1")*/;
		//Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
		//Assert.That(query.Where(c=>c.TenantId == TENANT_TWO).Count(), Is.EqualTo(1L));
	  }

	   [Test]   public virtual void testStartCaseInstanceWithLatestBindingSameVersion()
	  {

		IBpmnModelInstance callingProcess = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("callingProcess").StartEvent().CallActivity().CamundaCaseRef("Case_1").CamundaCaseBinding("latest").EndEvent().Done();

		DeploymentForTenant(TENANT_ONE, CMMN, callingProcess);
		DeploymentForTenant(TENANT_TWO, CMMN, callingProcess);

		runtimeService.CreateProcessInstanceByKey("callingProcess").SetProcessDefinitionTenantId(TENANT_ONE).Execute();
		runtimeService.CreateProcessInstanceByKey("callingProcess").SetProcessDefinitionTenantId(TENANT_TWO).Execute();

		//ICaseInstanceQuery query = caseService.CreateCaseInstanceQuery().CaseDefinitionKey("Case_1");
		//Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
		//Assert.That(query.Where(c=>c.TenantId == TENANT_TWO).Count(), Is.EqualTo(1L));
	  }

	   [Test]   public virtual void testStartCaseInstanceWithLatestBindingDifferentVersion()
	  {

		IBpmnModelInstance callingProcess = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("callingProcess").StartEvent().CallActivity().CamundaCaseRef("Case_1").CamundaCaseBinding("latest").EndEvent().Done();

		DeploymentForTenant(TENANT_ONE, CMMN, callingProcess);

		DeploymentForTenant(TENANT_TWO, CMMN, callingProcess);
		DeploymentForTenant(TENANT_TWO, CMMN);

		runtimeService.CreateProcessInstanceByKey("callingProcess").SetProcessDefinitionTenantId(TENANT_ONE).Execute();
		runtimeService.CreateProcessInstanceByKey("callingProcess").SetProcessDefinitionTenantId(TENANT_TWO).Execute();

		//ICaseInstanceQuery query = caseService.CreateCaseInstanceQuery().CaseDefinitionKey("Case_1");
		//Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));

		//ICaseDefinition latestCaseDefinitionTenantTwo = repositoryService.CreateCaseDefinitionQuery(c=>c.TenantId == TENANT_TWO)/*.LatestVersion()*/.First();
		//query = caseService.CreateCaseInstanceQuery(c=>c.CaseDefinitionId== latestCaseDefinitionTenantTwo.Id);
		//Assert.That(query.Count(), Is.EqualTo(1L));
	  }

	   [Test]   public virtual void testStartCaseInstanceWithVersionBinding()
	  {

		IBpmnModelInstance callingProcess = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("callingProcess").StartEvent().CallActivity().CamundaCaseRef("Case_1").CamundaCaseBinding("version").CamundaCaseVersion("1").EndEvent().Done();

		DeploymentForTenant(TENANT_ONE, CMMN, callingProcess);
		DeploymentForTenant(TENANT_TWO, CMMN, callingProcess);

		runtimeService.CreateProcessInstanceByKey("callingProcess").SetProcessDefinitionTenantId(TENANT_ONE).Execute();
		runtimeService.CreateProcessInstanceByKey("callingProcess").SetProcessDefinitionTenantId(TENANT_TWO).Execute();

		//ICaseInstanceQuery query = caseService.CreateCaseInstanceQuery().CaseDefinitionKey("Case_1");
		//Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
		//Assert.That(query.Where(c=>c.TenantId == TENANT_TWO).Count(), Is.EqualTo(1L));
	  }

	   [Test]   public virtual void testFailStartCaseInstanceFromOtherTenantWithDeploymentBinding()
	  {

		IBpmnModelInstance callingProcess = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("callingProcess").StartEvent().CallActivity().CamundaCaseRef("Case_1").CamundaCaseBinding("deployment").EndEvent().Done();

		DeploymentForTenant(TENANT_ONE, callingProcess);
		DeploymentForTenant(TENANT_TWO, CMMN);

		try
		{
		  runtimeService.CreateProcessInstanceByKey("callingProcess").SetProcessDefinitionTenantId(TENANT_ONE).Execute();

		  Assert.Fail("expected exception");
		}
		catch (ProcessEngineException e)
		{
		  Assert.That(e.Message, Does.Contain("no case definition deployed with key = 'Case_1'"));
		}
	  }

	   [Test]   public virtual void testFailStartCaseInstanceFromOtherTenantWithLatestBinding()
	  {

		IBpmnModelInstance callingProcess = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("callingProcess").StartEvent().CallActivity().CamundaCaseRef("Case_1").CamundaCaseBinding("latest").EndEvent().Done();

		DeploymentForTenant(TENANT_ONE, callingProcess);
		DeploymentForTenant(TENANT_TWO, CMMN);

		try
		{
		  runtimeService.CreateProcessInstanceByKey("callingProcess").SetProcessDefinitionTenantId(TENANT_ONE).Execute();

		  Assert.Fail("expected exception");
		}
		catch (ProcessEngineException e)
		{
		  Assert.That(e.Message, Does.Contain("no case definition deployed with key 'Case_1'"));
		}
	  }

	   [Test]   public virtual void testFailStartCaseInstanceFromOtherTenantWithVersionBinding()
	  {

		IBpmnModelInstance callingProcess = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("callingProcess").StartEvent().CallActivity().CamundaCaseRef("Case_1").CamundaCaseBinding("version").CamundaCaseVersion("2").EndEvent().Done();

		DeploymentForTenant(TENANT_ONE, CMMN, callingProcess);

		DeploymentForTenant(TENANT_TWO, CMMN);
		DeploymentForTenant(TENANT_TWO, CMMN);

		try
		{
		  runtimeService.CreateProcessInstanceByKey("callingProcess").SetProcessDefinitionTenantId(TENANT_ONE).Execute();

		  Assert.Fail("expected exception");
		}
		catch (ProcessEngineException e)
		{
		  Assert.That(e.Message, Does.Contain("no case definition deployed with key = 'Case_1'"));
		}
	  }

	   [Test]   public virtual void testCalledElementTenantIdConstant()
	  {

		IBpmnModelInstance callingProcess = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("callingProcess").StartEvent().CallActivity().CalledElement("subProcess").CamundaCalledElementTenantId(TENANT_ONE).EndEvent().Done();

		DeploymentForTenant(TENANT_ONE, SUB_PROCESS);
		Deployment(callingProcess);

		runtimeService.StartProcessInstanceByKey("callingProcess");

		 IQueryable<IProcessInstance> query = runtimeService.CreateProcessInstanceQuery(c=>c.ProcessDefinitionId == "subProcess");
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
	  }

	   [Test]   public virtual void testCalledElementTenantIdExpression()
	  {

		IBpmnModelInstance callingProcess = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("callingProcess").StartEvent().CallActivity().CalledElement("subProcess").CamundaCalledElementTenantId("${'" + TENANT_ONE + "'}").EndEvent().Done();

		DeploymentForTenant(TENANT_ONE, SUB_PROCESS);
		Deployment(callingProcess);

		runtimeService.StartProcessInstanceByKey("callingProcess");

		 IQueryable<IProcessInstance> query = runtimeService.CreateProcessInstanceQuery(c=>c.ProcessDefinitionId == "subProcess");
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
	  }

	   [Test]   public virtual void testCaseRefTenantIdConstant()
	  {

		IBpmnModelInstance callingProcess = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("callingProcess").StartEvent().CallActivity().CamundaCaseRef("Case_1").CamundaCaseTenantId(TENANT_ONE).EndEvent().Done();

		DeploymentForTenant(TENANT_ONE, CMMN);
		Deployment(callingProcess);

		runtimeService.StartProcessInstanceByKey("callingProcess");

		//ICaseInstanceQuery query = caseService.CreateCaseInstanceQuery().CaseDefinitionKey("Case_1");
		//Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));

	  }

	   [Test]   public virtual void testCaseRefTenantIdExpression()
	  {

		IBpmnModelInstance callingProcess = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("callingProcess").StartEvent().CallActivity().CamundaCaseRef("Case_1").CamundaCaseTenantId("${'" + TENANT_ONE + "'}").EndEvent().Done();

		DeploymentForTenant(TENANT_ONE, CMMN);
		Deployment(callingProcess);

		runtimeService.StartProcessInstanceByKey("callingProcess");

		//ICaseInstanceQuery query = caseService.CreateCaseInstanceQuery().CaseDefinitionKey("Case_1");
		//Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
	  }

	   [Test]   public virtual void testCaseRefTenantIdCompositeExpression()
	  {
		// given
		IBpmnModelInstance callingProcess = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("callingProcess").StartEvent().CallActivity().CamundaCaseRef("Case_1").CamundaCaseTenantId("tenant${'1'}").EndEvent().Done();

		DeploymentForTenant(TENANT_ONE, CMMN);
		Deployment(callingProcess);

		// when
		runtimeService.StartProcessInstanceByKey("callingProcess");

		// then
		//ICaseInstanceQuery query = caseService.CreateCaseInstanceQuery().CaseDefinitionKey("Case_1");
		//Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
	  }

	}

}