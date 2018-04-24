using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Variable;
using NUnit.Framework;

namespace Engine.Tests.Api.MultiTenancy
{
    

    public class MultiTenancyDecisionTableEvaluationTest : PluggableProcessEngineTestCase
	{

	  protected internal const string DMN_FILE = "resources/api/dmn/Example.Dmn";
	  protected internal const string DMN_FILE_SECOND_VERSION = "resources/api/dmn/Example_v2.Dmn";

	  protected internal const string DECISION_DEFINITION_KEY = "decision";

	  protected internal const string TENANT_ONE = "tenant1";
	  protected internal const string TENANT_TWO = "tenant2";

	  protected internal const string RESULT_OF_FIRST_VERSION = "ok";
	  protected internal const string RESULT_OF_SECOND_VERSION = "notok";

	   [Test]   public virtual void testFailToEvaluateDecisionByIdWithoutTenantId()
	  {
		Deployment(DMN_FILE);

		IDecisionDefinition decisionDefinition = repositoryService.CreateDecisionDefinitionQuery().First();

		try
		{
		  decisionService.EvaluateDecisionTableById(decisionDefinition.Id).SetVariables(createVariables()).DecisionDefinitionWithoutTenantId().Evaluate();
		  Assert.Fail("BadUserRequestException exception");
		}
		catch (BadUserRequestException e)
		{
		  Assert.That(e.Message, Does.Contain("Cannot specify a tenant-id"));
		}
	  }

	   [Test]   public virtual void testFailToEvaluateDecisionByIdWithTenantId()
	  {
		DeploymentForTenant(TENANT_ONE, DMN_FILE);

		IDecisionDefinition decisionDefinition = repositoryService.CreateDecisionDefinitionQuery().First();

		try
		{
		  decisionService.EvaluateDecisionTableById(decisionDefinition.Id).SetVariables(createVariables()).SetDecisionDefinitionTenantId(TENANT_ONE).Evaluate();
		  Assert.Fail("BadUserRequestException exception");
		}
		catch (BadUserRequestException e)
		{
		  Assert.That(e.Message, Does.Contain("Cannot specify a tenant-id"));
		}
	  }

	   [Test]   public virtual void testFailToEvaluateDecisionByKeyForNonExistingTenantID()
	  {
		DeploymentForTenant(TENANT_ONE, DMN_FILE);
		DeploymentForTenant(TENANT_TWO, DMN_FILE);

		try
		{
		  decisionService.EvaluateDecisionTableByKey(DECISION_DEFINITION_KEY).SetVariables(createVariables()).SetDecisionDefinitionTenantId("nonExistingTenantId").Evaluate();
		  Assert.Fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException e)
		{
		  Assert.That(e.Message, Does.Contain("no decision definition deployed with key 'decision' and tenant-id 'nonExistingTenantId'"));
		}
	  }

	   [Test]   public virtual void testFailToEvaluateDecisionByKeyForMultipleTenants()
	  {
		DeploymentForTenant(TENANT_ONE, DMN_FILE);
		DeploymentForTenant(TENANT_TWO, DMN_FILE);

		try
		{
		  decisionService.EvaluateDecisionTableByKey(DECISION_DEFINITION_KEY).SetVariables(createVariables()).Evaluate();
		  Assert.Fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException e)
		{
		  Assert.That(e.Message, Does.Contain("multiple tenants."));
		}
	  }

	   [Test]   public virtual void testEvaluateDecisionByKeyWithoutTenantId()
	  {
		Deployment(DMN_FILE);

		IDmnDecisionTableResult decisionResult = decisionService.EvaluateDecisionTableByKey(DECISION_DEFINITION_KEY).SetVariables(createVariables()).DecisionDefinitionWithoutTenantId().Evaluate();

		AssertThatDecisionHasResult(decisionResult, RESULT_OF_FIRST_VERSION);
	  }

