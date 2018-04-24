//using System;
//using System.IO;
//using System.Linq;
//using ESS.FW.Bpm.Engine.Impl.Cmd;
//using ESS.FW.Bpm.Engine.Impl.DB;
//using ESS.FW.Bpm.Engine.Impl.Interceptor;
//using ESS.FW.Bpm.Engine.Impl.Util;
//using ESS.FW.Bpm.Engine.Persistence.Entity;
//using ESS.FW.Bpm.Engine.Runtime;
//using NUnit.Framework;

//namespace ESS.FW.Bpm.Engine.Tests.Api.Mgmt
//{
//    /// <summary>
//    /// </summary>
//    [TestFixture]
//    public class JobQueryTest : PluggableProcessEngineTestCase
//    {
//        /// <summary>
//        ///     Setup will create
//        ///     - 3 process instances, each with one timer, each firing at t1/t2/t3 + 1 hour (see process)
//        ///     - 1 message
//        /// </summary>
//        [SetUp]
//        protected internal virtual void setUp()
//        {
//            //base.SetUp();

//            commandExecutor = processEngineConfiguration.CommandExecutorTxRequired;

//            deploymentId = repositoryService.CreateDeployment()
//                .AddClasspathResource("resources/api/mgmt/timerOnTask.bpmn20.xml")
//                .Deploy()
//                .Id;

//            // Create proc inst that has timer that will fire on t1 + 1 hour
//            var startTime = new DateTime();
//            //startTime.Set(DateTime.MILLISECOND, 0);

//            var t1 = startTime;
//            ClockUtil.CurrentTime = t1;
//            processInstanceIdOne = runtimeService.StartProcessInstanceByKey("timerOnTask")
//                .Id;
//            testStartTime = t1;
//            timerOneFireTime = new DateTime(t1.Ticks + ONE_HOUR);

//            // Create proc inst that has timer that will fire on t2 + 1 hour
//            startTime.AddHours(1);
//            var t2 = startTime; // t2 = t1 + 1 hour
//            ClockUtil.CurrentTime = t2;
//            processInstanceIdTwo = runtimeService.StartProcessInstanceByKey("timerOnTask")
//                .Id;
//            timerTwoFireTime = new DateTime(t2.Ticks + ONE_HOUR);

//            // Create proc inst that has timer that will fire on t3 + 1 hour
//            startTime.AddHours(1);
//            var t3 = startTime; // t3 = t2 + 1 hour
//            ClockUtil.CurrentTime = t3;
//            processInstanceIdThree = runtimeService.StartProcessInstanceByKey("timerOnTask")
//                .Id;
//            timerThreeFireTime = new DateTime(t3.Ticks + ONE_HOUR);

//            // Create one message
//            messageId = commandExecutor.Execute(new CommandAnonymousInnerClass(this));
//        }

//        [TearDown]
//        protected internal void tearDown()
//        {
//            repositoryService.DeleteDeployment(deploymentId, true);
//            commandExecutor.Execute(new DeleteJobsCmd(messageId, true));
//            TearDown();
//        }

//        private string deploymentId;
//        private string messageId;
//        private ICommandExecutor commandExecutor;
//        private TimerEntity timerEntity;

//        private DateTime testStartTime;
//        private DateTime timerOneFireTime;
//        private DateTime timerTwoFireTime;
//        private DateTime timerThreeFireTime;

//        private string processInstanceIdOne;
//        private string processInstanceIdTwo;
//        private string processInstanceIdThree;

//        private static readonly long ONE_HOUR = 60L * 60L * 1000L;
//        private const long ONE_SECOND = 1000L;

//        private const string EXCEPTION_MESSAGE =
//            "java.lang.RuntimeException: This is an exception thrown from scriptTask";

//        private class CommandAnonymousInnerClass : ICommand<string>
//        {
//            private readonly JobQueryTest outerInstance;

//            public CommandAnonymousInnerClass(JobQueryTest outerInstance)
//            {
//                this.outerInstance = outerInstance;
//            }

//            public virtual string Execute(CommandContext commandContext)
//            {
//                var message = new MessageEntity();
//                commandContext.JobManager.Send(message);
//                return message.Id;
//            }
//        }

//        public virtual void testQueryByNoCriteria()
//        {
//            var query = managementService.CreateJobQuery();
//            //verifyQueryResults(query, 4);
//        }

