using System;
using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;
using NUnit.Framework;

namespace Engine.Tests.JobExecutor
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class JobExecutorTest : JobExecutorTestCase
    {
        [Test]
        public virtual void TestBasicJobExecutorOperation()
        {
            var commandExecutor = processEngineConfiguration.CommandExecutorTxRequired;
            commandExecutor.Execute(new CommandAnonymousInnerClass(this));

            ExecuteAvailableJobs();

            ISet<string> messages = new HashSet<string>(TweetHandler.Messages);
            ISet<string> expectedMessages = new HashSet<string>();
            expectedMessages.Add("message-one");
            expectedMessages.Add("message-two");
            expectedMessages.Add("message-three");
            expectedMessages.Add("message-four");
            expectedMessages.Add("timer-one");
            expectedMessages.Add("timer-two");

            Assert.AreEqual(new SortedSet<string>(expectedMessages), new SortedSet<string>(messages));

            commandExecutor.Execute(new CommandAnonymousInnerClass2(this));
        }

        private class CommandAnonymousInnerClass : ICommand<object>
        {
            private readonly JobExecutorTest _outerInstance;

            public CommandAnonymousInnerClass(JobExecutorTest outerInstance)
            {
                _outerInstance = outerInstance;
            }

            public virtual object Execute(CommandContext commandContext)
            {
                var jobManager = commandContext.JobManager;
                jobManager.Send(_outerInstance.CreateTweetMessage("message-one"));
                jobManager.Send(_outerInstance.CreateTweetMessage("message-two"));
                jobManager.Send(_outerInstance.CreateTweetMessage("message-three"));
                jobManager.Send(_outerInstance.CreateTweetMessage("message-four"));

                jobManager.Schedule(_outerInstance.CreateTweetTimer("timer-one", DateTime.Now));
                jobManager.Schedule(_outerInstance.CreateTweetTimer("timer-two", DateTime.Now));
                return null;
            }
        }

        private class CommandAnonymousInnerClass2 : ICommand<object>
        {
            private readonly JobExecutorTest _outerInstance;

            public CommandAnonymousInnerClass2(JobExecutorTest outerInstance)
            {
                _outerInstance = outerInstance;
            }

            public virtual object Execute(CommandContext commandContext)
            {
                //throw new NotImplementedException();
                IList<IHistoricJobLog> historicJobLogs = commandContext.HistoricJobLogManager.GetAll().ToList().Cast< IHistoricJobLog>().ToList(); //ProcessEngineConfiguration.HistoryService.CreateHistoricJobLogQuery().ToList();
                foreach (IHistoricJobLog historicJobLog in historicJobLogs)
                {
                    commandContext.HistoricJobLogManager.DeleteHistoricJobLogById(historicJobLog.Id);
                }
                return null;
            }
        }

        [Test]
        public virtual void TestJobExecutorHintConfiguration()
        {
            var engineConfig1 = ProcessEngineConfiguration.CreateStandaloneInMemProcessEngineConfiguration();

            Assert.True(engineConfig1.HintJobExecutor, "default setting is true");

            var engineConfig2 =
                ProcessEngineConfiguration.CreateStandaloneInMemProcessEngineConfiguration().SetHintJobExecutor(false);

            Assert.IsFalse(engineConfig2.HintJobExecutor);

            var engineConfig3 =
                ProcessEngineConfiguration.CreateStandaloneInMemProcessEngineConfiguration().SetHintJobExecutor(true);

            Assert.True(engineConfig3.HintJobExecutor);
        }

        [Test]
        public virtual void TestAcquiredJobs()
        {
            IList<string> firstBatch = new List<string> {"a", "b", "c"};
            IList<string> secondBatch = new List<string> {"d", "e", "f"};
            IList<string> thirdBatch = new List<string> {"g"};

            var acquiredJobs = new AcquiredJobs(0);
            acquiredJobs.AddJobIdBatch(firstBatch);
            acquiredJobs.AddJobIdBatch(secondBatch);
            acquiredJobs.AddJobIdBatch(thirdBatch);

            Assert.AreEqual(firstBatch, acquiredJobs.JobIdBatches[0]);
            Assert.AreEqual(secondBatch, acquiredJobs.JobIdBatches[1]);
            Assert.AreEqual(thirdBatch, acquiredJobs.JobIdBatches[2]);

            acquiredJobs.RemoveJobId("a");
            Assert.AreEqual(new List<string> {"b", "c"}, acquiredJobs.JobIdBatches[0]);
            Assert.AreEqual(secondBatch, acquiredJobs.JobIdBatches[1]);
            Assert.AreEqual(thirdBatch, acquiredJobs.JobIdBatches[2]);

            Assert.AreEqual(3, acquiredJobs.JobIdBatches.Count);
            acquiredJobs.RemoveJobId("g");
            Assert.AreEqual(2, acquiredJobs.JobIdBatches.Count);
        }
    }
}