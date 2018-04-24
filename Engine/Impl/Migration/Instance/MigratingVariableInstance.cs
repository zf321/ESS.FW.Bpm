using System;
using ESS.FW.Bpm.Engine.History.Impl.Event;
using ESS.FW.Bpm.Engine.History.Impl.Producer;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.migration.instance
{
    /// <summary>
    ///     
    /// </summary>
    public class MigratingVariableInstance : IMigratingInstance
    {
        protected internal bool IsConcurrentLocalInParentScope;

        protected internal VariableInstanceEntity Variable;

        public MigratingVariableInstance(VariableInstanceEntity variable, bool isConcurrentLocalInParentScope)
        {
            this.Variable = variable;
            this.IsConcurrentLocalInParentScope = isConcurrentLocalInParentScope;
        }

        public virtual string VariableName
        {
            get { return Variable.Name; }
        }

        public virtual bool Detached
        {
            get { return ReferenceEquals(Variable.ExecutionId, null); }
        }

        public virtual void DetachState()
        {
            throw new NotImplementedException();
            //variable.Execution.removeVariableInternal(variable);
        }

        public virtual void AttachState(MigratingScopeInstance owningActivityInstance)
        {
            throw new NotImplementedException();
            var representativeExecution = owningActivityInstance.ResolveRepresentativeExecution();
            var currentScope = owningActivityInstance.CurrentScope;

            var newOwningExecution = representativeExecution;

            //if (currentScope.Scope && isConcurrentLocalInParentScope)
            //{
            //    newOwningExecution = representativeExecution.Parent;
            //}

            //newOwningExecution.addVariableInternal(variable);
        }

        public virtual void AttachState(MigratingTransitionInstance owningActivityInstance)
        {
            throw new NotImplementedException();
            var representativeExecution = owningActivityInstance.ResolveRepresentativeExecution();

            //representativeExecution.addVariableInternal(variable);
        }

        public virtual void MigrateState()
        {
            MigrateHistory();
        }

        public virtual void MigrateDependentEntities()
        {
            // nothing to do
        }

        protected internal virtual void MigrateHistory()
        {
            var historyLevel = context.Impl.Context.ProcessEngineConfiguration.HistoryLevel;

            if (historyLevel.IsHistoryEventProduced(HistoryEventTypes.VariableInstanceMigrate, this))
                HistoryEventProcessor.ProcessHistoryEvents(new HistoryEventCreatorAnonymousInnerClass(this));
        }

        private class HistoryEventCreatorAnonymousInnerClass : HistoryEventProcessor.HistoryEventCreator
        {
            private readonly MigratingVariableInstance _outerInstance;

            public HistoryEventCreatorAnonymousInnerClass(MigratingVariableInstance outerInstance)
            {
                this._outerInstance = outerInstance;
            }

            public override HistoryEvent CreateHistoryEvent(IHistoryEventProducer producer)
            {
                return producer.CreateHistoricVariableMigrateEvt(_outerInstance.Variable);
            }
        }
    }
}