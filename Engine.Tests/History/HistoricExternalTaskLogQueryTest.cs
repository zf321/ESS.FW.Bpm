//using System.Collections.Generic;
//using System.Linq;
//using ESS.FW.Bpm.Engine.exception;
//using ESS.FW.Bpm.Engine.Externaltask;
//using ESS.FW.Bpm.Engine.History;
//using ESS.FW.Bpm.Engine.Runtime;
//using ESS.FW.Bpm.Engine.Tests.Api.Authorization.Util;
//using ESS.FW.Bpm.Engine.Tests.Api.Runtime.Migration.Models.Builder;
//using ESS.FW.Bpm.Engine.Tests.Util;
//using NUnit.Framework;

//namespace ESS.FW.Bpm.Engine.Tests.History
//{
//    [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull)]
//    [TestFixture]
//    public class HistoricExternalTaskLogQueryTest
//    {
//        [SetUp]
//        public virtual void setUp()
//        {
//            runtimeService = engineRule.RuntimeService;
//            historyService = engineRule.HistoryService;
//            externalTaskService = engineRule.ExternalTaskService;
//        }

//        private readonly bool InstanceFieldsInitialized;

//        public HistoricExternalTaskLogQueryTest()
//        {
//            if (!InstanceFieldsInitialized)
//            {
//                InitializeInstanceFields();
//                InstanceFieldsInitialized = true;
//            }
//        }

//        private void InitializeInstanceFields()
//        {
//            authRule = new AuthorizationTestRule(engineRule);
//            testHelper = new ProcessEngineTestRule(engineRule);
//            ////ruleChain = RuleChain.outerRule(engineRule).around(authRule).around(testHelper);
//        }


//        protected internal readonly string WORKER_ID = "aWorkerId";
//        protected internal readonly long LOCK_DURATION = 5 * 60L * 1000L;

//        protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
//        protected internal AuthorizationTestRule authRule;
//        protected internal ProcessEngineTestRule testHelper;

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(engineRule).around(authRule).around(testHelper);
//        ////public RuleChain ruleChain;

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @Rule public final org.junit.Rules.ExpectedException thrown = org.junit.Rules.ExpectedException.None();
//        //public readonly ExpectedException thrown = ExpectedException.None();

//        protected internal IProcessInstance processInstance;
//        protected internal IRuntimeService runtimeService;
//        protected internal IHistoryService historyService;
//        protected internal IExternalTaskService externalTaskService;

//        [Test]
//        public virtual void testQueryByNonExistingId()
//        {
//            // given
//            startExternalTaskProcess();

//            // when
//            var log = historyService.CreateHistoricExternalTaskLogQuery()
//                .LogId("foo")
//                .First();

//            // then
//            Assert.IsNull(log);
//        }

//        [Test]
//        public virtual void testQueryByExternalTaskId()
//        {
//            // given
//            startExternalTaskProcesses(2);
//            var logExternalTaskId = retrieveFirstHistoricExternalTaskLog()
//                .ExternalTaskId;

//            // when
//            var log = historyService.CreateHistoricExternalTaskLogQuery()
//                .ExternalTaskId(logExternalTaskId)
//                .First();

//            // then
//            Assert.NotNull(log);
//            Assert.That(log.ExternalTaskId, Is.EqualTo(logExternalTaskId));
//        }

//        [Test]
//        [ExpectedException(typeof(NotValidException))]
//        public virtual void testQueryFailsByInvalidExternalTaskId()
//        {
//            // given
//            startExternalTaskProcess();

//            // then
//            //thrown.Expect(typeof(NotValidException));

//            // when
//            historyService.CreateHistoricExternalTaskLogQuery()
//                .ExternalTaskId(null)
//                .First();
//        }

//        [Test]
//        public virtual void testQueryByNonExistingExternalTaskId()
//        {
//            // given
//            startExternalTaskProcess();

//            // when
//            var log = historyService.CreateHistoricExternalTaskLogQuery()
//                .ExternalTaskId("foo")
//                .First();

//            // then
//            Assert.IsNull(log);
//        }

//        [Test]
//        public virtual void testQueryByTopicName()
//        {
//            // given
//            var dummyTopic = "dummy";
//            startExternalTaskProcessGivenTopicName(dummyTopic);
//            var task = startExternalTaskProcess();

