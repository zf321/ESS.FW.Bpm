using System.Collections.Generic;
using System.IO;
using System.Linq;
using Engine.Tests.Dmn.BusinessRuleTask;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.History.Impl.Event;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Task;
using ESS.FW.Bpm.Engine.Variable;
using NUnit.Framework;

namespace Engine.Tests.Api.History
{
    /// <summary>
    ///     Copy of <seealso cref="BulkHistoryDeleteTest" />, but testing another method:
    ///     <seealso cref="HistoryService#deleteHistoricProcessInstances(List)" />.
    /// </summary>
    [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull)]
    [TestFixture]
    public class HistoryDeleteTest
    {
        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(engineRule).around(testRule);
        //public RuleChain ruleChain;

        [SetUp]
        public virtual void init()
        {
            runtimeService = engineRule.RuntimeService;
            historyService = engineRule.HistoryService;
            taskService = engineRule.TaskService;
            formService = engineRule.FormService;
            externalTaskService = engineRule.ExternalTaskService;
        }

        protected internal const string ONE_TASK_PROCESS = "oneTaskProcess";

        public const int PROCESS_INSTANCE_COUNT = 5;

        public ProcessEngineRule engineRule = new ProcessEngineRule(true);
        private IExternalTaskService externalTaskService;
        private IFormService formService;

        private IHistoryService historyService;
        private readonly bool InstanceFieldsInitialized;
        private IRuntimeService runtimeService;
        private ITaskService taskService;
        public ProcessEngineTestRule testRule;

        public HistoryDeleteTest()
        {
            if (!InstanceFieldsInitialized)
            {
                InitializeInstanceFields();
                InstanceFieldsInitialized = true;
            }
        }

        private IVariableMap Variables
        {
            get
            {
                return ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                    .PutValue("aVariableName", "aVariableValue")
                    .PutValue("pojoVariableName", new TestPojo("someValue", 111.0));
            }
        }

        private void InitializeInstanceFields()
        {
            testRule = new ProcessEngineTestRule(engineRule);
            //ruleChain = RuleChain.outerRule(engineRule).around(testRule);
        }

        private string findAttachmentContentId(IList<IAttachment> attachments)
        {
            Assert.AreEqual(1, attachments.Count());
            return ((AttachmentEntity) attachments[0]).ContentId;
        }

        private void createProcessInstanceAttachmentWithContent(string ProcessInstanceId)
        {
            taskService.CreateAttachment("web page", null, ProcessInstanceId, "weatherforcast", "temperatures and more",
                new MemoryStream("someContent".GetBytes()));

            var taskAttachments = taskService.GetProcessInstanceAttachments(ProcessInstanceId);
            Assert.AreEqual(1, taskAttachments.Count());
            Assert.NotNull(taskService.GetAttachmentContent(taskAttachments[0].Id));
        }

        private void createTaskAttachmentWithContent(string taskId)
        {
            taskService.CreateAttachment("web page", taskId, null, "weatherforcast", "temperatures and more",
                new MemoryStream("someContent".GetBytes()));

            var taskAttachments = taskService.GetTaskAttachments(taskId);
            Assert.AreEqual(1, taskAttachments.Count());
            Assert.NotNull(taskService.GetAttachmentContent(taskAttachments[0].Id));
        }

        [Test][Deployment( "resources/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
        public virtual void testCleanupHistoricExternalTaskLog()
        {
            //given
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<String> ids = prepareHistoricProcesses("oneExternalTaskProcess");
            var ids = prepareHistoricProcesses("oneExternalTaskProcess");

            var workerId = "aWrokerId";
            var tasks = externalTaskService.FetchAndLock(1, workerId)
                //.Topic("externalTaskTopic", 10000L)
                // .Execute();
                ;
            externalTaskService.HandleFailure(tasks.First().Id, workerId, "errorMessage", "exceptionStackTrace", 5, 3000L);

            //remember errorDetailsByteArrayId
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String errorDetailsByteArrayId = findErrorDetailsByteArrayId("errorMessage");
            var errorDetailsByteArrayId = findErrorDetailsByteArrayId("errorMessage");

            runtimeService.DeleteProcessInstances(ids, null, true, true);

            //when
            historyService.DeleteHistoricProcessInstances(ids);

            //then
            Assert.AreEqual(0, historyService.CreateHistoricProcessInstanceQuery(c=>c.ProcessDefinitionKey==ONE_TASK_PROCESS)
                .Count());
            Assert.AreEqual(0, historyService.CreateHistoricExternalTaskLogQuery()
                .Count());
            //check that ByteArray was removed
            verifyByteArraysWereRemoved(errorDetailsByteArrayId);
        }

        private string findErrorDetailsByteArrayId(string errorMessage)
        {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<org.Camunda.bpm.Engine.history.IHistoricExternalTaskLog> historicExternalTaskLogs = historyService.CreateHistoricExternalTaskLogQuery().ErrorMessage(errorMessage).ToList();
            var historicExternalTaskLogs = historyService.CreateHistoricExternalTaskLogQuery()
                //.ErrorMessage(errorMessage)
                
                .ToList();
            Assert.AreEqual(1, historicExternalTaskLogs.Count());

            return ((HistoricExternalTaskLogEntity) historicExternalTaskLogs[0]).ErrorDetailsByteArrayId;
        }

        [Test][Deployment( "resources/api/mgmt/IncidentTest.TestShouldCreateOneIncident.bpmn" )]
        public virtual void testCleanupHistoricIncidents()
        {
            //given
            var ids = prepareHistoricProcesses("failingProcess");

            testRule.ExecuteAvailableJobs();

            runtimeService.DeleteProcessInstances(ids, null, true, true);

            //when
            historyService.DeleteHistoricProcessInstances(ids);

            //then
            Assert.AreEqual(0, historyService.CreateHistoricProcessInstanceQuery(c=>c.ProcessDefinitionKey=="failingProcess")
                .Count());
            Assert.AreEqual(0, historyService.CreateHistoricIncidentQuery()
                .Count());
        }

        [Test][Deployment( "resources/api/mgmt/IncidentTest.TestShouldCreateOneIncident.bpmn" ) ]
        public virtual void testCleanupHistoricJobLogs()
        {
            //given
            var ids = prepareHistoricProcesses("failingProcess", null, 1);

            testRule.ExecuteAvailableJobs();

            runtimeService.DeleteProcessInstances(ids, null, true, true);

            var byteArrayIds = findExceptionByteArrayIds();

            //when
            historyService.DeleteHistoricProcessInstances(ids);

            //then
            Assert.AreEqual(0, historyService.CreateHistoricProcessInstanceQuery(c=>c.ProcessDefinitionKey=="failingProcess")
                .Count());
            Assert.AreEqual(0, historyService.CreateHistoricJobLogQuery()
                .Count());

            verifyByteArraysWereRemoved(byteArrayIds.ToArray());
        }

        private IList<string> findExceptionByteArrayIds()
        {
            IList<string> exceptionByteArrayIds = new List<string>();
            var historicJobLogs = historyService.CreateHistoricJobLogQuery()
                
                .ToList();
            foreach (var historicJobLog in historicJobLogs)
            {
                var historicJobLogEventEntity = (HistoricJobLogEventEntity) historicJobLog;
                if (historicJobLogEventEntity.ExceptionByteArrayId != null)
                    exceptionByteArrayIds.Add(historicJobLogEventEntity.ExceptionByteArrayId);
            }
            return exceptionByteArrayIds;
        }

        [Test][Deployment( new [] {"resources/dmn/businessruletask/DmnBusinessRuleTaskTest.TestDecisionRef.bpmn20.xml", "resources/api/history/testDmnWithPojo.Dmn11.xml" }) ]
        public virtual void FAILING_testCleanupHistoryDecisionData()
        {
            //given
            var ids = prepareHistoricProcesses("testProcess", ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                .PutValue("pojo", new TestPojo("okay", 13.37)));

            runtimeService.DeleteProcessInstances(ids, null, true, true);

            //remember input and output ids
            var historicDecisionInstances = historyService.CreateHistoricDecisionInstanceQuery()
                /*.IncludeInputs()*/
                /*.IncludeOutputs()*/
                
                .ToList();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<String> historicDecisionInputIds = collectHistoricDecisionInputIds(historicDecisionInstances);
            var historicDecisionInputIds = collectHistoricDecisionInputIds(historicDecisionInstances);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<String> historicDecisionOutputIds = collectHistoricDecisionOutputIds(historicDecisionInstances);
            var historicDecisionOutputIds = collectHistoricDecisionOutputIds(historicDecisionInstances);

            //when
            historyService.DeleteHistoricProcessInstances(ids);

            //then
            Assert.AreEqual(0, historyService.CreateHistoricProcessInstanceQuery(c=>c.ProcessDefinitionKey=="testProcess")
                .Count());
            Assert.AreEqual(0, historyService.CreateHistoricDecisionInstanceQuery()
                .Count());

            //check that decision inputs and outputs were removed
            engineRule.ProcessEngineConfiguration.CommandExecutorTxRequired.Execute(new CommandAnonymousInnerClass(
                this, historicDecisionInputIds, historicDecisionOutputIds));
        }

        private IList<string> collectHistoricDecisionInputIds(IList<IHistoricDecisionInstance> historicDecisionInstances)
        {
            IList<string> result = new List<string>();
            foreach (var historicDecisionInstance in historicDecisionInstances)
            foreach (var inputInstanceEntity in historicDecisionInstance.Inputs)
                result.Add(inputInstanceEntity.Id);
            Assert.AreEqual(PROCESS_INSTANCE_COUNT, result.Count());
            return result;
        }

        private IList<string> collectHistoricDecisionOutputIds(
            IList<IHistoricDecisionInstance> historicDecisionInstances)
        {
            IList<string> result = new List<string>();
            foreach (var historicDecisionInstance in historicDecisionInstances)
            foreach (var outputInstanceEntity in historicDecisionInstance.Outputs)
                result.Add(outputInstanceEntity.Id);
            Assert.AreEqual(PROCESS_INSTANCE_COUNT, result.Count());
            return result;
        }

        private IList<string> prepareHistoricProcesses()
        {
            return prepareHistoricProcesses(ONE_TASK_PROCESS);
        }

        private IList<string> prepareHistoricProcesses(string businessKey)
        {
            return prepareHistoricProcesses(businessKey, null);
        }

        private IList<string> prepareHistoricProcesses(string businessKey, IVariableMap variables)
        {
            return prepareHistoricProcesses(businessKey, variables, PROCESS_INSTANCE_COUNT);
        }

        private IList<string> prepareHistoricProcesses(string businessKey, IVariableMap variables,
            int? processInstanceCount)
        {
            IList<string> processInstanceIds = new List<string>();

            for (var i = 0; i < processInstanceCount; i++)
            {
                var processInstance = runtimeService.StartProcessInstanceByKey(businessKey, variables);
                processInstanceIds.Add(processInstance.Id);
            }

            return processInstanceIds;
        }
        
        private void verifyByteArraysWereRemoved(params string[] errorDetailsByteArrayIds)
        {
            engineRule.ProcessEngineConfiguration.CommandExecutorTxRequired.Execute(new CommandAnonymousInnerClass2(
                this, errorDetailsByteArrayIds));
        }

        private class CommandAnonymousInnerClass : ICommand<object>
        {
            private readonly IList<string> historicDecisionInputIds;
            private readonly IList<string> historicDecisionOutputIds;
            private readonly HistoryDeleteTest outerInstance;

            public CommandAnonymousInnerClass(HistoryDeleteTest outerInstance, IList<string> historicDecisionInputIds,
                IList<string> historicDecisionOutputIds)
            {
                this.outerInstance = outerInstance;
                this.historicDecisionInputIds = historicDecisionInputIds;
                this.historicDecisionOutputIds = historicDecisionOutputIds;
            }

            public virtual object Execute(CommandContext commandContext)
            {
                //foreach (var inputId in historicDecisionInputIds)
                //    Assert.IsNull(commandContext.DbEntityManager.SelectById<HistoricDecisionInputInstanceEntity>(
                //        typeof(HistoricDecisionInputInstanceEntity), inputId));
                //foreach (var outputId in historicDecisionOutputIds)
                //    Assert.IsNull(commandContext.DbEntityManager.SelectById<HistoricDecisionOutputInstanceEntity>(
                //        typeof(HistoricDecisionOutputInstanceEntity), outputId));
                return null;
            }
        }

        private class CommandAnonymousInnerClass2 : ICommand<object>
        {
            private readonly string[] errorDetailsByteArrayIds;
            private readonly HistoryDeleteTest outerInstance;

            public CommandAnonymousInnerClass2(HistoryDeleteTest outerInstance, string[] errorDetailsByteArrayIds)
            {
                this.outerInstance = outerInstance;
                this.errorDetailsByteArrayIds = errorDetailsByteArrayIds;
            }

            public virtual object Execute(CommandContext commandContext)
            {
                foreach (var errorDetailsByteArrayId in errorDetailsByteArrayIds)
                    Assert.IsNull(commandContext.ByteArrayManager.Get( errorDetailsByteArrayId));
                return null;
            }
        }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void FAILING_testCleanupHistoricVariableInstancesAndHistoricDetails()
        {
            //given
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<String> ids = prepareHistoricProcesses();
            var ids = prepareHistoricProcesses();

            var taskList = taskService.CreateTaskQuery()
                
                .ToList();

            taskService.SetVariables(taskList[0].Id, Variables);

            runtimeService.DeleteProcessInstances(ids, null, true, true);

            //when
            historyService.DeleteHistoricProcessInstances(ids);

            //then
            Assert.AreEqual(0, historyService.CreateHistoricProcessInstanceQuery(c=>c.ProcessDefinitionKey==ONE_TASK_PROCESS)
                .Count());
            Assert.AreEqual(0, historyService.CreateHistoricDetailQuery()
                .Count());
            Assert.AreEqual(0, historyService.CreateHistoricVariableInstanceQuery()
                .Count());
        }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void FAILING_testCleanupProcessInstanceAttachmentWithContent()
        {
            //given
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<String> ids = prepareHistoricProcesses();
            var ids = prepareHistoricProcesses();

            var processInstanceWithAttachmentId = ids[0];
            createProcessInstanceAttachmentWithContent(processInstanceWithAttachmentId);
            //remember contentId
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String contentId = findAttachmentContentId(taskService.GetProcessInstanceAttachments(processInstanceWithAttachmentId));
            var contentId =
                findAttachmentContentId(taskService.GetProcessInstanceAttachments(processInstanceWithAttachmentId));

            runtimeService.DeleteProcessInstances(ids, null, true, true);

            //when
            historyService.DeleteHistoricProcessInstances(ids);

            //then
            Assert.AreEqual(0, historyService.CreateHistoricProcessInstanceQuery(c=>c.ProcessDefinitionKey==ONE_TASK_PROCESS)
                .Count());
            Assert.AreEqual(0, taskService.GetProcessInstanceAttachments(processInstanceWithAttachmentId)
                .Count());
            //check that attachment content was removed
            verifyByteArraysWereRemoved(contentId);
        }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void FAILING_testCleanupProcessInstanceComment()
        {
            //given
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<String> ids = prepareHistoricProcesses();
            var ids = prepareHistoricProcesses();

            var processInstanceWithCommentId = ids[0];
            taskService.CreateComment(null, processInstanceWithCommentId, "Some comment");

            runtimeService.DeleteProcessInstances(ids, null, true, true);

            //when
            historyService.DeleteHistoricProcessInstances(ids);

            //then
            Assert.AreEqual(0, historyService.CreateHistoricProcessInstanceQuery(c=>c.ProcessDefinitionKey==ONE_TASK_PROCESS)
                .Count());
            Assert.AreEqual(0, taskService.GetProcessInstanceComments(processInstanceWithCommentId)
                .Count());
        }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testCleanupHistoryActivityInstances()
        {
            //given
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<String> ids = prepareHistoricProcesses();
            var ids = prepareHistoricProcesses();

            runtimeService.DeleteProcessInstances(ids, null, true, true);

            //when
            historyService.DeleteHistoricProcessInstances(ids);

            //then
            Assert.AreEqual(0, historyService.CreateHistoricProcessInstanceQuery(c=>c.ProcessDefinitionKey==ONE_TASK_PROCESS)
                .Count());
            Assert.AreEqual(0, historyService.CreateHistoricActivityInstanceQuery()
                .Count());
        }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testCleanupHistoryEmptyProcessIdsException()
        {
            //given
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<String> ids = prepareHistoricProcesses();
            var ids = prepareHistoricProcesses();
            runtimeService.DeleteProcessInstances(ids, null, true, true);

            try
            {
                historyService.DeleteHistoricProcessInstances(null);
                Assert.Fail("Empty process instance ids exception was expected");
            }
            catch (BadUserRequestException)
            {
            }

            try
            {
                historyService.DeleteHistoricProcessInstances(new List<string>());
                Assert.Fail("Empty process instance ids exception was expected");
            }
            catch (BadUserRequestException)
            {
            }
        }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testCleanupHistoryTaskForm()
        {
            //given
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<String> ids = prepareHistoricProcesses();
            var ids = prepareHistoricProcesses();

            var taskList = taskService.CreateTaskQuery()
                
                .ToList();

            //formService.SubmitTaskForm(taskList[0].Id, Variables);

            foreach (var processInstance in runtimeService.CreateProcessInstanceQuery()
                
                .ToList())
                runtimeService.DeleteProcessInstance(processInstance.ProcessInstanceId, null);

            //when
            historyService.DeleteHistoricProcessInstances(ids);

            //then
            Assert.AreEqual(0, historyService.CreateHistoricProcessInstanceQuery(c=>c.ProcessDefinitionKey==ONE_TASK_PROCESS)
                .Count());
            Assert.AreEqual(0, historyService.CreateHistoricDetailQuery()
                .Count());
            Assert.AreEqual(0, historyService.CreateHistoricVariableInstanceQuery()
                .Count());
        }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testCleanupHistoryTaskIdentityLink()
        {
            //given
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<String> ids = prepareHistoricProcesses();
            var ids = prepareHistoricProcesses();
            var taskList = taskService.CreateTaskQuery()
                
                .ToList();
            taskService.AddUserIdentityLink(taskList[0].Id, "someUser", IdentityLinkType.Assignee);

            runtimeService.DeleteProcessInstances(ids, null, true, true);

            //when
            historyService.DeleteHistoricProcessInstances(ids);

            //then
            Assert.AreEqual(0, historyService.CreateHistoricProcessInstanceQuery(c=>c.ProcessDefinitionKey==ONE_TASK_PROCESS)
                .Count());
            Assert.AreEqual(0, historyService.CreateHistoricIdentityLinkLogQuery()
                .Count());
        }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testCleanupTaskAttachmentWithContent()
        {
            //given
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<String> ids = prepareHistoricProcesses();
            var ids = prepareHistoricProcesses();

            var taskList = taskService.CreateTaskQuery()
                
                .ToList();

            var taskWithAttachmentId = taskList[0].Id;
            createTaskAttachmentWithContent(taskWithAttachmentId);
            //remember contentId
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String contentId = findAttachmentContentId(taskService.GetTaskAttachments(taskWithAttachmentId));
            var contentId = findAttachmentContentId(taskService.GetTaskAttachments(taskWithAttachmentId));

            runtimeService.DeleteProcessInstances(ids, null, true, true);

            //when
            historyService.DeleteHistoricProcessInstances(ids);

            //then
            Assert.AreEqual(0, historyService.CreateHistoricProcessInstanceQuery(c=>c.ProcessDefinitionKey==ONE_TASK_PROCESS)
                .Count());
            Assert.AreEqual(0, taskService.GetTaskAttachments(taskWithAttachmentId)
                .Count());
            //check that attachment content was removed
            verifyByteArraysWereRemoved(contentId);
        }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testCleanupTaskComment()
        {
            //given
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<String> ids = prepareHistoricProcesses();
            var ids = prepareHistoricProcesses();

            var taskList = taskService.CreateTaskQuery()
                
                .ToList();

            var taskWithCommentId = taskList[2].Id;
            taskService.CreateComment(taskWithCommentId, null, "Some comment");

            runtimeService.DeleteProcessInstances(ids, null, true, true);

            //when
            historyService.DeleteHistoricProcessInstances(ids);

            //then
            Assert.AreEqual(0, historyService.CreateHistoricProcessInstanceQuery(c=>c.ProcessDefinitionKey==ONE_TASK_PROCESS)
                .Count());
            Assert.AreEqual(0, taskService.GetTaskComments(taskWithCommentId)
                .Count());
        }
    }
}