//        public virtual void testQueryByActivityId()
//        {
//            var jobDefinition = managementService.CreateJobDefinitionQuery()
//                .First();

//            var query = managementService.CreateJobQuery()
//                .ActivityId(jobDefinition.ActivityId);
//            //verifyQueryResults(query, 3);
//        }

//        //helper ////////////////////////////////////////////////////////////

//        private void setRetries(string ProcessInstanceId, int retries)
//        {
////JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
////ORIGINAL LINE: final org.Camunda.bpm.Engine.Runtime.IJob job = managementService.CreateJobQuery(c=>c.ProcessInstanceId == ProcessInstanceId).First();
//            var job = managementService.CreateJobQuery()
//                .Where(c=>c.ProcessInstanceId==ProcessInstanceId)
//                .First();
//            commandExecutor.Execute(new CommandAnonymousInnerClass2(this, retries, job));
//        }

//        private class CommandAnonymousInnerClass2 : ICommand<object>
//        {
//            private readonly IJob job;
//            private readonly JobQueryTest outerInstance;

//            private readonly int retries;

//            public CommandAnonymousInnerClass2(JobQueryTest outerInstance, int retries, IJob job)
//            {
//                this.outerInstance = outerInstance;
//                this.retries = retries;
//                this.job = job;
//            }


//            public virtual object Execute(CommandContext commandContext)
//            {
//                var timer = commandContext.DbEntityManager.SelectById<JobEntity>(typeof(JobEntity), job.Id);
//                timer.Retries = retries;
//                return null;
//            }
//        }

//        private IProcessInstance startProcessInstanceWithFailingJob()
//        {
//            // start a process with a failing job
//            var processInstance = runtimeService.StartProcessInstanceByKey("exceptionInJobExecution");

//            // The execution is waiting in the first usertask. This contains a boundary
//            // timer event which we will execute manual for testing purposes.
//            var timerJob = managementService.CreateJobQuery()
//                .Where(c=>c.ProcessInstanceId==processInstance.Id)
//                .First();

//            Assert.NotNull(timerJob, "No job found for process instance");

//            try
//            {
//                managementService.ExecuteJob(timerJob.Id);
//                Assert.Fail("RuntimeException from within the script task expected");
//            }
//            catch (Exception re)
//            {
//                AssertTextPresent(EXCEPTION_MESSAGE, re.Message);
//            }
//            return processInstance;
//        }

//        private void verifyFailedJob(IQueryable<IJob> query, IProcessInstance processInstance)
//        {
//            //verifyQueryResults(query, 1);

//            var failedJob = query.First();
//            Assert.NotNull(failedJob);
//            Assert.AreEqual(processInstance.Id, failedJob.ProcessInstanceId);
//            Assert.NotNull(failedJob.ExceptionMessage);
//            AssertTextPresent(EXCEPTION_MESSAGE, failedJob.ExceptionMessage);
//        }

//        private void verifyQueryResults(IQueryable<IJob> query, int countExpected)
//        {
//            Assert.AreEqual(countExpected, query
//                .Count());
//            Assert.AreEqual(countExpected, query.Count());

//            if (countExpected == 1)
//                Assert.NotNull(query.First());
//            else if (countExpected > 1)
//                verifySingleResultFails(query);
//            else if (countExpected == 0)
//                Assert.IsNull(query.First());
//        }

//        private void verifySingleResultFails(IQueryable<IJob> query)
//        {
//            try
//            {
//                query.First();
//                Assert.Fail();
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }

//        private void createJobWithoutExceptionMsg()
//        {
//            var commandExecutor = processEngineConfiguration.CommandExecutorTxRequired;
//            commandExecutor.Execute(new CommandAnonymousInnerClass3(this));
//        }

//        private class CommandAnonymousInnerClass3 : ICommand<object>
//        {
//            private readonly JobQueryTest outerInstance;

//            public CommandAnonymousInnerClass3(JobQueryTest outerInstance)
//            {
//                this.outerInstance = outerInstance;
//            }

//            public virtual object Execute(CommandContext commandContext)
//            {
//                var jobManager = commandContext.JobManager;

//                outerInstance.timerEntity = new TimerEntity();
//                outerInstance.timerEntity.LockOwner = Guid.NewGuid()
//                    .ToString();
//                outerInstance.timerEntity.Duedate = DateTime.Now;
//                outerInstance.timerEntity.Retries = 0;

