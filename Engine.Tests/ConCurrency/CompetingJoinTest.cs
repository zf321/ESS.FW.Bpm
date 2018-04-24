using System.Diagnostics;
using System.Linq;
using System.Threading;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Impl.Cmd;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.ConCurrency
{

    /// <summary>
    /// 
    /// </summary>
    public class CompetingJoinTest : PluggableProcessEngineTestCase
    {

        ////private static Logger LOG = ProcessEngineLogger.TEST_LOGGER.Logger;

        internal Thread testThread = Thread.CurrentThread;
        internal static ControllableThread activeThread;
        internal static string jobId;

        public class SignalThread : ControllableThread
        {
            private readonly CompetingJoinTest outerInstance;

            internal string executionId;
            internal OptimisticLockingException exception;
            public SignalThread(CompetingJoinTest outerInstance, string executionId)
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
                    outerInstance.processEngineConfiguration.CommandExecutorTxRequired.Execute(new ControlledCommand<object>(activeThread, new SignalCmd(executionId, null, null, null)));

                }
                catch (OptimisticLockingException e)
                {
                    this.exception = e;
                }
                Debug.WriteLine(TestContext.CurrentContext.Test.Name + " ends");
            }
        }

        [Deployment]
        public virtual void testCompetingJoins()
        {
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("CompetingJoinsProcess");
            IExecution execution1 = runtimeService.CreateExecutionQuery(c=>c.ProcessInstanceId ==processInstance.Id&& c.ActivityId =="wait1").First();

            IExecution execution2 = runtimeService.CreateExecutionQuery(c=>c.ProcessInstanceId ==processInstance.Id&& c.ActivityId =="wait2").First();

            Debug.WriteLine("test thread starts thread one");
            SignalThread threadOne = new SignalThread(this, execution1.Id);
            threadOne.startAndWaitUntilControlIsReturned();

            Debug.WriteLine("test thread continues to start thread two");
            SignalThread threadTwo = new SignalThread(this, execution2.Id);
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