//            // when
//            var log = historyService.CreateHistoricExternalTaskLogQuery()
//                .TopicName(DefaultExternalTaskModelBuilder.DefaultTopic)
//                .First();

//            // then
//            Assert.NotNull(log);
//            Assert.That(log.ExternalTaskId, Is.EqualTo(task.Id));
//        }

//        [Test]
//        [ExpectedException(typeof(NotValidException))]
//        public virtual void testQueryFailsByInvalidTopicName()
//        {
//            // given
//            startExternalTaskProcess();

//            // then
//            //thrown.Expect(typeof(NotValidException));

//            // when
//            historyService.CreateHistoricExternalTaskLogQuery()
//                .TopicName(null)
//                .First();
//        }

//        [Test]
//        public virtual void testQueryByNonExistingTopicName()
//        {
//            // given
//            startExternalTaskProcess();

//            // when
//            var log = historyService.CreateHistoricExternalTaskLogQuery()
//                .TopicName("foo bar")
//                .First();

//            // then
//            Assert.IsNull(log);
//        }
//        [Test]
//        public virtual void testQueryByWorkerId()
//        {
//            // given
//            var taskList = startExternalTaskProcesses(2);
//            var taskToCheck = taskList[1];
//            completeExternalTaskWithWorker(taskList[0].Id, "dummyWorker");
//            completeExternalTaskWithWorker(taskToCheck.Id, WORKER_ID);

//            // when
//            var log = historyService.CreateHistoricExternalTaskLogQuery()
//                .WorkerId(WORKER_ID)
//                .First();

//            // then
//            Assert.NotNull(log);
//            Assert.That(log.ExternalTaskId, Is.EqualTo(taskToCheck.Id));
//        }

//        [Test]
//        [ExpectedException(typeof(NotValidException))]
//        public virtual void testQueryFailsByInvalidWorkerId()
//        {
//            // given
//            var task = startExternalTaskProcess();
//            completeExternalTask(task.Id);

//            // then
//            //thrown.Expect(typeof(NotValidException));

//            // when
//            historyService.CreateHistoricExternalTaskLogQuery()
//                .WorkerId(null)
//                .First();
//        }

//        [Test]
//        public virtual void testQueryByNonExistingWorkerId()
//        {
//            // given
//            var task = startExternalTaskProcess();
//            completeExternalTask(task.Id);

//            // when
//            var log = historyService.CreateHistoricExternalTaskLogQuery()
//                .WorkerId("foo")
//                .First();

//            // then
//            Assert.IsNull(log);
//        }

//        [Test]
//        public virtual void testQueryByErrorMessage()
//        {
//            // given
//            var taskList = startExternalTaskProcesses(2);
//            var errorMessage = "This is an important error!";
//            reportExternalTaskFailure(taskList[0].Id, "Dummy error message");
//            reportExternalTaskFailure(taskList[1].Id, errorMessage);

//            // when
//            var log = historyService.CreateHistoricExternalTaskLogQuery()
//                .ErrorMessage(errorMessage)
//                .First();

//            // then
//            Assert.NotNull(log);
//            Assert.That(log.ExternalTaskId, Is.EqualTo(taskList[1].Id));
//        }

//        [Test]
//        [ExpectedException(typeof(NotValidException))]
//        public virtual void testQueryFailsByInvalidErrorMessage()
//        {
//            // given
//            startExternalTaskProcess();

//            // then
//            //thrown.Expect(typeof(NotValidException));

//            // when
//            historyService.CreateHistoricExternalTaskLogQuery()
//                .ErrorMessage(null)
//                .First();
//        }

//        [Test]
//        public virtual void testQueryByNonExistingErrorMessage()
//        {
//            // given
//            startExternalTaskProcess();

//            // when
//            var log = historyService.CreateHistoricExternalTaskLogQuery()
//                .ErrorMessage("asdfasdf")
//                .First();

//            // then
//            Assert.IsNull(log);
//        }

//        [Test]
//        public virtual void testQueryByActivityId()
//        {
//            // given
//            startExternalTaskProcessGivenActivityId("dummyName");
//            var task = startExternalTaskProcess();

//            // when
//            var log = historyService.CreateHistoricExternalTaskLogQuery()
//                .ActivityIdIn(DefaultExternalTaskModelBuilder.DefaultExternalTaskName)
//                .First();

