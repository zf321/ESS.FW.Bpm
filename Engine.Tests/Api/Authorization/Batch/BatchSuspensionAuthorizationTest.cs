using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Api.Authorization.Util;
using Engine.Tests.Api.Runtime.Migration.Models;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Batch;
using ESS.FW.Bpm.Engine.Migration;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.Api.Authorization.Batch
{



    /// <summary>
    /// 
    /// 
    /// </summary>
    //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
    //ORIGINAL LINE: @RunWith(Parameterized.class) public class BatchSuspensionAuthorizationTest
    public class BatchSuspensionAuthorizationTest
    {
        private bool InstanceFieldsInitialized = false;

        public BatchSuspensionAuthorizationTest()
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
            return AuthorizationTestRule.AsParameters(AuthorizationScenario.Scenario().WithoutAuthorizations().FailsDueToRequired(AuthorizationSpec.Grant(Resources.Batch, "batchId", "userId", Permissions.Update)),
                AuthorizationScenario.Scenario().WithAuthorizations(AuthorizationSpec.Grant(Resources.Batch, "batchId", "userId", Permissions.Update)).Succeeds());
        }

        protected internal IMigrationPlan migrationPlan;
        protected internal IBatch batch;
        protected internal bool cascade;

        [SetUp]
        public virtual void setUp()
        {
            authRule.CreateUserAndGroup("userId", "groupId");
        }

        [SetUp]
        public virtual void deployProcessesAndCreateMigrationPlan()
        {
            IProcessDefinition sourceDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            IProcessDefinition targetDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);

            migrationPlan = engineRule.RuntimeService.CreateMigrationPlan(sourceDefinition.Id, targetDefinition.Id).Build();
        }

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @After public void tearDown()
        public virtual void tearDown()
        {
            authRule.DeleteUsersAndGroups();
        }

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @After public void deleteBatch()
        public virtual void deleteBatch()
        {
            engineRule.ManagementService.DeleteBatch(batch.Id, true);
        }

        [Test]
        public virtual void testSuspendBatch()
        {

            // given
            IProcessInstance processInstance = engineRule.RuntimeService.StartProcessInstanceById(migrationPlan.SourceProcessDefinitionId);
            batch = engineRule.RuntimeService.NewMigration(migrationPlan).ProcessInstanceIds(new List<string> { processInstance.Id }).ExecuteAsync();

            // when
            authRule.Init(scenario).WithUser("userId").BindResource("batchId", batch.Id).Start();

            engineRule.ManagementService.SuspendBatchById(batch.Id);

            // then
            if (authRule.AssertScenario(scenario))
            {
                batch = engineRule.ManagementService.CreateBatchQuery(c=>c.Id== batch.Id).First();

                Assert.True(batch.Suspended);
            }
        }

        [Test]
        public virtual void testActivateBatch()
        {
            // given
            IProcessInstance processInstance = engineRule.RuntimeService.StartProcessInstanceById(migrationPlan.SourceProcessDefinitionId);
            batch = engineRule.RuntimeService.NewMigration(migrationPlan).ProcessInstanceIds(new List<string>() { processInstance.Id}).ExecuteAsync();

            engineRule.ManagementService.SuspendBatchById(batch.Id);

            // when
            authRule.Init(scenario).WithUser("userId").BindResource("batchId", batch.Id).Start();

            engineRule.ManagementService.ActivateBatchById(batch.Id);

            // then
            if (authRule.AssertScenario(scenario))
            {
                batch = engineRule.ManagementService.CreateBatchQuery(c=>c.Id== batch.Id).First();

                Assert.IsFalse(batch.Suspended);
            }
        }
    }

}