using Engine.Tests.Api.Runtime.Migration.Models;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;
using ESS.FW.Bpm.Engine.Migration;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime.Migration
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class MigrationSignallableServiceTaskTest
    {
        private readonly bool InstanceFieldsInitialized;


        protected internal ProcessEngineRule rule = new ProvidedProcessEngineRule();
        protected internal MigrationTestRule testHelper;

        public MigrationSignallableServiceTaskTest()
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

        public class SignallableServiceTaskDelegate : ISignallableActivityBehavior
        {
            public void Execute(IActivityExecution execution)
            {
            }

            public void Signal(IActivityExecution execution, string signalEvent, object signalData)
            {
                var transition = execution.Activity.OutgoingTransitions[0];
                execution.LeaveActivityViaTransition(transition);
            }
        }

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(rule).around(testHelper);
        //public RuleChain ruleChain;

        [Test]
        public virtual void testCannotMigrateActivityInstance()
        {
            // given
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
            var model = ProcessModels.NewModel()
                .StartEvent()
                .ServiceTask("serviceTask")
                .CamundaClass(typeof(SignallableServiceTaskDelegate).FullName)
                .EndEvent()
                .Done();

            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(model);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(model);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("serviceTask", "serviceTask")
                    .Build();

            // when
            try
            {
                testHelper.CreateProcessInstanceAndMigrate(migrationPlan);
                Assert.Fail("should Assert.Fail");
            }
            catch (MigratingProcessInstanceValidationException e)
            {
                // then
                MigratingProcessInstanceValidationReportAssert.That(e.ValidationReport)
                    .HasActivityInstanceFailures("serviceTask",
                        "The type of the source activity is not supported for activity instance migration");
            }
        }

        [Test]
        public virtual void testCannotMigrateAsyncActivityInstance()
        {
            // given
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
            var model = ProcessModels.NewModel()
                .StartEvent()
                .ServiceTask("serviceTask")
                //.CamundaAsyncBefore()
                .CamundaClass(typeof(SignallableServiceTaskDelegate).FullName)
                .EndEvent()
                .Done();

            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(model);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(model);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("serviceTask", "serviceTask")
                    .Build();

            var ProcessInstanceId = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id)
                .Id;
            testHelper.ExecuteAvailableJobs();

            // when
            try
            {
                rule.RuntimeService.NewMigration(migrationPlan)
                    .ProcessInstanceIds(ProcessInstanceId)
                    .Execute();

                Assert.Fail("should Assert.Fail");
            }
            catch (MigratingProcessInstanceValidationException e)
            {
                // then
                MigratingProcessInstanceValidationReportAssert.That(e.ValidationReport)
                    .HasActivityInstanceFailures("serviceTask",
                        "The type of the source activity is not supported for activity instance migration");
            }
        }
    }
}