using System;
using ESS.FW.Bpm.Engine.Runtime;

namespace ESS.FW.Bpm.Engine.History
{
    /// <summary>
    ///     Represents a historic <seealso cref="IIncident" /> that is stored permanently.
    ///     
    /// </summary>
    public interface IHistoricIncident
    {
        /// <summary>
        ///     Returns the unique identifier for this incident.
        /// </summary>
        string Id { get; }

        /// <summary>
        ///     Time when the incident happened.
        /// </summary>
        DateTime CreateTime { get; }

        /// <summary>
        ///     Time when the incident has been resolved or deleted.
        /// </summary>
        DateTime EndTime { get; }

        /// <summary>
        ///     Returns the type of this incident to identify the
        ///     kind of incident.
        ///     <para>
        ///         For example: <code>failedJobs</code> will be returned
        ///         in the case of an incident, which identify failed job
        ///         during the execution of a process instance.
        ///     </para>
        /// </summary>
        string IncidentType { get; }

        /// <summary>
        ///     Returns the incident message.
        /// </summary>
        string IncidentMessage { get; }

        /// <summary>
        ///     Returns the specific execution on which this
        ///     incident has happened.
        /// </summary>
        string ExecutionId { get; }

        /// <summary>
        ///     Returns the id of the activity of the process instance
        ///     on which this incident has happened.
        /// </summary>
        string ActivityId { get; }

        /// <summary>
        ///     Returns the specific process instance on which this
        ///     incident has happened.
        /// </summary>
        string ProcessInstanceId { get; }

        /// <summary>
        ///     Returns the id of the process definition of this
        ///     process instance on which the incident has happened.
        /// </summary>
        string ProcessDefinitionId { get; }

        /// <summary>
        ///     Returns the key of the process definition of this
        ///     process instance on which the incident has happened.
        /// </summary>
        string ProcessDefinitionKey { get; }

        /// <summary>
        ///     Returns the id of the incident on which this incident
        ///     has been triggered.
        /// </summary>
        string CauseIncidentId { get; }

        /// <summary>
        ///     Returns the id of the root incident on which
        ///     this transitive incident has been triggered.
        /// </summary>
        string RootCauseIncidentId { get; }

        /// <summary>
        ///     Returns the payload of this incident.
        /// </summary>
        string Configuration { get; }

        /// <summary>
        ///     Returns <code>true</code>, iff the corresponding incident
        ///     has not been deleted or resolved.
        /// </summary>
        bool Open { get; }

        /// <summary>
        ///     Returns <code>true</code>, iff the corresponding incident
        ///     has been <strong>deleted</strong>.
        /// </summary>
        bool Deleted { get; }

        /// <summary>
        ///     Returns <code>true</code>, iff the corresponding incident
        ///     has been <strong>resolved</strong>.
        /// </summary>
        bool Resolved { get; }

        /// <summary>
        ///     Returns the id of the tenant this incident belongs to. Can be <code>null</code>
        ///     if the incident belongs to no single tenant.
        /// </summary>
        string TenantId { get; }

        /// <summary>
        ///     Returns the id of the job definition the incident belongs to. Can be <code>null</code>
        ///     if the incident belongs to no job definition.
        /// </summary>
        string JobDefinitionId { get; }
    }
}