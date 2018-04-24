namespace ESS.FW.Bpm.Engine.Runtime
{
    /// <summary>
    ///     Fluent builder to update the suspension state of process instances.
    /// </summary>
    public interface IUpdateProcessInstanceSuspensionStateSelectBuilder
    {
        /// <summary>
        ///     Selects the process instance with the given id.
        /// </summary>
        /// <param name="processInstanceId">
        ///     id of the process instance
        /// </param>
        /// <returns> the builder </returns>
        IUpdateProcessInstanceSuspensionStateBuilder ByProcessInstanceId(string processInstanceId);

        /// <summary>
        ///     Selects the instances of the process definition with the given id.
        /// </summary>
        /// <param name="processDefinitionId">
        ///     id of the process definition
        /// </param>
        /// <returns> the builder </returns>
        IUpdateProcessInstanceSuspensionStateBuilder ByProcessDefinitionId(string processDefinitionId);

        /// <summary>
        ///     Selects the instances of the process definitions with the given key.
        /// </summary>
        /// <param name="processDefinitionKey">
        ///     key of the process definition
        /// </param>
        /// <returns> the builder </returns>
        IUpdateProcessInstanceSuspensionStateTenantBuilder ByProcessDefinitionKey(string processDefinitionKey);
    }
}