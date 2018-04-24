using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.EL;
using ESS.FW.Bpm.Engine.Impl.migration.instance;
using ESS.FW.Bpm.Engine.Impl.migration.instance.parser;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;
using ESS.FW.Bpm.Engine.Impl.task;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Impl.Core.Variable;

namespace ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior
{
    /// <summary>
    ///     activity implementation for the user Task.
    ///     
    ///     
    /// </summary>
    public class UserTaskActivityBehavior : TaskActivityBehavior, IMigrationObserverBehavior
    {
        private readonly TaskDecorator _taskDecorator;

        [Obsolete]
        public UserTaskActivityBehavior(ExpressionManager expressionManager, TaskDefinition taskDefinition)
        {
            _taskDecorator = new TaskDecorator(taskDefinition, expressionManager);
        }

        public UserTaskActivityBehavior(TaskDecorator taskDecorator)
        {
            this._taskDecorator = taskDecorator;
        }

        // getters

        public virtual TaskDefinition TaskDefinition
        {
            get { return _taskDecorator.TaskDefinition; }
        }

        public virtual ExpressionManager ExpressionManager
        {
            get { return _taskDecorator.ExpressionManager; }
        }

        public virtual TaskDecorator TaskDecorator
        {
            get { return _taskDecorator; }
        }

        // migration

        public virtual void MigrateScope(IActivityExecution scopeExecution)
        {
        }

        public virtual void OnParseMigratingInstance(MigratingInstanceParseContext parseContext,
            MigratingActivityInstance migratingInstance)
        {
            ExecutionEntity execution = migratingInstance.ResolveRepresentativeExecution();

            foreach (TaskEntity task in execution.Tasks)
            {
                migratingInstance.AddMigratingDependentInstance(new MigratingUserTaskInstance(task, migratingInstance));
                parseContext.Consume(task);

                ICollection<ICoreVariableInstance> variables = task.VariablesInternal;

                if (variables != null)
                {
                    foreach (VariableInstanceEntity variable in variables)
                    {
                        // we don't need to represent ITask variables in the migrating instance structure because
                        // they are migrated by the MigratingTaskInstance as well
                        parseContext.Consume(variable);
                    }
                }
            }
        }

        protected override void PerformExecution(IActivityExecution execution)
        {
            TaskEntity task = TaskEntity.CreateAndInsert(execution);

            _taskDecorator.Decorate(task, execution);

            Context.CommandContext.HistoricTaskInstanceManager.CreateHistoricTask(task);

            // All properties set, now firing 'create' event
            task.FireEvent(TaskListenerFields.EventnameCreate);
        }
        
        public override void Signal(IActivityExecution execution, string signalName, object signalData)
        {
            Leave(execution);
        }
    }
}