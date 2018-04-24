using System.Collections.Generic;
using Engine.Tests.Api.Authorization.Util;
using Engine.Tests.Api.Runtime.Migration;
using Engine.Tests.Api.Runtime.Migration.Models;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Migration;
using ESS.FW.Bpm.Engine.Repository;
using NUnit.Framework;

namespace Engine.Tests.Api.Authorization.Migration
{


    /// <summary>
    /// 
    /// 
    /// </summary>
    //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
    //ORIGINAL LINE: @RunWith(Parameterized.class) public class CreateMigrationPlanAuthorizationTest
    public class CreateMigrationPlanAuthorizationTest
    {
        private bool InstanceFieldsInitialized = false;

        public CreateMigrationPlanAuthorizationTest()
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


        public ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
        public AuthorizationTestRule authRule;
        public ProcessEngineTestRule testHelper;

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain chain = org.junit.Rules.RuleChain.outerRule(engineRule).around(authRule).around(testHelper);
        //public RuleChain chain;

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Parameter public api.authorization.util.AuthorizationScenario scenario;
        public AuthorizationScenario scenario;

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Parameters(name = "Scenario {index}") public static java.util.Collection<api.authorization.util.AuthorizationScenario[]> scenarios()
        public static ICollection<AuthorizationScenario[]> scenarios()
        {
            return AuthorizationTestRule.AsParameters(AuthorizationScenario.Scenario().WithoutAuthorizations()
                .FailsDueToRequired(AuthorizationSpec.Grant(Resources.ProcessDefinition, "sourceDefinitionKey", "userId", Permissions.Read)), 
                AuthorizationScenario.Scenario().WithAuthorizations(AuthorizationSpec.Grant(Resources.ProcessDefinition, "sourceDefinitionKey", "userId", Permissions.Read))
                .FailsDueToRequired(AuthorizationSpec.Grant(Resources.ProcessDefinition, "targetDefinitionKey", "userId", Permissions.Read)), 
                AuthorizationScenario.Scenario().WithAuthorizations(AuthorizationSpec.Grant(Resources.ProcessDefinition, "targetDefinitionKey", "userId", Permissions.Read))
                .FailsDueToRequired(AuthorizationSpec.Grant(Resources.ProcessDefinition, "sourceDefinitionKey", "userId", Permissions.Read)), 
                AuthorizationScenario.Scenario().WithAuthorizations(AuthorizationSpec.Grant(Resources.ProcessDefinition, "sourceDefinitionKey", "userId", Permissions.Read), 
                AuthorizationSpec.Grant(Resources.ProcessDefinition, "targetDefinitionKey", "userId", Permissions.Read)).Succeeds(), 
                AuthorizationScenario.Scenario().WithAuthorizations(AuthorizationSpec.Grant(Resources.ProcessDefinition, "*", "userId", Permissions.Read)).Succeeds());
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
        [Deployment(new string[] { "resources/api/authorization/oneIncidentProcess.bpmn20.xml" })]
        public virtual void testCreate()
        {

            // given
            IProcessDefinition sourceDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            IProcessDefinition targetDefinition = testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.OneTaskProcess).ChangeElementId(ProcessModels.ProcessKey, "new" + ProcessModels.ProcessKey));

            // when
            authRule.Init(scenario).WithUser("userId").BindResource("sourceDefinitionKey", sourceDefinition.Key).BindResource("targetDefinitionKey", targetDefinition.Key).Start();

            IMigrationPlan migrationPlan = engineRule.RuntimeService.CreateMigrationPlan(sourceDefinition.Id, targetDefinition.Id).MapEqualActivities().Build();

            // then
            if (authRule.AssertScenario(scenario))
            {
                Assert.NotNull(migrationPlan);
            }

        }
    }

}