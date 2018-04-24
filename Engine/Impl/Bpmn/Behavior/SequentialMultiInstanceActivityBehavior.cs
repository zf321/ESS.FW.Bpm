using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Pvm;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;

namespace ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior
{
    /// <summary>
    ///     
    /// </summary>
    public class SequentialMultiInstanceActivityBehavior : MultiInstanceActivityBehavior
    {
        protected internal new static readonly BpmnBehaviorLogger Log = ProcessEngineLogger.BpmnBehaviorLogger;
        
        protected internal override void CreateInstances(IActivityExecution execution, int nrOfInstances)
        {
            PrepareScope(execution, nrOfInstances);
            SetLoopVariable(execution, NumberOfActiveInstances, 1);

            var innerActivity = GetInnerActivity(execution.Activity);
            PerformInstance(execution, innerActivity, 0);
        }

        public override void Complete(IActivityExecution scopeExecution)
        {
            var loopCounter = GetLoopVariable(scopeExecution, LoopCounter) + 1;
            var nrOfInstances = GetLoopVariable(scopeExecution, NumberOfInstances);
            var nrOfCompletedInstances =
                GetLoopVariable(scopeExecution, NumberOfCompletedInstances) + 1;

            SetLoopVariable(scopeExecution, NumberOfCompletedInstances, nrOfCompletedInstances);

            if ((loopCounter == nrOfInstances) || CompletionConditionSatisfied(scopeExecution))
            {
                Leave(scopeExecution);
            }
            else
            {
                IPvmActivity innerActivity = GetInnerActivity(scopeExecution.Activity);
                PerformInstance(scopeExecution, innerActivity, loopCounter);
            }
        }

        public override void ConcurrentChildExecutionEnded(IActivityExecution scopeExecution,
            IActivityExecution endedExecution)
        {
            // cannot happen
        }

        protected internal virtual void PrepareScope(IActivityExecution scopeExecution, int totalNumberOfInstances)
        {
            SetLoopVariable(scopeExecution, NumberOfInstances, totalNumberOfInstances);
            SetLoopVariable(scopeExecution, NumberOfCompletedInstances, 0);
        }

        public override IList<IActivityExecution> InitializeScope(IActivityExecution scopeExecution, int nrOfInstances)
        {
            if (nrOfInstances > 1)
                Log.UnsupportedConcurrencyException(scopeExecution.ToString(), GetType().Name);

            IList<IActivityExecution> executions = new List<IActivityExecution>();

            PrepareScope(scopeExecution, nrOfInstances);
            SetLoopVariable(scopeExecution, NumberOfActiveInstances, nrOfInstances);

            if (nrOfInstances > 0)
            {
                SetLoopVariable(scopeExecution, LoopCounter, 0);
                executions.Add(scopeExecution);
            }

            return executions;
        }

        public override IActivityExecution CreateInnerInstance(IActivityExecution scopeExecution)
        {
            if (HasLoopVariable(scopeExecution, NumberOfActiveInstances) &&
                (GetLoopVariable(scopeExecution, NumberOfActiveInstances) > 0))
                throw Log.UnsupportedConcurrencyException(scopeExecution.ToString(), GetType().Name);
            var nrOfInstances = GetLoopVariable(scopeExecution, NumberOfInstances);

            SetLoopVariable(scopeExecution, LoopCounter, nrOfInstances);
            SetLoopVariable(scopeExecution, NumberOfInstances, nrOfInstances + 1);
            SetLoopVariable(scopeExecution, NumberOfActiveInstances, 1);

            return scopeExecution;
        }

        public override void DestroyInnerInstance(IActivityExecution scopeExecution)
        {
            RemoveLoopVariable(scopeExecution, LoopCounter);

            var nrOfActiveInstances = GetLoopVariable(scopeExecution, NumberOfActiveInstances);
            SetLoopVariable(scopeExecution, NumberOfActiveInstances, nrOfActiveInstances - 1);
        }
    }
}