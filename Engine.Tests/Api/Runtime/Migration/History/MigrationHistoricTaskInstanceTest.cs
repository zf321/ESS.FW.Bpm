using System.Linq;
using Engine.Tests.Api.Runtime.Migration.Models;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime.Migration.History
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class MigrationHistoricTaskInstanceTest
    {
        [SetUp]
        public virtual void initServices()
        {
            historyService = rule.HistoryService;
            runtimeService = rule.RuntimeService;
            taskService = rule.TaskService;
        }

        protected internal IHistoryService historyService;
        private readonly bool InstanceFieldsInitialized;


        protected internal ProcessEngineRule rule = new ProvidedProcessEngineRule();

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(rule).around(testHelper);
        //public RuleChain ruleChain;

        protected internal IRuntimeService runtimeService;
        protected internal ITaskService taskService;
        protected internal MigrationTestRule testHelper;

        public MigrationHistoricTaskInstanceTest()
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

        [Test]
        [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryActivity)]
        public virtual void testMigrateHistoryUserTaskInstance()
        {
            //given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.OneTaskProcess)
                    .ChangeElementId("Process", "Process2")
                    .ChangeElementId("userTask", "userTask2"));

            var migrationPlan =
                runtimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask", "userTask2")
                    .Build();

            var processInstance = runtimeService.StartProcessInstanceById(sourceProcessDefinition.Id);

            var sourceHistoryTaskInstanceQuery = historyService.CreateHistoricTaskInstanceQuery(c=>c.ProcessDefinitionId==sourceProcessDefinition.Id);
            var targetHistoryTaskInstanceQuery = historyService.CreateHistoricTaskInstanceQuery(c=>c.ProcessDefinitionId==targetProcessDefinition.Id);

            var activityInstance = runtimeService.GetActivityInstance(processInstance.Id);

            //when
            Assert.AreEqual(1, sourceHistoryTaskInstanceQuery.Count());
            Assert.AreEqual(0, targetHistoryTaskInstanceQuery.Count());
            var sourceProcessInstanceQuery = runtimeService.CreateProcessInstanceQuery(c=>c.ProcessDefinitionId == sourceProcessDefinition.Id);
            runtimeService.NewMigration(migrationPlan)
                .ProcessInstanceQuery(sourceProcessInstanceQuery)
                .Execute();

            //then
            Assert.AreEqual(0, sourceHistoryTaskInstanceQuery.Count());
            Assert.AreEqual(1, targetHistoryTaskInstanceQuery.Count());

            var instance = targetHistoryTaskInstanceQuery.First();
            Assert.AreEqual(targetProcessDefinition.Key, instance.ProcessDefinitionKey);
            Assert.AreEqual(targetProcessDefinition.Id, instance.ProcessDefinitionId);
            Assert.AreEqual("userTask2", instance.TaskDefinitionKey);
            Assert.AreEqual(activityInstance.GetActivityInstances("userTask")[0].Id, instance.ActivityInstanceId);
        }

        [Test]
        [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryActivity)]
        public virtual void testMigrateWithSubTask()
        {
            //given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);

            var migrationPlan =
                runtimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapEqualActivities()
                    .Build();

            var processInstance = runtimeService.StartProcessInstanceById(sourceProcessDefinition.Id);

            var task = taskService.CreateTaskQuery()
                .First();
            var subTask = taskService.NewTask();
            subTask.ParentTaskId = task.Id;
            taskService.SaveTask(subTask);

            // when
            runtimeService.NewMigration(migrationPlan)
                .ProcessInstanceIds(processInstance.Id)
                .Execute();

            // then the historic sub task instance is still the same
            var historicSubTaskAfterMigration = historyService.CreateHistoricTaskInstanceQuery(c=>c.TaskId==subTask.Id)
                .First();

            Assert.NotNull(historicSubTaskAfterMigration);
            Assert.IsNull(historicSubTaskAfterMigration.ProcessDefinitionId);
            Assert.IsNull(historicSubTaskAfterMigration.ProcessDefinitionKey);
            Assert.IsNull(historicSubTaskAfterMigration.ExecutionId);
            Assert.IsNull(historicSubTaskAfterMigration.ActivityInstanceId);
        }
    }
}