//                var stringWriter = new StringWriter();
//                var exception = new NullReferenceException();
//                //exception.printStackTrace(new PrintWriter(stringWriter));
//                outerInstance.timerEntity.ExceptionStacktrace = stringWriter.ToString();

//                jobManager.Insert(outerInstance.timerEntity);

//                Assert.NotNull(outerInstance.timerEntity.Id);

//                return null;
//            }
//        }

//        private void createJobWithoutExceptionStacktrace()
//        {
//            var commandExecutor = processEngineConfiguration.CommandExecutorTxRequired;
//            commandExecutor.Execute(new CommandAnonymousInnerClass4(this));
//        }

//        private class CommandAnonymousInnerClass4 : ICommand<object>
//        {
//            private readonly JobQueryTest outerInstance;

//            public CommandAnonymousInnerClass4(JobQueryTest outerInstance)
//            {
//                this.outerInstance = outerInstance;
//            }

//            public virtual object Execute(CommandContext commandContext)
//            {
//                var jobManager = commandContext.JobManager;

//                outerInstance.timerEntity = new TimerEntity();
//                outerInstance.timerEntity.LockOwner = Guid.NewGuid()
//                    .ToString();
//                outerInstance.timerEntity.Duedate = DateTime.Now;
//                outerInstance.timerEntity.Retries = 0;
//                outerInstance.timerEntity.ExceptionMessage = "I'm supposed to Assert.Fail";

//                jobManager.Insert(outerInstance.timerEntity);

//                Assert.NotNull(outerInstance.timerEntity.Id);

//                return null;
//            }
//        }

//        private void deleteJobInDatabase()
//        {
//            var commandExecutor = processEngineConfiguration.CommandExecutorTxRequired;
//            commandExecutor.Execute(new CommandAnonymousInnerClass5(this));
//        }

//        private class CommandAnonymousInnerClass5 : ICommand<object>
//        {
//            private readonly JobQueryTest outerInstance;

//            public CommandAnonymousInnerClass5(JobQueryTest outerInstance)
//            {
//                this.outerInstance = outerInstance;
//            }

//            public virtual object Execute(CommandContext commandContext)
//            {
//                outerInstance.timerEntity.Delete();

//                commandContext.HistoricJobLogManager.DeleteHistoricJobLogByJobId(outerInstance.timerEntity.Id);

//                var historicIncidents =
//                    context.Impl.Context.ProcessEngineConfiguration.HistoryService.CreateHistoricIncidentQuery()
//                        
//                        .ToList();

//                foreach (var historicIncident in historicIncidents)
//                    commandContext.DbEntityManager.Delete((IDbEntity) historicIncident);

//                return null;
//            }
//        }

//        [Test]
//        public virtual void testByInvalidJobDefinitionId()
//        {
//            var query = managementService.CreateJobQuery()
//                .JobDefinitionId("invalid");
//            //verifyQueryResults(query, 0);

//            try
//            {
//                managementService.CreateJobQuery()
//                    .JobDefinitionId(null)
//                    
//                    .ToList();
//                Assert.Fail();
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }

//        [Test]
//        public virtual void testByJobDefinitionId()
//        {
//            var jobDefinition = managementService.CreateJobDefinitionQuery()
//                .First();

//            var query = managementService.CreateJobQuery()
//                .JobDefinitionId(jobDefinition.Id);
//            //verifyQueryResults(query, 3);
//        }

//        [Test]
//        public virtual void testInvalidOnlyTimersUsage()
//        {
//            try
//            {
//                managementService.CreateJobQuery()
//                    /*.Timers()*/
//                    .Messages()
//                    
//                    .ToList();
//                Assert.Fail();
//            }
//            catch (ProcessEngineException e)
//            {
//                AssertTextPresent("Cannot combine onlyTimers() with onlyMessages() in the same query", e.Message);
//            }
//        }

//        [Test]
//        public virtual void testJobQueryWithExceptions()
//        {
//            createJobWithoutExceptionMsg();

//            var job = managementService.CreateJobQuery()
//                .JobId(timerEntity.Id)
//                .First();

//            Assert.NotNull(job);

//            var list = managementService.CreateJobQuery()
//                .WithException()
//                
//                .ToList();
//            Assert.AreEqual(list.Count, 1);

//            deleteJobInDatabase();

//            createJobWithoutExceptionStacktrace();

//            job = managementService.CreateJobQuery()
//                .JobId(timerEntity.Id)
//                .First();

//            Assert.NotNull(job);

