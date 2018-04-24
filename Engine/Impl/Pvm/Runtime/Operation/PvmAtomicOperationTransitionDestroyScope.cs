using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;

namespace ESS.FW.Bpm.Engine.Impl.Pvm.Runtime.Operation
{
    /// <summary>
    /// 
    /// </summary>
    public class PvmAtomicOperationTransitionDestroyScope : IPvmAtomicOperation
    {
        private static readonly PvmLogger Log = ProcessEngineLogger.PvmLogger;

        public virtual bool IsAsync(PvmExecutionImpl instance)
        {
            return false;
        }

        public virtual bool AsyncCapable
        {
            get { return false; }
        }

        public virtual void Execute(PvmExecutionImpl execution)
        {
            // calculate the propagating execution
            var propagatingExecution = execution;

            IPvmActivity activity = execution.Activity;
            var transitionsToTake = execution.TransitionsToTake;
            execution.TransitionsToTake = null;

            // check whether the current scope needs to be destroyed
            if (execution.IsScope && activity.IsScope)
            {
                if (!LegacyBehavior.DestroySecondNonScope(execution))
                    if (execution.IsConcurrent)
                    {
                        // legacy behavior
                        LegacyBehavior.DestroyConcurrentScope(execution);
                    }
                    else
                    {
                        propagatingExecution = (PvmExecutionImpl) execution.Parent;
                        propagatingExecution.Activity=(execution.Activity);
                        propagatingExecution.Transition=(execution.Transition);
                        propagatingExecution.IsActive = true;
                        Log.DebugDestroyScope(execution, propagatingExecution);
                        execution.Destroy();
                        execution.Remove();
                    }
            }
            else
            {
                // activity is not scope => nothing to do
                propagatingExecution = execution;
            }

            // take the specified transitions
            if (transitionsToTake==null|| transitionsToTake.Count == 0)
                throw new ProcessEngineException(execution + ": No outgoing transitions from " + "activity " + activity);
            if (transitionsToTake.Count == 1)
            {
                propagatingExecution.Transition=(transitionsToTake[0]);
                propagatingExecution.Take();
            }
            else
            {
                propagatingExecution.InActivate();

                IList<OutgoingExecution> outgoingExecutions = new List<OutgoingExecution>();

                for (var i = 0; i < transitionsToTake.Count; i++)
                {
                    var transition = transitionsToTake[i];

                    var scopeExecution = propagatingExecution.IsScope ? propagatingExecution : propagatingExecution.Parent;

                    // reuse concurrent, propagating execution for first transition
                    IActivityExecution concurrentExecution = null;
                    if (i == 0)
                    {
                        concurrentExecution = propagatingExecution;
                    }
                    else
                    {
                        concurrentExecution = scopeExecution.CreateConcurrentExecution();

                        if ((i == 1) && !propagatingExecution.IsConcurrent)
                        {
                            outgoingExecutions.RemoveAt(0);
                            // get a hold of the concurrent execution that replaced the scope propagating execution
                            IActivityExecution replacingExecution = null;
                            foreach (var concurrentChild in scopeExecution.NonEventScopeExecutions)
                                if (!(concurrentChild == propagatingExecution))
                                {
                                    replacingExecution = concurrentChild;
                                    break;
                                }

                            outgoingExecutions.Add(new OutgoingExecution(replacingExecution, transitionsToTake[0]));
                        }
                    }

                    outgoingExecutions.Add(new OutgoingExecution(concurrentExecution, transition));
                }

                // start executions in reverse order (order will be reversed again in command context with the effect that they are
                // actually be started in correct order :) )
                outgoingExecutions = outgoingExecutions.Reverse().ToList();

                foreach (var outgoingExecution in outgoingExecutions)
                    outgoingExecution.Take();
            }
        }

        public virtual string CanonicalName
        {
            get { return "transition-destroy-scope"; }
        }
    }
}