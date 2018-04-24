using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Runtime;

namespace ESS.FW.Bpm.Engine.Impl.Incident
{
    /// <summary>
    ///     <para>
    ///         An incident handler that logs incidents of a certain type
    ///         as instances of <seealso cref="IIncident" /> to the engine database.
    ///     </para>
    ///     <para>
    ///         By default, the process engine has two default handlers:
    ///         <ul>
    ///             <li>
    ///                 type <code>failedJob</code>: Indicates jobs without retries left. This incident handler is active by
    ///                 default and must be disabled
    ///                 via
    ///                 <seealso cref="ProcessEngineConfiguration#setCreateIncidentOnFailedJobEnabled(boolean)"></seealso>
    ///             </li>
    ///             <li>
    ///                 type <code>failedExternalTask</code>: Indicates external tasks without retries left
    ///             </li>
    ///         </ul>
    ///     </para>
    /// </summary>
    /// <seealso cref="IIncidentHandler"></seealso>
    public class DefaultIncidentHandler : IIncidentHandler
    {
        public DefaultIncidentHandler(string type)
        {
            IncidentHandlerType = type;
        }

        public virtual string IncidentHandlerType { get; }

        public virtual void HandleIncident(IncidentContext context, string message)
        {
            CreateIncident(context, message);
        }

        public virtual void ResolveIncident(IncidentContext context)
        {
            RemoveIncident(context, true);
        }

        public virtual void DeleteIncident(IncidentContext context)
        {
            RemoveIncident(context, false);
        }

        public virtual IIncident CreateIncident(IncidentContext context, string message)
        {
            var newIncident = IncidentEntity.CreateAndInsertIncident(IncidentHandlerType, context, message);

            if (!ReferenceEquals(context.ExecutionId, null))
                newIncident.CreateRecursiveIncidents();

            return newIncident;
        }

        protected internal virtual void RemoveIncident(IncidentContext context, bool incidentResolved)
        {
            var incidents =
                Context.CommandContext.IncidentManager.FindIncidentByConfiguration(context.Configuration);

            foreach (var currentIncident in incidents)
            {
                var incident = (IncidentEntity) currentIncident;
                if (incidentResolved)
                    incident.Resolve();
                else
                    incident.Delete();
            }
        }
    }
}