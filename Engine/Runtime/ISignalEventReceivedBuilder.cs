using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Authorization;

namespace ESS.FW.Bpm.Engine.Runtime
{
    /// <summary>
    ///     Fluent builder to notify the process engine that a signal event has been
    ///     received.
    /// </summary>
    public interface ISignalEventReceivedBuilder
    {
        /// <summary>
        ///     Add the given variables to the triggered executions.
        /// </summary>
        /// <param name="variables">
        ///     a map of variables added to the executions
        /// </param>
        /// <returns> the builder </returns>
        ISignalEventReceivedBuilder SetVariables(IDictionary<string, object> variables);

        /// <summary>
        ///     Specify a single execution to deliver the signal to.
        /// </summary>
        /// <param name="executionId">
        ///     the id of the process instance or the execution to deliver the
        ///     signal to
        /// </param>
        /// <returns> the builder </returns>
        ISignalEventReceivedBuilder SetExecutionId(string executionId);

        /// <summary>
        ///     Specify a tenant to deliver the signal to. The signal can only be received
        ///     on executions or process definitions which belongs to the given tenant.
        ///     Cannot be used in combination with <seealso cref="#executionId(String)" />.
        /// </summary>
        /// <param name="tenantId">
        ///     the id of the tenant
        /// </param>
        /// <returns> the builder </returns>
        ISignalEventReceivedBuilder SetTenantId(string tenantId);

        /// <summary>
        ///     Specify that the signal can only be received on executions or process
        ///     definitions which belongs to no tenant. Cannot be used in combination with
        ///     <seealso cref="#executionId(String)" />.
        /// </summary>
        /// <returns> the builder </returns>
        ISignalEventReceivedBuilder WithoutTenantId();

        /// <summary>
        ///     <para>
        ///         Delivers the signal to waiting executions and process definitions. The notification and instantiation happen
        ///         synchronously.
        ///     </para>
        ///     <para>
        ///         Note that the signal delivers to all tenants if no tenant is specified
        ///         using <seealso cref="#tenantId(String)" /> or <seealso cref="#withoutTenantId()" />.
        ///     </para>
        /// </summary>
        /// <exception cref="ProcessEngineException">
        ///     if a single execution is specified and no such execution exists
        ///     or has not subscribed to the signal
        /// </exception>
        /// <exception cref="AuthorizationException">
        ///     <li>
        ///         if notify an execution and the user has no
        ///         <seealso cref="Permissions#UPDATE" /> permission on
        ///         <seealso cref="Resources#PROCESS_INSTANCE" /> or no
        ///         <seealso cref="Permissions#UPDATE_INSTANCE" /> permission on
        ///         <seealso cref="Resources#PROCESS_DEFINITION" />.
        ///     </li>
        ///     <li>
        ///         if start a new process instance and the user has no
        ///         <seealso cref="Permissions#CREATE" /> permission on
        ///         <seealso cref="Resources#PROCESS_INSTANCE" /> and no
        ///         <seealso cref="Permissions#CREATE_INSTANCE" /> permission on
        ///         <seealso cref="Resources#PROCESS_DEFINITION" />.
        ///     </li>
        /// </exception>
        void Send();
    }
}