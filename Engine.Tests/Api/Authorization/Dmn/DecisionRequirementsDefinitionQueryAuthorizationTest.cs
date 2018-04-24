using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Api.Authorization.Util;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Repository;
using NUnit.Framework;

namespace Engine.Tests.Api.Authorization.Dmn
{



    //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
    //ORIGINAL LINE: @RunWith(Parameterized.class) public class DecisionRequirementsDefinitionQueryAuthorizationTest
    public class DecisionRequirementsDefinitionQueryAuthorizationTest
    {
        private bool InstanceFieldsInitialized = false;

        public DecisionRequirementsDefinitionQueryAuthorizationTest()
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


        protected internal const string DMN_FILE = "resources/dmn/deployment/drdScore.Dmn11.xml";
        protected internal const string ANOTHER_DMN = "resources/dmn/deployment/drdDish.Dmn11.xml";

        protected internal const string DEFINITION_KEY = "score";
        protected internal const string ANOTHER_DEFINITION_KEY = "dish";

        public ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
        public AuthorizationTestRule authRule;

        protected internal IRepositoryService repositoryService;

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain chain = org.junit.Rules.RuleChain.outerRule(engineRule).around(authRule);
        //public RuleChain chain;

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Parameter(0) public api.authorization.util.AuthorizationScenario scenario;
        public AuthorizationScenario scenario;

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Parameter(1) public String[] expectedDefinitionKeys;
        public string[] expectedDefinitionKeys;

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Parameters(name = "scenario {index}") public static java.util.Collection<Object[]> scenarios()
        public static ICollection<object[]> scenarios()
        {
            return (new object[][]
            {
            new object[] {AuthorizationScenario.Scenario().WithoutAuthorizations().Succeeds(), expectedDefinitions()},
            new object[] {AuthorizationScenario.Scenario().WithAuthorizations(AuthorizationSpec.Grant(Resources.DecisionRequirementsDefinition, DEFINITION_KEY, "userId", Permissions.Read)).Succeeds(), expectedDefinitions(DEFINITION_KEY)},
            new object[] {AuthorizationScenario.Scenario().WithAuthorizations(AuthorizationSpec.Grant(Resources.DecisionRequirementsDefinition, AuthorizationFields.Any, "userId", Permissions.Read)).Succeeds(), expectedDefinitions(DEFINITION_KEY, ANOTHER_DEFINITION_KEY)},
            new object[] {AuthorizationScenario.Scenario().WithAuthorizations(AuthorizationSpec.Grant(Resources.DecisionRequirementsDefinition, DEFINITION_KEY, "userId", Permissions.Read), AuthorizationSpec.Grant(Resources.DecisionRequirementsDefinition, AuthorizationFields.Any, "userId", Permissions.Read)).Succeeds(), expectedDefinitions(DEFINITION_KEY, ANOTHER_DEFINITION_KEY)}
            });
        }

        [SetUp]
        public virtual void setUp()
        {
            authRule.CreateUserAndGroup("userId", "groupId");
            repositoryService = engineRule.RepositoryService;
        }

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @After public void tearDown()
        public virtual void tearDown()
        {
            authRule.DeleteUsersAndGroups();
        }

        [Test]
        [Deployment(new string[] { DMN_FILE, ANOTHER_DMN })]
        public virtual void queryDecisionRequirementsDefinitions()
        {

            // when
            authRule.Init(scenario).WithUser("userId").BindResource("decisionRequirementsDefinitionKey", DEFINITION_KEY).Start();

            IQueryable<IDecisionRequirementsDefinition> query = engineRule.RepositoryService.CreateDecisionRequirementsDefinitionQuery();
            long Count = query.Count();

            // then
            if (authRule.AssertScenario(scenario))
            {
                Assert.That(Count, Is.EqualTo((long)expectedDefinitionKeys.Length));

                IList<string> definitionKeys = getDefinitionKeys(query.ToList());
                Assert.That(definitionKeys, Has.Member(expectedDefinitionKeys));
            }
        }

        protected internal virtual IList<string> getDefinitionKeys(IList<IDecisionRequirementsDefinition> definitions)
        {
            IList<string> definitionKeys = new List<string>();
            foreach (IDecisionRequirementsDefinition definition in definitions)
            {
                definitionKeys.Add(definition.Key);
            }
            return definitionKeys;
        }

        protected internal static string[] expectedDefinitions(params string[] keys)
        {
            return keys;
        }

    }

}