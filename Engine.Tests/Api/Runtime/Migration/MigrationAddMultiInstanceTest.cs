using System;
using Engine.Tests.Api.Runtime.Migration.Models;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine.Migration;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime.Migration
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class MigrationAddMultiInstanceTest
    {
        private readonly bool InstanceFieldsInitialized;


        protected internal ProcessEngineRule rule = new ProvidedProcessEngineRule();
        protected internal MigrationTestRule testHelper;

        public MigrationAddMultiInstanceTest()
        {
            if (!InstanceFieldsInitialized)
            {
                InitializeInstanceFields();
                InstanceFieldsInitialized = true;
            }
        }

        private void InitializeInstanceFields()
        {
            testHelper = new MigrationTestRule(rule);
            //ruleChain = RuleChain.outerRule(rule).around(testHelper);
        }

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(rule).around(testHelper);
        //public RuleChain ruleChain;

        [Test]
        public virtual void testAddMultiInstanceBody()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(MultiInstanceProcessModels.PAR_MI_ONE_TASK_PROCESS);

            try
            {
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask", "userTask")
                    .Build();
                Assert.Fail("Should not succeed");
            }
            catch (MigrationPlanValidationException e)
            {
                MigrationPlanValidationReportAssert.That(e.ValidationReport)
                    .HasInstructionFailures("userTask",
                        "Target activity 'userTask' is a descendant of multi-instance body 'userTask#multiInstanceBody' " +
                        "that is not mapped from the source process definition");
            }
        }

        [Test]
        public virtual void testRemoveAndAddMultiInstanceBody()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(MultiInstanceProcessModels.PAR_MI_ONE_TASK_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(MultiInstanceProcessModels.PAR_MI_ONE_TASK_PROCESS);

            try
            {
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask", "userTask")
                    .Build();
                Assert.Fail("Should not succeed");
            }
            catch (MigrationPlanValidationException e)
            {
                MigrationPlanValidationReportAssert.That(e.ValidationReport)
                    .HasInstructionFailures("userTask",
                        "Target activity 'userTask' is a descendant of multi-instance body 'userTask#multiInstanceBody' " +
                        "that is not mapped from the source process definition");
            }
        }

        [Test]
        public virtual void testAddMultiInstanceBodyWithDeeperNestedMapping()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(MultiInstanceProcessModels.PAR_MI_SUBPROCESS_PROCESS);

            try
            {
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask", "userTask")
                    .Build();
                Assert.Fail("Should not succeed");
            }
            catch (MigrationPlanValidationException e)
            {
                Console.WriteLine(e.ToString());
                Console.Write(e.StackTrace);
                MigrationPlanValidationReportAssert.That(e.ValidationReport)
                    .HasInstructionFailures("userTask",
                        "Target activity 'userTask' is a descendant of multi-instance body 'subProcess#multiInstanceBody' " +
                        "that is not mapped from the source process definition");
            }
        }
    }
}