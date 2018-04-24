using System.Diagnostics;
using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.Impl.Cmd;
using NUnit.Framework;

namespace Engine.Tests.ConCurrency
{
    [TestFixture]
    public class ConcurrentVariableUpdateTest : PluggableProcessEngineTestCase
    {

        //private static Logger LOG = ProcessEngineLogger.TEST_LOGGER.Logger;

        internal static ControllableThread activeThread;

        internal class SetTaskVariablesThread : ControllableThread
        {
            private readonly ConcurrentVariableUpdateTest outerInstance;


            internal OptimisticLockingException optimisticLockingException;
            internal System.Exception exception;

            protected internal object variableValue;
            protected internal string taskId;
            protected internal string VariableName;

            public SetTaskVariablesThread(ConcurrentVariableUpdateTest outerInstance, string taskId, string VariableName, object variableValue)
            {
                this.outerInstance = outerInstance;
                this.taskId = taskId;
                this.VariableName = VariableName;
                this.variableValue = variableValue;
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
                    outerInstance.processEngineConfiguration.CommandExecutorTxRequired.Execute(new ControlledCommand<object>(activeThread, new SetTaskVariablesCmd(taskId, Collections.SingletonMap(VariableName, variableValue), false)));

                }
                catch (OptimisticLockingException e)
                {
                    this.optimisticLockingException = e;
                }
                catch (System.Exception e)
                {
                    this.exception = e;
                }
                Debug.WriteLine(TestContext.CurrentContext.Test.Name + " ends");
            }
        }

        [Deployment(new string[] { "resources/concurrency/ConcurrentVariableUpdateTest.process.bpmn20.xml" })]
        public virtual void testConcurrentVariableCreate()
        {

            runtimeService.StartProcessInstanceByKey("testProcess", Collections.SingletonMap<string, object>("varName1", "someValue"));

            string VariableName = "varName";
            string taskId = taskService.CreateTaskQuery().First().Id;

            SetTaskVariablesThread thread1 = new SetTaskVariablesThread(this, taskId, VariableName, "someString");
            thread1.startAndWaitUntilControlIsReturned();

            // this should Assert.Fail with integrity constraint violation
            SetTaskVariablesThread thread2 = new SetTaskVariablesThread(this, taskId, VariableName, "someString");
            thread2.startAndWaitUntilControlIsReturned();

            thread1.proceedAndWaitTillDone();
            Assert.IsNull(thread1.exception);
            Assert.IsNull(thread1.optimisticLockingException);

            thread2.proceedAndWaitTillDone();
            Assert.NotNull(thread2.exception);
            Assert.IsNull(thread2.optimisticLockingException);

            // should not Assert.Fail with FK violation because one of the variables is not deleted.
            taskService.Complete(taskId);
        }

        [Deployment(new string[] { "resources/concurrency/ConcurrentVariableUpdateTest.process.bpmn20.xml" })]
        public virtual void testConcurrentVariableUpdate()
        {

            runtimeService.StartProcessInstanceByKey("testProcess");

            string taskId = taskService.CreateTaskQuery().First().Id;
            string VariableName = "varName";

            taskService.SetVariable(taskId, VariableName, "someValue");

            SetTaskVariablesThread thread1 = new SetTaskVariablesThread(this, taskId, VariableName, "someString");
            thread1.startAndWaitUntilControlIsReturned();

            // this fails with an optimistic locking exception
            SetTaskVariablesThread thread2 = new SetTaskVariablesThread(this, taskId, VariableName, "someOtherString");
            thread2.startAndWaitUntilControlIsReturned();

            thread1.proceedAndWaitTillDone();
            thread2.proceedAndWaitTillDone();

            Assert.IsNull(thread1.optimisticLockingException);
            Assert.NotNull(thread2.optimisticLockingException);

            // succeeds
            taskService.Complete(taskId);
        }

        [Deployment(new string[] { "resources/concurrency/ConcurrentVariableUpdateTest.process.bpmn20.xml" })]
        public virtual void testConcurrentVariableUpdateTypeChange()
        {

            runtimeService.StartProcessInstanceByKey("testProcess");

            string taskId = taskService.CreateTaskQuery().First().Id;
            string VariableName = "varName";

            taskService.SetVariable(taskId, VariableName, "someValue");

            SetTaskVariablesThread thread1 = new SetTaskVariablesThread(this, taskId, VariableName, 100l);
            thread1.startAndWaitUntilControlIsReturned();

            // this fails with an optimistic locking exception
            SetTaskVariablesThread thread2 = new SetTaskVariablesThread(this, taskId, VariableName, "someOtherString");
            thread2.startAndWaitUntilControlIsReturned();

            thread1.proceedAndWaitTillDone();
            thread2.proceedAndWaitTillDone();

            Assert.IsNull(thread1.optimisticLockingException);
            Assert.NotNull(thread2.optimisticLockingException);

            // succeeds
            taskService.Complete(taskId);
        }

    }

}