using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.migration.instance;
using ESS.FW.Bpm.Engine.Impl.migration.instance.parser;
using ESS.FW.Bpm.Engine.Impl.Pvm;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Impl.Pvm.Runtime;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior
{
    /// <summary>
    ///     
    /// </summary>
    public class ParallelMultiInstanceActivityBehavior : MultiInstanceActivityBehavior, IMigrationObserverBehavior
    {
        public virtual void MigrateScope(IActivityExecution scopeExecution)
        {
            // migrate already completed instances
            foreach (var child in scopeExecution.Executions)
                if (!child.IsActive)
                    ((PvmExecutionImpl) child).ProcessDefinition = ((PvmExecutionImpl) scopeExecution).ProcessDefinition;
        }

        public virtual void OnParseMigratingInstance(MigratingInstanceParseContext parseContext,
            MigratingActivityInstance migratingInstance)
        {
            ExecutionEntity scopeExecution = migratingInstance.ResolveRepresentativeExecution();

            IList<IActivityExecution> concurrentInActiveExecutions = scopeExecution.FindInactiveChildExecutions(GetInnerActivity((ActivityImpl)migratingInstance.SourceScope));

            //variables on ended inner instance executions need not be migrated anywhere
            //since they are also not represented in the tree of migrating instances, we remove
            // them from the parse context here to avoid a validation exception
            foreach (IActivityExecution execution in concurrentInActiveExecutions)
            {
                foreach (VariableInstanceEntity variable in ((ExecutionEntity)execution).VariablesInternal)
                {
                    parseContext.Consume(variable);
                }
            }
        }
        
        protected internal override void CreateInstances(IActivityExecution execution, int nrOfInstances)
        {
            IPvmActivity innerActivity = GetInnerActivity(execution.Activity);

            // initialize the scope and create the desired number of child executions
            PrepareScopeExecution(execution, nrOfInstances);

            IList<IActivityExecution> concurrentExecutions = new List<IActivityExecution>();
            for (var i = 0; i < nrOfInstances; i++)
                concurrentExecutions.Add(CreateConcurrentExecution(execution));

            // start the concurrent child executions
            // start executions in reverse order (order will be reversed again in command context with the effect that they are
            // actually be started in correct order :) )
            for (var i = nrOfInstances - 1; i >= 0; i--)
            {
                var activityExecution = concurrentExecutions[i];
                PerformInstance(activityExecution, innerActivity, i);
            }
        }

        protected internal virtual void PrepareScopeExecution(IActivityExecution scopeExecution, int nrOfInstances)
        {
            // set the MI-body scoped variables
            SetLoopVariable(scopeExecution, NumberOfInstances, nrOfInstances);
            SetLoopVariable(scopeExecution, NumberOfCompletedInstances, 0);
            SetLoopVariable(scopeExecution, NumberOfActiveInstances, nrOfInstances);
            scopeExecution.Activity = null;
            scopeExecution.InActivate();
        }

        protected internal virtual IActivityExecution CreateConcurrentExecution(IActivityExecution scopeExecution)
        {
            var concurrentChild = scopeExecution.CreateExecution();
            scopeExecution.ForceUpdate();
            concurrentChild.IsConcurrent = true;
            concurrentChild.IsScope = false;
            return concurrentChild;
        }

        public override void ConcurrentChildExecutionEnded(IActivityExecution scopeExecution,
            IActivityExecution endedExecution)
        {
            int nrOfCompletedInstances = GetLoopVariable(scopeExecution, NumberOfCompletedInstances) + 1;
            SetLoopVariable(scopeExecution, NumberOfCompletedInstances, nrOfCompletedInstances);
            int nrOfActiveInstances = GetLoopVariable(scopeExecution, NumberOfActiveInstances) - 1;
            SetLoopVariable(scopeExecution, NumberOfActiveInstances, nrOfActiveInstances);

            // inactivate the concurrent execution
            endedExecution.InActivate();
            endedExecution.ActivityInstanceId = null;

            // join
            scopeExecution.ForceUpdate();
            // TODO: should the completion condition be evaluated on the scopeExecution or on the endedExecution?
            if (CompletionConditionSatisfied(endedExecution) || AllExecutionsEnded(scopeExecution, endedExecution))
            {
                var childExecutions =
                    new List<IActivityExecution>(((PvmExecutionImpl) scopeExecution).NonEventScopeExecutions);
                foreach (var childExecution in childExecutions)
                    if (childExecution.IsActive || (childExecution.Activity == null))
                        ((PvmExecutionImpl) childExecution).DeleteCascade(
                            "Multi instance completion condition satisfied.");
                    else
                        childExecution.Remove();

                scopeExecution.Activity = (IPvmActivity) endedExecution.Activity.FlowScope;
                scopeExecution.IsActive = true;
                Leave(scopeExecution);
            }
        }

        protected internal virtual bool AllExecutionsEnded(IActivityExecution scopeExecution,
            IActivityExecution endedExecution)
        {
            var numberOfInactiveConcurrentExecutions =
                endedExecution.FindInactiveConcurrentExecutions(endedExecution.Activity).Count;
            var concurrentExecutions = scopeExecution.Executions.Count;

            // no active instances exist and all concurrent executions are inactive
            return (GetLocalLoopVariable(scopeExecution, NumberOfActiveInstances) <= 0) &&
                   (numberOfInactiveConcurrentExecutions == concurrentExecutions);
        }

        public override void Complete(IActivityExecution scopeExecution)
        {
            // can't happen
        }

        public override IList<IActivityExecution> InitializeScope(IActivityExecution scopeExecution,
            int numberOfInstances)
        {
            PrepareScopeExecution(scopeExecution, numberOfInstances);

            IList<IActivityExecution> executions = new List<IActivityExecution>();
            for (var i = 0; i < numberOfInstances; i++)
            {
                var concurrentChild = CreateConcurrentExecution(scopeExecution);
                SetLoopVariable(concurrentChild, LoopCounter, i);
                executions.Add(concurrentChild);
            }

            return executions;
        }

        public override IActivityExecution CreateInnerInstance(IActivityExecution scopeExecution)
        {
            // even though there is only one instance, there is always a concurrent child
            var concurrentChild = CreateConcurrentExecution(scopeExecution);

            var nrOfInstances = GetLoopVariable(scopeExecution, NumberOfInstances);
            SetLoopVariable(scopeExecution, NumberOfInstances, nrOfInstances + 1);
            var nrOfActiveInstances = GetLoopVariable(scopeExecution, NumberOfActiveInstances);
            SetLoopVariable(scopeExecution, NumberOfActiveInstances, nrOfActiveInstances + 1);

            SetLoopVariable(concurrentChild, LoopCounter, nrOfInstances);

            return concurrentChild;
        }

        public override void DestroyInnerInstance(IActivityExecution concurrentExecution)
        {
            var scopeExecution = concurrentExecution.Parent;
            concurrentExecution.Remove();
            scopeExecution.ForceUpdate();

            var nrOfActiveInstances = GetLoopVariable(scopeExecution, NumberOfActiveInstances);
            SetLoopVariable(scopeExecution, NumberOfActiveInstances, nrOfActiveInstances - 1);
        }
    }
}