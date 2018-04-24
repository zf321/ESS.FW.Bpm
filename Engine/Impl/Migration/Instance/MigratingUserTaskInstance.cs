using System;
using ESS.FW.Bpm.Engine.History.Impl.Event;
using ESS.FW.Bpm.Engine.History.Impl.Producer;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.migration.instance
{
    /// <summary>
    ///     
    /// </summary>
    public class MigratingUserTaskInstance : IMigratingInstance
    {
        public static readonly MigrationLogger MigrationLogger = ProcessEngineLogger.MigrationLogger;
        protected internal MigratingActivityInstance MigratingActivityInstance;

        protected internal TaskEntity UserTask;

        public MigratingUserTaskInstance(TaskEntity userTask, MigratingActivityInstance migratingActivityInstance)
        {
            this.UserTask = userTask;
            this.MigratingActivityInstance = migratingActivityInstance;
        }

        public virtual void MigrateDependentEntities()
        {
        }

        public virtual bool Detached
        {
            get { return ReferenceEquals(UserTask.ExecutionId, null); }
        }

        public virtual void DetachState()
        {
            //userTask.getExecution().removeTask(userTask);
            //userTask.setExecution(null);
        }

        public virtual void AttachState(MigratingScopeInstance owningInstance)
        {
            var representativeExecution = owningInstance.ResolveRepresentativeExecution();
            //representativeExecution.addTask(userTask);

            //foreach (VariableInstanceEntity variable in userTask.VariablesInternal)
            //{
            //    variable.Execution = representativeExecution;
            //}

            //userTask.setExecution(representativeExecution);
        }

        public virtual void AttachState(MigratingTransitionInstance targetTransitionInstance)
        {
            throw MigrationLogger.CannotAttachToTransitionInstance(this);
        }

        public virtual void MigrateState()
        {
            UserTask.ProcessDefinitionId = MigratingActivityInstance.TargetScope.ProcessDefinition.Id;
            //userTask.TaskDefinitionKey = migratingActivityInstance.TargetScope.Id;

            MigrateHistory();
        }

        protected internal virtual void MigrateHistory()
        {
            var historyLevel = context.Impl.Context.ProcessEngineConfiguration.HistoryLevel;

            if (historyLevel.IsHistoryEventProduced(HistoryEventTypes.TaskInstanceMigrate, this))
                HistoryEventProcessor.ProcessHistoryEvents(new HistoryEventCreatorAnonymousInnerClass(this));
        }

        private class HistoryEventCreatorAnonymousInnerClass : HistoryEventProcessor.HistoryEventCreator
        {
            private readonly MigratingUserTaskInstance _outerInstance;

            public HistoryEventCreatorAnonymousInnerClass(MigratingUserTaskInstance outerInstance)
            {
                this._outerInstance = outerInstance;
            }

            public override HistoryEvent CreateHistoryEvent(IHistoryEventProducer producer)
            {
                throw new NotImplementedException();
                //return producer.createTaskInstanceMigrateEvt((DelegateTask)outerInstance.userTask);
                return null;
            }
        }
    }
}