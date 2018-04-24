namespace ESS.FW.Bpm.Engine.Management
{
    /// <summary>
    ///     Fluent builder to update the suspension state of job definitions.
    /// </summary>
    public interface IUpdateJobDefinitionSuspensionStateSelectBuilder
    {
        /// <summary>
        ///     Selects the job definition with the given id.
        /// </summary>
        /// <param name="jobDefinitionId">
        ///     id of the job definition
        /// </param>
        /// <returns> the builder </returns>
        IUpdateJobDefinitionSuspensionStateBuilder ByJobDefinitionId(string jobDefinitionId);

        /// <summary>
        ///     Selects the job definitions of the process definition with the given id.
        /// </summary>
        /// <param name="processDefinitionId">
        ///     id of the process definition
        /// </param>
        /// <returns> the builder </returns>
        IUpdateJobDefinitionSuspensionStateBuilder ByProcessDefinitionId(string processDefinitionId);

        /// <summary>
        ///     Selects the job definitions of the process definitions with the given key.
        /// </summary>
        /// <param name="processDefinitionKey">
        ///     key of the process definition
        /// </param>
        /// <returns> the builder </returns>
        IUpdateJobDefinitionSuspensionStateTenantBuilder ByProcessDefinitionKey(string processDefinitionKey);
    }
}