

using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl
{
    //using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;

    /// <summary>
    ///      
    /// </summary>
    /// @param
    /// <T> the type of the extra param to determine the priority </param>
    public interface IPriorityProvider<in T>
    {
        /// <param name="execution">
        ///     may be null when the job is not created in the context of a
        ///     running process instance (e.g. a timer start event)
        /// </param>
        /// <param name="param"> extra parameter to determine priority on </param>
        /// <param name="jobDefinitionId"> the job definition id if related to a job </param>
        /// <returns> the determined priority </returns>
        long DeterminePriority(ExecutionEntity execution, T param, string jobDefinitionId);
    }
}