using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Api.Authorization.Util;
using Engine.Tests.Api.Runtime.Migration.Models;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Batch;
using ESS.FW.Bpm.Engine.Batch.History;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;

namespace Engine.Tests.Api.Authorization.Batch
{

    /// <summary>
    /// 
    /// </summary>
    //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
    //ORIGINAL LINE: @RunWith(Parameterized.class) public class BatchCreationAuthorizationTest
    public class BatchCreationAuthorizationTest
    {
        private bool InstanceFieldsInitialized = false;

        public BatchCreationAuthorizationTest()
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
            //ruleChain = RuleChain.outerRule(engineRule).around(authRule).around(testHelper);
        }

        protected internal const string TEST_REASON = "test reason";
        protected internal const string JOB_EXCEPTION_DEFINITION_XML = "resources/api/mgmt/ManagementServiceTest.TestGetJobExceptionStacktrace.bpmn20.xml";

        protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
        protected internal AuthorizationTestRule authRule;
        protected internal ProcessEngineTestRule testHelper;

        protected internal IProcessInstance processInstance;
        protected internal IRuntimeService runtimeService;
        protected internal IManagementService managementService;
        protected internal IHistoryService historyService;

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(engineRule).around(authRule).around(testHelper);
        //public RuleChain ruleChain;

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Parameterized.Parameter public api.authorization.util.AuthorizationScenario scenario;
        public AuthorizationScenario scenario;

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Parameterized.Parameters(name = "Scenario {index}") public static java.util.Collection<api.authorization.util.AuthorizationScenario[]> scenarios()
        public static ICollection<AuthorizationScenario[]> scenarios()
        {
            return AuthorizationTestRule.AsParameters(AuthorizationScenario.Scenario().WithoutAuthorizations()
                .FailsDueToRequired(AuthorizationSpec.Grant(Resources.Batch, "batchId", "userId", Permissions.Create)),
                AuthorizationScenario.Scenario().WithAuthorizations(AuthorizationSpec.Grant(Resources.Batch, "batchId", "userId", Permissions.Create)).Succeeds());
        }

        [SetUp]
        public virtual void setUp()
        {
            authRule.CreateUserAndGroup("userId", "groupId");
            runtimeService = engineRule.RuntimeService;
            managementService = engineRule.ManagementService;
            historyService = engineRule.HistoryService;
        }

        [SetUp]
        public virtual void deployProcesses()
        {
            IProcessDefinition sourceDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            processInstance = engineRule.RuntimeService.StartProcessInstanceById(sourceDefinition.Id);
        }

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @After public void tearDown()
        public virtual void tearDown()
        {
            authRule.DeleteUsersAndGroups();
        }

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @After public void cleanBatch()
        public virtual void cleanBatch()
        {
            IBatch batch = engineRule.ManagementService.CreateBatchQuery().First();
            if (batch != null)
            {
                engineRule.ManagementService.DeleteBatch(batch.Id, true);
            }

            IHistoricBatch historicBatch = engineRule.HistoryService.CreateHistoricBatchQuery().First();
            if (historicBatch != null)
            {
                engineRule.HistoryService.DeleteHistoricBatch(historicBatch.Id);
            }
        }

        [Test]
        public virtual void testBatchProcessInstanceDeletion()
        {
            //given
            authRule.Init(scenario).WithUser("userId").BindResource("batchId", "*").Start();

            // when
            IList<string> processInstanceIds = new List<string> { processInstance.Id}; //Collections.singletonList(processInstance.Id);
            runtimeService.DeleteProcessInstancesAsync(processInstanceIds, null, TEST_REASON);

            // then
            authRule.AssertScenario(scenario);
        }

        
        [Test]
        public virtual void createBatchModification()
        {
            //given
            IBpmnModelInstance instance = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process1").StartEvent().UserTask("user1").UserTask("user2").EndEvent().Done();
            IProcessDefinition processDefinition = testHelper.DeployAndGetDefinition(instance);

            IList<string> instances = new List<string>();
            for (int i = 0; i < 2; i++)
            {
                IProcessInstance processInstance = engineRule.RuntimeService.StartProcessInstanceByKey("process1");
                instances.Add(processInstance.Id);
            }

            authRule.Init(scenario).WithUser("userId").BindResource("batchId", "*").Start();

            // when
            // Todo: IRuntimeService.CreateModification(..)
            //engineRule.RuntimeService.createModification(processDefinition.Id).StartAfterActivity("user1").processInstanceIds(instances).ExecuteAsync();

            // then
            authRule.AssertScenario(scenario);
        }

        [Test]
        public virtual void testBatchSetJobRetriesByJobs()
        {
            //given
            IList<string> jobIds = setupFailedJobs();
            authRule.Init(scenario).WithUser("userId").BindResource("batchId", "*").Start();

            // when

            managementService.SetJobRetriesAsync(jobIds, 5);

            // then
            authRule.AssertScenario(scenario);
        }

        [Test]
        public virtual void testBatchSetJobRetriesByProcesses()
        {
            //given
            setupFailedJobs();
            IList<string> processInstanceIds = new List<string> {processInstance.Id}; //Collections.singletonList(processInstance.Id);
            authRule.Init(scenario).WithUser("userId").BindResource("batchId", "*").Start();

            // when

            managementService.SetJobRetriesAsync(processInstanceIds, ( IQueryable<IProcessInstance>)null, 5);

            // then
            authRule.AssertScenario(scenario);
        }

        protected internal virtual IList<string> setupFailedJobs()
        {
            IList<string> jobIds = new List<string>();

            IDeployment deploy = testHelper.Deploy(JOB_EXCEPTION_DEFINITION_XML);
            IProcessDefinition sourceDefinition = engineRule.RepositoryService.CreateProcessDefinitionQuery(c=> c.DeploymentId == deploy.Id).First();
            processInstance = engineRule.RuntimeService.StartProcessInstanceById(sourceDefinition.Id);

            IList<ESS.FW.Bpm.Engine.Runtime.IJob> jobs = managementService.CreateJobQuery(c=>c.ProcessInstanceId== processInstance.Id)
                .ToList();
            foreach (ESS.FW.Bpm.Engine.Runtime.IJob job in jobs)
            {
                jobIds.Add(job.Id);
            }
            return jobIds;
        }

        [Test]
        [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryAudit)]
        public virtual void testBatchHistoricProcessInstanceDeletion()
        {
            IList<string> historicProcessInstances = setupHistory();

            //given
            authRule.Init(scenario).WithUser("userId").BindResource("batchId", "*").Start();

            // when
            historyService.DeleteHistoricProcessInstancesAsync(historicProcessInstances, TEST_REASON);

            // then
            authRule.AssertScenario(scenario);
        }

        protected internal virtual IList<string> setupHistory()
        {
            runtimeService.DeleteProcessInstance(processInstance.Id, null);
            IList<string> historicProcessInstances = new List<string>();

            foreach (IHistoricProcessInstance hpi in historyService.CreateHistoricProcessInstanceQuery().ToList())
            {
                historicProcessInstances.Add(hpi.Id);
            }
            return historicProcessInstances;
        }


    }

}