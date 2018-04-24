using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Api.Authorization.Util;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.Api.Authorization.Batch
{

    /// <summary>
    /// 
    /// </summary>
    //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
    //ORIGINAL LINE: @RunWith(Parameterized.class) public class SetJobRetriesBatchAuthorizationTest extends AbstractBatchAuthorizationTest
    public class SetJobRetriesBatchAuthorizationTest : AbstractBatchAuthorizationTest
    {
        private bool InstanceFieldsInitialized = false;

        public SetJobRetriesBatchAuthorizationTest()
        {
            if (!InstanceFieldsInitialized)
            {
                InitializeInstanceFields();
                InstanceFieldsInitialized = true;
            }
        }

        private void InitializeInstanceFields()
        {
            //ruleChain = RuleChain.outerRule(engineRule).around(authRule).around(testHelper);
        }


        protected internal const string DEFINITION_XML = "resources/api/mgmt/ManagementServiceTest.TestGetJobExceptionStacktrace.bpmn20.xml";
        protected internal const long BATCH_OPERATIONS = 3;
        protected internal const int RETRIES = 5;

        protected internal virtual void AssertRetries(IList<string> allJobIds, int i)
        {
            foreach (string id in allJobIds)
            {
                Assert.That(managementService.CreateJobQuery(c=>c.Id ==id).First().Retries, Is.EqualTo(i));
            }
        }

        protected internal virtual IList<string> AllJobIds
        {
            get
            {
                List<string> result = new List<string>();
                foreach (ESS.FW.Bpm.Engine.Runtime.IJob job in managementService.CreateJobQuery(c=>c.ProcessDefinitionId ==sourceDefinition.Id).ToList())
                {
                    if (job.ProcessInstanceId != null)
                    {
                        result.Add(job.Id);
                    }
                }
                return result;
            }
        }

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(engineRule).around(authRule).around(testHelper);
        //public RuleChain ruleChain;

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Parameterized.Parameter public api.authorization.util.AuthorizationScenarioWithCount scenario;
        public AuthorizationScenarioWithCount scenario;

        [SetUp]
        public override void deployProcesses()
        {
            IDeployment deploy = testHelper.Deploy(DEFINITION_XML);
            sourceDefinition = engineRule.RepositoryService.CreateProcessDefinitionQuery(c=> c.DeploymentId == deploy.Id).First();
            processInstance = engineRule.RuntimeService.StartProcessInstanceById(sourceDefinition.Id);
            processInstance2 = engineRule.RuntimeService.StartProcessInstanceById(sourceDefinition.Id);
        }


        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Parameterized.Parameters(name = "Scenario {index}") public static java.util.Collection<api.authorization.util.AuthorizationScenario[]> scenarios()
        public static ICollection<AuthorizationScenario[]> scenarios()
        {
            return AuthorizationTestRule.AsParameters(AuthorizationScenarioWithCount.scenario().withCount(3).WithAuthorizations(AuthorizationSpec.Grant(Resources.Batch, "*", "userId", Permissions.Create),
                AuthorizationSpec.Grant(Resources.ProcessInstance, "processInstance1", "userId", Permissions.Read, Permissions.Update),
                AuthorizationSpec.Grant(Resources.ProcessInstance, "processInstance2", "userId", Permissions.Read)).FailsDueToRequired(AuthorizationSpec.Grant(Resources.ProcessInstance, "processInstance2", "userId", Permissions.Update),
                AuthorizationSpec.Grant(Resources.ProcessDefinition, "exceptionInJobExecution", "userId", Permissions.UpdateInstance)), AuthorizationScenarioWithCount.scenario().withCount(5).WithAuthorizations(AuthorizationSpec.Grant(Resources.Batch, "*", "userId", Permissions.Create),
                AuthorizationSpec.Grant(Resources.ProcessInstance, "processInstance1", "userId", Permissions.All),
                AuthorizationSpec.Grant(Resources.ProcessInstance, "processInstance2", "userId", Permissions.All)).Succeeds(), AuthorizationScenarioWithCount.scenario().withCount(5).WithAuthorizations(AuthorizationSpec.Grant(Resources.Batch, "*", "userId", Permissions.Create),
                AuthorizationSpec.Grant(Resources.ProcessDefinition, "Process", "userId", Permissions.ReadInstance, Permissions.UpdateInstance)).Succeeds());
        }

        [Test]
        public virtual void testWithTwoInvocationsJobsListBased()
        {
            engineRule.ProcessEngineConfiguration.InvocationsPerBatchJob = 2;
            setupAndExecuteJobsListBasedTest();

            // then
            AssertScenario();

            AssertRetries(AllJobIds, Convert.ToInt32(AuthorizationScenarioWithCount.scenario().Count));
        }

        [Test]
        public virtual void testWithTwoInvocationsJobsQueryBased()
        {
            engineRule.ProcessEngineConfiguration.InvocationsPerBatchJob = 2;
            setupAndExecuteJobsQueryBasedTest();

            // then
            AssertScenario();

            AssertRetries(AllJobIds, Convert.ToInt32(AuthorizationScenarioWithCount.scenario().Count));
        }

        [Test]
        public virtual void testJobsListBased()
        {
            setupAndExecuteJobsListBasedTest();
            // then
            AssertScenario();
        }

        [Test]
        public virtual void testJobsListQueryBased()
        {
            setupAndExecuteJobsQueryBasedTest();
            // then
            AssertScenario();
        }

        [Test]
        public virtual void testWithTwoInvocationsProcessListBased()
        {
            engineRule.ProcessEngineConfiguration.InvocationsPerBatchJob = 2;
            setupAndExecuteProcessListBasedTest();

            // then
            AssertScenario();

            AssertRetries(AllJobIds, Convert.ToInt32(AuthorizationScenarioWithCount.scenario().Count));
        }

        [Test]
        public virtual void testWithTwoInvocationsProcessQueryBased()
        {
            engineRule.ProcessEngineConfiguration.InvocationsPerBatchJob = 2;
            setupAndExecuteJobsQueryBasedTest();

            // then
            AssertScenario();

            AssertRetries(AllJobIds, Convert.ToInt32(AuthorizationScenarioWithCount.scenario().Count));
        }

        private void setupAndExecuteProcessListBasedTest()
        {
            //given
            IList<string> processInstances = (new string[] { processInstance.Id, processInstance2.Id });
            authRule.Init(scenario).WithUser("userId").BindResource("Process", sourceDefinition.Key).BindResource("processInstance1", processInstance.Id).BindResource("processInstance2", processInstance2.Id).Start();

            // when
            batch = managementService.SetJobRetriesAsync(processInstances, ( IQueryable<IProcessInstance>)null, RETRIES);

            executeSeedAndBatchJobs();
        }

        [Test]
        public virtual void testProcessList()
        {
            setupAndExecuteProcessListBasedTest();
            // then
            AssertScenario();
        }

        protected internal virtual void setupAndExecuteJobsListBasedTest()
        {
            //given
            IList<string> allJobIds = AllJobIds;
            authRule.Init(scenario).WithUser("userId").BindResource("Process", sourceDefinition.Key).BindResource("processInstance1", processInstance.Id).BindResource("processInstance2", processInstance2.Id).Start();

            // when
            batch = managementService.SetJobRetriesAsync(allJobIds, RETRIES);

            executeSeedAndBatchJobs();
        }

        protected internal virtual void setupAndExecuteJobsQueryBasedTest()
        {
            //given
            IQueryable<ESS.FW.Bpm.Engine.Runtime.IJob> jobQuery = managementService.CreateJobQuery();
            authRule.Init(scenario).WithUser("userId").BindResource("Process", sourceDefinition.Key).BindResource("processInstance1", processInstance.Id).BindResource("processInstance2", processInstance2.Id).Start();

            // when

            //batch = managementService.SetJobRetriesAsync(jobQuery, RETRIES);

            executeSeedAndBatchJobs();
        }

        protected internal override AuthorizationScenario Scenario
        {
            get { return scenario; }
        }



        protected internal virtual void AssertScenario()
        {
            if (authRule.AssertScenario(Scenario))
            {
                if (testHelper.HistoryLevelFull)
                {
                    Assert.That(engineRule.HistoryService.CreateUserOperationLogQuery().Count(), Is.EqualTo(BATCH_OPERATIONS));
                }
                AssertRetries(AllJobIds, 5);
            }
        }
    }

}