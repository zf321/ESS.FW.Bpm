namespace ESS.FW.Bpm.Engine.Management
{
    /// <summary>
    ///     Fluent builder to update the suspension state of jobs.
    /// </summary>
    public interface IUpdateJobSuspensionStateTenantBuilder : IUpdateJobSuspensionStateBuilder
    {
        /// <summary>
        ///     Specify that the process definition belongs to no tenant.
        /// </summary>
        /// <returns> the builder </returns>
        IUpdateJobSuspensionStateBuilder ProcessDefinitionWithoutTenantId();

        /// <summary>
        ///     Specify the id of the tenant the process definition belongs to.
        /// </summary>
        /// <param name="tenantId">
        ///     the id of the tenant
        /// </param>
        /// <returns> the builder </returns>
        IUpdateJobSuspensionStateBuilder ProcessDefinitionTenantId(string tenantId);
    }
}