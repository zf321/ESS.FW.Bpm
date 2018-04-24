using System;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using NUnit.Framework;

namespace Engine.Tests.JobExecutor
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class JobExecutorTestCase : PluggableProcessEngineTestCase
    {
        protected internal TweetHandler TweetHandler = new TweetHandler();

        [SetUp]
        public virtual void SetUp()
        {
            processEngineConfiguration.JobHandlers.Add(TweetHandler.Type, TweetHandler);
        }
        [TearDown]
        public virtual void tearDown()
        {
            processEngineConfiguration.JobHandlers.Remove(TweetHandler.Type);
        }

        protected internal virtual MessageEntity CreateTweetMessage(string msg)
        {
            var message = new MessageEntity();
            message.JobHandlerType = "tweet";
            message.JobHandlerConfigurationRaw = msg;
            return message;
        }

        protected internal virtual TimerEntity CreateTweetTimer(string msg, DateTime duedate)
        {
            var timer = new TimerEntity();
            timer.JobHandlerType = "tweet";
            timer.JobHandlerConfigurationRaw = msg;
            timer.Duedate = duedate;
            return timer;
        }
    }
}