//            list = managementService.CreateJobQuery()
//                .WithException()
//                
//                .ToList();
//            Assert.AreEqual(list.Count, 1);

//            deleteJobInDatabase();
//        }

//        [Test]
//        public virtual void testQueryByActive()
//        {
//            var query = managementService.CreateJobQuery()
//                .Active();
//            //verifyQueryResults(query, 4);
//        }

//        [Test]
//        public virtual void testQueryByDuedateCombinations()
//        {
//            var query = managementService.CreateJobQuery()
//                .DuedateHigherThan(testStartTime)
//                .DuedateLowerThan(new DateTime(timerThreeFireTime.Ticks + ONE_SECOND));
//            //verifyQueryResults(query, 3);

//            query = managementService.CreateJobQuery()
//                .DuedateHigherThan(new DateTime(timerThreeFireTime.Ticks + ONE_SECOND))
//                .DuedateLowerThan(testStartTime);
//            //verifyQueryResults(query, 0);
//        }

//        [Test]
//        public virtual void testQueryByDuedateHigherThen()
//        {
//            var query = managementService.CreateJobQuery()
//                .DuedateHigherThen(testStartTime);
//            //verifyQueryResults(query, 3);

//            query = managementService.CreateJobQuery()
//                .DuedateHigherThen(timerOneFireTime);
//            //verifyQueryResults(query, 2);

//            query = managementService.CreateJobQuery()
//                .DuedateHigherThen(timerTwoFireTime);
//            //verifyQueryResults(query, 1);

//            query = managementService.CreateJobQuery()
//                .DuedateHigherThen(timerThreeFireTime);
//            //verifyQueryResults(query, 0);
//        }

//        [Test]
//        public virtual void testQueryByDuedateHigherThenOrEqual()
//        {
//            var query = managementService.CreateJobQuery()
//                .DuedateHigherThenOrEquals(testStartTime);
//            //verifyQueryResults(query, 3);

//            query = managementService.CreateJobQuery()
//                .DuedateHigherThenOrEquals(timerOneFireTime);
//            //verifyQueryResults(query, 3);

//            query = managementService.CreateJobQuery()
//                .DuedateHigherThenOrEquals(new DateTime(timerOneFireTime.Ticks + ONE_SECOND));
//            //verifyQueryResults(query, 2);

//            query = managementService.CreateJobQuery()
//                .DuedateHigherThenOrEquals(timerThreeFireTime);
//            //verifyQueryResults(query, 1);

//            query = managementService.CreateJobQuery()
//                .DuedateHigherThenOrEquals(new DateTime(timerThreeFireTime.Ticks + ONE_SECOND));
//            //verifyQueryResults(query, 0);
//        }

//        [Test]
//        public virtual void testQueryByDuedateLowerThen()
//        {
//            var query = managementService.CreateJobQuery()
//                .DuedateLowerThen(testStartTime);
//            //verifyQueryResults(query, 0);

//            query = managementService.CreateJobQuery()
//                .DuedateLowerThen(new DateTime(timerOneFireTime.Ticks + ONE_SECOND));
//            //verifyQueryResults(query, 1);

//            query = managementService.CreateJobQuery()
//                .DuedateLowerThen(new DateTime(timerTwoFireTime.Ticks + ONE_SECOND));
//            //verifyQueryResults(query, 2);

//            query = managementService.CreateJobQuery()
//                .DuedateLowerThen(new DateTime(timerThreeFireTime.Ticks + ONE_SECOND));
//            //verifyQueryResults(query, 3);
//        }

//        [Test]
//        public virtual void testQueryByDuedateLowerThenOrEqual()
//        {
//            var query = managementService.CreateJobQuery()
//                .DuedateLowerThenOrEquals(testStartTime);
//            //verifyQueryResults(query, 0);

//            query = managementService.CreateJobQuery()
//                .DuedateLowerThenOrEquals(timerOneFireTime);
//            //verifyQueryResults(query, 1);

//            query = managementService.CreateJobQuery()
//                .DuedateLowerThenOrEquals(timerTwoFireTime);
//            //verifyQueryResults(query, 2);

//            query = managementService.CreateJobQuery()
//                .DuedateLowerThenOrEquals(timerThreeFireTime);
//            //verifyQueryResults(query, 3);
//        }

