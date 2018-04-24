using ESS.FW.Bpm.Engine.Impl.Core.Instance;
using ESS.FW.Bpm.Engine.Impl.Core.Operation;

namespace ESS.FW.Bpm.Engine.Impl.Interceptor
{
    //using CoreAtomicOperation = org.camunda.bpm.engine.impl.core.Operation.CoreAtomicOperation;

    /// <summary>
    ///     
    /// </summary>
    public class ContextLogger : ProcessEngineLogger
    {
        public virtual void DebugExecutingAtomicOperation<T1>(ICoreAtomicOperation<T1> executionOperation, CoreExecution execution) where T1: CoreExecution
        {
            LogDebug("001", "Executing atomic operation {0} on {1}", executionOperation, execution);
        }

        public virtual void DebugException(System.Exception throwable)
        {
            LogDebug("002", "Exception while closing command context: {0}{1}", throwable.Message, throwable);
        }

        public virtual void InfoException(System.Exception throwable)
        {
            LogInfo("003", "Exception while closing command context: {0}{1}", throwable.Message, throwable);
        }

        public virtual void ErrorException(System.Exception throwable)
        {
            LogError("004", "Exception while closing command context: {0} {1}", throwable.Message, throwable);
        }

        public virtual void ExceptionWhileInvokingOnCommandFailed(System.Exception t)
        {
            LogError("005", "Exception while invoking onCommandFailed({0})", t);
        }

        public virtual void BpmnStackTrace(string @string)
        {
            LogError("006", @string);
        }
    }
}