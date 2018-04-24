using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Api.Authorization.Util;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Variable;
using NUnit.Framework;

namespace Engine.Tests.Api.Authorization.Dmn
{


    /// <summary>
    /// 
    /// </summary>
    //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
    //ORIGINAL LINE: @RunWith(Parameterized.class) public class EvaluateDecisionAuthorizationTest
    public class EvaluateDecisionAuthorizationTest
    {
        private bool InstanceFieldsInitialized = false;

        public EvaluateDecisionAuthorizationTest()
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
            //chain = RuleChain.outerRule(engineRule).around(authRule);
        }


        protected internal const string DMN_FILE = "resources/api/dmn/Example.Dmn";
        protected internal const string DECISION_DEFINITION_KEY = "decision";

        public ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
        public AuthorizationTestRule authRule;

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain chain = org.junit.Rules.RuleChain.outerRule(engineRule).around(authRule);
        //public RuleChain chain;

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Parameter public api.authorization.util.AuthorizationScenario scenario;
        public AuthorizationScenario scenario;

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Parameters(name = "scenario {index}") public static java.util.Collection<api.authorization.util.AuthorizationScenario[]> scenarios()
        public static ICollection<AuthorizationScenario[]> scenarios()
        {
            return AuthorizationTestRule.AsParameters(AuthorizationScenario.Scenario().WithoutAuthorizations().FailsDueToRequired(AuthorizationSpec.Grant(Resources.DecisionDefinition, DECISION_DEFINITION_KEY, "userId", Permissions.CreateInstance)), AuthorizationScenario.Scenario().WithAuthorizations(AuthorizationSpec.Grant(Resources.DecisionDefinition, DECISION_DEFINITION_KEY, "userId", Permissions.CreateInstance)).Succeeds(), AuthorizationScenario.Scenario().WithAuthorizations(AuthorizationSpec.Grant(Resources.DecisionDefinition, "*", "userId", Permissions.CreateInstance)).Succeeds());
        }

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Before public void setUp()
        public virtual void setUp()
        {
            authRule.CreateUserAndGroup("userId", "groupId");
        }

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @After public void tearDown()
        public virtual void tearDown()
        {
            authRule.DeleteUsersAndGroups();
        }

        [Test]
        [Deployment(new string[] { DMN_FILE })]
        public virtual void evaluateDecisionById()
        {

            // given
            IDecisionDefinition decisionDefinition = engineRule.RepositoryService.CreateDecisionDefinitionQuery().First();

            // when
            authRule.Init(scenario).WithUser("userId").BindResource("decisionDefinitionKey", DECISION_DEFINITION_KEY).Start();

            IDmnDecisionTableResult decisionResult = engineRule.DecisionService.EvaluateDecisionTableById(decisionDefinition.Id, createVariables());

            // then
            if (authRule.AssertScenario(scenario))
            {
                AssertThatDecisionHasExpectedResult(decisionResult);
            }
        }

        [Test]
        [Deployment(new string[] { DMN_FILE })]
        public virtual void evaluateDecisionByKey()
        {

            // given
            IDecisionDefinition decisionDefinition = engineRule.RepositoryService.CreateDecisionDefinitionQuery().First();

            // when
            authRule.Init(scenario).WithUser("userId").BindResource("decisionDefinitionKey", DECISION_DEFINITION_KEY).Start();

            IDmnDecisionTableResult decisionResult = engineRule.DecisionService.EvaluateDecisionTableByKey(decisionDefinition.Key, createVariables());

            // then
            if (authRule.AssertScenario(scenario))
            {
                AssertThatDecisionHasExpectedResult(decisionResult);
            }
        }

        [Test]
        [Deployment(new string[] { DMN_FILE })]
        public virtual void evaluateDecisionByKeyAndVersion()
        {

            // given
            IDecisionDefinition decisionDefinition = engineRule.RepositoryService.CreateDecisionDefinitionQuery().First();

            // when
            authRule.Init(scenario).WithUser("userId").BindResource("decisionDefinitionKey", DECISION_DEFINITION_KEY).Start();

            IDmnDecisionTableResult decisionResult = engineRule.DecisionService.EvaluateDecisionTableByKeyAndVersion(decisionDefinition.Key, decisionDefinition.Version, createVariables());

            // then
            if (authRule.AssertScenario(scenario))
            {
                AssertThatDecisionHasExpectedResult(decisionResult);
            }
        }

        protected internal virtual IVariableMap createVariables()
        {
            return ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables().PutValue("status", "silver").PutValue("sum", 723);
        }

        protected internal virtual void AssertThatDecisionHasExpectedResult(IDmnDecisionTableResult decisionResult)
        {
            Assert.That(decisionResult, Is.Not.Null);
            Assert.That(decisionResult.Count, Is.EqualTo(1));
            string value = decisionResult.GetSingleResult().First().Value.ToString();
            Assert.That(value, Is.EqualTo("ok"));
        }

    }

}