//        [Test]
//        [Deployment("resources/api/mgmt/ManagementServiceTest.TestGetJobExceptionStacktrace.bpmn20.xml")]
//        public virtual void testQueryByException()
//        {
//            var query = managementService.CreateJobQuery()
//                .WithException();
//            //verifyQueryResults(query, 0);

//            var processInstance = startProcessInstanceWithFailingJob();

//            query = managementService.CreateJobQuery()
//                .WithException();
//            verifyFailedJob(query, processInstance);
//        }

//        [Test]
//        [Deployment("resources/api/mgmt/ManagementServiceTest.TestGetJobExceptionStacktrace.bpmn20.xml")]
//        public virtual void testQueryByExceptionMessage()
//        {
//            var query = managementService.CreateJobQuery()
//                .ExceptionMessage(EXCEPTION_MESSAGE);
//            //verifyQueryResults(query, 0);

//            var processInstance = startProcessInstanceWithFailingJob();

//            var job = managementService.CreateJobQuery()
//                .Where(c=>c.ProcessInstanceId==processInstance.Id)
//                .First();

//            query = managementService.CreateJobQuery()
//                .ExceptionMessage(job.ExceptionMessage);
//            verifyFailedJob(query, processInstance);
//        }

//        [Test]
//        [Deployment("resources/api/mgmt/ManagementServiceTest.TestGetJobExceptionStacktrace.bpmn20.xml")]
//        public virtual void testQueryByExceptionMessageEmpty()
//        {
//            var query = managementService.CreateJobQuery()
//                .ExceptionMessage("");
//            //verifyQueryResults(query, 0);

//            startProcessInstanceWithFailingJob();

//            query = managementService.CreateJobQuery()
//                .ExceptionMessage("");
//            //verifyQueryResults(query, 0);
//        }

//        [Test]
//        public virtual void testQueryByExceptionMessageNull()
//        {
//            try
//            {
//                managementService.CreateJobQuery()
//                    .ExceptionMessage(null);
//                Assert.Fail("ProcessEngineException expected");
//            }
//            catch (ProcessEngineException e)
//            {
//                Assert.AreEqual("Provided exception message is null", e.Message);
//            }
//        }

//        [Test]
//        public virtual void testQueryByExecutable()
//        {
//            ClockUtil.CurrentTime = new DateTime(timerThreeFireTime.Ticks + ONE_SECOND);
//            // all jobs should be executable at t3 + 1hour.1second
//            var query = managementService.CreateJobQuery()
//                /*.Executable()*/;
//            //verifyQueryResults(query, 4);

//            // Setting retries of one job to 0, makes it non-executable
//            setRetries(processInstanceIdOne, 0);
//            //verifyQueryResults(query, 3);

//            // Setting the clock before the start of the process instance, makes none of the jobs executable
//            ClockUtil.CurrentTime = testStartTime;
//            //verifyQueryResults(query, 1); // 1, since a message is always executable when retries > 0
//        }

//        [Test]
//        public virtual void testQueryByExecutionId()
//        {
//            var job = managementService.CreateJobQuery()
//                .Where(c=>c.ProcessInstanceId==processInstanceIdOne)
//                .First();
//            var query = managementService.CreateJobQuery()
//                .ExecutionId(job.ExecutionId);
//            Assert.AreEqual(query.First()
//                .Id, job.Id);
//            //verifyQueryResults(query, 1);
//        }

//        [Test]
//        public virtual void testQueryByInvalidActivityId()
//        {
//            var query = managementService.CreateJobQuery()
//                .ActivityId("invalid");
//            //verifyQueryResults(query, 0);

//            try
//            {
//                managementService.CreateJobQuery()
//                    .ActivityId(null)
//                    
//                    .ToList();
//                Assert.Fail();
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }

//        [Test]
//        public virtual void testQueryByInvalidExecutionId()
//        {
//            var query = managementService.CreateJobQuery()
//                .ExecutionId("invalid");
//            //verifyQueryResults(query, 0);

//            try
//            {
//                managementService.CreateJobQuery()
//                    .ExecutionId(null)
//                    
//                    .ToList();
//                Assert.Fail();
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }

//        [Test]
//        public virtual void testQueryByInvalidProcessDefinitionId()
//        {
//            var query = managementService.(c=>c.ProcessDefinitionId == "invalid");
//            //verifyQueryResults(query, 0);

//            try
//            {
//                managementService.CreateJobQuery(c=>c.ProcessDefinitionKey == null)
//                    .ToList();
//                Assert.Fail();
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }

