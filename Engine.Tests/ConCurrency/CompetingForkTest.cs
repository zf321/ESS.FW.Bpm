using System.Diagnostics;
using System.Linq;
using System.Threading;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Impl.Cmd;
using ESS.FW.Bpm.Engine.Task;
using NUnit.Framework;


namespace Engine.Tests.ConCurrency
{    
    [Ignore("")]
    public class CompetingForkTest : PluggableProcessEngineTestCase
    {

        ////private static Logger LOG = ProcessEngineLogger.TEST_LOGGER.Logger;

        internal Thread testThread = Thread.CurrentThread;
        internal static ControllableThread activeThread;
        internal static string jobId;

        public class CompleteTaskThread : ControllableThread
        {
            private readonly CompetingForkTest outerInstance;


            internal string taskId;
            internal OptimisticLockingException exception;

            public CompleteTaskThread(CompetingForkTest outerInstance, string taskId)
            {
                this.outerInstance = outerInstance;
                this.taskId = taskId;
            }
            public override void startAndWaitUntilControlIsReturned()
            {
                lock (this)
                {
                    activeThread = this;
                    base.startAndWaitUntilControlIsReturned();
                }
            }

            public virtual void run()
            {
                try
                {
                    outerInstance.processEngineConfiguration.CommandExecutorTxRequired.Execute<object>(new ControlledCommand<object>(activeThread, new CompleteTaskCmd(taskId, null)));

                }
                catch (OptimisticLockingException e)
                {
                    this.exception = e;
                }
                Debug.WriteLine(TestContext.CurrentContext.Test.Name + " ends");
            }
        }

        [Deployment]
        public virtual void FAILING_testCompetingFork()
        {
            runtimeService.StartProcessInstanceByKey("process");

            IQueryable<ITask> query = taskService.CreateTaskQuery();

            string task1 = query.Where(c=>c.TaskDefinitionKey=="task1").First().Id;

            string task2 = query.Where(c=>c.TaskDefinitionKey=="task2").First().Id;

            string task3 = query.Where(c=>c.TaskDefinitionKey=="task3").First().Id;

            Debug.WriteLine("test thread starts thread one");
            CompleteTaskThread threadOne = new CompleteTaskThread(this, task1);
            threadOne.startAndWaitUntilControlIsReturned();

            Debug.WriteLine("test thread thread two");
            CompleteTaskThread threadTwo = new CompleteTaskThread(this, task2);
            threadTwo.startAndWaitUntilControlIsReturned();

            Debug.WriteLine("test thread continues to start thread three");
            CompleteTaskThread threadThree = new CompleteTaskThread(this, task3);
            threadThree.startAndWaitUntilControlIsReturned();

            Debug.WriteLine("test thread notifies thread 1");
            threadOne.proceedAndWaitTillDone();
            Assert.IsNull(threadOne.exception);

            Debug.WriteLine("test thread notifies thread 2");
            threadTwo.proceedAndWaitTillDone();
            Assert.NotNull(threadTwo.exception);
            AssertTextPresent("was updated by another transaction concurrently", threadTwo.exception.Message);

            Debug.WriteLine("test thread notifies thread 3");
            threadThree.proceedAndWaitTillDone();
            Assert.NotNull(threadThree.exception);
            AssertTextPresent("was updated by another transaction concurrently", threadThree.exception.Message);
        }
    }

}