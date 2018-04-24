namespace ESS.FW.Bpm.Engine.Impl.Cfg.Multitenancy
{
    /// <summary>
    ///     SPI which can be implemented to assign tenant ids to process instances, case instances and historic decision
    ///     instances.
    ///     <para>
    ///         The SPI is invoked if the process definition, case definition or decision definition does not have a tenant id
    ///         or
    ///         execution does not have a tenant id.
    ///     </para>
    ///     <para>
    ///         An implementation of this SPI can be set on the <seealso cref="ProcessEngineConfigurationImpl" />.
    ///         
    ///         
    ///     </para>
    /// </summary>
    public interface ITenantIdProvider
    {
        /// <summary>
        ///     Invoked when a process instance is started and the Process Definition does not have a tenant id.
        ///     <para>
        ///         Implementors can either return a tenant id or null. If null is returned the process instance is not assigned a
        ///         tenant id.
        ///     </para>
        /// </summary>
        /// <param name="ctx"> holds information about the process instance which is about to be started. </param>
        /// <returns> a tenant id or null if case the implementation does not assign a tenant id to the process instance </returns>
        string ProvideTenantIdForProcessInstance(TenantIdProviderProcessInstanceContext ctx);

        /// <summary>
        ///     Invoked when a case instance is started and the Case Definition does not have a tenant id.
        ///     <para>
        ///         Implementors can either return a tenant id or null. If null is returned the case instance is not assigned a
        ///         tenant id.
        ///     </para>
        /// </summary>
        /// <param name="ctx"> holds information about the case instance which is about to be started. </param>
        /// <returns> a tenant id or null if case the implementation does not assign a tenant id to case process instance </returns>
        string ProvideTenantIdForCaseInstance(TenantIdProviderCaseInstanceContext ctx);

        /// <summary>
        ///     Invoked when a historic decision instance is created and the Decision Definition or the Execution does not have a
        ///     tenant id.
        ///     <para>
        ///         Implementors can either return a tenant id or null. If null is returned the historic decision instance is not
        ///         assigned a tenant id.
        ///     </para>
        /// </summary>
        /// <param name="ctx"> holds information about the decision definition and the execution. </param>
        /// <returns> a tenant id or null if case the implementation does not assign a tenant id to the historic decision instance </returns>
        string ProvideTenantIdForHistoricDecisionInstance(TenantIdProviderHistoricDecisionInstanceContext ctx);
    }
}