//        [Test]
//        public virtual void testQueryByInvalidProcessDefinitionKey()
//        {
//            var query = managementService.CreateJobQuery(c=>c.ProcessDefinitionKey =="invalid");
//            //verifyQueryResults(query, 0);

//            try
//            {
//                managementService.CreateJobQuery(c=>c.ProcessDefinitionKey == null)
//                    .ToList();
//                Assert.Fail();
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }

//        [Test]
//        public virtual void testQueryByInvalidProcessInstanceId()
//        {
//            var query = managementService.CreateJobQuery()
//                .Where(c=>c.ProcessInstanceId=="invalid");
//            //verifyQueryResults(query, 0);

//            try
//            {
//                managementService.CreateJobQuery()
//                    .Where(c=>c.ProcessInstanceId==null);
//                Assert.Fail();
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }

//        [Test]
//        public virtual void testQueryByNoRetriesLeft()
//        {
//            var query = managementService.CreateJobQuery()
//                .NoRetriesLeft();
//            //verifyQueryResults(query, 0);

//            setRetries(processInstanceIdOne, 0);
//            // Re-running the query should give only one jobs now, since three job has retries>0
//            //verifyQueryResults(query, 1);
//        }

//        [Test]
//        public virtual void testQueryByOnlyMessages()
//        {
//            var query = managementService.CreateJobQuery()
//                .Messages();
//            //verifyQueryResults(query, 1);
//        }

//        [Test]
//        public virtual void testQueryByOnlyTimers()
//        {
//            var query = managementService.CreateJobQuery()
//                /*.Timers()*/;
//            //verifyQueryResults(query, 3);
//        }

//        [Test]
//        public virtual void testQueryByProcessDefinitionId()
//        {
//            var processDefinition = repositoryService.CreateProcessDefinitionQuery()
//                .First();

//            var query = managementService.(c=>c.ProcessDefinitionId == processDefinition.Id);
//            //verifyQueryResults(query, 3);
//        }

//        [Test]
//        public virtual void testQueryByProcessDefinitionKey()
//        {
//            var query = managementService.CreateJobQuery(c=>c.ProcessDefinitionKey =="timerOnTask");
//            //verifyQueryResults(query, 3);
//        }

//        [Test]
//        public virtual void testQueryByProcessInstanceId()
//        {
//            var query = managementService.CreateJobQuery()
//                .Where(c=>c.ProcessInstanceId==processInstanceIdOne);
//            //verifyQueryResults(query, 1);
//        }

//        [Test]
//        public virtual void testQueryByRetriesLeft()
//        {
//            var query = managementService.CreateJobQuery()
//                .WithRetriesLeft();
//            //verifyQueryResults(query, 4);

//            setRetries(processInstanceIdOne, 0);
//            // Re-running the query should give only 3 jobs now, since one job has retries=0
//            //verifyQueryResults(query, 3);
//        }

//        [Test]
//        public virtual void testQueryBySuspended()
//        {
//            var query = managementService.CreateJobQuery()
//                .Suspended();
//            //verifyQueryResults(query, 0);

//            managementService.SuspendJobDefinitionByProcessDefinitionKey("timerOnTask", true);
//            //verifyQueryResults(query, 3);
//        }

//        [Test]
//        public virtual void testQueryInvalidSortingUsage()
//        {
//            try
//            {
//                managementService.CreateJobQuery()
//                    .OrderByJobId()
//                    
//                    .ToList();
//                Assert.Fail();
//            }
//            catch (ProcessEngineException e)
//            {
//                AssertTextPresent("call asc() or Desc() after using orderByXX()", e.Message);
//            }

//            try
//            {
//                managementService.CreateJobQuery()
//                    /*.Asc()*/;
//                Assert.Fail();
//            }
//            catch (ProcessEngineException e)
//            {
//                AssertTextPresent("You should call any of the orderBy methods first before specifying a direction",
//                    e.Message);
//            }
//        }

//        //sorting //////////////////////////////////////////

