using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using NUnit.Framework;

namespace Engine.Tests.ConCurrency
{

    /// <summary>
    /// This test makes sure that if one thread loads a variable
    /// of type object, it does not Assert.Fail with a OLE if the variable is also
    /// concurrently deleted.
    /// 
    /// Some context: this failed if the variable instance entity was first loaded,
    /// and, before loading the byte array, both the variable and the byte array were
    /// deleted by a concurrent transaction AND that transaction was comitted, before
    /// the bytearray was loaded.
    /// => loading the byte array returned null which triggered setting to null the
    /// byteArrayId value on the VariableInstanceEntity which in turn triggered an
    /// update of the variable instance entity itself which failed with OLE because
    /// the VariableInstanceEntity was already deleted.
    /// 
    /// +
    /// |
    /// |    Test Thread           Async Thread
    /// |   +-----------+         +------------+
    /// |      start PI
    /// |      (with var)               +
    /// |         +                     |
    /// |         |                     v
    /// |         |                 fetch VarInst
    /// |         |                 (not byte array)
    /// |         |                     +
    /// |         v                     |
    /// |      Delete PI                |
    /// |         +                     v
    /// |         |                 fetch byte array (=>null)
    /// |         |                     +
    /// |         |                     |
    /// |         |                     v
    /// |         |                 flush()
    /// |         |                 (this must not perform
    /// |         v                 update to VarInst)
    /// v  time
    /// 
    /// 
    /// 
    /// 
    /// </summary>
    public class CompetingVariableFetchingAndDeletionTest : ConcurrencyTestCase
    {

        private ThreadControl asyncThread;

        public virtual void testConcurrentFetchAndDelete()
        {

            Deployment(ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("test").StartEvent().UserTask().EndEvent().Done());

            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final java.util.List<String> listVar = java.util.("a", "b");
            IList<string> listVar = new List<string> { "a", "b"};
            string pid = runtimeService.StartProcessInstanceByKey("test", ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables().PutValue("listVar", listVar)).Id;

            // start a controlled Fetch variable command
            asyncThread = ExecuteControllableCommand<object>(new FetchVariableCmd(pid, "listVar"));

            // wait for async thread to load the variable (but not the byte array)
            asyncThread.WaitForSync();

            // now Delete the process instance
            runtimeService.DeleteProcessInstance(pid, null);

            // make the second thread continue
            // => this will a flush the FetchVariableCmd Context.
            // if the flush performs an update to the variable, it will Assert.Fail with an OLE
            asyncThread.MakeContinue();
            asyncThread.WaitUntilDone();

        }

        internal class FetchVariableCmd : ControllableCommand<object>
        {

            protected internal string executionId;
            protected internal string varName;

            public FetchVariableCmd(string executionId, string varName)
            {
                this.executionId = executionId;
                this.varName = varName;
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

                // now trigger the fetching of the byte array
                object value = varInstance.Value;
                Assert.IsNull(value, "Expecting the value to be null (deleted)");

                return null;
            }

        }

    }

}