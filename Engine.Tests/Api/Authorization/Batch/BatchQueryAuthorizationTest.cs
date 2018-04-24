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
    public class BatchQueryAuthorizationTest
    {
        private bool InstanceFieldsInitialized = false;

        public BatchQueryAuthorizationTest()
        {
            if (!InstanceFieldsInitialized)
            {
                InitializeInstanceFields();
                InstanceFieldsInitialized = true;
            }
        }

        private void InitializeInstanceFields()
        {
            authRule = new AuthorizationTestBaseRule(engineRule);
            testHelper = new ProcessEngineTestRule(engineRule);
            //ruleChain = RuleChain.outerRule(engineRule).around(authRule).around(testHelper);
        }


        public ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
        public AuthorizationTestBaseRule authRule;
        public ProcessEngineTestRule testHelper;

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(engineRule).around(authRule).around(testHelper);
        //public RuleChain ruleChain;

        protected internal IMigrationPlan migrationPlan;
        protected internal IBatch batch1;
        protected internal IBatch batch2;

        [SetUp]
        public virtual void setUp()
        {
            authRule.CreateUserAndGroup("user", "group");
        }

        [SetUp]
        public virtual void deployProcessesAndCreateMigrationPlan()
        {
            IProcessDefinition sourceDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            IProcessDefinition targetDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);

            migrationPlan = engineRule.RuntimeService.CreateMigrationPlan(sourceDefinition.Id, targetDefinition.Id).MapEqualActivities().Build();

            IProcessInstance pi = engineRule.RuntimeService.StartProcessInstanceById(sourceDefinition.Id);

            batch1 = engineRule.RuntimeService.NewMigration(migrationPlan).ProcessInstanceIds((pi.Id)).ExecuteAsync();

            batch2 = engineRule.RuntimeService.NewMigration(migrationPlan).ProcessInstanceIds((pi.Id)).ExecuteAsync();
        }

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @After public void tearDown()
        public virtual void tearDown()
        {
            authRule.DeleteUsersAndGroups();
        }

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @After public void deleteBatches()
        public virtual void deleteBatches()
        {
            engineRule.ManagementService.DeleteBatch(batch1.Id, true);
            engineRule.ManagementService.DeleteBatch(batch2.Id, true);
        }

        [Test]
        public virtual void testQueryList()
        {
            // given
            authRule.CreateGrantAuthorization(Resources.Batch, batch1.Id, "user", Permissions.Read);

            // when
            authRule.EnableAuthorization("user");
            IList<IBatch> batches = engineRule.ManagementService.CreateBatchQuery()
                .ToList();
            authRule.DisableAuthorization();

            // then
            Assert.AreEqual(1, batches.Count);
            Assert.AreEqual(batch1.Id, batches[0].Id);
        }

        [Test]
        public virtual void testQueryCount()
        {
            // given
            authRule.CreateGrantAuthorization(Resources.Batch, batch1.Id, "user", Permissions.Read);

            // when
            authRule.EnableAuthorization("user");
            long Count = engineRule.ManagementService.CreateBatchQuery().Count();
            authRule.DisableAuthorization();

            // then
            Assert.AreEqual(1, Count);
        }

        [Test]
        public virtual void testQueryNoAuthorizations()
        {
            // when
            authRule.EnableAuthorization("user");
            long Count = engineRule.ManagementService.CreateBatchQuery().Count();
            authRule.DisableAuthorization();

            // then
            Assert.AreEqual(0, Count);
        }

        [Test]
        public virtual void testQueryListAccessAll()
        {
            // given
            authRule.CreateGrantAuthorization(Resources.Batch, "*", "user", Permissions.Read);

            // when
            authRule.EnableAuthorization("user");
            IList<IBatch> batches = engineRule.ManagementService.CreateBatchQuery().ToList();
            authRule.DisableAuthorization();

            // then
            Assert.AreEqual(2, batches.Count);
        }

        [Test]
        public virtual void testQueryListMultiple()
        {
            // given
            authRule.CreateGrantAuthorization(Resources.Batch, batch1.Id, "user", Permissions.Read);
            authRule.CreateGrantAuthorization(Resources.Batch, "*", "user", Permissions.Read);

            // when
            authRule.EnableAuthorization("user");
            IList<IBatch> batches = engineRule.ManagementService.CreateBatchQuery().ToList();
            authRule.DisableAuthorization();

            // then
            Assert.AreEqual(2, batches.Count);
        }
    }

}