using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Parser;
using ESS.FW.Bpm.Engine.Impl.Pvm;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;

namespace ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior
{
    /// <summary>
    ///     Implementation of the Inclusive Gateway/OR gateway/inclusive data-based
    ///     gateway as defined in the BPMN specification.
    /// </summary>
    public class InclusiveGatewayActivityBehavior : GatewayActivityBehavior
    {
        protected internal new static BpmnBehaviorLogger Log = ProcessEngineLogger.BpmnBehaviorLogger;
        
        public override void Execute(IActivityExecution execution)
        {
            execution.InActivate();
            LockConcurrentRoot(execution);

            var activity = execution.Activity;
            if (ActivatesGateway(execution, activity))
            {
                Log.ActivityActivation(activity.Id);

                var joinedExecutions = execution.FindInactiveConcurrentExecutions(activity);
                var defaultSequenceFlow = (string) execution.Activity.GetProperty("default");
                IList<IPvmTransition> transitionsToTake = new List<IPvmTransition>();

                // find matching non-default sequence flows
                foreach (var outgoingTransition in execution.Activity.OutgoingTransitions)
                {
                    if (ReferenceEquals(defaultSequenceFlow, null) || !outgoingTransition.Id.Equals(defaultSequenceFlow))
                    {
                        var condition = (ICondition) outgoingTransition.GetProperty(BpmnParse.PropertynameCondition);
                        if ((condition == null) || condition.Evaluate(execution))
                            transitionsToTake.Add(outgoingTransition);
                    }
                }

                // if none found, add default flow
                if (transitionsToTake.Count == 0)
                {
                    if (!ReferenceEquals(defaultSequenceFlow, null))
                    {
                        var defaultTransition = execution.Activity.FindOutgoingTransition(defaultSequenceFlow);
                        if (defaultTransition == null)
                            throw Log.MissingDefaultFlowException(execution.Activity.Id, defaultSequenceFlow);

                        transitionsToTake.Add(defaultTransition);
                    }
                    else
                    {
                        // No sequence flow could be found, not even a default one
                        throw Log.StuckExecutionException(execution.Activity.Id);
                    }
                }
                // take the flows found
                execution.LeaveActivityViaTransitions(transitionsToTake, joinedExecutions);
            }
            else
            {
                Log.NoActivityActivation(activity.Id);
            }
        }

        protected internal virtual ICollection<IActivityExecution> GetLeafExecutions(IActivityExecution parent)
        {
            IList<IActivityExecution> executionlist = new List<IActivityExecution>();
            var subExecutions = parent.NonEventScopeExecutions;
            if (subExecutions.Count == 0)
                executionlist.Add(parent);
            else
                foreach (var concurrentExecution in subExecutions)
                    ((List<IActivityExecution>) executionlist).AddRange(GetLeafExecutions(concurrentExecution));

            return executionlist;
        }

        protected internal virtual bool ActivatesGateway(IActivityExecution execution, IPvmActivity gatewayActivity)
        {
            var numExecutionsGuaranteedToActivate = gatewayActivity.IncomingTransitions.Count;
            var scopeExecution = execution.IsScope ? execution : execution.Parent;

            var executionsAtGateway = execution.FindInactiveConcurrentExecutions(gatewayActivity);

            if (executionsAtGateway.Count >= numExecutionsGuaranteedToActivate)
                return true;
            var executionsNotAtGateway = GetLeafExecutions(scopeExecution);
            foreach (var it in executionsAtGateway)
                executionsNotAtGateway.Remove(it);

            foreach (var executionNotAtGateway in executionsNotAtGateway)
                if (CanReachActivity(executionNotAtGateway, gatewayActivity))
                    return false;

            // if no more token may arrive, then activate
            return true;
        }

        protected internal virtual bool CanReachActivity(IActivityExecution execution, IPvmActivity activity)
        {
            IPvmTransition pvmTransition = execution.Transition;
            if (pvmTransition != null)
                return IsReachable(pvmTransition.Destination, activity, new HashSet<IPvmActivity>());
            return IsReachable(execution.Activity, activity, new HashSet<IPvmActivity>());
        }

        protected internal virtual bool IsReachable(IPvmActivity srcActivity, IPvmActivity targetActivity,
            ISet<IPvmActivity> visitedActivities)
        {
            if (srcActivity.Equals(targetActivity))
                return true;

            // To avoid infinite looping, we must capture every node we visit and
            // check before going further in the graph if we have already visited the node.
            visitedActivities.Add(srcActivity);

            var outgoingTransitions = srcActivity.OutgoingTransitions;
            if (outgoingTransitions.Count == 0)
            {
                var flowScope = srcActivity.FlowScope;
                if ((flowScope == null) || !(flowScope is IPvmActivity))
                    return false;

                return IsReachable((IPvmActivity) flowScope, targetActivity, visitedActivities);
            }
            foreach (var pvmTransition in outgoingTransitions)
            {
                var destinationActivity = pvmTransition.Destination;
                if ((destinationActivity != null) && !visitedActivities.Contains(destinationActivity))
                {
                    var reachable = IsReachable(destinationActivity, targetActivity, visitedActivities);

                    // If false, we should investigate other paths, and not yet return the result
                    if (reachable)
                        return true;
                }
            }

            return false;
        }
    }
}