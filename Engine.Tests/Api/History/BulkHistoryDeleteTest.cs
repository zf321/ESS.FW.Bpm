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
    /// </summary>
    [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull)]
    [TestFixture]
    public class BulkHistoryDeleteTest
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
            caseService = engineRule.CaseService;
        }

        protected internal const string ONE_TASK_PROCESS = "oneTaskProcess";

        public const int PROCESS_INSTANCE_COUNT = 5;
        private ICaseService caseService;

        public ProcessEngineRule engineRule = new ProcessEngineRule(true);
        private IExternalTaskService externalTaskService;
        private IFormService formService;

        private IHistoryService historyService;
        private readonly bool InstanceFieldsInitialized;
        private IRuntimeService runtimeService;
        private ITaskService taskService;
        public ProcessEngineTestRule testRule;

        public BulkHistoryDeleteTest()
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
            Assert.AreEqual(1, attachments.Count);
            return ((AttachmentEntity) attachments[0]).ContentId;
        }

        private void createProcessInstanceAttachmentWithContent(string ProcessInstanceId)
        {
            taskService.CreateAttachment("web page", null, ProcessInstanceId, "weatherforcast", "temperatures and more",
                new MemoryStream("someContent".GetBytes()));

            var taskAttachments = taskService.GetProcessInstanceAttachments(ProcessInstanceId);
            Assert.AreEqual(1, taskAttachments.Count);
            Assert.NotNull(taskService.GetAttachmentContent(taskAttachments[0].Id));
        }

        private void createTaskAttachmentWithContent(string taskId)
        {
            taskService.CreateAttachment("web page", taskId, null, "weatherforcast", "temperatures and more",
                new MemoryStream("someContent".GetBytes()));

            var taskAttachments = taskService.GetTaskAttachments(taskId);
            Assert.AreEqual(1, taskAttachments.Count);
            Assert.NotNull(taskService.GetAttachmentContent(taskAttachments[0].Id));
        }

        [Test][Deployment( "resources/api/externaltask/oneExternalTaskProcess.bpmn20.xml") ]
        public virtual void testCleanupHistoricExternalTaskLog()
        {
            //given
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<String> ids = prepareHistoricProcesses("oneExternalTaskProcess");
            var ids = prepareHistoricProcesses("oneExternalTaskProcess");

            var workerId = "aWrokerId";
            var tasks = externalTaskService.FetchAndLock(1, workerId)
                //.Topic("externalTaskTopic", 10000L)
                //.Execute();
                ;
            externalTaskService.HandleFailure(tasks.First().Id, workerId, "errorMessage", "exceptionStackTrace", 5, 3000L);

            //remember errorDetailsByteArrayId
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String errorDetailsByteArrayId = findErrorDetailsByteArrayId("errorMessage");
            var errorDetailsByteArrayId = findErrorDetailsByteArrayId("errorMessage");

            runtimeService.DeleteProcessInstances(ids, null, true, true);

            //when
            historyService.DeleteHistoricProcessInstancesBulk(ids);

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
               // .ErrorMessage(errorMessage)
                
                .ToList();
            Assert.AreEqual(1, historicExternalTaskLogs.Count());

            return ((HistoricExternalTaskLogEntity) historicExternalTaskLogs[0]).ErrorDetailsByteArrayId;
        }

        [Test][Deployment(  "resources/api/mgmt/IncidentTest.TestShouldCreateOneIncident.bpmn" ) ]
        public virtual void testCleanupHistoricIncidents()
        {
            //given
            var ids = prepareHistoricProcesses("failingProcess");

            testRule.ExecuteAvailableJobs();

            runtimeService.DeleteProcessInstances(ids, null, true, true);

            //when
            historyService.DeleteHistoricProcessInstancesBulk(ids);

            //then
            Assert.AreEqual(0, historyService.CreateHistoricProcessInstanceQuery(c=>c.ProcessDefinitionKey=="failingProcess")
                .Count());
            Assert.AreEqual(0, historyService.CreateHistoricIncidentQuery()
                .Count());
        }

        [Test]
        [Deployment("resources/api/mgmt/IncidentTest.TestShouldCreateOneIncident.bpmn")]
        public virtual void testCleanupHistoricJobLogs()
        {
            //given
            var ids = prepareHistoricProcesses("failingProcess", null, 1);

            testRule.ExecuteAvailableJobs();

            runtimeService.DeleteProcessInstances(ids, null, true, true);

            var byteArrayIds = findExceptionByteArrayIds();

            //when
            historyService.DeleteHistoricProcessInstancesBulk(ids);

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
        [Test][Deployment( new []{"resources/dmn/businessruletask/DmnBusinessRuleTaskTest.TestDecisionRef.bpmn20.xml", "resources/api/history/testDmnWithPojo.Dmn11.xml" }) ]
        public virtual void testCleanupHistoryDecisionData()
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
//ORIGINAL LINE: final java.util.List<String> inputIds = new java.util.ArrayList<String>();
            IList<string> inputIds = new List<string>();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<String> inputByteArrayIds = new java.util.ArrayList<String>();
            IList<string> inputByteArrayIds = new List<string>();
            collectHistoricDecisionInputIds(historicDecisionInstances, inputIds, inputByteArrayIds);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<String> outputIds = new java.util.ArrayList<String>();
            IList<string> outputIds = new List<string>();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<String> outputByteArrayIds = new java.util.ArrayList<String>();
            IList<string> outputByteArrayIds = new List<string>();
            collectHistoricDecisionOutputIds(historicDecisionInstances, outputIds, outputByteArrayIds);

            //when
            historyService.DeleteHistoricDecisionInstancesBulk(extractIds(historicDecisionInstances));

            //then
            Assert.AreEqual(0, historyService.CreateHistoricDecisionInstanceQuery()
                .Count());

            //check that decision inputs and outputs were removed
            AssertDataDeleted(inputIds, inputByteArrayIds, outputIds, outputByteArrayIds);
        }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: void AssertDataDeleted(final java.util.List<String> inputIds, final java.util.List<String> inputByteArrayIds, final java.util.List<String> outputIds, final java.util.List<String> outputByteArrayIds)
        internal virtual void AssertDataDeleted(IList<string> inputIds, IList<string> inputByteArrayIds,
            IList<string> outputIds, IList<string> outputByteArrayIds)
        {
            engineRule.ProcessEngineConfiguration.CommandExecutorTxRequired.Execute(new CommandAnonymousInnerClass(
                this, inputIds, inputByteArrayIds, outputIds, outputByteArrayIds));
        }

        [Test][Deployment( "resources/api/history/testDmnWithPojo.Dmn11.xml" ) ]
        public virtual void testCleanupHistoryStandaloneDecisionData()
        {
            //given
            for (var i = 0; i < 5; i++)
                engineRule.DecisionService.EvaluateDecisionByKey("testDecision")
                    .Variables(ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                        .PutValue("pojo", new TestPojo("okay", 13.37)))
                    .Evaluate();

            //remember input and output ids
            var historicDecisionInstances = historyService.CreateHistoricDecisionInstanceQuery()
                /*.IncludeInputs()*/
                /*.IncludeOutputs()*/
                
                .ToList();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<String> inputIds = new java.util.ArrayList<String>();
            IList<string> inputIds = new List<string>();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<String> inputByteArrayIds = new java.util.ArrayList<String>();
            IList<string> inputByteArrayIds = new List<string>();
            collectHistoricDecisionInputIds(historicDecisionInstances, inputIds, inputByteArrayIds);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<String> outputIds = new java.util.ArrayList<String>();
            IList<string> outputIds = new List<string>();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<String> outputByteArrayIds = new java.util.ArrayList<String>();
            IList<string> outputByteArrayIds = new List<string>();
            collectHistoricDecisionOutputIds(historicDecisionInstances, outputIds, outputByteArrayIds);

            var decisionInstanceIds = extractIds(historicDecisionInstances);

            //when
            historyService.DeleteHistoricDecisionInstancesBulk(decisionInstanceIds);

            //then
            Assert.AreEqual(0, historyService.CreateHistoricProcessInstanceQuery(c=>c.ProcessDefinitionKey=="testProcess")
                .Count());
            Assert.AreEqual(0, historyService.CreateHistoricDecisionInstanceQuery()
                .Count());

            //check that decision inputs and outputs were removed
            AssertDataDeleted(inputIds, inputByteArrayIds, outputIds, outputByteArrayIds);
        }

        private IList<string> extractIds(IList<IHistoricDecisionInstance> historicDecisionInstances)
        {
            IList<string> decisionInstanceIds = new List<string>();
            foreach (var historicDecisionInstance in historicDecisionInstances)
                decisionInstanceIds.Add(historicDecisionInstance.Id);
            return decisionInstanceIds;
        }

        private void collectHistoricDecisionInputIds(IList<IHistoricDecisionInstance> historicDecisionInstances,
            IList<string> historicDecisionInputIds, IList<string> inputByteArrayIds)
        {
            foreach (var historicDecisionInstance in historicDecisionInstances)
            foreach (var inputInstanceEntity in historicDecisionInstance.Inputs)
            {
                historicDecisionInputIds.Add(inputInstanceEntity.Id);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String byteArrayValueId = ((org.Camunda.bpm.Engine.impl.history.Event.HistoricDecisionInputInstanceEntity) inputInstanceEntity).GetByteArrayValueId();
                var byteArrayValueId = ((HistoricDecisionInputInstanceEntity) inputInstanceEntity).ByteArrayId;
                if (!ReferenceEquals(byteArrayValueId, null))
                    inputByteArrayIds.Add(byteArrayValueId);
            }
            Assert.AreEqual(PROCESS_INSTANCE_COUNT, historicDecisionInputIds.Count);
        }

        private void collectHistoricDecisionOutputIds(IList<IHistoricDecisionInstance> historicDecisionInstances,
            IList<string> historicDecisionOutputIds, IList<string> outputByteArrayId)
        {
            foreach (var historicDecisionInstance in historicDecisionInstances)
            foreach (var outputInstanceEntity in historicDecisionInstance.Outputs)
            {
                historicDecisionOutputIds.Add(outputInstanceEntity.Id);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String byteArrayValueId = ((org.Camunda.bpm.Engine.impl.history.Event.HistoricDecisionOutputInstanceEntity) outputInstanceEntity).GetByteArrayValueId();
                var byteArrayValueId = ((HistoricDecisionOutputInstanceEntity) outputInstanceEntity).ByteArrayId;
                if (!ReferenceEquals(byteArrayValueId, null))
                    outputByteArrayId.Add(byteArrayValueId);
            }
            Assert.AreEqual(PROCESS_INSTANCE_COUNT, historicDecisionOutputIds.Count);
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

        [Test][Deployment(  "resources/api/cmmn/oneTaskCase.cmmn" ) ]
        public virtual void testCleanupHistoryCaseInstance()
        {
            // given
            // create case instances
            var instanceCount = 10;
            var caseInstanceIds = prepareHistoricCaseInstance(instanceCount);

            // assume
            var caseInstanceList = historyService.CreateHistoricCaseInstanceQuery()
                
                .ToList();
            Assert.AreEqual(instanceCount, caseInstanceList.Count);

            // when
            historyService.DeleteHistoricCaseInstancesBulk(caseInstanceIds);

            // then
            Assert.AreEqual(0, historyService.CreateHistoricCaseInstanceQuery()
                .Count());
            Assert.AreEqual(0, historyService.CreateHistoricTaskInstanceQuery()
                .Count());
        }

        [Test]
        [Deployment("resources/api/cmmn/oneTaskCase.cmmn")]
        public virtual void testCleanupHistoryCaseActivityInstance()
        {
            // given
            // create case instance
            var caseInstanceId = caseService.CreateCaseInstanceByKey("oneTaskCase")
                .Id;
            terminateAndCloseCaseInstance(caseInstanceId, null);

            // assume
            var activityInstances =
                historyService.CreateHistoricCaseActivityInstanceQuery()
                    
                    .ToList();
            Assert.AreEqual(1, activityInstances.Count);

            // when
            historyService.DeleteHistoricCaseInstancesBulk(new[] {caseInstanceId});

            // then
            activityInstances = historyService.CreateHistoricCaseActivityInstanceQuery()
                
                .ToList();
            Assert.AreEqual(0, activityInstances.Count);
        }

        [Test]
        [Deployment("resources/api/cmmn/oneTaskCase.cmmn")]
        public virtual void testCleanupHistoryCaseInstanceTask()
        {
            // given
            // create case instance
            var caseInstanceId = caseService.CreateCaseInstanceByKey("oneTaskCase")
                .Id;
            terminateAndCloseCaseInstance(caseInstanceId, null);

            // assume
            var taskInstances = historyService.CreateHistoricTaskInstanceQuery()
                
                .ToList();
            Assert.AreEqual(1, taskInstances.Count);

            // when
            historyService.DeleteHistoricCaseInstancesBulk(new[] {caseInstanceId});

            // then
            taskInstances = historyService.CreateHistoricTaskInstanceQuery()
                
                .ToList();
            Assert.AreEqual(0, taskInstances.Count);
        }

        [Test]
        [Deployment("resources/api/cmmn/oneTaskCase.cmmn")]
        public virtual void testCleanupHistoryCaseInstanceTaskComment()
        {
            // given
            // create case instance
            var caseInstanceId = caseService.CreateCaseInstanceByKey("oneTaskCase")
                .Id;

            var task = taskService.CreateTaskQuery()
                .First();
            taskService.CreateComment(task.Id, null, "This is a comment..");

            // assume
            var comments = taskService.GetTaskComments(task.Id);
            Assert.AreEqual(1, comments.Count);
            terminateAndCloseCaseInstance(caseInstanceId, null);

            // when
            historyService.DeleteHistoricCaseInstancesBulk(new[] {caseInstanceId});

            // then
            comments = taskService.GetTaskComments(task.Id);
            Assert.AreEqual(0, comments.Count);
        }

        [Test]
        [Deployment("resources/api/cmmn/oneTaskCase.cmmn")]
        public virtual void testCleanupHistoryCaseInstanceTaskDetails()
        {
            // given
            // create case instance
            var caseInstance = caseService.CreateCaseInstanceByKey("oneTaskCase");

            var task = taskService.CreateTaskQuery()
                .First();

            taskService.SetVariable(task.Id, "boo", new TestPojo("foo", 123.0));
            taskService.SetVariable(task.Id, "goo", 9);
            taskService.SetVariable(task.Id, "boo", new TestPojo("foo", 321.0));


            // assume
            var detailsList = historyService.CreateHistoricDetailQuery()
                
                .ToList();
            Assert.AreEqual(3, detailsList.Count);
            terminateAndCloseCaseInstance(caseInstance.Id, taskService.GetVariables(task.Id));

            // when
            historyService.DeleteHistoricCaseInstancesBulk(new[] {caseInstance.Id});

            // then
            detailsList = historyService.CreateHistoricDetailQuery()
                
                .ToList();
            Assert.AreEqual(0, detailsList.Count);
        }
        [Test]
        [Deployment("resources/api/cmmn/oneTaskCase.cmmn")]
        public virtual void testCleanupHistoryCaseInstanceTaskIdentityLink()
        {
            // given
            // create case instance
            var caseInstanceId = caseService.CreateCaseInstanceByKey("oneTaskCase")
                .Id;

            var task = taskService.CreateTaskQuery()
                .First();

            // assume
            taskService.AddGroupIdentityLink(task.Id, "accounting", IdentityLinkType.Candidate);
            var identityLinksForTask = taskService.GetIdentityLinksForTask(task.Id)
                .Count();
            Assert.AreEqual(1, identityLinksForTask);
            terminateAndCloseCaseInstance(caseInstanceId, null);

            // when
            historyService.DeleteHistoricCaseInstancesBulk(new[] {caseInstanceId});

            // then
            var historicIdentityLinkLog = historyService.CreateHistoricIdentityLinkLogQuery()
                
                .ToList();
            Assert.AreEqual(0, historicIdentityLinkLog.Count);
        }

        [Test]
        [Deployment("resources/api/cmmn/oneTaskCase.cmmn")]
        public virtual void testCleanupHistoryCaseInstanceTaskAttachmentByteArray()
        {
            // given
            // create case instance
            var caseInstance = caseService.CreateCaseInstanceByKey("oneTaskCase");

            var task = taskService.CreateTaskQuery()
                .First();
            var taskId = task.Id;
            taskService.CreateAttachment("foo", taskId, null, "something", null,
                new MemoryStream("someContent".GetBytes()));

            // assume
            var attachments = taskService.GetTaskAttachments(taskId);
            Assert.AreEqual(1, attachments.Count);
            var contentId = findAttachmentContentId(attachments);
            terminateAndCloseCaseInstance(caseInstance.Id, null);

            // when
            historyService.DeleteHistoricCaseInstancesBulk(new[] {caseInstance.Id});

            // then
            attachments = taskService.GetTaskAttachments(taskId);
            Assert.AreEqual(0, attachments.Count);
            verifyByteArraysWereRemoved(contentId);
        }

        [Test]
        [Deployment("resources/api/cmmn/oneTaskCase.cmmn")]
        public virtual void testCleanupHistoryCaseInstanceTaskAttachmentUrl()
        {
            // given
            // create case instance
            var caseInstanceId = caseService.CreateCaseInstanceByKey("oneTaskCase")
                .Id;

            var task = taskService.CreateTaskQuery()
                .First();
            taskService.CreateAttachment("foo", task.Id, null, "something", null, "http://camunda.org");

            // assume
            var attachments = taskService.GetTaskAttachments(task.Id);
            Assert.AreEqual(1, attachments.Count);
            terminateAndCloseCaseInstance(caseInstanceId, null);

            // when
            historyService.DeleteHistoricCaseInstancesBulk(new[] {caseInstanceId});

            // then
            attachments = taskService.GetTaskAttachments(task.Id);
            Assert.AreEqual(0, attachments.Count);
        }

        [Test]
        [Deployment("resources/api/cmmn/oneTaskCase.cmmn")]
        public virtual void testCleanupHistoryCaseInstanceVariables()
        {
            // given
            // create case instances
            IList<string> caseInstanceIds = new List<string>();
            var instanceCount = 10;
            for (var i = 0; i < instanceCount; i++)
            {
                var variables = ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables();
                var caseInstance = caseService.CreateCaseInstanceByKey("oneTaskCase",
                    variables.PutValue("name" + i, "theValue"));
                caseInstanceIds.Add(caseInstance.Id);
                terminateAndCloseCaseInstance(caseInstance.Id, variables);
            }
            // assume
            var variablesInstances = historyService.CreateHistoricVariableInstanceQuery()
                
                .ToList();
            Assert.AreEqual(instanceCount, variablesInstances.Count);

            // when
            historyService.DeleteHistoricCaseInstancesBulk(caseInstanceIds);

            // then
            variablesInstances = historyService.CreateHistoricVariableInstanceQuery()
                
                .ToList();
            Assert.AreEqual(0, variablesInstances.Count);
        }

        [Test]
        [Deployment("resources/api/cmmn/oneTaskCase.cmmn")]
        public virtual void testCleanupHistoryCaseInstanceComplexVariable()
        {
            // given
            // create case instances
            var variables = ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables();
            var caseInstance = caseService.CreateCaseInstanceByKey("oneTaskCase",
                variables.PutValue("pojo", new TestPojo("okay", 13.37)));

            caseService.SetVariable(caseInstance.Id, "pojo", "theValue");

            // assume
            var variablesInstances = historyService.CreateHistoricVariableInstanceQuery()
                
                .ToList();
            Assert.AreEqual(1, variablesInstances.Count);
            var detailsList = historyService.CreateHistoricDetailQuery()
                
                .ToList();
            Assert.AreEqual(2, detailsList.Count);
            terminateAndCloseCaseInstance(caseInstance.Id, variables);

            // when
            historyService.DeleteHistoricCaseInstancesBulk(new[] {caseInstance.Id});

            // then
            variablesInstances = historyService.CreateHistoricVariableInstanceQuery()
                
                .ToList();
            Assert.AreEqual(0, variablesInstances.Count);
            detailsList = historyService.CreateHistoricDetailQuery()
                
                .ToList();
            Assert.AreEqual(0, detailsList.Count);
        }

        [Test]
        [Deployment("resources/api/cmmn/oneTaskCase.cmmn")]
        public virtual void testCleanupHistoryCaseInstanceDetails()
        {
            // given
            // create case instances
            var variableNameCase1 = "varName1";
            var caseInstance1 = caseService.CreateCaseInstanceByKey("oneTaskCase", ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                .PutValue(variableNameCase1, "value1"));
            var caseInstance2 = caseService.CreateCaseInstanceByKey("oneTaskCase", ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                .PutValue("varName2", "value2"));

            caseService.SetVariable(caseInstance1.Id, variableNameCase1, "theValue");

            // assume
            var detailsList = historyService.CreateHistoricDetailQuery()
                
                .ToList();
            Assert.AreEqual(3, detailsList.Count);
            caseService.TerminateCaseExecution(caseInstance1.Id, caseService.GetVariables(caseInstance1.Id));
            caseService.TerminateCaseExecution(caseInstance2.Id, caseService.GetVariables(caseInstance2.Id));
            caseService.CloseCaseInstance(caseInstance1.Id);
            caseService.CloseCaseInstance(caseInstance2.Id);

            // when
            historyService.DeleteHistoricCaseInstancesBulk(new[] {caseInstance1.Id, caseInstance2.Id});

            // then
            detailsList = historyService.CreateHistoricDetailQuery()
                
                .ToList();
            Assert.AreEqual(0, detailsList.Count);
        }

        private IList<string> prepareHistoricCaseInstance(int instanceCount)
        {
            IList<string> caseInstanceIds = new List<string>();
            for (var i = 0; i < instanceCount; i++)
            {
                var caseInstance = caseService.CreateCaseInstanceByKey("oneTaskCase");
                var caseInstanceId = caseInstance.Id;
                caseInstanceIds.Add(caseInstanceId);
                terminateAndCloseCaseInstance(caseInstanceId, null);
            }
            return caseInstanceIds;
        }

        private void terminateAndCloseCaseInstance(string caseInstanceId, IDictionary<string, object> variables)
        {
            if (variables == null)
                caseService.TerminateCaseExecution(caseInstanceId, variables);
            else
                caseService.TerminateCaseExecution(caseInstanceId);
            caseService.CloseCaseInstance(caseInstanceId);
        }

        private class CommandAnonymousInnerClass : ICommand<object>
        {
            private readonly IList<string> inputByteArrayIds;

            private readonly IList<string> inputIds;
            private readonly BulkHistoryDeleteTest outerInstance;
            private readonly IList<string> outputByteArrayIds;
            private readonly IList<string> outputIds;

            public CommandAnonymousInnerClass(BulkHistoryDeleteTest outerInstance, IList<string> inputIds,
                IList<string> inputByteArrayIds, IList<string> outputIds, IList<string> outputByteArrayIds)
            {
                this.outerInstance = outerInstance;
                this.inputIds = inputIds;
                this.inputByteArrayIds = inputByteArrayIds;
                this.outputIds = outputIds;
                this.outputByteArrayIds = outputByteArrayIds;
            }

            public virtual object Execute(CommandContext commandContext)
            {
                foreach (var inputId in inputIds)
                    //Assert.IsNull(commandContext.DbEntityManager.SelectById<HistoricDecisionInputInstanceEntity>(
                    //    typeof(HistoricDecisionInputInstanceEntity), inputId));
                foreach (var inputByteArrayId in inputByteArrayIds)
                    Assert.IsNull(commandContext.ByteArrayManager.Get(
                        inputByteArrayId));
                foreach (var outputId in outputIds)
                    //Assert.IsNull(commandContext.DbEntityManager.SelectById<HistoricDecisionOutputInstanceEntity>(
                    //    typeof(HistoricDecisionOutputInstanceEntity), outputId));
                foreach (var outputByteArrayId in outputByteArrayIds)
                    Assert.IsNull(commandContext.ByteArrayManager.Get(
                        outputByteArrayId));
                return null;
            }
        }

        private class CommandAnonymousInnerClass2 : ICommand<object>
        {
            private readonly string[] errorDetailsByteArrayIds;
            private readonly BulkHistoryDeleteTest outerInstance;

            public CommandAnonymousInnerClass2(BulkHistoryDeleteTest outerInstance, string[] errorDetailsByteArrayIds)
            {
                this.outerInstance = outerInstance;
                this.errorDetailsByteArrayIds = errorDetailsByteArrayIds;
            }

            public virtual object Execute(CommandContext commandContext)
            {
                foreach (var errorDetailsByteArrayId in errorDetailsByteArrayIds)
                    Assert.IsNull(commandContext.ByteArrayManager.Get(errorDetailsByteArrayId));
                return null;
            }
        }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testCleanupHistoricVariableInstancesAndHistoricDetails()
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
            historyService.DeleteHistoricProcessInstancesBulk(ids);

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
        public virtual void testCleanupHistoryActivityInstances()
        {
            //given
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<String> ids = prepareHistoricProcesses();
            var ids = prepareHistoricProcesses();

            runtimeService.DeleteProcessInstances(ids, null, true, true);

            //when
            historyService.DeleteHistoricProcessInstancesBulk(ids);

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
                historyService.DeleteHistoricProcessInstancesBulk(null);
                Assert.Fail("Empty process instance ids exception was expected");
            }
            catch (BadUserRequestException)
            {
            }

            try
            {
                historyService.DeleteHistoricProcessInstancesBulk(new List<string>());
                Assert.Fail("Empty process instance ids exception was expected");
            }
            catch (BadUserRequestException)
            {
            }
        }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testCleanupHistoryProcessesNotFinishedException()
        {
            //given
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<String> ids = prepareHistoricProcesses();
            var ids = prepareHistoricProcesses();
            runtimeService.DeleteProcessInstances(ids.Skip(1)
                .ToArray(), null, true, true);

            try
            {
                historyService.DeleteHistoricProcessInstancesBulk(ids);
                Assert.Fail("Not all processes are finished exception was expected");
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
            historyService.DeleteHistoricProcessInstancesBulk(ids);

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
            historyService.DeleteHistoricProcessInstancesBulk(ids);

            //then
            Assert.AreEqual(0, historyService.CreateHistoricProcessInstanceQuery(c=>c.ProcessDefinitionKey==ONE_TASK_PROCESS)
                .Count());
            Assert.AreEqual(0, historyService.CreateHistoricIdentityLinkLogQuery()
                .Count());
        }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testCleanupProcessInstanceAttachmentWithContent()
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
            historyService.DeleteHistoricProcessInstancesBulk(ids);

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
        public virtual void testCleanupProcessInstanceComment()
        {
            //given
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<String> ids = prepareHistoricProcesses();
            var ids = prepareHistoricProcesses();

            var processInstanceWithCommentId = ids[0];
            taskService.CreateComment(null, processInstanceWithCommentId, "Some comment");

            runtimeService.DeleteProcessInstances(ids, null, true, true);

            //when
            historyService.DeleteHistoricProcessInstancesBulk(ids);

            //then
            Assert.AreEqual(0, historyService.CreateHistoricProcessInstanceQuery(c=>c.ProcessDefinitionKey==ONE_TASK_PROCESS)
                .Count());
            Assert.AreEqual(0, taskService.GetProcessInstanceComments(processInstanceWithCommentId)
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
            historyService.DeleteHistoricProcessInstancesBulk(ids);

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
            historyService.DeleteHistoricProcessInstancesBulk(ids);

            //then
            Assert.AreEqual(0, historyService.CreateHistoricProcessInstanceQuery(c=>c.ProcessDefinitionKey==ONE_TASK_PROCESS)
                .Count());
            Assert.AreEqual(0, taskService.GetTaskComments(taskWithCommentId)
                .Count());
        }
    }
}