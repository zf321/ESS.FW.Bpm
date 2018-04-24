using System.Collections.Generic;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Externaltask;
using ESS.FW.Bpm.Engine.Impl.Cmd;
using ESS.FW.Bpm.Engine.Impl.Externaltask;
using NUnit.Framework;

namespace Engine.Tests.ConCurrency
{



    /// <summary>
    /// 
    /// 
    /// </summary>
    public class CompetingExternalTaskFetchingTest : PluggableProcessEngineTestCase
    {

        public class ExternalTaskFetcherThread : ControllableThread
        {
            private readonly CompetingExternalTaskFetchingTest outerInstance;


            protected internal string workerId;
            protected internal int results;
            protected internal string topic;

            protected internal IList<ILockedExternalTask> fetchedTasks;
            protected internal OptimisticLockingException exception;

            public ExternalTaskFetcherThread(CompetingExternalTaskFetchingTest outerInstance, string workerId, int results, string topic)
            {
                this.outerInstance = outerInstance;
                this.workerId = workerId;
                this.results = results;
                this.topic = topic;
            }

            public virtual void run()
            {
                IDictionary<string, TopicFetchInstruction> instructions = new Dictionary<string, TopicFetchInstruction>();

                TopicFetchInstruction instruction = new TopicFetchInstruction(topic, 10000L);
                instructions[topic] = instruction;

                try
                {
                    fetchedTasks = outerInstance.processEngineConfiguration.CommandExecutorTxRequired.Execute(new FetchExternalTasksCmd(workerId, results, instructions));
                }
                catch (OptimisticLockingException e)
                {
                    exception = e;
                }
            }
        }

        [Deployment]
        public virtual void testCompetingExternalTaskFetching()
        {
            runtimeService.StartProcessInstanceByKey("oneExternalTaskProcess");

            ExternalTaskFetcherThread thread1 = new ExternalTaskFetcherThread(this, "thread1", 5, "externalTaskTopic");
            ExternalTaskFetcherThread thread2 = new ExternalTaskFetcherThread(this, "thread2", 5, "externalTaskTopic");

            // both threads fetch the same task and wait before flushing the lock
            thread1.startAndWaitUntilControlIsReturned();
            thread2.startAndWaitUntilControlIsReturned();

            // thread1 succeeds
            thread1.proceedAndWaitTillDone();
            Assert.IsNull(thread1.exception);
            Assert.AreEqual(1, thread1.fetchedTasks.Count);

            // thread2 does not succeed in locking the job
            thread2.proceedAndWaitTillDone();
            Assert.AreEqual(0, thread2.fetchedTasks.Count);
            // but does not Assert.Fail with an OptimisticLockingException
            Assert.IsNull(thread2.exception);
        }
    }

}