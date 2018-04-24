using System;
using ESS.FW.Bpm.Engine.Authorization;

namespace ESS.FW.Bpm.Engine.Management
{
    /// <summary>
    ///     Fluent builder to update the suspension state of job definitions.
    /// </summary>
    public interface IUpdateJobDefinitionSuspensionStateBuilder
    {
        /// <summary>
        ///     Specify if the suspension states of the jobs of the provided job
        ///     definitions should also be updated. Default is <code>false</code>.
        /// </summary>
        /// <param name="includeJobs">
        ///     if <code>true</code>, all related jobs will be activated /
        ///     suspended too.
        /// </param>
        /// <returns> the builder </returns>
        IUpdateJobDefinitionSuspensionStateBuilder SetIncludeJobs(bool includeJobs);

        /// <summary>
        ///     Specify when the suspension state should be updated. Note that the
        ///     <b>
        ///         job
        ///         executor
        ///     </b>
        ///     needs to be active to use this.
        /// </summary>
        /// <param name="executionDate">
        ///     the date on which the job definition will be activated /
        ///     suspended. If <code>null</code>, the job definition is activated /
        ///     suspended immediately.
        /// </param>
        /// <returns> the builder </returns>
        IUpdateJobDefinitionSuspensionStateBuilder ExecutionDate(DateTime? executionDate);

        /// <summary>
        ///     Activates the provided job definitions.
        /// </summary>
        /// <exception cref="AuthorizationException">
        ///     <li>
        ///         if the current user has no <seealso cref="Permissions#UPDATE" />
        ///         permission on <seealso cref="Resources#PROCESS_DEFINITION" />
        ///     </li>
        ///     <li>
        ///         If <seealso cref="#includeJobs(boolean)" /> is set to <code>true</code>
        ///         and the user have no <seealso cref="Permissions#UPDATE_INSTANCE" />
        ///         permission on <seealso cref="Resources#PROCESS_DEFINITION" />
        ///         <seealso cref="Permissions#UPDATE" /> permission on any
        ///         <seealso cref="Resources#PROCESS_INSTANCE" />
        ///     </li>
        /// </exception>
        void Activate();

        /// <summary>
        ///     Suspends the provided job definitions. If a job definition is in state
        ///     suspended, it will be ignored by the job executor.
        /// </summary>
        /// <exception cref="AuthorizationException">
        ///     <li>
        ///         if the current user has no <seealso cref="Permissions#UPDATE" />
        ///         permission on <seealso cref="Resources#PROCESS_DEFINITION" />
        ///     </li>
        ///     <li>
        ///         If <seealso cref="#includeJobs(boolean)" /> is set to <code>true</code>
        ///         and the user have no <seealso cref="Permissions#UPDATE_INSTANCE" />
        ///         permission on <seealso cref="Resources#PROCESS_DEFINITION" />
        ///         <seealso cref="Permissions#UPDATE" /> permission on any
        ///         <seealso cref="Resources#PROCESS_INSTANCE" />
        ///     </li>
        /// </exception>
        void Suspend();

        string ProcessDefinitionKey { get; }

        string ProcessDefinitionId { get; }

        string ProcessDefinitionTenantId { get; }

        bool ProcessDefinitionTenantIdSet { get; }

        string JobDefinitionId { get; }

        bool IncludeJobs { get; }

        DateTime? GetExecutionDate();
    }
}