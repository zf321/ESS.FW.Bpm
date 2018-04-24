using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using NUnit.Framework;

namespace Engine.Tests.ConCurrency
{


    /// <summary>
    /// thread1:
    ///  t=1: fetch byte variable
    ///  t=4: update byte variable value
    /// 
    /// thread2:
    ///  t=2: fetch and Delete byte variable and entity
    ///  t=3: commit transaction
    /// 
    /// This test ensures that thread1's command fails with an OptimisticLockingException,
    /// not with a NullPointerException or something in that direction.
    /// 
    /// 
    /// </summary>
    public class CompetingByteVariableAccessTest : ConcurrencyTestCase
    {

        private ThreadControl asyncThread;

        public virtual void testConcurrentVariableRemoval()
        {
            Deployment(ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("test").StartEvent().UserTask().EndEvent().Done());

            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final byte[] byteVar = "asd".GetBytes();
            byte[] byteVar = "asd".GetBytes();

            string pid = runtimeService.StartProcessInstanceByKey("test", ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables().PutValue("byteVar", byteVar)).Id;

            // start a controlled Fetch and Update variable command
            asyncThread = ExecuteControllableCommand<object>(new FetchAndUpdateVariableCmd(pid, "byteVar", "bsd".GetBytes()));

            asyncThread.WaitForSync();

            // now Delete the process instance, deleting the variable and its byte array entity
            runtimeService.DeleteProcessInstance(pid, null);

            // make the second thread continue
            // => this will a flush the FetchVariableCmd Context.
            // if the flush performs an update to the variable, it will Assert.Fail with an OLE
            asyncThread.ReportInterrupts();
            asyncThread.WaitUntilDone();

            System.Exception exception = asyncThread.Exception;
            Assert.NotNull(exception);
            Assert.True(exception is OptimisticLockingException);


        }

        internal class FetchAndUpdateVariableCmd : ControllableCommand<object>
        {

            protected internal string executionId;
            protected internal string varName;
            protected internal object newValue;

            public FetchAndUpdateVariableCmd(string executionId, string varName, object newValue)
            {
                this.executionId = executionId;
                this.varName = varName;
                this.newValue = newValue;
            }

            public override object Execute(CommandContext commandContext)
            {

                ExecutionEntity execution = commandContext.ExecutionManager.FindExecutionById(executionId);

                // fetch the variable instance but not the value (make sure the byte array is lazily fetched)
                VariableInstanceEntity varInstance = (VariableInstanceEntity)execution.GetVariableInstanceLocal(varName);
                string byteArrayValueId = varInstance.ByteArrayId;
                Assert.NotNull("Byte array id is expected to be not null", byteArrayValueId);

                var cachedByteArray = commandContext.ByteArrayManager.Get(byteArrayValueId);

                Assert.IsNull(cachedByteArray, "Byte array is expected to be not fetched yet / lazily fetched.");

                Monitor.Sync();

                // now update the value
                execution.SetVariableLocal(varInstance.Name, newValue);

                return null;
            }

        }
    }

}