//            // then
//            Assert.NotNull(log);
//            Assert.That(log.ExternalTaskId, Is.EqualTo(task.Id));
//        }

//        [Test]
//        [ExpectedException(typeof(NotValidException))]
//        public virtual void testQueryFailsByActivityIdsIsNull()
//        {
//            // given
//            startExternalTaskProcess();

//            // then
//            //thrown.Expect(typeof(NotValidException));

//            // when
//            historyService.CreateHistoricExternalTaskLogQuery()
//                .ActivityIdIn(null)
//                .First();
//        }

//        [Test]
//        [ExpectedException(typeof(NotValidException))]
//        public virtual void testQueryFailsByActivityIdsContainNull()
//        {
//            // given
//            startExternalTaskProcess();

//            // then
//            //thrown.Expect(typeof(NotValidException));

//            // when
//            string[] activityIdsContainNull = {"a", null, "b"};
//            historyService.CreateHistoricExternalTaskLogQuery()
//                .ActivityIdIn(activityIdsContainNull)
//                .First();
//        }

//        [Test]
//        [ExpectedException(typeof(NotValidException))]
//        public virtual void testQueryFailsByActivityIdsContainEmptyString()
//        {
//            // given
//            startExternalTaskProcess();

//            // then
//            //thrown.Expect(typeof(NotValidException));

//            // when
//            string[] activityIdsContainEmptyString = {"a", "", "b"};
//            historyService.CreateHistoricExternalTaskLogQuery()
//                .ActivityIdIn(activityIdsContainEmptyString)
//                .First();
//        }

//        [Test]
//        public virtual void testQueryByNonExistingActivityIds()
//        {
//            // given
//            startExternalTaskProcess();

//            // when
//            var log = historyService.CreateHistoricExternalTaskLogQuery()
//                .ActivityIdIn("foo")
//                .First();

//            // then
//            Assert.IsNull(log);
//        }

//        [Test]
//        public virtual void testQueryByActivityInstanceIds()
//        {
//            // given
//            startExternalTaskProcessGivenActivityId("dummyName");
//            var task = startExternalTaskProcess();
//            var activityInstanceId = historyService.CreateHistoricActivityInstanceQuery(c=> c.ActivityId ==DefaultExternalTaskModelBuilder.DefaultExternalTaskName)
//                .First()
//                .Id;

//            // when
//            var log = historyService.CreateHistoricExternalTaskLogQuery()
//                .ActivityInstanceIdIn(activityInstanceId)
//                .First();

//            // then
//            Assert.NotNull(log);
//            Assert.That(log.ExternalTaskId, Is.EqualTo(task.Id));
//        }
//        [Test]
//        [ExpectedException(typeof(NotValidException))]
//        public virtual void testQueryFailsByActivityInstanceIdsIsNull()
//        {
//            // given
//            startExternalTaskProcess();

//            // then
//            //thrown.Expect(typeof(NotValidException));

//            // when
//            historyService.CreateHistoricExternalTaskLogQuery()
//                .ActivityInstanceIdIn(null)
//                .First();
//        }

//        [Test]
//        [ExpectedException(typeof(NotValidException))]
//        public virtual void testQueryFailsByActivityInstanceIdsContainNull()
//        {
//            // given
//            startExternalTaskProcess();

//            // then
//            //thrown.Expect(typeof(NotValidException));

//            // when
//            string[] activityIdsContainNull = {"a", null, "b"};
//            historyService.CreateHistoricExternalTaskLogQuery()
//                .ActivityInstanceIdIn(activityIdsContainNull)
//                .First();
//        }

//        [Test]
//        [ExpectedException(typeof(NotValidException))]
//        public virtual void testQueryFailsByActivityInstanceIdsContainEmptyString()
//        {
//            // given
//            startExternalTaskProcess();

//            // then
//            //thrown.Expect(typeof(NotValidException));

//            // when
//            string[] activityIdsContainEmptyString = {"a", "", "b"};
//            historyService.CreateHistoricExternalTaskLogQuery()
//                .ActivityInstanceIdIn(activityIdsContainEmptyString)
//                .First();
//        }

//        [Test]
//        public virtual void testQueryByNonExistingActivityInstanceIds()
//        {
//            // given
//            startExternalTaskProcess();