	   [Test]   public virtual void testEvaluateDecisionByKeyForAnyTenants()
	  {
		DeploymentForTenant(TENANT_ONE, DMN_FILE);

		IDmnDecisionTableResult decisionResult = decisionService.EvaluateDecisionTableByKey(DECISION_DEFINITION_KEY).SetVariables(createVariables()).Evaluate();

		AssertThatDecisionHasResult(decisionResult, RESULT_OF_FIRST_VERSION);
	  }


	   [Test]   public virtual void testEvaluateDecisionByKeyAndTenantId()
	  {
		DeploymentForTenant(TENANT_ONE, DMN_FILE);
		DeploymentForTenant(TENANT_TWO, DMN_FILE_SECOND_VERSION);

		IDmnDecisionTableResult decisionResult = decisionService.EvaluateDecisionTableByKey(DECISION_DEFINITION_KEY).SetVariables(createVariables()).SetDecisionDefinitionTenantId(TENANT_ONE).Evaluate();

		AssertThatDecisionHasResult(decisionResult, RESULT_OF_FIRST_VERSION);
	  }

	   [Test]   public virtual void testEvaluateDecisionByKeyLatestVersionAndTenantId()
	  {
		DeploymentForTenant(TENANT_ONE, DMN_FILE);
		DeploymentForTenant(TENANT_ONE, DMN_FILE_SECOND_VERSION);

		IDmnDecisionTableResult decisionResult = decisionService.EvaluateDecisionTableByKey(DECISION_DEFINITION_KEY).SetVariables(createVariables()).SetDecisionDefinitionTenantId(TENANT_ONE).Evaluate();

		AssertThatDecisionHasResult(decisionResult, RESULT_OF_SECOND_VERSION);
	  }

	   [Test]   public virtual void testEvaluateDecisionByKeyVersionAndTenantId()
	  {
		DeploymentForTenant(TENANT_ONE, DMN_FILE);

		DeploymentForTenant(TENANT_TWO, DMN_FILE);
		DeploymentForTenant(TENANT_TWO, DMN_FILE_SECOND_VERSION);

		IDmnDecisionTableResult decisionResult = decisionService.EvaluateDecisionTableByKey(DECISION_DEFINITION_KEY).SetVariables(createVariables()).SetVersion(1).SetDecisionDefinitionTenantId(TENANT_TWO).Evaluate();

		AssertThatDecisionHasResult(decisionResult, RESULT_OF_FIRST_VERSION);
	  }

	   [Test]   public virtual void testEvaluateDecisionByKeyWithoutTenantIdNoAuthenticatedTenants()
	  {
		identityService.SetAuthentication("user", null, null);

		Deployment(DMN_FILE);

		IDmnDecisionTableResult decisionResult = decisionService.EvaluateDecisionTableByKey(DECISION_DEFINITION_KEY).DecisionDefinitionWithoutTenantId().SetVariables(createVariables()).Evaluate();

		AssertThatDecisionHasResult(decisionResult, RESULT_OF_FIRST_VERSION);
	  }

	   [Test]   public virtual void testFailToEvaluateDecisionByKeyNoAuthenticatedTenants()
	  {
		identityService.SetAuthentication("user", null, null);

		DeploymentForTenant(TENANT_ONE, DMN_FILE);

		try
		{
		  decisionService.EvaluateDecisionTableByKey(DECISION_DEFINITION_KEY).SetVariables(createVariables()).Evaluate();

		  Assert.Fail("expected exception");
		}
		catch (ProcessEngineException e)
		{
		  Assert.That(e.Message, Does.Contain("no decision definition deployed with key 'decision'"));
		}
	  }

	   [Test]   public virtual void testFailToEvaluateDecisionByKeyWithTenantIdNoAuthenticatedTenants()
	  {
		identityService.SetAuthentication("user", null, null);

		DeploymentForTenant(TENANT_ONE, DMN_FILE);

		try
		{
		  decisionService.EvaluateDecisionTableByKey(DECISION_DEFINITION_KEY).SetDecisionDefinitionTenantId(TENANT_ONE).SetVariables(createVariables()).Evaluate();

		  Assert.Fail("expected exception");
		}
		catch (ProcessEngineException e)
		{
		  Assert.That(e.Message, Does.Contain("Cannot evaluate the decision"));
		}
	  }

