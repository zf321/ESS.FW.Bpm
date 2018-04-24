using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Cmd;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;
using ESS.FW.Bpm.Engine.Impl.Util;
using NUnit.Framework;

namespace Engine.Tests.JobExecutor
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class JobExecutorCmdHappyTest : JobExecutorTestCase
    {
        [Test]
        public virtual void TestJobCommandsWithMessage()
        {
            var commandExecutor = processEngineConfiguration.CommandExecutorTxRequired;
            var jobExecutor = processEngineConfiguration.JobExecutor;
            var jobId = commandExecutor.Execute(new CommandAnonymousInnerClass(this));

            var acquiredJobs = commandExecutor.Execute(new AcquireJobsCmd(jobExecutor));
            var jobIdsList = acquiredJobs.JobIdBatches;
            Assert.AreEqual(1, jobIdsList.Count);

            var jobIds = jobIdsList[0];

            IList<string> expectedJobIds = new List<string>();
            expectedJobIds.Add(jobId);

            Assert.AreEqual(expectedJobIds, new List<string>(jobIds));
            Assert.AreEqual(0, TweetHandler.Messages.Count);

            ExecuteJobHelper.ExecuteJob(jobId, commandExecutor);

            Assert.AreEqual("i'm coding a test", TweetHandler.Messages[0]);
            Assert.AreEqual(1, TweetHandler.Messages.Count);

            ClearDatabase();
        }

        private class CommandAnonymousInnerClass : ICommand<string>
        {
            private readonly JobExecutorCmdHappyTest _outerInstance;

            public CommandAnonymousInnerClass(JobExecutorCmdHappyTest outerInstance)
            {
                _outerInstance = outerInstance;
            }


            public virtual string Execute(CommandContext commandContext)
            {
                var message = _outerInstance.CreateTweetMessage("i'm coding a test");
                commandContext.JobManager.Send(message);
                return message.Id;
            }
        }

        internal const long SomeTime = 928374923546L;
        internal const long Second = 1000;

        [Test]
        public virtual void TestJobCommandsWithTimer()
        {
            // clock gets automatically reset in LogTestCase.RunTest
            ClockUtil.CurrentTime = new DateTime(SomeTime);

            var commandExecutor = processEngineConfiguration.CommandExecutorTxRequired;
            var jobExecutor = processEngineConfiguration.JobExecutor;

            var jobId = commandExecutor.Execute(new CommandAnonymousInnerClass2(this));

            var acquiredJobs = commandExecutor.Execute(new AcquireJobsCmd(jobExecutor));
            var jobIdsList = acquiredJobs.JobIdBatches;
            Assert.AreEqual(0, jobIdsList.Count);

            IList<string> expectedJobIds = new List<string>();

            //ClockUtil.CurrentTime = new DateTime(SomeTime).AddSeconds(20 * Second);
            ClockUtil.CurrentTime = ClockUtil.CurrentTime.AddSeconds(20 * Second);

            acquiredJobs = commandExecutor.Execute(new AcquireJobsCmd(jobExecutor, jobExecutor.MaxJobsPerAcquisition));
            jobIdsList = acquiredJobs.JobIdBatches;
            Assert.AreEqual(1, jobIdsList.Count);

            var jobIds = jobIdsList[0];

            expectedJobIds.Add(jobId);
            Assert.AreEqual(expectedJobIds, new List<string>(jobIds));

            Assert.AreEqual(0, TweetHandler.Messages.Count);

            ExecuteJobHelper.ExecuteJob(jobId, commandExecutor);

            Assert.AreEqual("i'm coding a test", TweetHandler.Messages[0]);
            Assert.AreEqual(1, TweetHandler.Messages.Count);

            ClearDatabase();
        }

        private class CommandAnonymousInnerClass2 : ICommand<string>
        {
            private readonly JobExecutorCmdHappyTest _outerInstance;

            public CommandAnonymousInnerClass2(JobExecutorCmdHappyTest outerInstance)
            {
                _outerInstance = outerInstance;
            }


            public virtual string Execute(CommandContext commandContext)
            {
                var timer = _outerInstance.CreateTweetTimer("i'm coding a test", ClockUtil.CurrentTime.AddSeconds(10 * Second)/*new DateTime(SomeTime).AddSeconds(10 * Second)*/);
                commandContext.JobManager.Schedule(timer);
                return timer.Id;
            }
        }

        protected internal virtual void ClearDatabase()
        {
            processEngineConfiguration.CommandExecutorTxRequired.Execute(new CommandAnonymousInnerClass3(this));
        }

        private class CommandAnonymousInnerClass3 : ICommand<object>
        {
            private readonly JobExecutorCmdHappyTest _outerInstance;

            public CommandAnonymousInnerClass3(JobExecutorCmdHappyTest outerInstance)
            {
                _outerInstance = outerInstance;
            }

            public virtual object Execute(CommandContext commandContext)
            {
                //IList<IHistoricJobLog> historicJobLogs = processEngineConfiguration.HistoryService.CreateHistoricJobLogQuery().ToList();

                //foreach (IHistoricJobLog historicJobLog in historicJobLogs)
                //{
                //  commandContext.HistoricJobLogManager.DeleteHistoricJobLogById(historicJobLog.Id);
                //}

                return null;
            }
        }
    }
}