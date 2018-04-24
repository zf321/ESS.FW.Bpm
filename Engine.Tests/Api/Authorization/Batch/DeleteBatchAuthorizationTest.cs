using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Api.Authorization.Util;
using Engine.Tests.Api.Runtime.Migration.Models;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Batch;
using ESS.FW.Bpm.Engine.History.Impl;
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
    //ORIGINAL LINE: @RunWith(Parameterized.class) public class DeleteBatchAuthorizationTest
    public class DeleteBatchAuthorizationTest
    {
        private bool InstanceFieldsInitialized = false;

        public DeleteBatchAuthorizationTest()
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
                .FailsDueToRequired(AuthorizationSpec.Grant(Resources.Batch, "batchId", "userId", Permissions.Delete)),
                AuthorizationScenario.Scenario()
                .WithAuthorizations(AuthorizationSpec.Grant(Resources.Batch, "batchId", "userId", Permissions.Delete)).Succeeds());
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
            if (authRule.ScenarioFailed())
            {
                engineRule.ManagementService.DeleteBatch(batch.Id, true);
            }
            else
            {
                if (!cascade && engineRule.ProcessEngineConfiguration.HistoryLevel == HistoryLevelFields.HistoryLevelFull)
                {
                    engineRule.HistoryService.DeleteHistoricBatch(batch.Id);
                }
            }
        }

        [Test]
        public virtual void testDeleteBatch()
        {

            // given
            IProcessInstance processInstance = engineRule.RuntimeService.StartProcessInstanceById(migrationPlan.SourceProcessDefinitionId);
            batch = engineRule.RuntimeService.NewMigration(migrationPlan).ProcessInstanceIds((processInstance.Id)).ExecuteAsync();

            // when
            authRule.Init(scenario).WithUser("userId").BindResource("batchId", batch.Id).Start();

            cascade = false;
            engineRule.ManagementService.DeleteBatch(batch.Id, cascade);

            // then
            if (authRule.AssertScenario(scenario))
            {
                Assert.AreEqual(0, engineRule.ManagementService.CreateBatchQuery().Count());
            }
        }

        /// <summary>
        /// Requires no additional DELETE_HISTORY authorization => consistent with DeleteDeployment
        /// </summary>
        [Test]
        public virtual void testDeleteBatchCascade()
        {
            // given
            IProcessInstance processInstance = engineRule.RuntimeService.StartProcessInstanceById(migrationPlan.SourceProcessDefinitionId);
            batch = engineRule.RuntimeService.NewMigration(migrationPlan).ProcessInstanceIds((processInstance.Id)).ExecuteAsync();

            // when
            authRule.Init(scenario).WithUser("userId").BindResource("batchId", batch.Id).Start();

            cascade = true;
            engineRule.ManagementService.DeleteBatch(batch.Id, cascade);

            // then
            if (authRule.AssertScenario(scenario))
            {
                Assert.AreEqual(0, engineRule.ManagementService.CreateBatchQuery().Count());
                Assert.AreEqual(0, engineRule.HistoryService.CreateHistoricBatchQuery().Count());
            }
        }
    }

}