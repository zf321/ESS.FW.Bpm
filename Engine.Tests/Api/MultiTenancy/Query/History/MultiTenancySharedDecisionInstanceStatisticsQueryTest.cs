

//using System.Collections.Generic;
//using System.Linq;
//using ESS.FW.Bpm.Engine.History;
//using ESS.FW.Bpm.Engine.Impl.Cfg;
//using ESS.FW.Bpm.Engine.Repository;
//using ESS.FW.Bpm.Engine.Tests.Util;
//using NUnit.Framework;
//using ProcessEngineBootstrapRule = ESS.FW.Bpm.Engine.Tests.Util.ProcessEngineBootstrapRule;

//namespace ESS.FW.Bpm.Engine.Tests.Api.MultiTenancy.Query.History
//{


//    /// <summary>
//    /// 
//    /// </summary>
//    [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull)]
//    public class MultiTenancySharedDecisionInstanceStatisticsQueryTest
//	{
//		private bool InstanceFieldsInitialized = false;

//		public MultiTenancySharedDecisionInstanceStatisticsQueryTest()
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
//			//tenantRuleChain = RuleChain.outerRule(engineRule).around(testRule);
//		}


//	  protected internal const string TENANT_ONE = "tenant1";
//	  protected internal const string DISH_DRG_DMN = "resources/dmn/deployment/drdDish.Dmn11.xml";

//	  protected internal const string DISH_DECISION = "dish-decision";
//	  protected internal const string TEMPERATURE = "temperature";
//	  protected internal const string DAY_TYPE = "dayType";
//	  protected internal const string WEEKEND = "Weekend";
//	  protected internal const string USER_ID = "user";

//	  protected internal IDecisionService decisionService;
//	  protected internal IRepositoryService repositoryService;
//	  protected internal IHistoryService historyService;
//	  protected internal IIdentityService identityService;

//	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule(bootstrapRule);
//	  protected internal ProcessEngineTestRule testRule;

//	  protected internal static StaticTenantIdTestProvider tenantIdProvider;

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @ClassRule public static util.ProcessEngineBootstrapRule bootstrapRule = new util.ProcessEngineBootstrapRule()
//	  public static ProcessEngineBootstrapRule bootstrapRule = new ProcessEngineBootstrapRuleAnonymousInnerClass();

//	  private class ProcessEngineBootstrapRuleAnonymousInnerClass : ProcessEngineBootstrapRule
//	  {
//		  public ProcessEngineBootstrapRuleAnonymousInnerClass()
//		  {
//		  }

//		  public override ProcessEngineConfiguration ConfigureEngine(ProcessEngineConfigurationImpl configuration)
//		  {

//		  tenantIdProvider = new StaticTenantIdTestProvider(TENANT_ONE);
//		  configuration.SetTenantIdProvider(tenantIdProvider);

//		  return configuration;
//		  }
//	  }
        

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @Before public void setUp()
//    [SetUp]
//	  public virtual void setUp()
//	  {
//		decisionService = engineRule.DecisionService;
//		repositoryService = engineRule.RepositoryService;
//		historyService = engineRule.HistoryService;
//		identityService = engineRule.IdentityService;

//		testRule.Deploy(DISH_DRG_DMN);

//		decisionService.EvaluateDecisionByKey(DISH_DECISION).Variables(Variable.Variables.CreateVariables().PutValue(TEMPERATURE, 21).PutValue(DAY_TYPE, WEEKEND)).Evaluate();
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @Test public void testQueryNoAuthenticatedTenants()
//	   [Test]   public virtual void testQueryNoAuthenticatedTenants()
//	  {
//		IDecisionRequirementsDefinition decisionRequirementsDefinition = repositoryService.CreateDecisionRequirementsDefinitionQuery().First();

//		identityService.SetAuthentication(USER_ID, null, null);

//		IHistoricDecisionInstanceStatisticsQuery query = historyService.CreateHistoricDecisionInstanceStatisticsQuery(decisionRequirementsDefinition.Id);

//		Assert.That(query.Count(), Is.EqualTo(0L));
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @Test public void testQueryAuthenticatedTenant()
//	   [Test]   public virtual void testQueryAuthenticatedTenant()
//	  {
//		IDecisionRequirementsDefinition decisionRequirementsDefinition = repositoryService.CreateDecisionRequirementsDefinitionQuery().First();

//		identityService.SetAuthentication(USER_ID, null,new List<string>(){TENANT_ONE});

//		IHistoricDecisionInstanceStatisticsQuery query = historyService.CreateHistoricDecisionInstanceStatisticsQuery(decisionRequirementsDefinition.Id);

//		Assert.That(query.Count(), Is.EqualTo(3L));
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @Test public void testQueryDisabledTenantCheck()
//	   [Test]   public virtual void testQueryDisabledTenantCheck()
//	  {
//		IDecisionRequirementsDefinition decisionRequirementsDefinition = repositoryService.CreateDecisionRequirementsDefinitionQuery().First();

//		engineRule.ProcessEngineConfiguration.SetTenantCheckEnabled(false);
//		identityService.SetAuthentication(USER_ID, null, null);

//		IHistoricDecisionInstanceStatisticsQuery query = historyService.CreateHistoricDecisionInstanceStatisticsQuery(decisionRequirementsDefinition.Id);

//		Assert.That(query.Count(), Is.EqualTo(3L));
//	  }
//	}

//}