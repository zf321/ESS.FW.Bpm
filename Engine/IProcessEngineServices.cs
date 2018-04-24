namespace ESS.FW.Bpm.Engine
{
    /// <summary>
    ///     <para>
    ///         Base interface providing access to the process engine's
    ///         public API services.
    ///     </para>
    ///     
    /// </summary>
    public interface IProcessEngineServices
    {
        /// <summary>
        ///     Returns the process engine's <seealso cref="RuntimeService" />.
        /// </summary>
        /// <returns> the <seealso cref="RuntimeService" /> object. </returns>
        IRuntimeService RuntimeService { get; }

        /// <summary>
        ///     Returns the process engine's <seealso cref="RepositoryService" />.
        /// </summary>
        /// <returns> the <seealso cref="RepositoryService" /> object. </returns>
        IRepositoryService RepositoryService { get; }

        /// <summary>
        ///     Returns the process engine's <seealso cref="FormService" />.
        /// </summary>
        /// <returns> the <seealso cref="FormService" /> object. </returns>
        IFormService FormService { get; }

        /// <summary>
        ///     Returns the process engine's <seealso cref="TaskService" />.
        /// </summary>
        /// <returns> the <seealso cref="TaskService" /> object. </returns>
        ITaskService TaskService { get; }

        /// <summary>
        ///     Returns the process engine's <seealso cref="HistoryService" />.
        /// </summary>
        /// <returns> the <seealso cref="HistoryService" /> object. </returns>
        IHistoryService HistoryService { get; }

        /// <summary>
        ///     Returns the process engine's <seealso cref="IdentityService" />.
        /// </summary>
        /// <returns> the <seealso cref="IdentityService" /> object. </returns>
        IIdentityService IdentityService { get; }

        /// <summary>
        ///     Returns the process engine's <seealso cref="ManagementService" />.
        /// </summary>
        /// <returns> the <seealso cref="ManagementService" /> object. </returns>
        IManagementService ManagementService { get; }

        /// <summary>
        ///     Returns the process engine's <seealso cref="AuthorizationService" />.
        /// </summary>
        /// <returns> the <seealso cref="AuthorizationService" /> object. </returns>
        IAuthorizationService AuthorizationService { get; }

        /// <summary>
        ///     Returns the engine's <seealso cref="CaseService" />.
        /// </summary>
        /// <returns>
        ///     the <seealso cref="CaseService" /> object.
        /// </returns>
        ICaseService CaseService { get; }

        /// <summary>
        ///     Returns the engine's <seealso cref="FilterService" />.
        /// </summary>
        /// <returns>
        ///     the <seealso cref="FilterService" /> object.
        /// </returns>
        IFilterService FilterService { get; }

        /// <summary>
        ///     Returns the engine's <seealso cref="ExternalTaskService" />.
        /// </summary>
        /// <returns> the <seealso cref="ExternalTaskService" /> object. </returns>
        IExternalTaskService ExternalTaskService { get; }

        /// <summary>
        ///     Returns the engine's <seealso cref="DecisionService" />.
        /// </summary>
        /// <returns> the <seealso cref="DecisionService" /> object. </returns>
        IDecisionService DecisionService { get; }
    }
}