using System.Linq;
using Engine.Tests.Api.Runtime.Migration.Models;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime.Migration.History
{
    /// <summary>
    /// </summary>
    [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull)]
    [TestFixture]
    public class MigrationHistoricIncidentTest
    {
        [SetUp]
        public virtual void initServices()
        {
            historyService = rule.HistoryService;
            runtimeService = rule.RuntimeService;
            managementService = rule.ManagementService;
            repositoryService = rule.RepositoryService;
        }

        protected internal IHistoryService historyService;
        private readonly bool InstanceFieldsInitialized;
        protected internal IManagementService managementService;
        protected internal IRepositoryService repositoryService;


        protected internal ProcessEngineRule rule = new ProvidedProcessEngineRule();

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(rule).around(testHelper);
        //public RuleChain ruleChain;

        protected internal IRuntimeService runtimeService;
        protected internal MigrationTestRule testHelper;

        public MigrationHistoricIncidentTest()
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
        public virtual void testMigrateHistoricIncident()
        {
            // given
            var sourceProcess = testHelper.DeployAndGetDefinition(AsyncProcessModels.ASYNC_BEFORE_USER_TASK_PROCESS);
            var targetProcess =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(
                        AsyncProcessModels.ASYNC_BEFORE_USER_TASK_PROCESS)
                    .ChangeElementId(ProcessModels.ProcessKey, "new" + ProcessModels.ProcessKey)
                    .ChangeElementId("userTask", "newUserTask"));

            var targetJobDefinition = managementService.CreateJobDefinitionQuery(c=>c.ProcessDefinitionId==targetProcess.Id)
                .First();

            var migrationPlan = runtimeService.CreateMigrationPlan(sourceProcess.Id, targetProcess.Id)
                .MapActivities("userTask", "newUserTask")
                .Build();

            var processInstance = runtimeService.StartProcessInstanceById(sourceProcess.Id);

            var job = managementService.CreateJobQuery()
                .First();
            managementService.SetJobRetries(job.Id, 0);

            var incidentBeforeMigration = historyService.CreateHistoricIncidentQuery()
                .First();

            // when
            runtimeService.NewMigration(migrationPlan)
                .ProcessInstanceIds(processInstance.Id)
                .Execute();

            // then
            var historicIncident = historyService.CreateHistoricIncidentQuery()
                .First();
            Assert.NotNull(historicIncident);

            Assert.AreEqual("newUserTask", historicIncident.ActivityId);
            Assert.AreEqual(targetJobDefinition.Id, historicIncident.JobDefinitionId);
            Assert.AreEqual(targetProcess.Id, historicIncident.ProcessDefinitionId);
            Assert.AreEqual(targetProcess.Key, historicIncident.ProcessDefinitionKey);
            Assert.AreEqual(processInstance.Id, historicIncident.ExecutionId);

            // and other properties have not changed
            Assert.AreEqual(incidentBeforeMigration.CreateTime, historicIncident.CreateTime);
            Assert.AreEqual(incidentBeforeMigration.ProcessInstanceId, historicIncident.ProcessInstanceId);
        }

        [Test]
        public virtual void testMigrateHistoricIncidentAddScope()
        {
            // given
            var sourceProcess = testHelper.DeployAndGetDefinition(AsyncProcessModels.ASYNC_BEFORE_USER_TASK_PROCESS);
            var targetProcess =
                testHelper.DeployAndGetDefinition(AsyncProcessModels.ASYNC_BEFORE_SUBPROCESS_USER_TASK_PROCESS);

            var migrationPlan = runtimeService.CreateMigrationPlan(sourceProcess.Id, targetProcess.Id)
                .MapActivities("userTask", "userTask")
                .Build();

            var processInstance = runtimeService.StartProcessInstanceById(sourceProcess.Id);

            var job = managementService.CreateJobQuery()
                .First();
            managementService.SetJobRetries(job.Id, 0);

            // when
            runtimeService.NewMigration(migrationPlan)
                .ProcessInstanceIds(processInstance.Id)
                .Execute();

            // then
            var activityInstance = runtimeService.GetActivityInstance(processInstance.Id);

            var historicIncident = historyService.CreateHistoricIncidentQuery()
                .First();
            Assert.NotNull(historicIncident);
            Assert.AreEqual(activityInstance.GetTransitionInstances("userTask")[0].ExecutionId,
                historicIncident.ExecutionId);
        }
    }
}