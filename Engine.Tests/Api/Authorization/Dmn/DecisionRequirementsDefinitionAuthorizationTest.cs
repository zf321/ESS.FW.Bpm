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

    //JAVA TO C# CONVERTER TODO Resources.Task: This Java 'import static' statement cannot be converted to C#:
    //	import static org.camunda.bpm.Engine.authorization.Resources.Resources.DecisionRequirementsDefinition;
    //JAVA TO C# CONVERTER TODO Resources.Task: This Java 'import static' statement cannot be converted to C#:
    //	import static api.authorization.util.AuthorizationScenario.scenario;
    //JAVA TO C# CONVERTER TODO Resources.Task: This Java 'import static' statement cannot be converted to C#:
    //	import static api.authorization.util.AuthorizationSpec.grant;
    //JAVA TO C# CONVERTER TODO Resources.Task: This Java 'import static' statement cannot be converted to C#:
    //	import static org.junit.Assert.NotNull;


    //using IDecisionRequirementsDefinition = IDecisionRequirementsDefinition;
    //using AuthorizationScenario = AuthorizationScenario;
    //using AuthorizationTestRule = AuthorizationTestRule;
    //using ProvidedProcessEngineRule = ProvidedProcessEngineRule;
    //using RuleChain = org.junit.Rules.RuleChain;

    /// 
    /// <summary>
    /// 
    /// 
    /// </summary>

    //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
    //ORIGINAL LINE: @RunWith(Parameterized.class) public class DecisionRequirementsDefinitionAuthorizationTest
    public class DecisionRequirementsDefinitionAuthorizationTest
    {
        private bool InstanceFieldsInitialized = false;

        public DecisionRequirementsDefinitionAuthorizationTest()
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


        protected internal const string DMN_FILE = "resources/dmn/deployment/drdDish.Dmn11.xml";
        protected internal const string DRD_FILE = "resources/dmn/deployment/drdDish.png";

        protected internal const string DEFINITION_KEY = "dish";

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
        //ORIGINAL LINE: @Parameters(name = "Scenario {index}") public static java.util.Collection<api.authorization.util.AuthorizationScenario[]> scenarios()
        public static ICollection<AuthorizationScenario[]> scenarios()
        {
            return AuthorizationTestRule.AsParameters(AuthorizationScenario.Scenario().WithoutAuthorizations()
                .FailsDueToRequired(AuthorizationSpec.Grant(Resources.DecisionRequirementsDefinition, DEFINITION_KEY, "userId", Permissions.Read)),
                AuthorizationScenario.Scenario().WithAuthorizations(AuthorizationSpec.Grant(Resources.DecisionRequirementsDefinition, DEFINITION_KEY, "userId", Permissions.Read)).Succeeds(), AuthorizationScenario.Scenario().WithAuthorizations(AuthorizationSpec.Grant(Resources.DecisionRequirementsDefinition, "*", "userId", Permissions.Read)).Succeeds());
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
        [Deployment(new string[] { DMN_FILE })]
        public virtual void getDecisionRequirementsDefinition()
        {

            string decisionRequirementsDefinitionId = repositoryService.CreateDecisionRequirementsDefinitionQuery(c=>c.Key==DEFINITION_KEY).First().Id;

            // when
            authRule.Init(scenario).WithUser("userId").BindResource("decisionRequirementsDefinitionKey", DEFINITION_KEY).Start();

            IDecisionRequirementsDefinition decisionRequirementsDefinition = repositoryService.GetDecisionRequirementsDefinition(decisionRequirementsDefinitionId);

            if (authRule.AssertScenario(scenario))
            {
                Assert.NotNull(decisionRequirementsDefinition);
            }
        }

        [Test]
        [Deployment(new string[] { DMN_FILE })]
        public virtual void getDecisionRequirementsModel()
        {

            // given
            string decisionRequirementsDefinitionId = repositoryService.CreateDecisionRequirementsDefinitionQuery(c=>c.Key==DEFINITION_KEY).First().Id;

            // when
            authRule.Init(scenario).WithUser("userId").BindResource("decisionRequirementsDefinitionKey", DEFINITION_KEY).Start();

            System.IO.Stream decisionRequirementsModel = repositoryService.GetDecisionRequirementsModel(decisionRequirementsDefinitionId);

            if (authRule.AssertScenario(scenario))
            {
                Assert.NotNull(decisionRequirementsModel);
            }
        }

        [Test]
        [Deployment(new string[] { DMN_FILE, DRD_FILE })]
        public virtual void getDecisionRequirementsDiagram()
        {

            // given
            string decisionRequirementsDefinitionId = repositoryService.CreateDecisionRequirementsDefinitionQuery(c=>c.Key==DEFINITION_KEY).First().Id;

            // when
            authRule.Init(scenario).WithUser("userId").BindResource("decisionRequirementsDefinitionKey", DEFINITION_KEY).Start();

            System.IO.Stream decisionRequirementsDiagram = repositoryService.GetDecisionRequirementsDiagram(decisionRequirementsDefinitionId);

            if (authRule.AssertScenario(scenario))
            {
                Assert.NotNull(decisionRequirementsDiagram);
            }
        }
    }

}