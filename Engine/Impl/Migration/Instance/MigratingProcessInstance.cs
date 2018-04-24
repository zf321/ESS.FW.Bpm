using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Migration;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Runtime;

namespace ESS.FW.Bpm.Engine.Impl.migration.instance
{
    /// <summary>
    ///     
    /// </summary>
    public class MigratingProcessInstance
    {
        protected internal static readonly MigrationLogger Logger = ProcessEngineLogger.MigrationLogger;
        protected internal IList<MigratingActivityInstance> migratingActivityInstances;

        protected internal IList<MigratingCompensationEventSubscriptionInstance>
            migratingCompensationSubscriptionInstances;

        protected internal IList<MigratingEventScopeInstance> migratingEventScopeInstances;
        protected internal IList<MigratingTransitionInstance> migratingTransitionInstances;

        protected internal string processInstanceId;
        protected internal MigratingActivityInstance rootInstance;
        protected internal ProcessDefinitionEntity sourceDefinition;
        protected internal ProcessDefinitionEntity targetDefinition;

        public MigratingProcessInstance(string processInstanceId, ProcessDefinitionEntity sourceDefinition,
            ProcessDefinitionEntity targetDefinition)
        {
            this.processInstanceId = processInstanceId;
            migratingActivityInstances = new List<MigratingActivityInstance>();
            migratingTransitionInstances = new List<MigratingTransitionInstance>();
            migratingEventScopeInstances = new List<MigratingEventScopeInstance>();
            migratingCompensationSubscriptionInstances = new List<MigratingCompensationEventSubscriptionInstance>();
            this.sourceDefinition = sourceDefinition;
            this.targetDefinition = targetDefinition;
        }

        public virtual MigratingActivityInstance RootInstance
        {
            get { return rootInstance; }
            set { rootInstance = value; }
        }


        public virtual ICollection<MigratingActivityInstance> MigratingActivityInstances
        {
            get { return migratingActivityInstances; }
        }

        public virtual ICollection<MigratingTransitionInstance> MigratingTransitionInstances
        {
            get { return migratingTransitionInstances; }
        }

        public virtual ICollection<MigratingEventScopeInstance> MigratingEventScopeInstances
        {
            get { return migratingEventScopeInstances; }
        }

        public virtual ICollection<MigratingCompensationEventSubscriptionInstance>
            MigratingCompensationSubscriptionInstances
        {
            get { return migratingCompensationSubscriptionInstances; }
        }

        public virtual ICollection<MigratingScopeInstance> MigratingScopeInstances
        {
            get
            {
                ISet<MigratingScopeInstance> result = new HashSet<MigratingScopeInstance>();

                result.UnionWith(migratingActivityInstances);
                result.UnionWith(migratingEventScopeInstances);

                return result;
            }
        }

        public virtual ProcessDefinitionEntity SourceDefinition
        {
            get { return sourceDefinition; }
        }

        public virtual ProcessDefinitionEntity TargetDefinition
        {
            get { return targetDefinition; }
        }

        public virtual string ProcessInstanceId
        {
            get { return processInstanceId; }
        }

        public virtual MigratingActivityInstance AddActivityInstance(IMigrationInstruction migrationInstruction,
            IActivityInstance activityInstance, ScopeImpl sourceScope, ScopeImpl targetScope,
            ExecutionEntity scopeExecution)
        {
            var migratingActivityInstance = new MigratingActivityInstance(activityInstance, migrationInstruction,
                sourceScope, targetScope, scopeExecution);

            migratingActivityInstances.Add(migratingActivityInstance);

            if (processInstanceId.Equals(activityInstance.Id))
                rootInstance = migratingActivityInstance;

            return migratingActivityInstance;
        }

        public virtual MigratingTransitionInstance AddTransitionInstance(IMigrationInstruction migrationInstruction,
            ITransitionInstance transitionInstance, ScopeImpl sourceScope, ScopeImpl targetScope,
            ExecutionEntity asyncExecution)
        {
            var migratingTransitionInstance = new MigratingTransitionInstance(transitionInstance, migrationInstruction,
                sourceScope, targetScope, asyncExecution);

            migratingTransitionInstances.Add(migratingTransitionInstance);

            return migratingTransitionInstance;
        }

        public virtual MigratingEventScopeInstance AddEventScopeInstance(IMigrationInstruction migrationInstruction,
            ExecutionEntity eventScopeExecution, ScopeImpl sourceScope, ScopeImpl targetScope,
            IMigrationInstruction eventSubscriptionInstruction, EventSubscriptionEntity eventSubscription,
            ScopeImpl eventSubscriptionSourceScope, ScopeImpl eventSubscriptionTargetScope)
        {
            var compensationInstance = new MigratingEventScopeInstance(migrationInstruction, eventScopeExecution,
                sourceScope, targetScope, eventSubscriptionInstruction, eventSubscription, eventSubscriptionSourceScope,
                eventSubscriptionTargetScope);

            migratingEventScopeInstances.Add(compensationInstance);

            return compensationInstance;
        }

        public virtual MigratingCompensationEventSubscriptionInstance AddCompensationSubscriptionInstance(
            IMigrationInstruction eventSubscriptionInstruction, EventSubscriptionEntity eventSubscription,
            ScopeImpl sourceScope, ScopeImpl targetScope)
        {
            var compensationInstance = new MigratingCompensationEventSubscriptionInstance(eventSubscriptionInstruction,
                sourceScope, targetScope, eventSubscription);

            migratingCompensationSubscriptionInstances.Add(compensationInstance);

            return compensationInstance;
        }
    }
}