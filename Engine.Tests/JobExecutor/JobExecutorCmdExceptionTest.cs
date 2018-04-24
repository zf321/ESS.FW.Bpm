using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.Impl.Cmd;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

/// 

namespace Engine.Tests.JobExecutor
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class JobExecutorCmdExceptionTest : PluggableProcessEngineTestCase
    {
        protected internal TweetExceptionHandler TweetExceptionHandler = new TweetExceptionHandler();

        protected internal TweetNestedCommandExceptionHandler NestedCommandExceptionHandler = new TweetNestedCommandExceptionHandler();




        [SetUp]
        public void setUp()
        {
            processEngineConfiguration.FailedJobCommandFactory = new FoxFailedJobCommandFactory();
            processEngineConfiguration.JobHandlers.Add(TweetExceptionHandler.Type, TweetExceptionHandler);
            processEngineConfiguration.JobHandlers.Add(NestedCommandExceptionHandler.Type, NestedCommandExceptionHandler);
        }
        [TearDown]
        public void tearDown()
        {
            processEngineConfiguration.JobHandlers.Remove(TweetExceptionHandler.Type);
            processEngineConfiguration.JobHandlers.Remove(NestedCommandExceptionHandler.Type);
            ClearDatabase();
        }

        [Test]
        public virtual void TestJobCommandsWith2Exceptions()
        {
            // create a job
            CreateJob(TweetExceptionHandler.TYPE);

            // Execute the existing job
            ExecuteAvailableJobs();

            // the job was successfully Executed
            var query = managementService.CreateJobQuery(c => c.RetriesFromPersistence == 0).ToList();
            Assert.AreEqual(0, query.Count());
        }

        [Test]
        public virtual void TestJobCommandsWith3Exceptions()
        {
            // set the execptionsRemaining to 3 so that
            // the created job will Assert.Fail 3 times and a failed
            // job exists
            TweetExceptionHandler.ExceptionsRemaining = 3;

            // create a job
            CreateJob(TweetExceptionHandler.TYPE);

            // Execute the existing job
            ExecuteAvailableJobs();

            // the job execution failed (job.Retries = 0)
            var job = managementService.CreateJobQuery(c => c.RetriesFromPersistence == 0).FirstOrDefault();
            Assert.NotNull(job);
            Assert.AreEqual(0, job.Retries);
        }

        [Test]
        public virtual void TestMultipleFailingJobs()
        {
            // set the execptionsRemaining to 600 so that
            // each created job will Assert.Fail 3 times and 40 failed
            // job exists
            TweetExceptionHandler.ExceptionsRemaining = 600;

            // create 40 jobs
            for (var i = 0; i < 40; i++)
                CreateJob(TweetExceptionHandler.TYPE);

            // Execute the existing jobs
            ExecuteAvailableJobs();

            // now there are 40 jobs with retries = 0:
            var jobList = managementService.CreateJobQuery().ToList();
            Assert.AreEqual(40, jobList.Count);

            foreach (var job in jobList)
                Assert.AreEqual(0, job.Retries);
        }

        [Test]
        public virtual void TestJobCommandsWithNestedFailingCommand()
        {
            // create a job
            CreateJob(TweetNestedCommandExceptionHandler.TYPE);

            // Execute the existing job
            var job = managementService.CreateJobQuery().First();

            Assert.AreEqual(3, job.Retries);

            try
            {
                managementService.ExecuteJob(job.Id);
                Assert.Fail("Exception expected");
            }
            catch (System.Exception)
            {
                // expected
            }

            job = managementService.CreateJobQuery().First();
            Assert.AreEqual(2, job.Retries);

            ExecuteAvailableJobs();

            // the job execution failed (job.Retries = 0)
            job = managementService.CreateJobQuery(c => c.RetriesFromPersistence == 0).First();
            Assert.NotNull(job);
            Assert.AreEqual(0, job.Retries);
        }

        [Test]
        [Deployment("resources/jobexecutor/jobFailingOnFlush.bpmn20.xml")]
        public virtual void TestJobRetriesDecrementedOnFailedFlush()
        {
            runtimeService.StartProcessInstanceByKey("testProcess");

            // there should be 1 job created:
            var job = managementService.CreateJobQuery().SingleOrDefault();
            Assert.NotNull(job);
            // with 3 retries
            Assert.AreEqual(3, job.Retries);

            // if we Execute the job
            WaitForJobExecutorToProcessAllJobs(6000);

            // the job is still present
            job = managementService.CreateJobQuery().FirstOrDefault();
            Assert.NotNull(job);
            // but has no more retires
            Assert.AreEqual(0, job.Retries);
        }

        [Test]
        [Ignore("TransactionListener没有实现")]
        public virtual void TestFailingTransactionListener()
        {
            Deployment(
                ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("testProcess")
                    .StartEvent()
                    .ServiceTask()
                    .CamundaClass(typeof(FailingTransactionListenerDelegate).AssemblyQualifiedName)
                    .CamundaAsyncBefore()
                    .EndEvent()
                    .Done());

            runtimeService.StartProcessInstanceByKey("testProcess");

            // there should be 1 job created:
            var job = managementService.CreateJobQuery().First();
            Assert.NotNull(job);
            // with 3 retries
            Assert.AreEqual(3, job.Retries);

            // if we Execute the job
            WaitForJobExecutorToProcessAllJobs(6000);

            // the job is still present
            job = managementService.CreateJobQuery().First();
            Assert.NotNull(job);
            // but has no more retires
            Assert.AreEqual(0, job.Retries);
            Assert.AreEqual("exception in transaction listener", job.ExceptionMessage);

            var stacktrace = managementService.GetJobExceptionStacktrace(job.Id);
            Assert.NotNull(stacktrace);
            Assert.True(stacktrace.Contains("java.lang.RuntimeException: exception in transaction listener"),
                "unexpected stacktrace, was <" + stacktrace + ">");

        }

        protected internal virtual void CreateJob(string handlerType)
        {
            processEngineConfiguration.CommandExecutorTxRequired.Execute(new CommandAnonymousInnerClass(this,
                handlerType));
        }

        private class CommandAnonymousInnerClass : ICommand<string>
        {
            private readonly string _handlerType;
            private readonly JobExecutorCmdExceptionTest _outerInstance;

            public CommandAnonymousInnerClass(JobExecutorCmdExceptionTest outerInstance, string handlerType)
            {
                _outerInstance = outerInstance;
                _handlerType = handlerType;
            }


            public virtual string Execute(CommandContext commandContext)
            {
                var message = _outerInstance.CreateMessage(_handlerType);
                commandContext.JobManager.Send(message);
                return message.Id;
            }
        }

        protected internal virtual MessageEntity CreateMessage(string handlerType)
        {
            var message = new MessageEntity();
            message.JobHandlerType = handlerType;
            return message;
        }

        protected internal virtual void ClearDatabase()
        {
            processEngineConfiguration.CommandExecutorTxRequired.Execute(new CommandAnonymousInnerClass2(this));
        }

        private class CommandAnonymousInnerClass2 : ICommand<object>
        {
            private readonly JobExecutorCmdExceptionTest _outerInstance;

            public CommandAnonymousInnerClass2(JobExecutorCmdExceptionTest outerInstance)
            {
                _outerInstance = outerInstance;
            }

            public virtual object Execute(CommandContext commandContext)
            {
                IList<IJob> jobs = commandContext.JobManager.GetAll().ToList().Cast<IJob>().ToList();
                foreach (var job in jobs)
                {
                    new DeleteJobCmd(job.Id).Execute(commandContext);
                    commandContext.HistoricJobLogManager.DeleteHistoricJobLogByJobId(job.Id);
                }

                //IList<IHistoricIncident> historicIncidents = commandContext.HistoricIncidentManager.GetAll().ToList().Cast<IHistoricIncident>().ToList();                
                //foreach (IHistoricIncident historicIncident in historicIncidents)
                //    commandContext.HistoricIncidentManager.Delete((HistoricIncidentEntity)historicIncident);
                commandContext.HistoricIncidentManager.DeleteAll();

                
                //IList<IHistoricJobLog> historicJobLogs = commandContext.HistoricJobLogManager.GetAll().ToList().Cast<IHistoricJobLog>().ToList();
                //foreach (IHistoricJobLog historicJobLog in historicJobLogs)
                //    commandContext.HistoricJobLogManager.DeleteHistoricJobLogById(historicJobLog.Id);
                commandContext.HistoricJobLogManager.DeleteAll();

                return null;
            }
        }
    }
}