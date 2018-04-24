using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Core.Variable.mapping.value;
using ESS.FW.Bpm.Engine.Impl.migration.instance;
using ESS.FW.Bpm.Engine.Impl.migration.instance.parser;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior
{
    /// <summary>
    ///     Implements behavior of external ITask activities, i.e. all service-ITask-like
    ///     activities that have camunda:type="external".
    ///     
    /// </summary>
    public class ExternalTaskActivityBehavior : AbstractBpmnActivityBehavior, IMigrationObserverBehavior
    {
        protected internal IParameterValueProvider priorityValueProvider;

        protected internal string TopicName;

        public ExternalTaskActivityBehavior(string topicName, IParameterValueProvider paramValueProvider)
        {
            this.TopicName = topicName;
            priorityValueProvider = paramValueProvider;
        }

        public virtual IParameterValueProvider PriorityValueProvider
        {
            get { return priorityValueProvider; }
        }

        public virtual void MigrateScope(IActivityExecution scopeExecution)
        {
        }

        public virtual void OnParseMigratingInstance(MigratingInstanceParseContext parseContext,
            MigratingActivityInstance migratingInstance)
        {
            var execution = migratingInstance.ResolveRepresentativeExecution();

            foreach (ExternalTaskEntity task in execution.ExternalTasks)
            {
                var migratingTask = new MigratingExternalTaskInstance(task, migratingInstance);
                migratingInstance.AddMigratingDependentInstance(migratingTask);
                parseContext.Consume(task);
                parseContext.Submit(migratingTask);
            }
        }
       
        public override void Execute(IActivityExecution execution)
        {
            var executionEntity = (ExecutionEntity) execution;
            var provider = context.Impl.Context.ProcessEngineConfiguration.ExternalTaskPriorityProvider;
            var priority = provider.DeterminePriority(executionEntity, this, null);
            ExternalTaskEntity.CreateAndInsert(executionEntity, TopicName, priority);
        }
        
        public override void Signal(IActivityExecution execution, string signalName, object signalData)
        {
            Leave(execution);
        }

        /// <summary>
        ///     Overrides the propagateBpmnError method to made it public.
        ///     Is used to propagate the bpmn error from an external ITask.
        /// </summary>
        /// <param name="error"> the error which should be propagated </param>
        /// <param name="execution"> the current activity execution </param>
        /// <exception cref="exception"> throwsn an exception if no handler was found </exception>
        protected internal override void PropagateBpmnError(BpmnError error, IActivityExecution execution)
        {
            base.PropagateBpmnError(error, execution);
        }
    }
}