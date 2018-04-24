namespace ESS.FW.Bpm.Engine.Runtime
{
    /// <summary>
    ///     Fluent builder to update the suspension state of process instances.
    /// </summary>
    public interface IUpdateProcessInstanceSuspensionStateTenantBuilder : IUpdateProcessInstanceSuspensionStateBuilder
    {
        /// <summary>
        ///     Specify that the process definition belongs to no tenant.
        /// </summary>
        /// <returns> the builder </returns>
        IUpdateProcessInstanceSuspensionStateTenantBuilder ProcessDefinitionWithoutTenantId();

        /// <summary>
        ///     Specify the id of the tenant the process definition belongs to.
        /// </summary>
        /// <param name="tenantId">
        ///     the id of the tenant
        /// </param>
        /// <returns> the builder </returns>
        IUpdateProcessInstanceSuspensionStateTenantBuilder ProcessDefinitionTenantId(string tenantId);
    }
}