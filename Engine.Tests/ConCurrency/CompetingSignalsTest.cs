using System.Diagnostics;
using System.Threading;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;


namespace Engine.Tests.ConCurrency
{

    /// <summary>
    /// 
    /// </summary>
    public class CompetingSignalsTest : PluggableProcessEngineTestCase
    {

        //private static Logger LOG = ProcessEngineLogger.TEST_LOGGER.Logger;

        internal Thread testThread = Thread.CurrentThread;
        internal static ControllableThread activeThread;

        public class SignalThread : ControllableThread
        {
            private readonly CompetingSignalsTest outerInstance;


            internal string executionId;
            internal OptimisticLockingException exception;

            public SignalThread(CompetingSignalsTest outerInstance, string executionId)
            {
                this.outerInstance = outerInstance;
                this.executionId = executionId;
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
                    outerInstance.runtimeService.Signal(executionId);
                }
                catch (OptimisticLockingException e)
                {
                    this.exception = e;
                }
                Debug.WriteLine(TestContext.CurrentContext.Test.Name + " ends");
            }
        }

        public class ControlledConcurrencyBehavior : IActivityBehavior
        {
            public void Execute(IActivityExecution execution)
            {
                activeThread.returnControlToTestThreadAndWait();
            }
            
        }

        [Deployment]
        public virtual void testCompetingSignals()
        {
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("CompetingSignalsProcess");
            string ProcessInstanceId = processInstance.Id;

            Debug.WriteLine("test thread starts thread one");
            SignalThread threadOne = new SignalThread(this, ProcessInstanceId);
            threadOne.startAndWaitUntilControlIsReturned();

            Debug.WriteLine("test thread continues to start thread two");
            SignalThread threadTwo = new SignalThread(this, ProcessInstanceId);
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