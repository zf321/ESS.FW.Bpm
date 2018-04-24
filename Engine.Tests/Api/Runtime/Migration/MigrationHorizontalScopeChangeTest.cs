using Engine.Tests.Api.Runtime.Migration.Models;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine.Migration;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime.Migration
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class MigrationHorizontalScopeChangeTest
    {
        private readonly bool InstanceFieldsInitialized;


        protected internal ProcessEngineRule rule = new ProvidedProcessEngineRule();
        protected internal MigrationTestRule testHelper;

        public MigrationHorizontalScopeChangeTest()
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
        public virtual void testCannotMigrateHorizontallyBetweenScopes()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ParallelSubprocessProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ParallelSubprocessProcess);

            // when
            try
            {
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("subProcess1", "subProcess1")
                    .MapActivities("subProcess2", "subProcess2")
                    .MapActivities("userTask1", "userTask2")
                    .MapActivities("userTask2", "userTask1")
                    .Build();

                Assert.Fail("should Assert.Fail");
            }
            catch (MigrationPlanValidationException e)
            {
                MigrationPlanValidationReportAssert.That(e.ValidationReport)
                    .HasInstructionFailures("userTask1",
                        "The closest mapped ancestor 'subProcess1' is mapped to scope 'subProcess1' which is not an ancestor of target scope 'userTask2'")
                    .HasInstructionFailures("userTask2",
                        "The closest mapped ancestor 'subProcess2' is mapped to scope 'subProcess2' which is not an ancestor of target scope 'userTask1'");
            }
        }
    }
}