//            // when
//            var log = historyService.CreateHistoricExternalTaskLogQuery()
//                .ActivityInstanceIdIn("foo")
//                .First();

//            // then
//            Assert.IsNull(log);
//        }
//        [Test]
//        public virtual void testQueryByExecutionIds()
//        {
//            // given
//            startExternalTaskProcesses(2);
//            var taskLog = retrieveFirstHistoricExternalTaskLog();

//            // when
//            var log = historyService.CreateHistoricExternalTaskLogQuery()
//                .ExecutionIdIn(taskLog.ExecutionId)
//                .First();

//            // then
//            Assert.NotNull(log);
//            Assert.That(log.Id, Is.EqualTo(taskLog.Id));
//        }

//        [Test]
//        [ExpectedException(typeof(NotValidException))]
//        public virtual void testQueryFailsByExecutionIdsIsNull()
//        {
//            // given
//            startExternalTaskProcess();

//            // then
//            //thrown.Expect(typeof(NotValidException));

//            // when
//            historyService.CreateHistoricExternalTaskLogQuery()
//                .ExecutionIdIn(null)
//                .First();
//        }

//        [Test]
//        [ExpectedException(typeof(NotValidException))]
//        public virtual void testQueryFailsByExecutionIdsContainNull()
//        {
//            // given
//            startExternalTaskProcess();

//            // then
//            //thrown.Expect(typeof(NotValidException));

//            // when
//            string[] activityIdsContainNull = {"a", null, "b"};
//            historyService.CreateHistoricExternalTaskLogQuery()
//                .ExecutionIdIn(activityIdsContainNull)
//                .First();
//        }

//        [Test]
//        [ExpectedException(typeof(NotValidException))]
//        public virtual void testQueryFailsByExecutionIdsContainEmptyString()
//        {
//            // given
//            startExternalTaskProcess();

//            // then
//            //thrown.Expect(typeof(NotValidException));

//            // when
//            string[] activityIdsContainEmptyString = {"a", "", "b"};
//            historyService.CreateHistoricExternalTaskLogQuery()
//                .ExecutionIdIn(activityIdsContainEmptyString)
//                .First();
//        }

//        [Test]
//        public virtual void testQueryByNonExistingExecutionIds()
//        {
//            // given
//            startExternalTaskProcess();

//            // when
//            var log = historyService.CreateHistoricExternalTaskLogQuery()
//                .ExecutionIdIn("foo")
//                .First();

//            // then
//            Assert.IsNull(log);
//        }

//        [Test]
//        public virtual void testQueryByProcessInstanceId()
//        {
//            // given
//            startExternalTaskProcesses(2);
//            var ProcessInstanceId = retrieveFirstHistoricExternalTaskLog()
//                .ProcessInstanceId;

//            // when
//            var log = historyService.CreateHistoricExternalTaskLogQuery(c=>c.ProcessInstanceId== ProcessInstanceId)
//                .First();

//            // then
//            Assert.NotNull(log);
//            Assert.That(log.ProcessInstanceId, Is.EqualTo(ProcessInstanceId));
//        }

//        [Test]
//        [ExpectedException(typeof(NotValidException))]
//        public virtual void testQueryFailsByInvalidProcessInstanceId()
//        {
//            // given
//            startExternalTaskProcess();

//            // then
//            //thrown.Expect(typeof(NotValidException));

//            // when
//            historyService.CreateHistoricExternalTaskLogQuery(c=>c.ProcessInstanceId== null)
//                .First();
//        }

//        [Test]
//        public virtual void testQueryByNonExistingProcessInstanceId()
//        {
//            // given
//            startExternalTaskProcess();

//            // when
//            var log = historyService.CreateHistoricExternalTaskLogQuery(c=>c.ProcessInstanceId== "foo")
//                .First();

//            // then
//            Assert.IsNull(log);
//        }

//        [Test]
//        public virtual void testQueryByProcessDefinitionId()
//        {
//            // given
//            startExternalTaskProcesses(2);
//            var definitionId = retrieveFirstHistoricExternalTaskLog()
//                .ProcessDefinitionId;

//            // when
//            var log = historyService.CreateHistoricExternalTaskLogQuery(c=>c.ProcessDefinitionId==definitionId)
//                .First();

