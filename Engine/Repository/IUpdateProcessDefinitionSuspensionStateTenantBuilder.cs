namespace ESS.FW.Bpm.Engine.Repository
{
    /// <summary>
    ///     Fluent builder to update the suspension state of process definitions.
    /// </summary>
    public interface IUpdateProcessDefinitionSuspensionStateTenantBuilder : IUpdateProcessDefinitionSuspensionStateBuilder
    {
        /// <summary>
        ///     Specify that the process definition belongs to no tenant.
        /// </summary>
        /// <returns> the builder </returns>
        IUpdateProcessDefinitionSuspensionStateBuilder ProcessDefinitionWithoutTenantId();

        /// <summary>
        ///     Specify the id of the tenant the process definition belongs to.
        /// </summary>
        /// <param name="tenantId">
        ///     the id of the tenant
        /// </param>
        /// <returns> the builder </returns>
        IUpdateProcessDefinitionSuspensionStateBuilder ProcessDefinitionTenantId(string tenantId);
    }
}