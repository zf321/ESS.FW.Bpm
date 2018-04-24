using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Api.Authorization.Util;
using Engine.Tests.Api.Runtime.Migration;
using Engine.Tests.Api.Runtime.Migration.Models;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Migration;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.Api.Authorization.Migration
{


    /// <summary>
    /// 
    /// 
    /// </summary>
    //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
    //ORIGINAL LINE: @RunWith(Parameterized.class) public class MigrateProcessInstanceSyncTest
    public class MigrateProcessInstanceSyncTest
    {
        private bool InstanceFieldsInitialized = false;

        public MigrateProcessInstanceSyncTest()
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
            return AuthorizationTestRule.AsParameters(AuthorizationScenario.Scenario()
                .WithAuthorizations(AuthorizationSpec.Grant(Resources.ProcessInstance, "processInstance", "userId", Permissions.Read))
                .FailsDueToRequired(AuthorizationSpec.Grant(Resources.ProcessDefinition, "sourceDefinitionKey", "userId", Permissions.MigrateInstance),
                AuthorizationSpec.Grant(Resources.ProcessDefinition, "targetDefinitionKey", "userId", Permissions.MigrateInstance)),
                AuthorizationScenario.Scenario().WithAuthorizations(AuthorizationSpec.Grant(Resources.ProcessInstance, "processInstance", "userId", Permissions.Read),
                AuthorizationSpec.Grant(Resources.ProcessDefinition, "sourceDefinitionKey", "userId", Permissions.MigrateInstance))
                .FailsDueToRequired(AuthorizationSpec.Grant(Resources.ProcessDefinition, "sourceDefinitionKey", "userId", Permissions.MigrateInstance),
                AuthorizationSpec.Grant(Resources.ProcessDefinition, "targetDefinitionKey", "userId", Permissions.MigrateInstance)),
                AuthorizationScenario.Scenario().WithAuthorizations(AuthorizationSpec.Grant(Resources.ProcessInstance, "processInstance", "userId", Permissions.Read),
                AuthorizationSpec.Grant(Resources.ProcessDefinition, "targetDefinitionKey", "userId", Permissions.MigrateInstance))
                .FailsDueToRequired(AuthorizationSpec.Grant(Resources.ProcessDefinition, "sourceDefinitionKey", "userId", Permissions.MigrateInstance),
                AuthorizationSpec.Grant(Resources.ProcessDefinition, "targetDefinitionKey", "userId", Permissions.MigrateInstance)), AuthorizationScenario.Scenario()
                .WithAuthorizations(AuthorizationSpec.Grant(Resources.ProcessInstance, "processInstance", "userId", Permissions.Read),
                AuthorizationSpec.Grant(Resources.ProcessDefinition, "sourceDefinitionKey", "userId", Permissions.MigrateInstance),
                AuthorizationSpec.Grant(Resources.ProcessDefinition, "targetDefinitionKey", "userId", Permissions.MigrateInstance)).Succeeds(),
                AuthorizationScenario.Scenario().WithAuthorizations(AuthorizationSpec.Grant(Resources.ProcessInstance, "processInstance", "userId", Permissions.Read),
                AuthorizationSpec.Grant(Resources.ProcessDefinition, "*", "userId", Permissions.MigrateInstance)).Succeeds());
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
        public virtual void testMigrate()
        {

            // given
            IProcessDefinition sourceDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            IProcessDefinition targetDefinition = testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.OneTaskProcess).ChangeElementId(ProcessModels.ProcessKey, "new" + ProcessModels.ProcessKey));

            IProcessInstance processInstance = engineRule.RuntimeService.StartProcessInstanceById(sourceDefinition.Id);

            IMigrationPlan migrationPlan = engineRule.RuntimeService.CreateMigrationPlan(sourceDefinition.Id, targetDefinition.Id).MapEqualActivities().Build();

            // when
            authRule.Init(scenario).WithUser("userId").BindResource("sourceDefinitionKey", sourceDefinition.Key).BindResource("targetDefinitionKey", targetDefinition.Key).BindResource("processInstance", processInstance.Id).Start();

            engineRule.RuntimeService.NewMigration(migrationPlan).ProcessInstanceIds((processInstance.Id)).Execute();

            // then
            if (authRule.AssertScenario(scenario))
            {
                IProcessInstance processInstanceAfterMigration = engineRule.RuntimeService.CreateProcessInstanceQuery().First();

                Assert.AreEqual(targetDefinition.Id, processInstanceAfterMigration.ProcessDefinitionId);
            }
        }
        
        [Test]
        public virtual void testMigrateWithQuery()
        {
            // given
            IProcessDefinition sourceDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            IProcessDefinition targetDefinition = testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.OneTaskProcess).ChangeElementId(ProcessModels.ProcessKey, "new" + ProcessModels.ProcessKey));

            IProcessInstance processInstance = engineRule.RuntimeService.StartProcessInstanceById(sourceDefinition.Id);

            IMigrationPlan migrationPlan = engineRule.RuntimeService.CreateMigrationPlan(sourceDefinition.Id, targetDefinition.Id).MapEqualActivities().Build();

             IQueryable<IProcessInstance> query = engineRule.RuntimeService.CreateProcessInstanceQuery();

            // when
            authRule.Init(scenario).WithUser("userId").BindResource("sourceDefinitionKey", sourceDefinition.Key).BindResource("targetDefinitionKey", targetDefinition.Key).BindResource("processInstance", processInstance.Id).Start();

            engineRule.RuntimeService.NewMigration(migrationPlan).ProcessInstanceQuery(query).Execute();

            // then
            if (authRule.AssertScenario(scenario))
            {
                IProcessInstance processInstanceAfterMigration = engineRule.RuntimeService.CreateProcessInstanceQuery().First();

                Assert.AreEqual(targetDefinition.Id, processInstanceAfterMigration.ProcessDefinitionId);
            }
        }
    }

}