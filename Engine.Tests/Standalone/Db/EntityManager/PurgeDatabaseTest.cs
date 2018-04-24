using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine.History.Impl;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Persistence.Deploy.Cache;
using NUnit.Framework;

namespace Engine.Tests.Standalone.Db.EntityManager
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class PurgeDatabaseTest
    {
        [SetUp]
        public virtual void SetUp()
        {
            _processEngineConfiguration = EngineRule.ProcessEngineConfiguration;
            _processEngineConfiguration.SetDbMetricsReporterActivate(true);
            _databaseTablePrefix = _processEngineConfiguration.DatabaseTablePrefix;
        }

        [TearDown]
        public virtual void CleanUp()
        {
            _processEngineConfiguration.SetDbMetricsReporterActivate(false);
        }

        protected internal const string ProcessDefKey = "test";
        protected internal const string ProcessModelName = "test.bpmn20.xml";
//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public test.ProcessEngineRule engineRule = new test.Util.ProvidedProcessEngineRule();
        public ProcessEngineRule EngineRule = new ProvidedProcessEngineRule();
        private ProcessEngineConfigurationImpl _processEngineConfiguration;
        private string _databaseTablePrefix;

        private void CreateAuthenticationData()
        {
            var identityService = EngineRule.IdentityService;
            var group = identityService.NewGroup("group");
            identityService.SaveGroup(group);
            var user = identityService.NewUser("user");
            var user2 = identityService.NewUser("user2");
            identityService.SaveUser(user);
            identityService.SaveUser(user2);
            var tenant = identityService.NewTenant("tenant");
            identityService.SaveTenant(tenant);
            var tenant2 = identityService.NewTenant("tenant2");
            identityService.SaveTenant(tenant2);
            identityService.CreateMembership("user", "group");
            identityService.CreateTenantUserMembership("tenant", "user");
            identityService.CreateTenantUserMembership("tenant2", "user2");


            //          ITestResource resource1 = new TestResource("resource1", 100);
            //          // create global authorization which grants all permissions to all users (on resource1):
            //          IAuthorizationService authorizationService = EngineRule.AuthorizationService;
            //System.Net.Authorization globalAuth = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeGlobal);
            //globalAuth.Resource = resource1;
            //globalAuth.ResourceId = AuthorizationFields.Any;
            //globalAuth.AddPermission(Permissions.All);
            //authorizationService.SaveAuthorization(globalAuth);

            //          //grant user read auth on resource2
            //          TestResource resource2 = new TestResource("resource2", 200);
            //          IAuthorization userGrant = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeGrant);
            //userGrant.UserId = "user";
            //userGrant.Resource = resource2;
            //userGrant.ResourceId = AuthorizationFields.Any;
            //userGrant.AddPermission(Permissions.Read);
            //authorizationService.SaveAuthorization(userGrant);

            identityService.AuthenticatedUserId = "user";
        }

        private void ExecuteComplexBpmnProcess(bool complete)
        {
            var variables = ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables();
            variables.Add("key", "value");
            EngineRule.RuntimeService.StartProcessInstanceByKey(ProcessDefKey, variables);
            //execute start event
            var job = EngineRule.ManagementService.CreateJobQuery().First();
            EngineRule.ManagementService.ExecuteJob(job.Id);

            //fetch tasks and jobs
            //var externalTasks =
            //    EngineRule.ExternalTaskService.FetchAndLock(1, "worker").Ex/*.Topic("external", 1500)*/ecute();
            job = EngineRule.ManagementService.CreateJobQuery().First();
            var task = EngineRule.TaskService.CreateTaskQuery().First();

            //complete
            if (complete)
            {
                EngineRule.ManagementService.SetJobRetries(job.Id, 0);
                EngineRule.ManagementService.ExecuteJob(job.Id);
                //EngineRule.ExternalTaskService.Complete(externalTasks[0].Id, "worker");
                EngineRule.TaskService.Complete(task.Id);
            }
        }

        [Test]
        public virtual void TestPurge()
        {
            // given data
            var test = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(ProcessDefKey).StartEvent().EndEvent().Done();
            EngineRule.RepositoryService.CreateDeployment().AddModelInstance(ProcessModelName, test).Deploy();
            EngineRule.RuntimeService.StartProcessInstanceByKey(ProcessDefKey);

            // when purge is executed
            var managementService = (ManagementServiceImpl) EngineRule.ManagementService;
            managementService.Purge();

            // then no more data exist
            TestHelper.AssertAndEnsureCleanDbAndCache(EngineRule.ProcessEngine, true);
        }

        // CMMN //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        [Test]
        public virtual void TestPurgeCmmnProcess()
        {
            // given cmmn process which is not managed by process engine rule

            EngineRule.RepositoryService.CreateDeployment()
                .AddClasspathResource(
                    "resources/standalone/db/entitymanager/PurgeDatabaseTest.TestPurgeCmmnProcess.cmmn")
                .Deploy();
            var variables = ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables();
            variables.Add("key", "value");
            EngineRule.CaseService.CreateCaseInstanceByKey(ProcessDefKey, variables);

            // when purge is executed
            var managementService = (ManagementServiceImpl) EngineRule.ManagementService;
            var purge = managementService.Purge();

            // then database and cache is cleaned
            TestHelper.AssertAndEnsureCleanDbAndCache(EngineRule.ProcessEngine, true);

            // and report contains deleted entities
            Assert.IsFalse(purge.CachePurgeReport.Empty);
            var cachePurgeReport = purge.CachePurgeReport;
            Assert.AreEqual(1, cachePurgeReport.GetReportValue(CachePurgeReport.CaseDefCache).Count);

            var databasePurgeReport = purge.DatabasePurgeReport;
            Assert.AreEqual(1, (long) databasePurgeReport.GetReportValue(_databaseTablePrefix + "ACT_RE_DEPLOYMENT"));
            Assert.AreEqual(1, (long) databasePurgeReport.GetReportValue(_databaseTablePrefix + "ACT_RU_TASK"));
            Assert.AreEqual(1, (long) databasePurgeReport.GetReportValue(_databaseTablePrefix + "ACT_GE_BYTEARRAY"));
            Assert.AreEqual(1, (long) databasePurgeReport.GetReportValue(_databaseTablePrefix + "ACT_RE_CASE_DEF"));
            Assert.AreEqual(3, (long) databasePurgeReport.GetReportValue(_databaseTablePrefix + "ACT_RU_CASE_EXECUTION"));
            Assert.AreEqual(1, (long) databasePurgeReport.GetReportValue(_databaseTablePrefix + "ACT_RU_VARIABLE"));
            Assert.AreEqual(2,
                (long) databasePurgeReport.GetReportValue(_databaseTablePrefix + "ACT_RU_CASE_SENTRY_PART"));

            if (_processEngineConfiguration.HistoryLevel.Equals(HistoryLevelFields.HistoryLevelFull))
            {
                Assert.AreEqual(1, (long) databasePurgeReport.GetReportValue(_databaseTablePrefix + "ACT_HI_DETAIL"));
                Assert.AreEqual(1, (long) databasePurgeReport.GetReportValue(_databaseTablePrefix + "ACT_HI_TASKINST"));
                Assert.AreEqual(1, (long) databasePurgeReport.GetReportValue(_databaseTablePrefix + "ACT_HI_VARINST"));
                Assert.AreEqual(1, (long) databasePurgeReport.GetReportValue(_databaseTablePrefix + "ACT_HI_CASEINST"));
                Assert.AreEqual(2,
                    (long) databasePurgeReport.GetReportValue(_databaseTablePrefix + "ACT_HI_CASEACTINST"));
            }
        }

        [Test]
        public virtual void TestPurgeComplexProcess()
        {
            // given complex process with authentication
            // process is executed two times
            // metrics are reported

            //IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ProcessDefKey).StartEvent().CamundaAsyncBefore().ParallelGateway("parallel").ServiceTask("external").CamundaType("external").CamundaTopic("external").BoundaryEvent().Message("message").MoveToNode("parallel").ServiceTask().CamundaAsyncBefore().CamundaExpression("${1/0}").MoveToLastGateway().UserTask().Done();

            CreateAuthenticationData();
            //EngineRule.RepositoryService.CreateDeployment().AddModelInstance(ProcessModelName, modelInstance).Deploy();

            ExecuteComplexBpmnProcess(true);
            ExecuteComplexBpmnProcess(false);

            _processEngineConfiguration.DbMetricsReporter.ReportNow();

            // when purge is executed
            var managementService = (ManagementServiceImpl) EngineRule.ManagementService;
            var purge = managementService.Purge();

            // then database and cache are empty
            TestHelper.AssertAndEnsureCleanDbAndCache(EngineRule.ProcessEngine, true);

            // and report contains deleted data
            Assert.IsFalse(purge.CachePurgeReport.Empty);
            var cachePurgeReport = purge.CachePurgeReport;
            Assert.AreEqual(1, cachePurgeReport.GetReportValue(CachePurgeReport.ProcessDefCache).Count);

            var databasePurgeReport = purge.DatabasePurgeReport;
            Assert.AreEqual(2, (long) databasePurgeReport.GetReportValue(_databaseTablePrefix + "ACT_ID_TENANT_MEMBER"));
            Assert.AreEqual(1, (long) databasePurgeReport.GetReportValue(_databaseTablePrefix + "ACT_RU_EVENT_SUBSCR"));
            Assert.AreEqual(1, (long) databasePurgeReport.GetReportValue(_databaseTablePrefix + "ACT_RE_DEPLOYMENT"));
            Assert.AreEqual(1, (long) databasePurgeReport.GetReportValue(_databaseTablePrefix + "ACT_RU_EXT_TASK"));
            Assert.AreEqual(1, (long) databasePurgeReport.GetReportValue(_databaseTablePrefix + "ACT_ID_MEMBERSHIP"));
            Assert.AreEqual(1, (long) databasePurgeReport.GetReportValue(_databaseTablePrefix + "ACT_RU_TASK"));
            Assert.AreEqual(1, (long) databasePurgeReport.GetReportValue(_databaseTablePrefix + "ACT_RU_JOB"));
            Assert.AreEqual(1, (long) databasePurgeReport.GetReportValue(_databaseTablePrefix + "ACT_GE_BYTEARRAY"));
            Assert.AreEqual(2, (long) databasePurgeReport.GetReportValue(_databaseTablePrefix + "ACT_RU_JOBDEF"));
            Assert.AreEqual(2, (long) databasePurgeReport.GetReportValue(_databaseTablePrefix + "ACT_ID_USER"));
            Assert.AreEqual(5, (long) databasePurgeReport.GetReportValue(_databaseTablePrefix + "ACT_RU_EXECUTION"));
            Assert.AreEqual(10, (long) databasePurgeReport.GetReportValue(_databaseTablePrefix + "ACT_RU_METER_LOG"));
            Assert.AreEqual(1, (long) databasePurgeReport.GetReportValue(_databaseTablePrefix + "ACT_RU_VARIABLE"));
            Assert.AreEqual(1, (long) databasePurgeReport.GetReportValue(_databaseTablePrefix + "ACT_RE_PROCDEF"));
            Assert.AreEqual(2, (long) databasePurgeReport.GetReportValue(_databaseTablePrefix + "ACT_ID_TENANT"));
            Assert.AreEqual(1, (long) databasePurgeReport.GetReportValue(_databaseTablePrefix + "ACT_ID_GROUP"));
            Assert.AreEqual(2, (long) databasePurgeReport.GetReportValue(_databaseTablePrefix + "ACT_RU_AUTHORIZATION"));

            if (_processEngineConfiguration.HistoryLevel.Equals(HistoryLevelFields.HistoryLevelFull))
            {
                Assert.AreEqual(1, (long) databasePurgeReport.GetReportValue(_databaseTablePrefix + "ACT_HI_INCIDENT"));
                Assert.AreEqual(9, (long) databasePurgeReport.GetReportValue(_databaseTablePrefix + "ACT_HI_ACTINST"));
                Assert.AreEqual(2, (long) databasePurgeReport.GetReportValue(_databaseTablePrefix + "ACT_HI_PROCINST"));
                Assert.AreEqual(2, (long) databasePurgeReport.GetReportValue(_databaseTablePrefix + "ACT_HI_DETAIL"));
                Assert.AreEqual(2, (long) databasePurgeReport.GetReportValue(_databaseTablePrefix + "ACT_HI_TASKINST"));
                Assert.AreEqual(7, (long) databasePurgeReport.GetReportValue(_databaseTablePrefix + "ACT_HI_JOB_LOG"));
                Assert.AreEqual(2, (long) databasePurgeReport.GetReportValue(_databaseTablePrefix + "ACT_HI_VARINST"));
                Assert.AreEqual(3, (long) databasePurgeReport.GetReportValue(_databaseTablePrefix + "ACT_HI_OP_LOG"));
            }
        }

        // DMN ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        [Test]
        public virtual void TestPurgeDmnProcess()
        {
            // given dmn process which is not managed by process engine rule
            EngineRule.RepositoryService.CreateDeployment()
                .AddClasspathResource(
                    "resources/standalone/db/entitymanager/PurgeDatabaseTest.TestPurgeDmnProcess.Dmn")
                .Deploy();
            var variables = ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables().PutValue("key", "value").PutValue("season", "Test");
            EngineRule.DecisionService.EvaluateDecisionByKey("decisionId").Variables(variables).Evaluate();

            // when purge is executed
            var managementService = (ManagementServiceImpl) EngineRule.ManagementService;
            var purge = managementService.Purge();

            // then database and cache is cleaned
            TestHelper.AssertAndEnsureCleanDbAndCache(EngineRule.ProcessEngine, true);

            // and report contains deleted entities
            Assert.IsFalse(purge.CachePurgeReport.Empty);
            var cachePurgeReport = purge.CachePurgeReport;
            Assert.AreEqual(2, cachePurgeReport.GetReportValue(CachePurgeReport.DmnDefCache).Count);
            Assert.AreEqual(1, cachePurgeReport.GetReportValue(CachePurgeReport.DmnReqDefCache).Count);

            var databasePurgeReport = purge.DatabasePurgeReport;
            Assert.AreEqual(1, (long) databasePurgeReport.GetReportValue(_databaseTablePrefix + "ACT_RE_DEPLOYMENT"));
            Assert.AreEqual(1, (long) databasePurgeReport.GetReportValue(_databaseTablePrefix + "ACT_GE_BYTEARRAY"));
            Assert.AreEqual(1,
                (long) databasePurgeReport.GetReportValue(_databaseTablePrefix + "ACT_RE_DECISION_REQ_DEF"));
            Assert.AreEqual(2, (long) databasePurgeReport.GetReportValue(_databaseTablePrefix + "ACT_RE_DECISION_DEF"));

            if (_processEngineConfiguration.HistoryLevel.Equals(HistoryLevelFields.HistoryLevelFull))
            {
                Assert.AreEqual(1, (long) databasePurgeReport.GetReportValue(_databaseTablePrefix + "ACT_HI_DECINST"));
                Assert.AreEqual(1, (long) databasePurgeReport.GetReportValue(_databaseTablePrefix + "ACT_HI_DEC_IN"));
                Assert.AreEqual(1, (long) databasePurgeReport.GetReportValue(_databaseTablePrefix + "ACT_HI_DEC_OUT"));
            }
        }

        [Test]
        public virtual void TestPurgeWithAsyncProcessInstance()
        {
            // given process with variable and async process instance
            var test =
                ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(ProcessDefKey)
                    .StartEvent()
                    .CamundaAsyncBefore()
                    .UserTask()
                    .UserTask()
                    .EndEvent()
                    .Done();
            EngineRule.RepositoryService.CreateDeployment().AddModelInstance(ProcessModelName, test).Deploy();

            var variables = ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables();
            variables.Add("key", "value");
            EngineRule.RuntimeService.StartProcessInstanceByKey(ProcessDefKey, variables);
            var job = EngineRule.ManagementService.CreateJobQuery().First();
            EngineRule.ManagementService.ExecuteJob(job.Id);
            var task = EngineRule.TaskService.CreateTaskQuery().First();
            EngineRule.TaskService.Complete(task.Id);

            // when purge is executed
            var managementService = (ManagementServiceImpl) EngineRule.ManagementService;
            managementService.Purge();

            // then no more data exist
            TestHelper.AssertAndEnsureCleanDbAndCache(EngineRule.ProcessEngine, true);
        }

        [Test]
        public virtual void TestPurgeWithExistingProcessInstance()
        {
            //given process with variable and staying process instance in second user task
            var test =
                ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(ProcessDefKey)
                    .StartEvent()
                    .UserTask()
                    .UserTask()
                    .EndEvent()
                    .Done();
            EngineRule.RepositoryService.CreateDeployment().AddModelInstance(ProcessModelName, test).Deploy();

            var variables = ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables();
            variables.Add("key", "value");
            EngineRule.RuntimeService.StartProcessInstanceByKey(ProcessDefKey, variables);
            var task = EngineRule.TaskService.CreateTaskQuery().First();
            EngineRule.TaskService.Complete(task.Id);

            // when purge is executed
            var managementService = (ManagementServiceImpl) EngineRule.ManagementService;
            managementService.Purge();

            // then no more data exist
            TestHelper.AssertAndEnsureCleanDbAndCache(EngineRule.ProcessEngine, true);
        }
    }
}