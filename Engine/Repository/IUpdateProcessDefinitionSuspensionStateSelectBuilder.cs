namespace ESS.FW.Bpm.Engine.Repository
{
    /// <summary>
    ///     Fluent builder to update the suspension state of process definitions.
    /// </summary>
    public interface IUpdateProcessDefinitionSuspensionStateSelectBuilder
    {
        /// <summary>
        ///     Selects the process definition with the given id.
        /// </summary>
        /// <param name="processDefinitionId">
        ///     id of the process definition
        /// </param>
        /// <returns> the builder </returns>
        IUpdateProcessDefinitionSuspensionStateBuilder ByProcessDefinitionId(string processDefinitionId);

        /// <summary>
        ///     Selects the process definitions with the given key.
        /// </summary>
        /// <param name="processDefinitionKey">
        ///     key of the process definition
        /// </param>
        /// <returns> the builder </returns>
        IUpdateProcessDefinitionSuspensionStateTenantBuilder ByProcessDefinitionKey(string processDefinitionKey);
    }
}