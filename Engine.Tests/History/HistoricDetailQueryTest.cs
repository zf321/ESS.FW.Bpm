using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Variable;
using NUnit.Framework;

namespace Engine.Tests.History
{


    /// 
    /// <summary>
    /// 
    /// 
    /// </summary>
    [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull)]

    [TestFixture]
    public class HistoricDetailQueryTest
    {
        private bool InstanceFieldsInitialized = false;

        public HistoricDetailQueryTest()
        {
            if (!InstanceFieldsInitialized)
            {
                InitializeInstanceFields();
                InstanceFieldsInitialized = true;
            }
        }

        private void InitializeInstanceFields()
        {
            testHelper = new ProcessEngineTestRule(engineRule);
            //chain = RuleChain.outerRule(engineRule).around(testHelper);
        }


        protected internal const string PROCESS_KEY = "oneTaskProcess";

        public ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
        public ProcessEngineTestRule testHelper;

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain chain = org.junit.Rules.RuleChain.outerRule(engineRule).around(testHelper);
        //public RuleChain chain;


        protected internal IRuntimeService runtimeService;
        protected internal IManagementService managementService;
        protected internal IHistoryService historyService;
        protected internal ITaskService taskService;
        private IIdentityService identityService;

        [SetUp]
        public virtual void initServices()
        {
            runtimeService = engineRule.RuntimeService;
            managementService = engineRule.ManagementService;
            historyService = engineRule.HistoryService;
            taskService = engineRule.TaskService;
            identityService = engineRule.IdentityService;
        }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testQueryByUserOperationId()
        {
            startProcessInstance(PROCESS_KEY);

            identityService.AuthenticatedUserId = "demo";

            string taskId = taskService.CreateTaskQuery().First().Id;

            // when
            taskService.ResolveTask(taskId, Variables);

            //then
            string userOperationId = historyService.CreateHistoricDetailQuery().First().UserOperationId;

            IQueryable<IHistoricDetail> query = historyService.CreateHistoricDetailQuery(c => c.UserOperationId == userOperationId);

            Assert.AreEqual(1, query.Count());
            Assert.AreEqual(1, query.Count());
        }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testQueryByInvalidUserOperationId()
        {
            startProcessInstance(PROCESS_KEY);

            string taskId = taskService.CreateTaskQuery().First().Id;

            // when
            taskService.ResolveTask(taskId, Variables);

            //then
            IQueryable<IHistoricDetail> query = historyService.CreateHistoricDetailQuery(c => c.UserOperationId == "invalid");

            Assert.AreEqual(0, query.Count());
            Assert.AreEqual(0, query.Count());

            try
            {
                query.Where(c => c.UserOperationId == null);
                Assert.Fail("It was possible to set a null value as userOperationId.");
            }
            catch (ProcessEngineException)
            {
            }
        }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testQueryByExecutionId()
        {
            startProcessInstance(PROCESS_KEY);

            string taskId = taskService.CreateTaskQuery().First().Id;

            // when
            taskService.ResolveTask(taskId, Variables);

            //then
            string executionId = historyService.CreateHistoricDetailQuery().First().ExecutionId;

            IQueryable<IHistoricDetail> query = historyService.CreateHistoricDetailQuery(c => c.Id == executionId);

            Assert.AreEqual(1, query.Count());
            Assert.AreEqual(1, query.Count());
        }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testQueryByInvalidExecutionId()
        {
            startProcessInstance(PROCESS_KEY);

            string taskId = taskService.CreateTaskQuery().First().Id;

            // when
            taskService.ResolveTask(taskId, Variables);

            //then
            IQueryable<IHistoricDetail> query = historyService.CreateHistoricDetailQuery(c => c.Id == "invalid");

            Assert.AreEqual(0, query.Count());
            Assert.AreEqual(0, query.Count());
        }


        protected internal virtual IVariableMap Variables
        {
            get
            {
                return ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables().PutValue("aVariableName", "aVariableValue");
            }
        }

        protected internal virtual void startProcessInstance(string key)
        {
            startProcessInstances(key, 1);
        }

        protected internal virtual void startProcessInstances(string key, int numberOfInstances)
        {
            for (int i = 0; i < numberOfInstances; i++)
            {
                runtimeService.StartProcessInstanceByKey(key);
            }
            testHelper.ExecuteAvailableJobs();
        }
    }
}