//            // then
//            Assert.NotNull(log);
//            Assert.That(log.ProcessDefinitionId, Is.EqualTo(definitionId));
//        }

//        [Test]
//        [ExpectedException(typeof(NotValidException))]
//        public virtual void testQueryFailsByInvalidProcessDefinitionId()
//        {
//            // given
//            startExternalTaskProcess();

//            // then
//            //thrown.Expect(typeof(NotValidException));

//            // when
//            historyService.CreateHistoricExternalTaskLogQuery(c=>c.ProcessDefinitionId==null)
//                .First();
//        }
//        [Test]
//        public virtual void testQueryByNonExistingProcessDefinitionId()
//        {
//            // given
//            startExternalTaskProcess();

//            // when
//            var log = historyService.CreateHistoricExternalTaskLogQuery(c=>c.ProcessDefinitionId=="foo")
//                .First();

//            // then
//            Assert.IsNull(log);
//        }

//        [Test]
//        public virtual void testQueryByProcessDefinitionKey()
//        {
//            // given
//            startExternalTaskProcessGivenProcessDefinitionKey("dummyProcess");
//            var task = startExternalTaskProcessGivenProcessDefinitionKey("Process");

//            // when
//            var log = historyService.CreateHistoricExternalTaskLogQuery(c=>c.ProcessDefinitionKey==task.ProcessDefinitionKey)
//                .First();

//            // then
//            Assert.NotNull(log);
//            Assert.That(log.ExternalTaskId, Is.EqualTo(task.Id));
//        }
//        [Test]
//        [ExpectedException(typeof(NotValidException))]
//        public virtual void testQueryFailsByInvalidProcessDefinitionKey()
//        {
//            // given
//            startExternalTaskProcess();

//            // then
//            //thrown.Expect(typeof(NotValidException));

//            // when
//            historyService.CreateHistoricExternalTaskLogQuery(c=>c.ProcessDefinitionKey==null)
//                .First();
//        }

//        [Test]
//        public virtual void testQueryByNonExistingProcessDefinitionKey()
//        {
//            // given
//            startExternalTaskProcess();

//            // when
//            var log = historyService.CreateHistoricExternalTaskLogQuery(c=>c.ProcessDefinitionKey=="foo")
//                .First();

//            // then
//            Assert.IsNull(log);
//        }

//        [Test]
//        public virtual void testQueryByCreationLog()
//        {
//            // given
//            var task = startExternalTaskProcess();
//            completeExternalTask(task.Id);

//            // when
//            var log = historyService.CreateHistoricExternalTaskLogQuery(c=> c.State == JobStateFields.Created.StateCode)
//                .First();

//            // then
//            Assert.NotNull(log);
//            Assert.That(log.ExternalTaskId, Is.EqualTo(task.Id));
//        }

//        [Test]
//        public virtual void testQueryByFailureLog()
//        {
//            // given
//            var task = startExternalTaskProcess();
//            reportExternalTaskFailure(task.Id, "Dummy error message!");

//            // when
//            var log = historyService.CreateHistoricExternalTaskLogQuery(c=>c.State == JobStateFields.Failed.StateCode)
//                .First();

//            // then
//            Assert.NotNull(log);
//            Assert.That(log.ExternalTaskId, Is.EqualTo(task.Id));
//        }

//        [Test]
//        public virtual void testQueryBySuccessLog()
//        {
//            // given
//            var task = startExternalTaskProcess();
//            completeExternalTask(task.Id);

//            // when
//            var log = historyService.CreateHistoricExternalTaskLogQuery(c=>c.State == JobStateFields.Successful.StateCode)
//                .First();

//            // then
//            Assert.NotNull(log);
//            Assert.That(log.ExternalTaskId, Is.EqualTo(task.Id));
//        }

//        [Test]
//        public virtual void testQueryByDeletionLog()
//        {
//            // given
//            var task = startExternalTaskProcess();
//            runtimeService.DeleteProcessInstance(task.ProcessInstanceId, null);

//            // when
//            var log = historyService.CreateHistoricExternalTaskLogQuery(c=> c.State == JobStateFields.Deleted.StateCode)
//                .First();

//            // then
//            Assert.NotNull(log);
//            Assert.That(log.ExternalTaskId, Is.EqualTo(task.Id));
//        }

