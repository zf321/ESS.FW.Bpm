
 

using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Externaltask
{
    /// <summary>
    ///     Represents the logger for the external ITask.
    ///      
    /// </summary>
    public class ExternalTaskLogger : ProcessEngineLogger
    {
        /// <summary>
        ///     Logs that the priority could not be determined in the given context.
        /// </summary>
        /// <param name="execution"> the context that is used for determining the priority </param>
        /// <param name="value"> the default value </param>
        /// <param name="e"> the exception which was catched </param>
        public virtual void CouldNotDeterminePriority(ExecutionEntity execution, object value, ProcessEngineException e)
        {
            LogWarn("001",
                "Could not determine priority for external ITask created in context of execution {}. Using default priority {}",
                execution, value, e);
        }
    }
}

