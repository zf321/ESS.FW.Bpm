using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Api.Authorization.Util;
using Engine.Tests.Api.Runtime.Migration;
using Engine.Tests.Api.Runtime.Migration.Models;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Batch;
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
    //ORIGINAL LINE: @RunWith(Parameterized.class) public class MigrateProcessInstanceAsyncTest
    public class MigrateProcessInstanceAsyncTest
    {
        private bool InstanceFieldsInitialized = false;

        public MigrateProcessInstanceAsyncTest()
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

        protected internal IBatch batch;

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
                .WithAuthorizations(AuthorizationSpec.Grant(Resources.Batch, "*", "userId", Permissions.Create),
                AuthorizationSpec.Grant(Resources.ProcessInstance, "processInstance", "userId", Permissions.Read))
                .FailsDueToRequired(AuthorizationSpec.Grant(Resources.ProcessDefinition, "sourceDefinitionKey", "userId", Permissions.MigrateInstance),
                AuthorizationSpec.Grant(Resources.ProcessDefinition, "targetDefinitionKey", "userId", Permissions.MigrateInstance)),
                AuthorizationScenario.Scenario().WithAuthorizations(AuthorizationSpec.Grant(Resources.Batch, "*", "userId", Permissions.Create),
                AuthorizationSpec.Grant(Resources.ProcessInstance, "processInstance", "userId", Permissions.Read),
                AuthorizationSpec.Grant(Resources.ProcessDefinition, "sourceDefinitionKey", "userId", Permissions.MigrateInstance))
                .FailsDueToRequired(AuthorizationSpec.Grant(Resources.ProcessDefinition, "sourceDefinitionKey", "userId", Permissions.MigrateInstance),
                AuthorizationSpec.Grant(Resources.ProcessDefinition, "targetDefinitionKey", "userId", Permissions.MigrateInstance)),
                AuthorizationScenario.Scenario().WithAuthorizations(AuthorizationSpec.Grant(Resources.Batch, "*", "userId", Permissions.Create),
                AuthorizationSpec.Grant(Resources.ProcessInstance, "processInstance", "userId", Permissions.Read),
                AuthorizationSpec.Grant(Resources.ProcessDefinition, "targetDefinitionKey", "userId", Permissions.MigrateInstance))
                .FailsDueToRequired(AuthorizationSpec.Grant(Resources.ProcessDefinition, "sourceDefinitionKey", "userId", Permissions.MigrateInstance),
                AuthorizationSpec.Grant(Resources.ProcessDefinition, "targetDefinitionKey", "userId", Permissions.MigrateInstance)),
                AuthorizationScenario.Scenario().WithAuthorizations(AuthorizationSpec.Grant(Resources.Batch, "*", "userId", Permissions.Create),
                AuthorizationSpec.Grant(Resources.ProcessInstance, "processInstance", "userId", Permissions.Read),
                AuthorizationSpec.Grant(Resources.ProcessDefinition, "sourceDefinitionKey", "userId", Permissions.MigrateInstance),
                AuthorizationSpec.Grant(Resources.ProcessDefinition, "targetDefinitionKey", "userId", Permissions.MigrateInstance)).Succeeds(),
                AuthorizationScenario.Scenario().WithAuthorizations(AuthorizationSpec.Grant(Resources.Batch, "*", "userId", Permissions.Create),
                AuthorizationSpec.Grant(Resources.ProcessInstance, "processInstance", "userId", Permissions.Read),
                AuthorizationSpec.Grant(Resources.ProcessDefinition, "*", "userId", Permissions.MigrateInstance)).Succeeds(),
                AuthorizationScenario.Scenario().WithAuthorizations(AuthorizationSpec.Grant(Resources.ProcessInstance, "processInstance", "userId", Permissions.Read))
                .FailsDueToRequired(AuthorizationSpec.Grant(Resources.Batch, "*", "userId", Permissions.Create)));
        }

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Before public void setUp()
        public virtual void setUp()
        {
            batch = null;
            authRule.CreateUserAndGroup("userId", "groupId");
        }

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @After public void tearDown()
        public virtual void tearDown()
        {
            if (batch != null)
            {
                engineRule.ManagementService.DeleteBatch(batch.Id, true);
            }
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

            batch = engineRule.RuntimeService.NewMigration(migrationPlan).ProcessInstanceIds((processInstance.Id)).ExecuteAsync();

            // then
            if (authRule.AssertScenario(scenario))
            {
                Assert.AreEqual(1, engineRule.ManagementService.CreateBatchQuery().Count());
            }

        }

        [Test]
        [Deployment(new string[] { "resources/api/authorization/oneIncidentProcess.bpmn20.xml" })]
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

            batch = engineRule.RuntimeService.NewMigration(migrationPlan).ProcessInstanceQuery(query).ExecuteAsync();

            // then
            if (authRule.AssertScenario(scenario))
            {
                Assert.AreEqual(1, engineRule.ManagementService.CreateBatchQuery().Count());
            }

        }
    }

}