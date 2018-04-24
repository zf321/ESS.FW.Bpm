using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Api.Authorization.Util;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Repository;
using NUnit.Framework;

namespace Engine.Tests.Api.Authorization.History
{
    //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
    //ORIGINAL LINE: @RunWith(Parameterized.class) public class HistoricDecisionInstanceStatisticsAuthorizationTest
    public class HistoricDecisionInstanceStatisticsAuthorizationTest
    {
        private bool InstanceFieldsInitialized = false;

        public HistoricDecisionInstanceStatisticsAuthorizationTest()
        {
            if (!InstanceFieldsInitialized)
            {
                InitializeInstanceFields();
                InstanceFieldsInitialized = true;
            }
        }

        private void InitializeInstanceFields()
        {
            authRule = new AuthorizationTestRule(engineRule);
            testHelper = new ProcessEngineTestRule(engineRule);
            //ruleChain = RuleChain.outerRule(engineRule).around(authRule).around(testHelper);
        }


        protected internal const string DISH_DRG_DMN = "resources/dmn/deployment/drdDish.Dmn11.xml";

        protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
        protected internal AuthorizationTestRule authRule;
        protected internal ProcessEngineTestRule testHelper;
        protected internal IDecisionService decisionService;
        protected internal IHistoryService historyService;
        protected internal IRepositoryService repositoryService;

        protected internal IDecisionRequirementsDefinition decisionRequirementsDefinition;

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(engineRule).around(authRule).around(testHelper);
        //public RuleChain ruleChain;

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Parameterized.Parameter public api.authorization.util.AuthorizationScenario scenario;
        public AuthorizationScenario scenario;

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Parameterized.Parameters(name = "Scenario {index}") public static java.util.Collection<api.authorization.util.AuthorizationScenario[]> scenarios()
        public static ICollection<AuthorizationScenario[]> scenarios()
        {
            return AuthorizationTestRule.AsParameters(AuthorizationScenario.Scenario()
                .WithoutAuthorizations().FailsDueToRequired(AuthorizationSpec.Grant(Resources.DecisionRequirementsDefinition, "dish", "userId", Permissions.Read)), AuthorizationScenario.Scenario().WithAuthorizations(AuthorizationSpec.Grant(Resources.DecisionRequirementsDefinition, "drd", "userId", Permissions.Read)).Succeeds());
        }

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Before public void setUp()
        public virtual void setUp()
        {
            testHelper.Deploy(DISH_DRG_DMN);
            decisionService = engineRule.DecisionService;
            historyService = engineRule.HistoryService;
            repositoryService = engineRule.RepositoryService;

            authRule.CreateUserAndGroup("userId", "groupId");

            decisionService.EvaluateDecisionTableByKey("dish-decision").SetVariables(ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables().PutValue("temperature", 21).PutValue("dayType", "Weekend")).Evaluate();

            decisionRequirementsDefinition = repositoryService.CreateDecisionRequirementsDefinitionQuery().First();
        }

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @After public void tearDown()
        public virtual void tearDown()
        {
            authRule.DeleteUsersAndGroups();
        }

        [Test]
        public virtual void testCreateStatistics()
        {
            //given
            authRule.Init(scenario).WithUser("userId").BindResource("drd", "*").Start();

            // when
            historyService.CreateHistoricDecisionInstanceStatisticsQuery(decisionRequirementsDefinition.Id)
                .ToList();

            // then
            authRule.AssertScenario(scenario);
        }

    }

}