using System;
using ESS.FW.Bpm.Engine.Runtime;

namespace ESS.FW.Bpm.Engine.History
{
    /// <summary>
    ///     A single execution of a case definition that is stored permanently.
    ///     
    /// </summary>
    public interface IHistoricCaseInstance
    {
        /// <summary>
        ///     The case instance id (== as the id of the runtime <seealso cref="ICaseInstance" />).
        /// </summary>
        string Id { get; }

        /// <summary>
        ///     The user provided unique reference to this process instance.
        /// </summary>
        string BusinessKey { get; }

        /// <summary>
        ///     The case definition reference.
        /// </summary>
        string CaseDefinitionId { get; }

        /// <summary>
        ///     The case definition key
        /// </summary>
        string CaseDefinitionKey { get; }

        /// <summary>
        ///     The case definition name
        /// </summary>
        string CaseDefinitionName { get; }

        /// <summary>
        ///     The time the case was created.
        /// </summary>
        DateTime? CreateTime { get; }

        /// <summary>
        ///     The time the case was closed.
        /// </summary>
        DateTime? CloseTime { get; }

        /// <summary>
        ///     The difference between <seealso cref="#getCloseTime()" /> and <seealso cref="#getCreateTime()" />.
        /// </summary>
        long? DurationInMillis { get; }

        /// <summary>
        ///     The authenticated user that created this case instance.
        /// </summary>
        /// <seealso cref= IdentityService# setAuthenticatedUserId( String
        /// )
        /// </seealso>
        string CreateUserId { get; }

        /// <summary>
        ///     The case instance id of a potential super case instance or null if no super case instance exists.
        /// </summary>
        string SuperCaseInstanceId { get; }

        /// <summary>
        ///     The process instance id of a potential super process instance or null if no super process instance exists.
        /// </summary>
        string SuperProcessInstanceId { get; }

        /// <summary>
        ///     The id of the tenant this historic case instance belongs to. Can be <code>null</code>
        ///     if the historic case instance belongs to no single tenant.
        /// </summary>
        string TenantId { get; }

        /// <summary>
        ///     Check if the case is active.
        /// </summary>
        bool Active { get; }

        /// <summary>
        ///     Check if the case is completed.
        /// </summary>
        bool Completed { get; }

        /// <summary>
        ///     Check if the case is terminated.
        /// </summary>
        bool Terminated { get; }

        /// <summary>
        ///     Check if the case is closed.
        /// </summary>
        bool Closed { get; }
    }
}