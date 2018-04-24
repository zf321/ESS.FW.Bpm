using System;
using ESS.FW.Bpm.Engine.Authorization;

namespace ESS.FW.Bpm.Engine.Repository
{
    /// <summary>
    ///     Fluent builder to update the suspension state of process definitions.
    /// </summary>
    public interface IUpdateProcessDefinitionSuspensionStateBuilder
    {
        /// <summary>
        ///     Specify if the suspension states of the process instances of the provided
        ///     process definitions should also be updated. Default is <code>false</code>.
        /// </summary>
        /// <param name="includeProcessInstances">
        ///     if <code>true</code>, all related process instances will be
        ///     activated / suspended too.
        /// </param>
        /// <returns> the builder </returns>
        IUpdateProcessDefinitionSuspensionStateBuilder IncludeProcessInstances(bool includeProcessInstances);

        /// <summary>
        ///     Specify when the suspension state should be updated. Note that the
        ///     <b>
        ///         job
        ///         executor
        ///     </b>
        ///     needs to be active to use this.
        /// </summary>
        /// <param name="executionDate">
        ///     the date on which the process definition will be activated /
        ///     suspended. If <code>null</code>, the process definition is
        ///     activated / suspended immediately.
        /// </param>
        /// <returns> the builder </returns>
        IUpdateProcessDefinitionSuspensionStateBuilder ExecutionDate(DateTime executionDate);

        /// <summary>
        ///     Activates the provided process definitions.
        /// </summary>
        /// <exception cref="ProcessEngineException">
        ///     If no such processDefinition can be found.
        /// </exception>
        /// <exception cref="AuthorizationException">
        ///     <li>
        ///         if the user has no <seealso cref="Permissions#UPDATE" /> permission on
        ///         <seealso cref="Resources#PROCESS_DEFINITION" />
        ///     </li>
        ///     <li>
        ///         if <seealso cref="#includeProcessInstances(boolean)" /> is set to
        ///         <code>true</code> and the user have no <seealso cref="Permissions#UPDATE" />
        ///         permission on <seealso cref="Resources#PROCESS_INSTANCE" /> or no
        ///         <seealso cref="Permissions#UPDATE_INSTANCE" /> permission on
        ///         <seealso cref="Resources#PROCESS_DEFINITION" />.
        ///     </li>
        /// </exception>
        void Activate();

        /// <summary>
        ///     Suspends the provided process definitions. If a process definition is in
        ///     state suspended, it will not be possible to start new process instances
        ///     based on this process definition.
        /// </summary>
        /// <exception cref="ProcessEngineException">
        ///     If no such processDefinition can be found.
        /// </exception>
        /// <exception cref="AuthorizationException">
        ///     <li>
        ///         if the user has no <seealso cref="Permissions#UPDATE" /> permission on
        ///         <seealso cref="Resources#PROCESS_DEFINITION" />
        ///     </li>
        ///     <li>
        ///         if <seealso cref="#includeProcessInstances(boolean)" /> is set to
        ///         <code>true</code> and the user have no <seealso cref="Permissions#UPDATE" />
        ///         permission on <seealso cref="Resources#PROCESS_INSTANCE" /> or no
        ///         <seealso cref="Permissions#UPDATE_INSTANCE" /> permission on
        ///         <seealso cref="Resources#PROCESS_DEFINITION" />.
        ///     </li>
        /// </exception>
        void Suspend();
    }
}