//        [Test]
//        public virtual void testQueryByLowerThanOrEqualAPriority()
//        {
//            // given
//            startExternalTaskProcesses(5);

//            // when
//            var externalTaskLogs = historyService.CreateHistoricExternalTaskLogQuery()
//                .PriorityLowerThanOrEquals(2L)
                
//                .ToList();

//            // then
//            Assert.That(externalTaskLogs.Count, Is.EqualTo(3));
//            foreach (var log in externalTaskLogs)
//                Assert.True(log.Priority <= 2);
//        }

//        [Test]
//        public virtual void testQueryByHigherThanOrEqualAPriority()
//        {
//            // given
//            startExternalTaskProcesses(5);

//            // when
//            var externalTaskLogs = historyService.CreateHistoricExternalTaskLogQuery()
//                //.PriorityHigherThanOrEquals(2L)
                
//                .ToList();

//            // then
//            Assert.That(externalTaskLogs.Count, Is.EqualTo(3));
//            foreach (var log in externalTaskLogs)
//                Assert.True(log.Priority >= 2);
//        }

//        [Test]
//        public virtual void testQueryByPriorityRange()
//        {
//            // given
//            startExternalTaskProcesses(5);

//            // when
//            var externalTaskLogs = historyService.CreateHistoricExternalTaskLogQuery()
//                .PriorityLowerThanOrEquals(3L)
//                //.PriorityHigherThanOrEquals(1L)
                
//                .ToList();

//            // then
//            Assert.That(externalTaskLogs.Count, Is.EqualTo(3));
//            foreach (var log in externalTaskLogs)
//                Assert.True(log.Priority <= 3 && log.Priority >= 1);
//        }

//        [Test]
//        public virtual void testQueryByDisjunctivePriorityStatements()
//        {
//            // given
//            startExternalTaskProcesses(5);

//            // when
//            var externalTaskLogs = historyService.CreateHistoricExternalTaskLogQuery()
//                .PriorityLowerThanOrEquals(1L)
//                //.PriorityHigherThanOrEquals(4L)
                
//                .ToList();

//            // then
//            Assert.That(externalTaskLogs.Count, Is.EqualTo(0));
//        }


//        // helper methods

//        protected internal virtual IHistoricExternalTaskLog retrieveFirstHistoricExternalTaskLog()
//        {
//            return historyService.CreateHistoricExternalTaskLogQuery()
                
//                .First();
//        }

//        protected internal virtual void completeExternalTaskWithWorker(string externalTaskId, string workerId)
//        {
//            completeExternalTask(externalTaskId, DefaultExternalTaskModelBuilder.DefaultTopic, workerId, false);
//        }

//        protected internal virtual void completeExternalTask(string externalTaskId)
//        {
//            completeExternalTask(externalTaskId, DefaultExternalTaskModelBuilder.DefaultTopic, WORKER_ID, false);
//        }

//        protected internal virtual void completeExternalTask(string externalTaskId, string topic, string workerId,
//            bool usePriority)
//        {
//            var list = externalTaskService.FetchAndLock(100, workerId, usePriority)
//                .Topic(topic, LOCK_DURATION)
//                .Execute();
//            externalTaskService.Complete(externalTaskId, workerId);
//            // unlock the remaining tasks
//            foreach (var lockedExternalTask in list)
//                if (!lockedExternalTask.Id.Equals(externalTaskId))
//                    externalTaskService.Unlock(lockedExternalTask.Id);
//        }

//        protected internal virtual void reportExternalTaskFailure(string externalTaskId, string errorMessage)
//        {
//            reportExternalTaskFailure(externalTaskId, DefaultExternalTaskModelBuilder.DefaultTopic, WORKER_ID, 1, false,
//                errorMessage);
//        }

//        protected internal virtual void reportExternalTaskFailure(string externalTaskId, string topic, string workerId,
//            int retries, bool usePriority, string errorMessage)
//        {
//            var list = externalTaskService.FetchAndLock(100, workerId, usePriority)
//                .Topic(topic, LOCK_DURATION)
//                .Execute();
//            externalTaskService.HandleFailure(externalTaskId, workerId, errorMessage, retries, 0L);

//            foreach (var lockedExternalTask in list)
//                externalTaskService.Unlock(lockedExternalTask.Id);
//        }

