using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Variable;
using ESS.FW.Bpm.Engine.Variable.Value;
using NUnit.Framework;

namespace Engine.Tests.Api.MultiTenancy.TenantCheck
{


    /// <summary>
    /// 
    /// </summary>
    [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull)]
    public class MultiTenancyCaseInstanceCmdsTenantCheckTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyCaseInstanceCmdsTenantCheckTest()
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


	  protected internal const string VARIABLE_NAME = "myVar";
	  protected internal const string VARIABLE_VALUE = "myValue";

	  protected internal const string TENANT_ONE = "tenant1";

	  protected internal const string CMMN_MODEL = "resources/api/cmmn/twoTaskCase.cmmn";

	  protected internal const string ACTIVITY_ID = "PI_HumanTask_1";

	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();

	  protected internal ProcessEngineTestRule testRule;

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(engineRule).around(testRule);
	  //public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.Rules.ExpectedException //thrown= org.junit.Rules.ExpectedException.None();
	  //public ExpectedException //thrown = ExpectedException.None();

	  protected internal IIdentityService identityService;
	  protected internal ICaseService caseService;
	  protected internal IHistoryService historyService;
	  protected internal ProcessEngineConfiguration processEngineConfiguration;

	  protected internal string caseInstanceId;
	  protected internal string caseExecutionId;

      [SetUp]
	  public virtual void setUp()
	  {
		processEngineConfiguration = engineRule.ProcessEngineConfiguration;
		identityService = engineRule.IdentityService;
		caseService = engineRule.CaseService;
		historyService = engineRule.HistoryService;

		testRule.DeployForTenant(TENANT_ONE, CMMN_MODEL);

		caseInstanceId = createCaseInstance(null);

		caseExecutionId = CaseExecution.Id;
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void manuallyStartCaseExecutionNoAuthenticatedTenants()
	  public virtual void manuallyStartCaseExecutionNoAuthenticatedTenants()
	  {
		identityService.SetAuthentication("user", null, null);

		////thrown.Expect(typeof(ProcessEngineException));
		////thrown.ExpectMessage("Cannot update the case execution");

		caseService.ManuallyStartCaseExecution(caseExecutionId);
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void manuallyStartCaseExecutionWithAuthenticatedTenant()
	  public virtual void manuallyStartCaseExecutionWithAuthenticatedTenant()
	  {
		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		caseService.ManuallyStartCaseExecution(caseExecutionId);

		identityService.ClearAuthentication();

		ICaseExecution caseExecution = CaseExecution;

		Assert.That(caseExecution.Active, Is.EqualTo(true));
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void manuallyStartCaseExecutionDisabledTenantCheck()
	  public virtual void manuallyStartCaseExecutionDisabledTenantCheck()
	  {
		identityService.SetAuthentication("user", null, null);
		processEngineConfiguration.SetTenantCheckEnabled(false);

		caseService.ManuallyStartCaseExecution(caseExecutionId);

		identityService.ClearAuthentication();

		ICaseExecution caseExecution = CaseExecution;

		Assert.That(caseExecution.Active, Is.EqualTo(true));
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void disableCaseExecutionNoAuthenticatedTenants()
	  public virtual void disableCaseExecutionNoAuthenticatedTenants()
	  {
		identityService.SetAuthentication("user", null, null);

		////thrown.Expect(typeof(ProcessEngineException));
		////thrown.ExpectMessage("Cannot update the case execution");

		caseService.DisableCaseExecution(caseExecutionId);
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void disableCaseExecutionWithAuthenticatedTenant()
	  public virtual void disableCaseExecutionWithAuthenticatedTenant()
	  {
		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		caseService.DisableCaseExecution(caseExecutionId);

		identityService.ClearAuthentication();

		IHistoricCaseActivityInstance historicCaseActivityInstance = HistoricCaseActivityInstance;

		Assert.That(historicCaseActivityInstance!=null);
		Assert.That(historicCaseActivityInstance.Disabled, Is.EqualTo(true));
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void disableCaseExecutionDisabledTenantCheck()
	  public virtual void disableCaseExecutionDisabledTenantCheck()
	  {
		identityService.SetAuthentication("user", null, null);
		processEngineConfiguration.SetTenantCheckEnabled(false);

		caseService.DisableCaseExecution(caseExecutionId);

		identityService.ClearAuthentication();

		IHistoricCaseActivityInstance historicCaseActivityInstance = HistoricCaseActivityInstance;

		Assert.That(historicCaseActivityInstance!=null);
		Assert.That(historicCaseActivityInstance.Disabled, Is.EqualTo(true));
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void reenableCaseExecutionNoAuthenticatedTenants()
	  public virtual void reenableCaseExecutionNoAuthenticatedTenants()
	  {
		caseService.DisableCaseExecution(caseExecutionId);

		identityService.SetAuthentication("user", null, null);

		////thrown.Expect(typeof(ProcessEngineException));
		////thrown.ExpectMessage("Cannot update the case execution");

		caseService.ReenableCaseExecution(caseExecutionId);
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void reenableCaseExecutionWithAuthenticatedTenant()
	  public virtual void reenableCaseExecutionWithAuthenticatedTenant()
	  {
		caseService.DisableCaseExecution(caseExecutionId);

		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		caseService.ReenableCaseExecution(caseExecutionId);

		identityService.ClearAuthentication();

		ICaseExecution caseExecution = CaseExecution;

		Assert.That(caseExecution.Enabled, Is.EqualTo(true));
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void reenableCaseExecutionDisabledTenantCheck()
	  public virtual void reenableCaseExecutionDisabledTenantCheck()
	  {
		caseService.DisableCaseExecution(caseExecutionId);

		identityService.SetAuthentication("user", null, null);
		processEngineConfiguration.SetTenantCheckEnabled(false);

		caseService.ReenableCaseExecution(caseExecutionId);

		identityService.ClearAuthentication();

		ICaseExecution caseExecution = CaseExecution;

		Assert.That(caseExecution.Enabled, Is.EqualTo(true));
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void completeCaseExecutionNoAuthenticatedTenants()
	  public virtual void completeCaseExecutionNoAuthenticatedTenants()
	  {
		caseService.ManuallyStartCaseExecution(caseExecutionId);

		identityService.SetAuthentication("user", null, null);

		////thrown.Expect(typeof(ProcessEngineException));
		////thrown.ExpectMessage("Cannot update the case execution");

		caseService.CompleteCaseExecution(caseExecutionId);
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void completeCaseExecutionWithAuthenticatedTenant()
	  public virtual void completeCaseExecutionWithAuthenticatedTenant()
	  {
		caseService.ManuallyStartCaseExecution(caseExecutionId);

		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		caseService.CompleteCaseExecution(caseExecutionId);

		identityService.ClearAuthentication();

		IHistoricCaseActivityInstance historicCaseActivityInstance = HistoricCaseActivityInstance;

		//Assert.That(historicCaseActivityInstance!=PrimitiveValueTypeImpl.NullTypeImpl);
		Assert.That(historicCaseActivityInstance.Completed, Is.EqualTo(true));
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void completeCaseExecutionDisabledTenantCheck()
	  public virtual void completeCaseExecutionDisabledTenantCheck()
	  {
		caseService.ManuallyStartCaseExecution(caseExecutionId);

		identityService.SetAuthentication("user", null, null);
		processEngineConfiguration.SetTenantCheckEnabled(false);

		caseService.CompleteCaseExecution(caseExecutionId);

		identityService.ClearAuthentication();

		IHistoricCaseActivityInstance historicCaseActivityInstance = HistoricCaseActivityInstance;

		Assert.That(historicCaseActivityInstance!=null);
		Assert.That(historicCaseActivityInstance.Completed, Is.EqualTo(true));
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void closeCaseInstanceNoAuthenticatedTenants()
	  public virtual void closeCaseInstanceNoAuthenticatedTenants()
	  {
		caseService.CompleteCaseExecution(caseInstanceId);

		identityService.SetAuthentication("user", null, null);

		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot update the case execution");

		caseService.CloseCaseInstance(caseInstanceId);
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void closeCaseInstanceWithAuthenticatedTenant()
	  public virtual void closeCaseInstanceWithAuthenticatedTenant()
	  {
		caseService.CompleteCaseExecution(caseInstanceId);

		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		caseService.CloseCaseInstance(caseInstanceId);

		identityService.ClearAuthentication();

		IHistoricCaseInstance historicCaseInstance = IHistoricCaseInstance;

		Assert.That(historicCaseInstance!=null);
		Assert.That(historicCaseInstance.Closed, Is.EqualTo(true));
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void closeCaseInstanceDisabledTenantCheck()
	  public virtual void closeCaseInstanceDisabledTenantCheck()
	  {
		caseService.CompleteCaseExecution(caseInstanceId);

		identityService.SetAuthentication("user", null, null);
		processEngineConfiguration.SetTenantCheckEnabled(false);

		caseService.CloseCaseInstance(caseInstanceId);

		identityService.ClearAuthentication();

		IHistoricCaseInstance historicCaseInstance = IHistoricCaseInstance;

		Assert.That(historicCaseInstance!=null);
		Assert.That(historicCaseInstance.Closed, Is.EqualTo(true));
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void terminateCaseInstanceNoAuthenticatedTenants()
	  public virtual void terminateCaseInstanceNoAuthenticatedTenants()
	  {

		identityService.SetAuthentication("user", null, null);

		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot update the case execution");

		caseService.TerminateCaseExecution(caseInstanceId);
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void terminateCaseExecutionWithAuthenticatedTenant()
	  public virtual void terminateCaseExecutionWithAuthenticatedTenant()
	  {
		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		caseService.TerminateCaseExecution(caseInstanceId);

		IHistoricCaseInstance historicCaseInstance = IHistoricCaseInstance;

		Assert.That(historicCaseInstance!=null);
		Assert.That(historicCaseInstance.Terminated, Is.EqualTo(true));

	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void terminateCaseExecutionDisabledTenantCheck()
	  public virtual void terminateCaseExecutionDisabledTenantCheck()
	  {

		identityService.SetAuthentication("user", null, null);
		processEngineConfiguration.SetTenantCheckEnabled(false);

		caseService.TerminateCaseExecution(caseInstanceId);

		IHistoricCaseInstance historicCaseInstance = IHistoricCaseInstance;

		Assert.That(historicCaseInstance!=null);
		Assert.That(historicCaseInstance.Terminated, Is.EqualTo(true));
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getVariablesNoAuthenticatedTenants()
	  public virtual void getVariablesNoAuthenticatedTenants()
	  {
		identityService.SetAuthentication("user", null, null);

		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot get the case execution");

		caseService.GetVariables(caseExecutionId);
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getVariablesWithAuthenticatedTenant()
	  public virtual void getVariablesWithAuthenticatedTenant()
	  {
		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		IDictionary<string, object> variables = caseService.GetVariables(caseExecutionId);

		Assert.That(variables!=null);
		Assert.Contains(variables.Keys, new List<string>{VARIABLE_NAME});
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getVariablesDisabledTenantCheck()
	  public virtual void getVariablesDisabledTenantCheck()
	  {
		identityService.SetAuthentication("user", null, null);
		processEngineConfiguration.SetTenantCheckEnabled(false);

		IDictionary<string, object> variables = caseService.GetVariables(caseExecutionId);

		Assert.That(variables!=null);
            Assert.Contains(variables.Keys, new List<string> { VARIABLE_NAME });
        }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getVariableNoAuthenticatedTenants()
	  public virtual void getVariableNoAuthenticatedTenants()
	  {
		identityService.SetAuthentication("user", null, null);

		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot get the case execution");

		caseService.GetVariable(caseExecutionId, VARIABLE_NAME);
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getVariableWithAuthenticatedTenant()
	  public virtual void getVariableWithAuthenticatedTenant()
	  {
		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		string variableValue = (string) caseService.GetVariable(caseExecutionId, VARIABLE_NAME);

		Assert.That(variableValue, Is.EqualTo(VARIABLE_VALUE));
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getVariableDisabledTenantCheck()
	  public virtual void getVariableDisabledTenantCheck()
	  {
		identityService.SetAuthentication("user", null, null);
		processEngineConfiguration.SetTenantCheckEnabled(false);

		string variableValue = (string) caseService.GetVariable(caseExecutionId, VARIABLE_NAME);

		Assert.That(variableValue, Is.EqualTo(VARIABLE_VALUE));
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getVariableTypedNoAuthenticatedTenants()
	  public virtual void getVariableTypedNoAuthenticatedTenants()
	  {
		identityService.SetAuthentication("user", null, null);

		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot get the case execution");

		caseService.GetVariableTyped<IStringValue>(caseExecutionId, VARIABLE_NAME);
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getVariableTypedWithAuthenticatedTenant()
	  public virtual void getVariableTypedWithAuthenticatedTenant()
	  {
		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		IStringValue variable = caseService.GetVariableTyped<IStringValue>(caseExecutionId, VARIABLE_NAME);

		Assert.That(variable.Value, Is.EqualTo(VARIABLE_VALUE));
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getVariableTypedDisabledTenantCheck()
	  public virtual void getVariableTypedDisabledTenantCheck()
	  {
		identityService.SetAuthentication("user", null, null);
		processEngineConfiguration.SetTenantCheckEnabled(false);

		IStringValue variable = caseService.GetVariableTyped<IStringValue>(caseExecutionId, VARIABLE_NAME);

		Assert.That(variable.Value, Is.EqualTo(VARIABLE_VALUE));
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void removeVariablesNoAuthenticatedTenants()
	  public virtual void removeVariablesNoAuthenticatedTenants()
	  {
		identityService.SetAuthentication("user", null, null);

		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot update the case execution");

		caseService.RemoveVariable(caseExecutionId, VARIABLE_NAME);
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void removeVariablesWithAuthenticatedTenant()
	  public virtual void removeVariablesWithAuthenticatedTenant()
	  {
		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		caseService.RemoveVariable(caseExecutionId, VARIABLE_NAME);

		identityService.ClearAuthentication();

		IDictionary<string, object> variables = caseService.GetVariables(caseExecutionId);
		Assert.That(variables.Count == 0, Is.EqualTo(true));
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void removeVariablesDisabledTenantCheck()
	  public virtual void removeVariablesDisabledTenantCheck()
	  {
		identityService.SetAuthentication("user", null, null);
		processEngineConfiguration.SetTenantCheckEnabled(false);

		caseService.RemoveVariable(caseExecutionId, VARIABLE_NAME);

		identityService.ClearAuthentication();

		IDictionary<string, object> variables = caseService.GetVariables(caseExecutionId);
		Assert.That(variables.Count == 0, Is.EqualTo(true));
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void setVariableNoAuthenticatedTenants()
	  public virtual void setVariableNoAuthenticatedTenants()
	  {
		identityService.SetAuthentication("user", null, null);

		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot update the case execution");

		caseService.SetVariable(caseExecutionId, "newVar", "newValue");
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void setVariableWithAuthenticatedTenant()
	  public virtual void setVariableWithAuthenticatedTenant()
	  {
		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		caseService.SetVariable(caseExecutionId, "newVar", "newValue");

		identityService.ClearAuthentication();

		IDictionary<string, object> variables = caseService.GetVariables(caseExecutionId);
		Assert.That(variables!=null);
            Assert.Contains(variables.Keys, new List<string> { VARIABLE_NAME, "newVar" });
        }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void setVariableDisabledTenantCheck()
	  public virtual void setVariableDisabledTenantCheck()
	  {
		identityService.SetAuthentication("user", null, null);
		processEngineConfiguration.SetTenantCheckEnabled(false);

		caseService.SetVariable(caseExecutionId, "newVar", "newValue");

		identityService.ClearAuthentication();

		IDictionary<string, object> variables = caseService.GetVariables(caseExecutionId);
		Assert.That(variables!=null);
            Assert.Contains(variables.Keys, new List<string> { VARIABLE_NAME, "newVar" });
        }

	  protected internal virtual string createCaseInstance(string tenantId)
	  {
		IVariableMap variables = ESS.FW.Bpm.Engine.Variable.Variables.PutValue(VARIABLE_NAME, VARIABLE_VALUE);
		ICaseInstanceBuilder builder = caseService.WithCaseDefinitionByKey("twoTaskCase").SetVariables(variables);
		if (string.ReferenceEquals(tenantId, null))
		{
		  return builder.Create().Id;
		}
		else
		{
		  return builder.CaseDefinitionTenantId(tenantId).Create().Id;
		}
	  }

	  protected internal virtual ICaseExecution CaseExecution
	  {
		  get
		  {
			return caseService.CreateCaseExecutionQuery(c=>c.ActivityId == ACTIVITY_ID).First();
		  }
	  }

	  protected internal virtual IHistoricCaseActivityInstance HistoricCaseActivityInstance
	  {
		  get
		  {
		      return historyService.CreateHistoricCaseActivityInstanceQuery()
		          .Where(o => o.CaseActivityId == ACTIVITY_ID).First();
		      //.First();
		  }
	  }

	  protected internal virtual IHistoricCaseInstance IHistoricCaseInstance
	  {
		  get
		  {
			return historyService.CreateHistoricCaseInstanceQuery().Where(o => o.SuperCaseInstanceId== caseInstanceId).First();
		  }
	  }

	}

}