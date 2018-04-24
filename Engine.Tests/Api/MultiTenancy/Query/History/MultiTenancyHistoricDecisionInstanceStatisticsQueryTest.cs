

//using System.Collections.Generic;
//using System.Linq;
//using ESS.FW.Bpm.Engine.History;
//using ESS.FW.Bpm.Engine.Repository;
//using ESS.FW.Bpm.Engine.Tests.Util;
//using NUnit.Framework;

//namespace ESS.FW.Bpm.Engine.Tests.Api.MultiTenancy.Query.History
//{


//    [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull)]
//    public class MultiTenancyHistoricDecisionInstanceStatisticsQueryTest
//    {
//        private bool InstanceFieldsInitialized = false;

//        public MultiTenancyHistoricDecisionInstanceStatisticsQueryTest()
//        {
//            if (!InstanceFieldsInitialized)
//            {
//                InitializeInstanceFields();
//                InstanceFieldsInitialized = true;
//            }
//        }

//        private void InitializeInstanceFields()
//        {
//            testRule = new ProcessEngineTestRule(engineRule);
//            //ruleChain = RuleChain.outerRule(engineRule).around(testRule);
//        }


//        protected internal const string TENANT_ONE = "tenant1";
//        protected internal const string DISH_DRG_DMN = "resources/dmn/deployment/drdDish.Dmn11.xml";

//        protected internal const string DISH_DECISION = "dish-decision";
//        protected internal const string TEMPERATURE = "temperature";
//        protected internal const string DAY_TYPE = "dayType";
//        protected internal const string WEEKEND = "Weekend";
//        protected internal const string USER_ID = "user";

//        protected internal IDecisionService decisionService;
//        protected internal IRepositoryService repositoryService;
//        protected internal IHistoryService historyService;
//        protected internal IIdentityService identityService;

//        public ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
//        public ProcessEngineTestRule testRule;

//        [SetUp]
//        public virtual void setUp()
//        {
//            decisionService = engineRule.DecisionService;
//            repositoryService = engineRule.RepositoryService;
//            historyService = engineRule.HistoryService;
//            identityService = engineRule.IdentityService;

//            testRule.DeployForTenant(TENANT_ONE, DISH_DRG_DMN);

//            decisionService.EvaluateDecisionByKey(DISH_DECISION).DecisionDefinitionTenantId(TENANT_ONE).Variables(Variable.Variables.CreateVariables().PutValue(TEMPERATURE, 21).PutValue(DAY_TYPE, WEEKEND)).Evaluate();

//        }

//        [Test]
//        public virtual void testQueryNoAuthenticatedTenants()
//        {
//            IDecisionRequirementsDefinition decisionRequirementsDefinition = repositoryService.CreateDecisionRequirementsDefinitionQuery(c=>c.TenantId == TENANT_ONE).First();

//            identityService.SetAuthentication(USER_ID, null, null);

//            IHistoricDecisionInstanceStatisticsQuery query = historyService.CreateHistoricDecisionInstanceStatisticsQuery(decisionRequirementsDefinition.Id);

//            Assert.That(query.Count(), Is.EqualTo(0L));
//        }

//        [Test]
//        public virtual void testQueryAuthenticatedTenant()
//        {
//            IDecisionRequirementsDefinition decisionRequirementsDefinition = repositoryService.CreateDecisionRequirementsDefinitionQuery(c=>c.TenantId == TENANT_ONE).First();

//            identityService.SetAuthentication(USER_ID, null, new List<string>() { TENANT_ONE });

//            IHistoricDecisionInstanceStatisticsQuery query = historyService.CreateHistoricDecisionInstanceStatisticsQuery(decisionRequirementsDefinition.Id);

//            Assert.That(query.Count(), Is.EqualTo(3L));
//        }

//        [Test]
//        public virtual void testQueryDisabledTenantCheck()
//        {
//            IDecisionRequirementsDefinition decisionRequirementsDefinition = repositoryService.CreateDecisionRequirementsDefinitionQuery(c=>c.TenantId == TENANT_ONE).First();

//            engineRule.ProcessEngineConfiguration.SetTenantCheckEnabled(false);
//            identityService.SetAuthentication(USER_ID, null, null);

//            IHistoricDecisionInstanceStatisticsQuery query = historyService.CreateHistoricDecisionInstanceStatisticsQuery(decisionRequirementsDefinition.Id);

//            Assert.That(query.Count(), Is.EqualTo(3L));
//        }

//    }

//}