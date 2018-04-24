using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.migration.instance.parser
{
    /// <summary>
    ///     
    /// </summary>
    public class IncidentInstanceHandler : IMigratingInstanceParseHandler<IncidentEntity>
    {
        public virtual void Handle(MigratingInstanceParseContext parseContext, IncidentEntity incident)
        {
            if (ReferenceEquals(incident.Configuration, null) &&
                (IsFailedJobIncident(incident) || IsExternalTaskIncident(incident)))
                HandleCallActivityIncident(parseContext, incident);
            else if (IsFailedJobIncident(incident))
                HandleFailedJobIncident(parseContext, incident);
            else if (IsExternalTaskIncident(incident))
                HandleExternalTaskIncident(parseContext, incident);
        }

        protected internal virtual void HandleCallActivityIncident(MigratingInstanceParseContext parseContext,
            IncidentEntity incident)
        {
            //var owningInstance = parseContext.getMigratingActivityInstanceById(incident.Execution.ActivityInstanceId);
            //if (owningInstance != null)
            //{
            //    parseContext.consume(incident);
            //    var migratingIncident = new MigratingIncident(incident, owningInstance.TargetScope);
            //    owningInstance.addMigratingDependentInstance(migratingIncident);
            //}
        }

        protected internal virtual bool IsFailedJobIncident(IncidentEntity incident)
        {
            //return IncidentEntity.FAILED_JOB_HANDLER_TYPE.Equals(incident.IncidentType);
            return false;
        }

        protected internal virtual void HandleFailedJobIncident(MigratingInstanceParseContext parseContext,
            IncidentEntity incident)
        {
            var owningInstance = parseContext.GetMigratingJobInstanceById(incident.Configuration);
            if (owningInstance != null)
            {
                parseContext.Consume(incident);
                if (owningInstance.Migrates())
                {
                    var migratingIncident = new MigratingIncident(incident, owningInstance.TargetScope);
                    var targetJobDefinitionEntity = owningInstance.TargetJobDefinitionEntity;
                    if (targetJobDefinitionEntity != null)
                        migratingIncident.TargetJobDefinitionId = targetJobDefinitionEntity.Id;
                    owningInstance.AddMigratingDependentInstance(migratingIncident);
                }
            }
        }

        protected internal virtual bool IsExternalTaskIncident(IncidentEntity incident)
        {
            //return IncidentEntity.EXTERNAL_TASK_HANDLER_TYPE.Equals(incident.IncidentType);
            return true;
        }

        protected internal virtual void HandleExternalTaskIncident(MigratingInstanceParseContext parseContext,
            IncidentEntity incident)
        {
            var owningInstance = parseContext.GetMigratingExternalTaskInstanceById(incident.Configuration);
            if (owningInstance != null)
            {
                parseContext.Consume(incident);
                var migratingIncident = new MigratingIncident(incident, owningInstance.TargetScope);
                owningInstance.AddMigratingDependentInstance(migratingIncident);
            }
        }
    }
}