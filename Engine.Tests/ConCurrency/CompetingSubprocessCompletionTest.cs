using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Impl.Cmd;
using ESS.FW.Bpm.Engine.Task;
using NUnit.Framework;

namespace Engine.Tests.ConCurrency
{

    /// <summary>
    /// 
    /// </summary>
    public class CompetingSubprocessCompletionTest : PluggableProcessEngineTestCase
    {

        //private static Logger LOG = ProcessEngineLogger.TEST_LOGGER.Logger;

        internal static ControllableThread activeThread;

        public class CompleteTaskThread : ControllableThread
        {
            private readonly CompetingSubprocessCompletionTest outerInstance;

            internal string taskId;
            internal OptimisticLockingException exception;
            public CompleteTaskThread(CompetingSubprocessCompletionTest outerInstance, string taskId)
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
                    outerInstance.processEngineConfiguration.CommandExecutorTxRequired.Execute(new ControlledCommand<object>(activeThread, new CompleteTaskCmd(taskId, null)));

                }
                catch (OptimisticLockingException e)
                {
                    this.exception = e;
                }
                Debug.WriteLine(TestContext.CurrentContext.Test.Name + " ends");
            }
        }

        [Deployment]
        public virtual void testCompetingSubprocessEnd()
        {
            runtimeService.StartProcessInstanceByKey("CompetingSubprocessEndProcess");

            IList<ITask> tasks = taskService.CreateTaskQuery().ToList();
            Assert.AreEqual(3, tasks.Count);

            Debug.WriteLine("test thread starts thread one");
            CompleteTaskThread threadOne = new CompleteTaskThread(this, tasks[0].Id);
            threadOne.startAndWaitUntilControlIsReturned();

            Debug.WriteLine("test thread continues to start thread two");
            CompleteTaskThread threadTwo = new CompleteTaskThread(this, tasks[1].Id);
            threadTwo.startAndWaitUntilControlIsReturned();

            Debug.WriteLine("test thread notifies thread 1");
            threadOne.proceedAndWaitTillDone();
            Assert.IsNull(threadOne.exception);

            Debug.WriteLine("test thread notifies thread 2");
            threadTwo.proceedAndWaitTillDone();
            Assert.NotNull(threadTwo.exception);
            AssertTextPresent("was updated by another transaction concurrently", threadTwo.exception.Message);
        }

    }

}