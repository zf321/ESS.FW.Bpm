namespace ESS.FW.Bpm.Engine.Management
{
    /// <summary>
    ///     Fluent builder to update the suspension state of job definitions.
    /// </summary>
    public interface IUpdateJobDefinitionSuspensionStateTenantBuilder : IUpdateJobDefinitionSuspensionStateBuilder
    {
        /// <summary>
        ///     Specify that the process definition belongs to no tenant.
        /// </summary>
        /// <returns> the builder </returns>
        IUpdateJobDefinitionSuspensionStateBuilder GetProcessDefinitionWithoutTenantId();

        /// <summary>
        ///     Specify the id of the tenant the process definition belongs to.
        /// </summary>
        /// <param name="tenantId">
        ///     the id of the tenant
        /// </param>
        /// <returns> the builder </returns>
        IUpdateJobDefinitionSuspensionStateBuilder SetProcessDefinitionTenantId(string tenantId);
    }
}