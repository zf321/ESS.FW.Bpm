using System;
using System.Linq;
using Engine.Tests.Api.Authorization.Util;
using Engine.Tests.Api.Runtime.Migration.Models.Builder;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Externaltask;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.History
{
    [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull)]

    [TestFixture]
    public class HistoricExternalTaskLogTest
    {
        [SetUp]
        public virtual void setUp()
        {
            runtimeService = engineRule.RuntimeService;
            historyService = engineRule.HistoryService;
            externalTaskService = engineRule.ExternalTaskService;
        }

        [TearDown]
        public virtual void tearDown()
        {
            var list = externalTaskService.CreateExternalTaskQuery(c=>c.WorkerId==WORKER_ID)
                
                .ToList();
            foreach (var externalTask in list)
                externalTaskService.Unlock(externalTask.Id);
        }

        private readonly bool InstanceFieldsInitialized;

        public HistoricExternalTaskLogTest()
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


        protected internal readonly string WORKER_ID = "aWorkerId";
        protected internal readonly string ERROR_MESSAGE = "This is an error!";
        protected internal readonly string ERROR_DETAILS = "These are the error details!";
        protected internal readonly long LOCK_DURATION = 5 * 60L * 1000L;

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public final org.junit.Rules.ExpectedException thrown = org.junit.Rules.ExpectedException.None();
        //public readonly ExpectedException thrown = ExpectedException.None();
        protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
        protected internal AuthorizationTestRule authRule;
        protected internal ProcessEngineTestRule testHelper;
//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(engineRule).around(authRule).around(testHelper);
        //public RuleChain ruleChain;
        protected internal IProcessInstance processInstance;
        protected internal IRuntimeService runtimeService;
        protected internal IHistoryService historyService;
        protected internal IExternalTaskService externalTaskService;

        [Test]
        public virtual void testHistoricExternalTaskLogCreateProperties()
        {
            // given
            var task = startExternalTaskProcess();

            // when
            var log = historyService.CreateHistoricExternalTaskLogQuery(c=> c.State == ExternalTaskStateFields.Created.StateCode.ToString())
                .First();

            // then
            AssertHistoricLogPropertiesAreProperlySet(task, log);
            Assert.AreEqual(null, log.WorkerId);
            AssertLogIsInCreatedState(log);
        }

        [Test]
        public virtual void testHistoricExternalTaskLogFailedProperties()
        {
            // given
            var task = startExternalTaskProcess();
            reportExternalTaskFailure(task.Id);
            task = externalTaskService.CreateExternalTaskQuery()
                .First();

            // when
            var log = historyService.CreateHistoricExternalTaskLogQuery(c=>c.State == ExternalTaskStateFields.Failed.StateCode.ToString())
                .First();

            // then
            AssertHistoricLogPropertiesAreProperlySet(task, log);
            Assert.AreEqual(WORKER_ID, log.WorkerId);
            AssertLogIsInFailedState(log);
        }

        [Test]
        public virtual void testHistoricExternalTaskLogSuccessfulProperties()
        {
            // given
            var task = startExternalTaskProcess();
            completeExternalTask(task.Id);

            // when
            var log = historyService.CreateHistoricExternalTaskLogQuery(c=>c.State == ExternalTaskStateFields.Successful.StateCode.ToString())
                .First();

            // then
            AssertHistoricLogPropertiesAreProperlySet(task, log);
            Assert.AreEqual(WORKER_ID, log.WorkerId);
            AssertLogIsInSuccessfulState(log);
        }

        [Test]
        public virtual void testHistoricExternalTaskLogDeletedProperties()
        {
            // given
            var task = startExternalTaskProcess();
            runtimeService.DeleteProcessInstance(task.ProcessInstanceId, "Dummy reason for deletion!");

            // when
            var log = historyService.CreateHistoricExternalTaskLogQuery(c=> c.State == ExternalTaskStateFields.Deleted.StateCode.ToString())
                .First();

            // then
            AssertHistoricLogPropertiesAreProperlySet(task, log);
            Assert.AreEqual(null, log.WorkerId);
            AssertLogIsInDeletedState(log);
        }

        [Test]
        public virtual void testRetriesAndWorkerIdWhenFirstFailureAndThenComplete()
        {
            // given
            var task = startExternalTaskProcess();
            reportExternalTaskFailure(task.Id);
            completeExternalTask(task.Id);

            // when
            var log = historyService.CreateHistoricExternalTaskLogQuery(c=>c.State == ExternalTaskStateFields.Successful.StateCode.ToString())
                .First();

            // then
            Assert.AreEqual(WORKER_ID, log.WorkerId);
            Assert.AreEqual(Convert.ToInt32(1), log.Retries);
            AssertLogIsInSuccessfulState(log);
        }

        [Test]
        public virtual void testErrorDetails()
        {
            // given
            var task = startExternalTaskProcess();
            reportExternalTaskFailure(task.Id);

            // when
            var failedHistoricExternalTaskLogId = historyService.CreateHistoricExternalTaskLogQuery(c=>c.State == ExternalTaskStateFields.Failed.StateCode.ToString())
                .First()
                .Id;

            // then
            var stacktrace = historyService.GetHistoricExternalTaskLogErrorDetails(failedHistoricExternalTaskLogId);
            Assert.NotNull(stacktrace);
            Assert.AreEqual(ERROR_DETAILS, stacktrace);
        }

        [Test]
        public virtual void testErrorDetailsWithTwoDifferentErrorsThrown()
        {
            // given
            var task = startExternalTaskProcess();
            var firstErrorDetails = "Dummy error details!";
            var secondErrorDetails = ERROR_DETAILS;
            reportExternalTaskFailure(task.Id, ERROR_MESSAGE, firstErrorDetails);
            ensureEnoughTimePassedByForTimestampOrdering();
            reportExternalTaskFailure(task.Id, ERROR_MESSAGE, secondErrorDetails);

            // when
            var list = historyService.CreateHistoricExternalTaskLogQuery(c=>c.State == ExternalTaskStateFields.Failed.StateCode.ToString())
                /*.OrderByTimestamp()*/
                /*.Asc()*/
                
                .ToList();

            var firstFailedLogId = list[0].Id;
            var secondFailedLogId = list[1].Id;

            // then
            var stacktrace1 = historyService.GetHistoricExternalTaskLogErrorDetails(firstFailedLogId);
            var stacktrace2 = historyService.GetHistoricExternalTaskLogErrorDetails(secondFailedLogId);
            Assert.NotNull(stacktrace1);
            Assert.NotNull(stacktrace2);
            Assert.AreEqual(firstErrorDetails, stacktrace1);
            Assert.AreEqual(secondErrorDetails, stacktrace2);
        }


        [Test]
        public virtual void testGetExceptionStacktraceForNonexistentExternalTaskId()
        {
            try
            {
                historyService.GetHistoricExternalTaskLogErrorDetails("foo");
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException re)
            {
                var expectedMessage = "No historic external task log found with id foo";
                Assert.True(re.Message.Contains(expectedMessage));
            }
        }

        [Test]
        public virtual void testGetExceptionStacktraceForNullExternalTaskId()
        {
            try
            {
                historyService.GetHistoricExternalTaskLogErrorDetails(null);
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException re)
            {
                var expectedMessage = "historicExternalTaskLogId is null";
                Assert.True(re.Message.Contains(expectedMessage));
            }
        }

        [Test]
        public virtual void testErrorMessageTruncation()
        {
            // given
            var exceptionMessage = createStringOfLength(1000);
            var task = startExternalTaskProcess();
            reportExternalTaskFailure(task.Id, exceptionMessage, ERROR_DETAILS);

            // when
            var failedLog = historyService.CreateHistoricExternalTaskLogQuery(c=>c.State == ExternalTaskStateFields.Failed.StateCode.ToString())
                .First();

            var errorMessage = failedLog.ErrorMessage;
            var expectedErrorMessage = exceptionMessage.Substring(0, ExternalTaskEntity.MaxExceptionMessageLength);

            // then
            Assert.NotNull(failedLog);
            Assert.AreEqual(ExternalTaskEntity.MaxExceptionMessageLength, errorMessage.Length);
            Assert.AreEqual(expectedErrorMessage, errorMessage);
        }

        // helper

        protected internal virtual void AssertLogIsInCreatedState(IHistoricExternalTaskLog log)
        {
            Assert.True(log.CreationLog);
            Assert.IsFalse(log.FailureLog);
            Assert.IsFalse(log.SuccessLog);
            Assert.IsFalse(log.DeletionLog);
        }

        protected internal virtual void AssertLogIsInFailedState(IHistoricExternalTaskLog log)
        {
            Assert.IsFalse(log.CreationLog);
            Assert.True(log.FailureLog);
            Assert.IsFalse(log.SuccessLog);
            Assert.IsFalse(log.DeletionLog);
        }

        protected internal virtual void AssertLogIsInSuccessfulState(IHistoricExternalTaskLog log)
        {
            Assert.IsFalse(log.CreationLog);
            Assert.IsFalse(log.FailureLog);
            Assert.True(log.SuccessLog);
            Assert.IsFalse(log.DeletionLog);
        }

        protected internal virtual void AssertLogIsInDeletedState(IHistoricExternalTaskLog log)
        {
            Assert.IsFalse(log.CreationLog);
            Assert.IsFalse(log.FailureLog);
            Assert.IsFalse(log.SuccessLog);
            Assert.True(log.DeletionLog);
        }

        protected internal virtual void AssertHistoricLogPropertiesAreProperlySet(IExternalTask task,
            IHistoricExternalTaskLog log)
        {
            Assert.NotNull(log);
            Assert.NotNull(log.Id);
            Assert.NotNull(log.TimeStamp);

            Assert.AreEqual(task.Id, log.ExternalTaskId);
            Assert.AreEqual(task.ActivityId, log.ActivityId);
            Assert.AreEqual(task.ActivityInstanceId, log.ActivityInstanceId);
            Assert.AreEqual(task.TopicName, log.TopicName);
            Assert.AreEqual(task.Retries, log.Retries);
            Assert.AreEqual(task.ExecutionId, log.ExecutionId);
            Assert.AreEqual(task.ProcessInstanceId, log.ProcessInstanceId);
            Assert.AreEqual(task.ProcessDefinitionId, log.ProcessDefinitionId);
            Assert.AreEqual(task.ProcessDefinitionKey, log.ProcessDefinitionKey);
            Assert.AreEqual(task.Priority, log.Priority);
        }

        protected internal virtual void completeExternalTask(string externalTaskId)
        {
            externalTaskService.FetchAndLock(100, WORKER_ID, false)
                //.Topic(DefaultExternalTaskModelBuilder.DefaultTopic, LOCK_DURATION)
                //.Execute()
                ;
            externalTaskService.Complete(externalTaskId, WORKER_ID);
        }

        protected internal virtual void reportExternalTaskFailure(string externalTaskId)
        {
            reportExternalTaskFailure(externalTaskId, ERROR_MESSAGE, ERROR_DETAILS);
        }

        protected internal virtual void reportExternalTaskFailure(string externalTaskId, string errorMessage,
            string errorDetails)
        {
            externalTaskService.FetchAndLock(100, WORKER_ID, false)
                //.Topic(DefaultExternalTaskModelBuilder.DefaultTopic, LOCK_DURATION)
                //.Execute()
                ;
            externalTaskService.HandleFailure(externalTaskId, WORKER_ID, errorMessage, errorDetails, 1, 0L);
        }

        protected internal virtual IExternalTask startExternalTaskProcess()
        {
            var oneExternalTaskProcess = DefaultExternalTaskModelBuilder.CreateDefaultExternalTaskModel()
                .Build();
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(oneExternalTaskProcess);
            var pi = runtimeService.StartProcessInstanceById(sourceProcessDefinition.Id);
            return externalTaskService.CreateExternalTaskQuery(c=>c.ProcessInstanceId== pi.Id)
                .First();
        }

        protected internal virtual string createStringOfLength(int Count)
        {
            return repeatString(Count, "a");
        }

        protected internal virtual string repeatString(int Count, string with)
        {
            return new string(new char[Count]).Replace("\0", with);
        }

        protected internal virtual void ensureEnoughTimePassedByForTimestampOrdering()
        {
            var timeToAddInSeconds = 5 * 1000L;
            var nowPlus5Seconds = new DateTime(ClockUtil.CurrentTime.TimeOfDay.Ticks + timeToAddInSeconds);
            ClockUtil.CurrentTime = nowPlus5Seconds;
        }
    }
}