//        [Test]
//        public virtual void testQuerySorting()
//        {
//            // asc
//            Assert.AreEqual(4, managementService.CreateJobQuery()
//                .OrderByJobId()
//                /*.Asc()*/
//                .Count());
//            Assert.AreEqual(4, managementService.CreateJobQuery()
//                /*.OrderByJobDuedate()*/
//                /*.Asc()*/
//                .Count());
//            Assert.AreEqual(4, managementService.CreateJobQuery()
//                //.OrderByExecutionId()
//                /*.Asc()*/
//                .Count());
//            Assert.AreEqual(4, managementService.CreateJobQuery()
//                //.OrderByProcessInstanceId()
//                /*.Asc()*/
//                .Count());
//            Assert.AreEqual(4, managementService.CreateJobQuery()
//                .OrderByJobRetries()
//                /*.Asc()*/
//                .Count());
//            Assert.AreEqual(4, managementService.CreateJobQuery()
//                /*.OrderByProcessDefinitionId()*/
//                /*.Asc()*/
//                .Count());
//            Assert.AreEqual(4, managementService.CreateJobQuery()
//                //.OrderByProcessDefinitionKey()
//                /*.Asc()*/
//                .Count());

//            // Desc
//            Assert.AreEqual(4, managementService.CreateJobQuery()
//                .OrderByJobId()
//                /*.Desc()*/
//                .Count());
//            Assert.AreEqual(4, managementService.CreateJobQuery()
//                /*.OrderByJobDuedate()*/
//                /*.Desc()*/
//                .Count());
//            Assert.AreEqual(4, managementService.CreateJobQuery()
//                //.OrderByExecutionId()
//                /*.Desc()*/
//                .Count());
//            Assert.AreEqual(4, managementService.CreateJobQuery()
//                //.OrderByProcessInstanceId()
//                /*.Desc()*/
//                .Count());
//            Assert.AreEqual(4, managementService.CreateJobQuery()
//                .OrderByJobRetries()
//                /*.Desc()*/
//                .Count());
//            Assert.AreEqual(4, managementService.CreateJobQuery()
//                /*.OrderByProcessDefinitionId()*/
//                /*.Desc()*/
//                .Count());
//            Assert.AreEqual(4, managementService.CreateJobQuery()
//                //.OrderByProcessDefinitionKey()
//                /*.Desc()*/
//                .Count());

//            // sorting on multiple fields
//            setRetries(processInstanceIdTwo, 2);
//            ClockUtil.CurrentTime = new DateTime(timerThreeFireTime.Ticks + ONE_SECOND);
//            // make sure all timers can fire

//            var query = managementService.CreateJobQuery()
//                /*.Timers()*/
//                /*.Executable()*/
//                .OrderByJobRetries()
//                /*.Asc()*/
//                /*.OrderByJobDuedate()*/
//                /*.Desc()*/;

//            var jobs = query
//                .ToList();
//            Assert.AreEqual(3, jobs.Count);

//            Assert.AreEqual(2, jobs[0].Retries);
//            Assert.AreEqual(3, jobs[1].Retries);
//            Assert.AreEqual(3, jobs[2].Retries);

//            Assert.AreEqual(processInstanceIdTwo, jobs[0].ProcessInstanceId);
//            Assert.AreEqual(processInstanceIdThree, jobs[1].ProcessInstanceId);
//            Assert.AreEqual(processInstanceIdOne, jobs[2].ProcessInstanceId);
//        }

//        [Test]
//        [Deployment]
//        public virtual void testTimeCycleQueryByProcessDefinitionId()
//        {
//            var processDefinitionId = repositoryService.CreateProcessDefinitionQuery(c=>c.Key == "process")
//                .First()
//                .Id;

//            var query = managementService.CreateJobQuery(c=>c.ProcessDefinitionId == processDefinitionId);

//            //verifyQueryResults(query, 1);

//            var jobId = query.First()
//                .Id;
//            managementService.ExecuteJob(jobId);

//            //verifyQueryResults(query, 1);

//            var anotherJobId = query.First()
//                .Id;
//            Assert.IsFalse(jobId.Equals(anotherJobId));
//        }

//        [Test]
//        [Deployment("resources/api/mgmt/JobQueryTest.TestTimeCycleQueryByProcessDefinitionId.bpmn20.xml")]
//        public virtual void testTimeCycleQueryByProcessDefinitionKey()
//        {
//            var query = managementService.CreateJobQuery(c=>c.ProcessDefinitionKey == "process");

//            //verifyQueryResults(query, 1);

//            var jobId = query.First()
//                .Id;
//            managementService.ExecuteJob(jobId);

//            //verifyQueryResults(query, 1);

//            var anotherJobId = query.First()
//                .Id;
//            Assert.IsFalse(jobId.Equals(anotherJobId));
//        }
//    }
//}