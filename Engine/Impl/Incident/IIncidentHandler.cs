using ESS.FW.Bpm.Engine.Impl.Cfg;

namespace ESS.FW.Bpm.Engine.Impl.Incident
{
    /// <summary>
    ///     The <seealso cref="IIncidentHandler" /> interface may be implemented by components
    ///     that handle and resolve incidents of a specific type that occur during the
    ///     execution of a process instance.
    ///     <para>
    ///         Custom implementations of this interface may be wired through
    ///         <seealso
    ///             cref="ProcessEngineConfigurationImpl#setCustomIncidentHandlers(java.Util.List)" />
    ///         .
    ///     </para>
    /// </summary>
    /// <seealso cref= "FailedJobIncidentHandler"</seealso>
    /// <seealso cref= "org.camunda.bpm.engine.runtime.Incident"
    /// </seealso>
    public interface IIncidentHandler
    {
        /// <summary>
        ///     Returns the incident type this handler activates for.
        /// </summary>
        string IncidentHandlerType { get; }

        /// <summary>
        ///     Handle an incident that arose in the context of an execution.
        /// </summary>
        void HandleIncident(IncidentContext context, string message);

        /// <summary>
        ///     Called in situations in which an incidenthandler may wich to resolve existing incidents
        ///     The implementation receives this callback to enable it to resolve any open incidents that
        ///     may exist.
        /// </summary>
        void ResolveIncident(IncidentContext context);

        /// <summary>
        ///     Called in situations in which an incidenthandler may wich to delete existing incidents
        ///     Example: when a scope is ended or a job is deleted. The implementation receives
        ///     this callback to enable it to delete any open incidents that may exist.
        /// </summary>
        void DeleteIncident(IncidentContext context);
    }
}