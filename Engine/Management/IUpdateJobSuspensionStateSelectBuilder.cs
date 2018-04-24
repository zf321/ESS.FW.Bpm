namespace ESS.FW.Bpm.Engine.Management
{
    /// <summary>
    ///     Fluent builder to update the suspension state of jobs.
    /// </summary>
    public interface IUpdateJobSuspensionStateSelectBuilder
    {
        /// <summary>
        ///     Selects the job with the given id.
        /// </summary>
        /// <param name="jobId">
        ///     id of the job
        /// </param>
        /// <returns> the builder </returns>
        IUpdateJobSuspensionStateBuilder ByJobId(string jobId);

        /// <summary>
        ///     Selects the jobs of the job definition with the given id.
        /// </summary>
        /// <param name="jobDefinitionId">
        ///     id of the job definition
        /// </param>
        /// <returns> the builder </returns>
        IUpdateJobSuspensionStateBuilder ByJobDefinitionId(string jobDefinitionId);

        /// <summary>
        ///     Selects the jobs of the process instance with the given id.
        /// </summary>
        /// <param name="processInstanceId">
        ///     id of the process instance
        /// </param>
        /// <returns> the builder </returns>
        IUpdateJobSuspensionStateBuilder ByProcessInstanceId(string processInstanceId);

        /// <summary>
        ///     Selects the jobs of the process definition with the given id.
        /// </summary>
        /// <param name="processDefinitionId">
        ///     id of the process definition
        /// </param>
        /// <returns> the builder </returns>
        IUpdateJobSuspensionStateBuilder ByProcessDefinitionId(string processDefinitionId);

        /// <summary>
        ///     Selects the jobs of the process definitions with the given key.
        /// </summary>
        /// <param name="processDefinitionKey">
        ///     key of the process definition
        /// </param>
        /// <returns> the builder </returns>
        IUpdateJobSuspensionStateTenantBuilder ByProcessDefinitionKey(string processDefinitionKey);
    }
}