//        protected internal virtual IList<IExternalTask> startExternalTaskProcesses(int Count)
//        {
//            IList<IExternalTask> list = new List<IExternalTask>();
//            for (var ithPrio = 0; ithPrio < Count; ithPrio++)
//                list.Add(startExternalTaskProcessGivenPriority(ithPrio));
//            return list;
//        }

//        protected internal virtual IExternalTask startExternalTaskProcessGivenTopicName(string topicName)
//        {
//            var processModelWithCustomTopic = DefaultExternalTaskModelBuilder.CreateDefaultExternalTaskModel()
//                .Topic(topicName)
//                .Build();
//            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(processModelWithCustomTopic);
//            var pi = runtimeService.StartProcessInstanceById(sourceProcessDefinition.Id);
//            return externalTaskService.CreateExternalTaskQuery(c=>c.ProcessInstanceId== pi.Id)
//                .First();
//        }

//        protected internal virtual IExternalTask startExternalTaskProcessGivenActivityId(string activityId)
//        {
//            var processModelWithCustomActivityId = DefaultExternalTaskModelBuilder.CreateDefaultExternalTaskModel()
//                .ExternalTaskName(activityId)
//                .Build();
//            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(processModelWithCustomActivityId);
//            var pi = runtimeService.StartProcessInstanceById(sourceProcessDefinition.Id);
//            return externalTaskService.CreateExternalTaskQuery(c=>c.ProcessInstanceId== pi.Id)
//                .First();
//        }

//        protected internal virtual IExternalTask startExternalTaskProcessGivenProcessDefinitionKey(
//            string processDefinitionKey)
//        {
//            var processModelWithCustomKey = DefaultExternalTaskModelBuilder.CreateDefaultExternalTaskModel()
//                .ProcessKey(processDefinitionKey)
//                .Build();
//            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(processModelWithCustomKey);
//            var pi = runtimeService.StartProcessInstanceById(sourceProcessDefinition.Id);
//            return externalTaskService.CreateExternalTaskQuery(c=>c.ProcessInstanceId== pi.Id)
//                .First();
//        }

//        protected internal virtual IExternalTask startExternalTaskProcessGivenPriority(int priority)
//        {
//            var processModelWithCustomPriority = DefaultExternalTaskModelBuilder.CreateDefaultExternalTaskModel()
//                .Priority(priority)
//                .Build();
//            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(processModelWithCustomPriority);
//            var pi = runtimeService.StartProcessInstanceById(sourceProcessDefinition.Id);
//            return externalTaskService.CreateExternalTaskQuery(c=>c.ProcessInstanceId== pi.Id)
//                .First();
//        }

//        protected internal virtual IExternalTask startExternalTaskProcess()
//        {
//            var oneExternalTaskProcess = DefaultExternalTaskModelBuilder.CreateDefaultExternalTaskModel()
//                .Build();
//            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(oneExternalTaskProcess);
//            var pi = runtimeService.StartProcessInstanceById(sourceProcessDefinition.Id);
//            return externalTaskService.CreateExternalTaskQuery(c=>c.ProcessInstanceId== pi.Id)
//                .First();
//        }

//        [Test]
//        public virtual void testQuery()
//        {
//            // given
//            var task = startExternalTaskProcess();

//            // when
//            var log = historyService.CreateHistoricExternalTaskLogQuery()
//                .First();

//            // then
//            Assert.NotNull(log);
//            Assert.That(log.ExternalTaskId, Is.EqualTo(task.Id));
//        }

//        [Test]
//        public virtual void testQueryById()
//        {
//            // given
//            startExternalTaskProcesses(2);
//            var logId = retrieveFirstHistoricExternalTaskLog()
//                .Id;

//            // when
//            var log = historyService.CreateHistoricExternalTaskLogQuery()
//                .LogId(logId)
//                .First();

//            // then
//            Assert.NotNull(log);
//            Assert.That(log.Id, Is.EqualTo(logId));
//        }

//        [Test]
//        [ExpectedException(typeof(NotValidException))]
//        public virtual void testQueryFailsByInvalidId()
//        {
//            // given
//            startExternalTaskProcess();

//            // then
//            //Expect(typeof(NotValidException));

//            // when
//            historyService.CreateHistoricExternalTaskLogQuery()
//                .LogId(null)
//                .First();
//        }
//    }
//}