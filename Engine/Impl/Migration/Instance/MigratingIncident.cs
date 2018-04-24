using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.History.Impl.Event;
using ESS.FW.Bpm.Engine.History.Impl.Producer;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Runtime;

namespace ESS.FW.Bpm.Engine.Impl.migration.instance
{
    public class MigratingIncident : IMigratingInstance
    {
        protected internal IncidentEntity Incident;
        protected internal string targetJobDefinitionId;
        protected internal ScopeImpl TargetScope;

        public MigratingIncident(IncidentEntity incident, ScopeImpl targetScope)
        {
            this.Incident = incident;
            this.TargetScope = targetScope;
        }

        public virtual string TargetJobDefinitionId
        {
            set { targetJobDefinitionId = value; }
        }

        public virtual bool Detached
        {
            get { return ReferenceEquals(Incident.ExecutionId, null); }
        }

        public virtual void DetachState()
        {
            //incident.Execution = null;
        }

        public virtual void AttachState(MigratingScopeInstance newOwningInstance)
        {
            AttachTo(newOwningInstance.ResolveRepresentativeExecution());
        }

        public virtual void AttachState(MigratingTransitionInstance targetTransitionInstance)
        {
            AttachTo(targetTransitionInstance.ResolveRepresentativeExecution());
        }

        public virtual void MigrateState()
        {
            Incident.ActivityId = TargetScope.Id;
            Incident.ProcessDefinitionId = TargetScope.ProcessDefinition.Id;
            Incident.JobDefinitionId = targetJobDefinitionId;

            MigrateHistory();
        }

        public virtual void MigrateDependentEntities()
        {
            // nothing to do
        }

        protected internal virtual void MigrateHistory()
        {
            var historyLevel = Context.ProcessEngineConfiguration.HistoryLevel;

            if (historyLevel.IsHistoryEventProduced(HistoryEventTypes.IncidentMigrate, this))
                HistoryEventProcessor.ProcessHistoryEvents(new HistoryEventCreatorAnonymousInnerClass(this));
        }

        protected internal virtual void AttachTo(ExecutionEntity execution)
        {
            Incident.Execution = execution;
        }

        private class HistoryEventCreatorAnonymousInnerClass : HistoryEventProcessor.HistoryEventCreator
        {
            private readonly MigratingIncident _outerInstance;

            public HistoryEventCreatorAnonymousInnerClass(MigratingIncident outerInstance)
            {
                this._outerInstance = outerInstance;
            }

            public override HistoryEvent CreateHistoryEvent(IHistoryEventProducer producer)
            {
                return producer.CreateHistoricIncidentMigrateEvt((IIncident) _outerInstance.Incident);
            }
        }
    }
}