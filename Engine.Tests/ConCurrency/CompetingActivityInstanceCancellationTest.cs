using System.Diagnostics;
using System.Threading;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Impl.Cmd;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.ConCurrency
{

    /// <summary>
    /// 
    /// 
    /// </summary>
    public class CompetingActivityInstanceCancellationTest : PluggableProcessEngineTestCase
    {

        ////private static Logger LOG = ProcessEngineLogger.TEST_LOGGER.Logger;

        internal Thread testThread = Thread.CurrentThread;
        internal static ControllableThread activeThread;
        internal static string jobId;

        public class CancelActivityInstance : ControllableThread
        {
            private readonly CompetingActivityInstanceCancellationTest outerInstance;


            internal string ProcessInstanceId;
            internal string activityInstanceId;
            internal OptimisticLockingException exception;

            public CancelActivityInstance(CompetingActivityInstanceCancellationTest outerInstance, string ProcessInstanceId, string activityInstanceId)
            {
                this.outerInstance = outerInstance;
                this.ProcessInstanceId = ProcessInstanceId;
                this.activityInstanceId = activityInstanceId;
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
                    // Todo: ControlledCommand<T>构造，传参是否正确。
                    outerInstance.processEngineConfiguration.CommandExecutorTxRequired.Execute(new ControlledCommand<ActivityInstanceCancellationCmd>(activeThread, new ActivityInstanceCancellationCmd(ProcessInstanceId, activityInstanceId) as ICommand<ActivityInstanceCancellationCmd>));
                    //outerInstance.processEngineConfiguration.CommandExecutorTxRequired.Execute(new ControlledCommand<ActivityInstanceCancellationCmd>(activeThread, new ActivityInstanceCancellationCmd(ProcessInstanceId, activityInstanceId)));

                }
                catch (OptimisticLockingException e)
                {
                    this.exception = e;
                }
                Debug.WriteLine(TestContext.CurrentContext.Test.Name + " ends");
            }
        }

        [Deployment(new string[] { "resources/concurrency/CompetingForkTest.TestCompetingFork.bpmn20.xml" })]
        public virtual void testCompetingCancellation()
        {
            string ProcessInstanceId = runtimeService.StartProcessInstanceByKey("process").Id;

            IActivityInstance activityInstance = runtimeService.GetActivityInstance(ProcessInstanceId);
            IActivityInstance[] children = activityInstance.ChildActivityInstances;

            string task1ActivityInstanceId = null;
            string task2ActivityInstanceId = null;
            string task3ActivityInstanceId = null;

            foreach (IActivityInstance currentInstance in children)
            {

                string id = currentInstance.Id;
                string activityId = currentInstance.ActivityId;

                if ("task1".Equals(activityId))
                {
                    task1ActivityInstanceId = id;
                }
                else if ("task2".Equals(activityId))
                {
                    task2ActivityInstanceId = id;
                }
                else if ("task3".Equals(activityId))
                {
                    task3ActivityInstanceId = id;
                }
                else
                {
                    Assert.Fail();
                }
            }

            Debug.WriteLine("test thread starts thread one");
            CancelActivityInstance threadOne = new CancelActivityInstance(this, ProcessInstanceId, task1ActivityInstanceId);
            threadOne.startAndWaitUntilControlIsReturned();

            Debug.WriteLine("test thread thread two");
            CancelActivityInstance threadTwo = new CancelActivityInstance(this, ProcessInstanceId, task2ActivityInstanceId);
            threadTwo.startAndWaitUntilControlIsReturned();

            Debug.WriteLine("test thread continues to start thread three");
            CancelActivityInstance threadThree = new CancelActivityInstance(this, ProcessInstanceId, task3ActivityInstanceId);
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