	   [Test]   public virtual void testFailToEvaluateDecisionByIdNoAuthenticatedTenants()
	  {
		DeploymentForTenant(TENANT_ONE, DMN_FILE);

		IDecisionDefinition decisionDefinition = repositoryService.CreateDecisionDefinitionQuery().First();

		identityService.SetAuthentication("user", null, null);

		try
		{
		  decisionService.EvaluateDecisionTableById(decisionDefinition.Id).SetVariables(createVariables()).Evaluate();

		  Assert.Fail("expected exception");
		}
		catch (ProcessEngineException e)
		{
		  Assert.That(e.Message, Does.Contain("Cannot evaluate the decision"));
		}
	  }

	   [Test]   public virtual void testEvaluateDecisionByKeyWithTenantIdAuthenticatedTenant()
	  {
		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		DeploymentForTenant(TENANT_ONE, DMN_FILE);
		DeploymentForTenant(TENANT_TWO, DMN_FILE);

		IDmnDecisionTableResult decisionResult = decisionService.EvaluateDecisionTableByKey(DECISION_DEFINITION_KEY).SetDecisionDefinitionTenantId(TENANT_ONE).SetVariables(createVariables()).Evaluate();

		AssertThatDecisionHasResult(decisionResult, RESULT_OF_FIRST_VERSION);
	  }

	   [Test]   public virtual void testEvaluateDecisionByIdAuthenticatedTenant()
	  {
		DeploymentForTenant(TENANT_ONE, DMN_FILE);

		IDecisionDefinition decisionDefinition = repositoryService.CreateDecisionDefinitionQuery().First();

		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		IDmnDecisionTableResult decisionResult = decisionService.EvaluateDecisionTableById(decisionDefinition.Id).SetVariables(createVariables()).Evaluate();

		AssertThatDecisionHasResult(decisionResult, RESULT_OF_FIRST_VERSION);
	  }

	   [Test]   public virtual void testEvaluateDecisionByKeyWithAuthenticatedTenant()
	  {
		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		DeploymentForTenant(TENANT_ONE, DMN_FILE);
		DeploymentForTenant(TENANT_TWO, DMN_FILE_SECOND_VERSION);

		IDmnDecisionTableResult decisionResult = decisionService.EvaluateDecisionTableByKey(DECISION_DEFINITION_KEY).SetVariables(createVariables()).Evaluate();

		AssertThatDecisionHasResult(decisionResult, RESULT_OF_FIRST_VERSION);
	  }

	   [Test]   public virtual void testEvaluateDecisionByKeyWithTenantIdDisabledTenantCheck()
	  {
		processEngineConfiguration.SetTenantCheckEnabled(false);
		identityService.SetAuthentication("user", null, null);

		DeploymentForTenant(TENANT_ONE, DMN_FILE);

		IDmnDecisionTableResult decisionResult = decisionService.EvaluateDecisionTableByKey(DECISION_DEFINITION_KEY).SetDecisionDefinitionTenantId(TENANT_ONE).SetVariables(createVariables()).Evaluate();

		AssertThatDecisionHasResult(decisionResult, RESULT_OF_FIRST_VERSION);
	  }

	  protected internal virtual IVariableMap createVariables()
	  {
		return ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables().PutValue("status", "silver").PutValue("sum", 723);
	  }

	  protected internal virtual void AssertThatDecisionHasResult(IDmnDecisionTableResult decisionResult, object expectedValue)
	  {
		Assert.That(decisionResult!=null);
		Assert.That(decisionResult.Count, Is.EqualTo(1));
		string value = decisionResult.GetSingleEntry<string>();//.SingleResult.FirstEntry;
		Assert.That(value, Is.EqualTo(expectedValue));
	  }

	}

}