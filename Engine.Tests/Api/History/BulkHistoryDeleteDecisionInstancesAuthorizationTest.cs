using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Api.Authorization.Util;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Variable;
using NUnit.Framework;

namespace Engine.Tests.Api.History
{
    /// <summary>
    /// </summary>
    //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
    //ORIGINAL LINE: @RunWith(Parameterized.class) 
    [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull)]
    [TestFixture]
    public class BulkHistoryDeleteDecisionInstancesAuthorizationTest
    {
        public const string DECISION = "decision";
        public AuthorizationTestRule authRule;
        private IDecisionService decisionService;

        public ProcessEngineRule engineRule = new ProvidedProcessEngineRule();

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain chain = org.junit.Rules.RuleChain.outerRule(engineRule).around(authRule).around(testHelper);
        //public RuleChain chain;

        private IHistoryService historyService;
        private readonly bool InstanceFieldsInitialized;

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameterized.Parameter public api.authorization.util.AuthorizationScenario scenario;
        public static AuthorizationScenario scenario;
        public ProcessEngineTestRule testHelper;

        public BulkHistoryDeleteDecisionInstancesAuthorizationTest()
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
            //chain = RuleChain.outerRule(engineRule).around(authRule).around(testHelper);
        }
        [SetUp]
        public virtual void init()
        {
            historyService = engineRule.HistoryService;
            decisionService = engineRule.DecisionService;

            authRule.CreateUserAndGroup("demo", "groupId");
        }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameterized.Parameters(name = "Scenario {index}") public static java.util.Collection<api.authorization.util.AuthorizationScenario[]> scenarios()
        public static ICollection<AuthorizationScenario[]> scenarios()
        {
            return AuthorizationTestRule.AsParameters(scenario
                    .FailsDueToRequired(AuthorizationSpec.Grant(Resources.DecisionDefinition, "*", "demo",
                        Permissions.DeleteHistory)),
                scenario
                    .WithAuthorizations(AuthorizationSpec.Grant(Resources.DecisionDefinition, "someId", "demo",
                        Permissions.DeleteHistory))
                    .FailsDueToRequired(AuthorizationSpec.Grant(Resources.DecisionDefinition, "*", "demo",
                        Permissions.DeleteHistory)),
                scenario
                    .WithAuthorizations(AuthorizationSpec.Grant(Resources.DecisionDefinition, "*", "demo",
                        Permissions.DeleteHistory))
                    .Succeeds());
        }

        [TearDown]
        public virtual void tearDown()
        {
            authRule.DeleteUsersAndGroups();
        }

        [Test][Deployment(  "resources/api/dmn/Example.Dmn") ]
        public virtual void testCleanupHistory()
        {
            //given
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<String> ids = prepareHistoricDecisions();
            var ids = prepareHistoricDecisions();

            // when
            authRule.Init(scenario)
                .WithUser("demo")
                .Start();

            //historyService.DeleteHistoricDecisionInstancesBulk(ids);

            //then
            if (authRule.AssertScenario(scenario))
                Assert.AreEqual(0, historyService.CreateHistoricDecisionInstanceQuery(c=>c.DecisionDefinitionKey== DECISION)
                    .Count());
        }

        private IList<string> prepareHistoricDecisions()
        {
            for (var i = 0; i < 5; i++)
                decisionService.EvaluateDecisionByKey(DECISION)
                    .Variables(createVariables())
                    .Evaluate();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<org.Camunda.bpm.Engine.history.IHistoricDecisionInstance> decisionInstances = historyService.CreateHistoricDecisionInstanceQuery().ToList();
            var decisionInstances = historyService.CreateHistoricDecisionInstanceQuery()
                
                .ToList();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<String> decisionInstanceIds = new java.util.ArrayList<String>();
            IList<string> decisionInstanceIds = new List<string>();
            foreach (var decisionInstance in decisionInstances)
                decisionInstanceIds.Add(decisionInstance.Id);
            return decisionInstanceIds;
        }

        protected internal virtual IVariableMap createVariables()
        {
            return ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                .PutValue("status", "silver")
                .PutValue("sum", 723);
        }
    }
}