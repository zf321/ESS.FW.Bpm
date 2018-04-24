using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Task;
using ESS.FW.Bpm.Engine.Variable;
using ESS.FW.Bpm.Engine.Variable.Value;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;

namespace Engine.Tests.Api.Authorization.History
{

    [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull)]
    public class UserOperationIdTest
    {
        private bool InstanceFieldsInitialized = false;

        public UserOperationIdTest()
        {
            if (!InstanceFieldsInitialized)
            {
                InitializeInstanceFields();
                InstanceFieldsInitialized = true;
            }
        }

        private void InitializeInstanceFields()
        {
            testRule = new ProcessEngineTestRule(engineRule);
        }

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Rule public ProcessEngineRule engineRule = new ProcessEngineRule();
        public ProcessEngineRule engineRule = new ProcessEngineRule();

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Rule public util.ProcessEngineTestRule testRule = new util.ProcessEngineTestRule(engineRule);
        public ProcessEngineTestRule testRule;

        protected internal const string PROCESS_KEY = "oneTaskProcess";
        protected internal string deploymentId;

        protected internal IRuntimeService runtimeService;
        protected internal IRepositoryService repositoryService;
        protected internal IHistoryService historyService;
        protected internal ITaskService taskService;
        protected internal IFormService formService;
        protected internal IIdentityService identityService;

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Before public void init()
        public virtual void init()
        {
            runtimeService = engineRule.RuntimeService;
            repositoryService = engineRule.RepositoryService;
            historyService = engineRule.HistoryService;
            taskService = engineRule.TaskService;
            formService = engineRule.FormService;
            identityService = engineRule.IdentityService;

        }

        [Test]
        [Deployment(new string[] { "resources/api/oneTaskProcess.bpmn20.xml" })]
        public virtual void testResolveTaskOperationId()
        {
            // given
            identityService.AuthenticatedUserId = "demo";
            runtimeService.StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = taskService.CreateTaskQuery().First().Id;

            // when
            taskService.ResolveTask(taskId, Variables);

            //then
            IList<IUserOperationLogEntry> userOperationLogEntries = historyService.CreateUserOperationLogQuery(c=>c.OperationType ==UserOperationLogEntryFields.OperationTypeResolve)
                //.TaskId(taskId)
                .Where(o=>o.Id==taskId)
                .ToList();
            IList<IHistoricDetail> historicDetails = historyService.CreateHistoricDetailQuery().ToList();
            verifySameOperationId(userOperationLogEntries, historicDetails);
        }

        [Test]
        [Deployment(new string[] { "resources/api/oneTaskProcess.bpmn20.xml" })]
        public virtual void testSubmitTaskFormOperationId()
        {
            // given
            identityService.AuthenticatedUserId = "demo";
            runtimeService.StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = taskService.CreateTaskQuery().First().Id;

            // when
            formService.SubmitTaskForm(taskId, Variables as IDictionary<string, ITypedValue>);

            //then
            IList<IUserOperationLogEntry> userOperationLogEntries = historyService.CreateUserOperationLogQuery(c=>c.OperationType ==UserOperationLogEntryFields.OperationTypeComplete)
               // .TaskId(taskId)
                .ToList();
            IList<IHistoricDetail> historicDetails = historyService.CreateHistoricDetailQuery()
                .ToList();
            verifySameOperationId(userOperationLogEntries, historicDetails);
        }

        [Test]
        [Deployment(new string[] { "resources/api/oneTaskProcess.bpmn20.xml" })]
        public virtual void testSetTaskVariablesOperationId()
        {
            // given
            identityService.AuthenticatedUserId = "demo";
            runtimeService.StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = taskService.CreateTaskQuery().First().Id;

            // when
            taskService.SetVariables(taskId, Variables);

            //then
            IList<IUserOperationLogEntry> userOperationLogEntries = historyService.CreateUserOperationLogQuery(c=>c.OperationType == UserOperationLogEntryFields.OperationTypeSetVariable)
                //.TaskId(taskId)
                .ToList();
        IList<IHistoricDetail> historicDetails = historyService.CreateHistoricDetailQuery()
                .ToList();
            verifySameOperationId(userOperationLogEntries, historicDetails);
        }

        [Test]
        [Deployment(new string[] { "resources/api/oneTaskProcess.bpmn20.xml" })]
        public virtual void testWithoutAuthentication()
        {
            // given
            runtimeService.StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = taskService.CreateTaskQuery().First().Id;

            // when
            taskService.ResolveTask(taskId, Variables);

            //then
            IList<IUserOperationLogEntry> userOperationLogEntries = historyService.CreateUserOperationLogQuery(c=>c.TaskId==taskId)
                .ToList();
            Assert.AreEqual(0, userOperationLogEntries.Count);
            IList<IHistoricDetail> historicDetails = historyService.CreateHistoricDetailQuery()
                .ToList();
            Assert.True(historicDetails.Count > 0);
            //history detail records must have null userOperationId as user operation log was not created
            foreach (IHistoricDetail historicDetail in historicDetails)
            {
                Assert.IsNull(historicDetail.UserOperationId);
            }
        }

        [Test]
        public virtual void testSetTaskVariablesInServiceTask()
        {
            // given
            IBpmnModelInstance bpmnModelInstance = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_KEY).StartEvent().UserTask().ServiceTask().CamundaExpression("${execution.SetVariable('foo', 'bar')}").EndEvent().Done();
            testRule.Deploy(bpmnModelInstance);

            identityService.AuthenticatedUserId = "demo";
            runtimeService.StartProcessInstanceByKey(PROCESS_KEY);
            ITask task = taskService.CreateTaskQuery().First();

            // when
            taskService.Complete(task.Id);

            //then
            IHistoricDetail historicDetail = historyService.CreateHistoricDetailQuery().First();
            // no user operation log id is set for this update, as it is not written as part of the user operation
            Assert.IsNull(historicDetail.UserOperationId);
        }

        private void verifySameOperationId(IList<IUserOperationLogEntry> userOperationLogEntries, IList<IHistoricDetail> historicDetails)
        {
            Assert.True(userOperationLogEntries.Count > 0, "Operation log entry must exist");
            string operationId = userOperationLogEntries[0].OperationId;
            Assert.NotNull(operationId);
            Assert.True(historicDetails.Count > 0, "Some historic details are expected to be present");
            foreach (IUserOperationLogEntry userOperationLogEntry in userOperationLogEntries)
            {
                Assert.AreEqual("OperationIds must be the same", operationId, userOperationLogEntry.OperationId);
            }
            foreach (IHistoricDetail historicDetail in historicDetails)
            {
                Assert.AreEqual("OperationIds must be the same", operationId, historicDetail.UserOperationId);
            }
        }

        protected internal virtual IVariableMap Variables
        {
            get
            {
                return ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables().PutValue("aVariableName", "aVariableValue").PutValue("anotherVariableName", "anotherVariableValue");
            }
        }

    }

}