using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.context
{

    /// <summary>
    ///     Represents a delegation execution context which should return the current
    ///     delegation execution.
    /// </summary>
    public class DelegateExecutionContext
    {
        /// <summary>
        ///     Returns the current delegation execution or null if the
        ///     execution is not available.
        /// </summary>
        /// <returns> the current delegation execution or null if not available </returns>
        public static IDelegateExecution CurrentDelegationExecution
        {
            get
            {
                var bpmnExecutionContext = Impl.Context.BpmnExecutionContext;
                ExecutionEntity executionEntity = null;
                if (bpmnExecutionContext != null)
                {
                    //executionEntity = bpmnExecutionContext.Execution;
                    executionEntity = bpmnExecutionContext.GetExecution<ExecutionEntity>();
                }
                return executionEntity;
            }
        }
    }
}

