using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Helper;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Parser;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Impl.tree;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Variable;
using SubProcessActivityBehavior = ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior.SubProcessActivityBehavior;

namespace ESS.FW.Bpm.Engine.Impl.Pvm.Runtime
{
    /// <summary>
    ///     This class encapsulates legacy runtime behavior for the process engine.
    ///     <para>
    ///         Since 7.3 the behavior of certain bpmn elements has changed slightly.
    ///     </para>
    ///     <para>
    ///         1. Some elements which did not used to be scopes are now scopes:
    ///         <ul>
    ///             <li>Sequential multi instance Embedded Subprocess: is now a scope, used to be non-scope.</li>
    ///             <li>Event subprocess: is now a scope, used to be non-scope.</li>
    ///         </ul>
    ///         2. In certain situations, executions which were both scope and concurrent were created.
    ///         This used to be the case if a scope execution already had a single scope child execution
    ///         and then concurrency was introduced (by a on interrupting boundary event or
    ///         a non-interrupting event subprocess).  In that case the existing scope execution
    ///         was made concurrent. Starting from 7.3 this behavior is considered legacy.
    ///         The new behavior is that the existing scope execution will not be made concurrent, instead,
    ///         a new, concurrent execution will be created and be interleaved between the parent and the
    ///         existing scope execution.
    ///     </para>
    ///     <para>
    ///         
    ///         
    ///     </para>
    /// </summary>
    public class LegacyBehavior
    {
        private static readonly BpmnBehaviorLogger Log = ProcessEngineLogger.BpmnBehaviorLogger;

        // concurrent scopes ///////////////////////////////////////////

        /// <summary>
        ///     Prunes a concurrent scope. This can only happen if
        ///     (a) the process instance has been migrated from a previous version to a new version of the process engine
        ///     This is an inverse operation to <seealso cref="#createConcurrentScope(PvmExecutionImpl)" />.
        ///     See: javadoc of this class for note about concurrent scopes.
        /// </summary>
        /// <param name="execution"> </param>
        public static void PruneConcurrentScope(IActivityExecution execution)
        {
            EnsureConcurrentScope(execution);
            Log.DebugConcurrentScopeIsPruned(execution);
            execution.IsConcurrent = false;
        }

        /// <summary>
        ///     Cancels an execution which is both concurrent and scope. This can only happen if
        ///     (a) the process instance has been migrated from a previous version to a new version of the process engine
        ///     See: javadoc of this class for note about concurrent scopes.
        /// </summary>
        /// <param name="execution"> the concurrent scope execution to destroy </param>
        /// <param name="cancellingActivity">
        ///     the activity that cancels the execution; it must hold that
        ///     cancellingActivity's event scope is the scope the execution is responsible for
        /// </param>
        public static void CancelConcurrentScope(IActivityExecution execution, IPvmActivity cancelledScopeActivity)
        {
            EnsureConcurrentScope(execution);
            Log.DebugCancelConcurrentScopeExecution(execution);

            execution.Interrupt("Scope " + cancelledScopeActivity + " cancelled.");
            // <!> HACK set to event scope activity and leave activity instance
            execution.Activity=(ActivityImpl) (cancelledScopeActivity);
            execution.LeaveActivityInstance();
            execution.Interrupt("Scope " + cancelledScopeActivity + " cancelled.");
            execution.Destroy();
        }

        /// <summary>
        ///     Destroys a concurrent scope Execution. This can only happen if
        ///     (a) the process instance has been migrated from a previous version to a 7.3+ version of the process engine
        ///     See: javadoc of this class for note about concurrent scopes.
        /// </summary>
        /// <param name="execution"> the execution to destroy </param>
        public static void DestroyConcurrentScope(IActivityExecution execution)
        {
            EnsureConcurrentScope(execution);
            Log.DestroyConcurrentScopeExecution(execution);
            execution.Destroy();
        }

        // sequential multi instance /////////////////////////////////

        public static bool EventSubprocessComplete(IActivityExecution scopeExecution)
        {
            var performLegacyBehavior = IsLegacyBehaviorRequired(scopeExecution);

            if (performLegacyBehavior)
            {
                Log.CompleteNonScopeEventSubprocess();
                scopeExecution.End(false);
            }

            return performLegacyBehavior;
        }

        public static bool EventSubprocessConcurrentChildExecutionEnded(IActivityExecution scopeExecution,
            IActivityExecution endedExecution)
        {
            var performLegacyBehavior = IsLegacyBehaviorRequired(endedExecution);

            if (performLegacyBehavior)
            {
                Log.EndConcurrentExecutionInEventSubprocess();
                // notify the grandparent flow scope in a similar way PvmAtomicOperationAcitivtyEnd does
                var flowScope = endedExecution.Activity.FlowScope;
                if (flowScope != null)
                {
                    flowScope = flowScope.FlowScope;

                    if (flowScope != null)
                        if (flowScope == endedExecution.Activity.ProcessDefinition)
                        {
                            endedExecution.Remove();
                            scopeExecution.TryPruneLastConcurrentChild();
                            scopeExecution.ForceUpdate();
                        }
                        else
                        {
                            var flowScopeActivity = (IPvmActivity) flowScope;

                            var activityBehavior = flowScopeActivity.ActivityBehavior;
                            if (activityBehavior is ICompositeActivityBehavior)
                                ((ICompositeActivityBehavior) activityBehavior).ConcurrentChildExecutionEnded(
                                    scopeExecution, endedExecution);
                        }
                }
            }

            return performLegacyBehavior;
        }

        /// <summary>
        ///     Destroy an execution for an activity that was previously not a scope and now is
        ///     (e.g. event subprocess)
        /// </summary>
        public static bool DestroySecondNonScope(IActivityExecution execution)
        {
            EnsureScope(execution);
            var performLegacyBehavior = IsLegacyBehaviorRequired(execution);

            if (performLegacyBehavior)
            {
                // legacy behavior is to do nothing
            }

            return performLegacyBehavior;
        }

        /// <summary>
        ///     This method
        /// </summary>
        /// <param name="scopeExecution"> </param>
        /// <param name="isLegacyBehaviorTurnedOff">
        ///     @return
        /// </param>
        protected internal static bool IsLegacyBehaviorRequired(IActivityExecution scopeExecution)
        {
            // legacy behavior is turned off: the current activity was parsed as scope.
            // now we need to check whether a scope execution was correctly created for the
            // event subprocess.

            // first create the mapping:
            var activityExecutionMapping = scopeExecution.CreateActivityExecutionMapping();
            // if the scope execution for the current activity is the same as for the parent scope
            // -> we need to perform legacy behavior
            IPvmScope activity = scopeExecution.Activity;
            if (!activity.IsScope)
                activity = activity.FlowScope;
            return activityExecutionMapping[(ScopeImpl) activity] == activityExecutionMapping[activity.FlowScope];
        }

        internal static void RepairParentRelationships(ICollection<ActivityInstanceImpl> values, string processInstanceId)
        {
            foreach (ActivityInstanceImpl activityInstance in values)
            {
                // if the determined activity instance id and the parent activity instance are equal,
                // just put the activity instance under the process instance
                //if (activityInstance.Id== activityInstance.ParentActivityInstanceId)
                if((string.IsNullOrEmpty(activityInstance.Id) && string.IsNullOrEmpty(activityInstance.ParentActivityInstanceId))||(!string.IsNullOrEmpty(activityInstance.Id) && activityInstance.Id.Equals(activityInstance.ParentActivityInstanceId)))
                {
                    activityInstance.ParentActivityInstanceId=processInstanceId;
                }
            }
        }

        /// <summary>
        ///     In case the process instance was migrated from a previous version, activities which are now parsed as scopes
        ///     do not have scope executions. Use the flow scopes of these activities in order to find their execution.
        ///     - For an event subprocess this is the scope execution of the scope in which the event subprocess is embeded in
        ///     - For a multi instance sequential subprocess this is the multi instace scope body.
        /// </summary>
        /// <param name="targetScope"> </param>
        /// <param name="activityExecutionMapping">
        ///     @return
        /// </param>
        public static PvmExecutionImpl GetScopeExecution(ScopeImpl scope,
            IDictionary<ScopeImpl, PvmExecutionImpl> activityExecutionMapping)
        {
            var flowScope = scope.FlowScope;
            return activityExecutionMapping[flowScope];
        }

        // helpers ////////////////////////////////////////////////

        protected internal static void EnsureConcurrentScope(IActivityExecution execution)
        {
            EnsureScope(execution);
            EnsureConcurrent(execution);
        }

        protected internal static void EnsureConcurrent(IActivityExecution execution)
        {
            if (!execution.IsConcurrent)
                throw new ProcessEngineException("Execution must be concurrent.");
        }

        protected internal static void EnsureScope(IActivityExecution execution)
        {
            if (!execution.IsScope)
                throw new ProcessEngineException("Execution must be scope.");
        }

        /// <summary>
        ///     Creates an activity execution mapping, when the scope hierarchy and the execution hierarchy are out of sync.
        /// </summary>
        /// <param name="scopeExecutions"> </param>
        /// <param name="scopes">
        ///     @return
        /// </param>
        public static IDictionary<ScopeImpl, PvmExecutionImpl> CreateActivityExecutionMapping(
            IList<PvmExecutionImpl> scopeExecutions, IList<ScopeImpl> scopes)
        {
            var deepestExecution = scopeExecutions[0];
            if (IsLegacyAsyncAtMultiInstance(deepestExecution))
                scopes.RemoveAt(0);

            // The trees are out of sync.
            // We are missing executions:
            var numOfMissingExecutions = scopes.Count - scopeExecutions.Count;

            // We need to find out which executions are missing.
            // Consider: elements which did not use to be scopes are now scopes.
            // But, this does not mean that all instances of elements which became scopes
            // are missing their executions. We could have created new instances in the
            // lower part of the tree after legacy behavior was turned off while instances of these elements
            // further up the hierarchy miss scopes. So we need to iterate from the top down and skip all scopes which
            // were not scopes before:
            scopeExecutions =scopeExecutions.Reverse().ToList();
            scopes=scopes.Reverse().ToList();

            IDictionary<ScopeImpl, PvmExecutionImpl> mapping = new Dictionary<ScopeImpl, PvmExecutionImpl>();
            // process definition / process instance.
            mapping[scopes[0]] = scopeExecutions[0];
            // nested activities
            var executionCounter = 0;
            for (var i = 1; i < scopes.Count; i++)
            {
                var scope = (ActivityImpl)scopes[i];

                PvmExecutionImpl scopeExecutionCandidate = null;
                if (executionCounter + 1 < scopeExecutions.Count)
                    scopeExecutionCandidate = scopeExecutions[executionCounter + 1];

                if ((numOfMissingExecutions > 0) && WasNoScope(scope, scopeExecutionCandidate))
                    numOfMissingExecutions--;
                else
                    executionCounter++;

                if (executionCounter >= scopeExecutions.Count)
                    throw new ProcessEngineException("Cannot construct activity-execution mapping: there are " +
                                                     "more scope executions missing than explained by the flow scope hierarchy.");

                var execution = scopeExecutions[executionCounter];
                mapping[scope] = execution;
            }

            return mapping;
        }

        /// <summary>
        ///     Determines whether the given scope was a scope in previous versions
        /// </summary>
        protected internal static bool WasNoScope(IPvmActivity activity, PvmExecutionImpl scopeExecutionCandidate)
        {
            return WasNoScope72(activity) || WasNoScope73(activity, scopeExecutionCandidate);
        }

        protected internal static bool WasNoScope72(IPvmActivity activity)
        {
            var activityBehavior = (IActivityBehavior) activity.ActivityBehavior;
            var parentActivityBehavior =
                (IActivityBehavior) (activity.FlowScope != null ? activity.FlowScope.ActivityBehavior : null);
            return activityBehavior is EventSubProcessActivityBehavior ||
                   (activityBehavior is SubProcessActivityBehavior &&
                    parentActivityBehavior is SequentialMultiInstanceActivityBehavior) ||
                   (activityBehavior is ReceiveTaskActivityBehavior &&
                    parentActivityBehavior is MultiInstanceActivityBehavior);
        }

        protected internal static bool WasNoScope73(IPvmActivity activity, PvmExecutionImpl scopeExecutionCandidate)
        {
            var activityBehavior = (IActivityBehavior) activity.ActivityBehavior;
            return activityBehavior is CompensationEventActivityBehavior ||
                   activityBehavior is CancelEndEventActivityBehavior ||
                   IsMultiInstanceInCompensation(activity, scopeExecutionCandidate);
        }

        protected internal static bool IsMultiInstanceInCompensation(IPvmActivity activity,
            PvmExecutionImpl scopeExecutionCandidate)
        {
            return activity.ActivityBehavior is MultiInstanceActivityBehavior &&
                   (((scopeExecutionCandidate != null) &&
                     (FindCompensationThrowingAncestorExecution(scopeExecutionCandidate) != null)) ||
                    (scopeExecutionCandidate == null));
        }

        /// <summary>
        ///     This returns true only if the provided execution has reached its wait state in a legacy engine version, because
        ///     only in that case, it can be async and waiting at the inner activity wrapped by the miBody. In versions >= 7.3,
        ///     the execution would reference the multi-instance body instead.
        /// </summary>
        protected internal static bool IsLegacyAsyncAtMultiInstance(PvmExecutionImpl execution)
        {
            ActivityImpl activity = execution.Activity as ActivityImpl;

            if (activity != null)
            {
                var isAsync = ReferenceEquals(execution.ActivityInstanceId, null);
                var isAtMultiInstance = (((ActivityImpl)activity).ParentFlowScopeActivity != null) &&
                                        ((ActivityImpl)activity).ParentFlowScopeActivity.ActivityBehavior is
                                            MultiInstanceActivityBehavior;


                return isAsync && isAtMultiInstance;
            }
            return false;
        }

        /// <summary>
        ///     Tolerates the broken execution trees fixed with CAM-3727 where there may be more
        ///     ancestor scope executions than ancestor flow scopes;
        ///     In that case, the argument execution is removed, the parent execution of the argument
        ///     is returned such that one level of mismatch is corrected.
        ///     Note that this does not necessarily skip the correct scope execution, since
        ///     the broken parent-child relationships may be anywhere in the tree (e.g. consider a non-interrupting
        ///     boundary event followed by a subprocess (i.e. scope), when the subprocess ends, we would
        ///     skip the subprocess's execution).
        /// </summary>
        public static IActivityExecution DeterminePropagatingExecutionOnEnd(IActivityExecution propagatingExecution,
            IDictionary<ScopeImpl, PvmExecutionImpl> activityExecutionMapping)
        {
            if (!propagatingExecution.IsScope)
                return propagatingExecution;
            // superfluous scope executions won't be contained in the activity-execution mapping
            if (activityExecutionMapping.Values.Contains((PvmExecutionImpl) propagatingExecution))
                return propagatingExecution;
            // skip one scope
            propagatingExecution.Remove();
            var parent = propagatingExecution.Parent;
            parent.Activity=(propagatingExecution.Activity);
            return propagatingExecution.Parent;
        }

        /// <summary>
        ///     Concurrent + scope executions are legacy and could occur in processes with non-interrupting
        ///     boundary events or event subprocesses
        /// </summary>
        public static bool IsConcurrentScope(PvmExecutionImpl propagatingExecution)
        {
            return propagatingExecution.IsConcurrent && propagatingExecution.IsScope;
        }

        /// <summary>
        ///     <para>
        ///         Required for migrating active sequential MI receive tasks. These activities were formerly not scope,
        ///         but are now. This has the following implications:
        ///     </para>
        ///     <para>
        ///         Before migration:
        ///         <ul>
        ///             <li> the event subscription is attached to the miBody scope execution
        ///         </ul>
        ///     </para>
        ///     <para>
        ///         After migration:
        ///         <ul>
        ///             <li>
        ///                 a new subscription is created for every instance
        ///                 <li>
        ///                     the new subscription is attached to a dedicated scope execution as a child of the miBody scope
        ///                     execution
        ///         </ul>
        ///     </para>
        ///     <para>
        ///         Thus, this method removes the subscription on the miBody scope
        ///     </para>
        /// </summary>
        public static void RemoveLegacySubscriptionOnParent(ExecutionEntity execution,
            EventSubscriptionEntity eventSubscription)
        {
            ActivityImpl activity = (ActivityImpl)execution.Activity;
            if (activity == null)
            {
                return;
            }

            IActivityBehavior behavior = activity.ActivityBehavior;
            IActivityBehavior parentBehavior = (activity.FlowScope != null ? activity.FlowScope.ActivityBehavior : null);

            if (behavior is ReceiveTaskActivityBehavior && parentBehavior is MultiInstanceActivityBehavior)
            {
                IList<EventSubscriptionEntity> parentSubscriptions = ((ExecutionEntity)execution.Parent).EventSubscriptions;

                foreach (EventSubscriptionEntity subscription in parentSubscriptions)
                {
                    // distinguish a boundary event on the mi body with the same message name from the receive task subscription
                    if (AreEqualEventSubscriptions(subscription, eventSubscription))
                    {
                        subscription.Delete();
                    }
                }
            }
        }

        /// <summary>
        ///     Checks if the parameters are the same apart from the execution id
        /// </summary>
        protected internal static bool AreEqualEventSubscriptions(EventSubscriptionEntity subscription1,
            EventSubscriptionEntity subscription2)
        {
            return ValuesEqual(subscription1.EventType, subscription2.EventType) &&
                   ValuesEqual(subscription1.EventName, subscription2.EventName) &&
                   ValuesEqual(subscription1.ActivityId, subscription2.ActivityId);
        }

        protected internal static bool ValuesEqual<T>(T value1, T value2)
        {
            return ((value1 == null) && (value2 == null)) || ((value1 != null) && value1.Equals(value2));
        }

        /// <summary>
        ///     Remove all entries for legacy non-scopes given that the assigned scope execution is also responsible for another
        ///     scope
        /// </summary>
        public static void RemoveLegacyNonScopesFromMapping(IDictionary<ScopeImpl, PvmExecutionImpl> mapping)
        {
            IDictionary<PvmExecutionImpl, IList<ScopeImpl>> scopesForExecutions = new Dictionary<PvmExecutionImpl, IList<ScopeImpl>>();

            foreach (var mappingEntry in mapping)
            {
                //var scopesForExecution = scopesForExecutions.ContainsKey(mappingEntry.Value)? scopesForExecutions[mappingEntry.Value]:null;
                var scopesForExecution = scopesForExecutions.GetValueOrNull(mappingEntry.Value);
                if (scopesForExecution == null)
                {
                    scopesForExecution = new List<ScopeImpl>();
                    scopesForExecutions[mappingEntry.Value] = scopesForExecution;
                }

                scopesForExecution.Add(mappingEntry.Key);
            }

            foreach (var scopesForExecution in scopesForExecutions)
            {
                var scopes = scopesForExecution.Value;

                if (scopes.Count > 1)
                {
                    var topMostScope = GetTopMostScope(scopes);

                    foreach (var scope in scopes)
                        if ((scope != scope.ProcessDefinition) && (scope != topMostScope))
                            mapping.Remove(scope);
                }
            }
        }

        protected internal static ScopeImpl GetTopMostScope(IList<ScopeImpl> scopes)
        {
            ScopeImpl topMostScope = null;

            foreach (var candidateScope in scopes)
                if (topMostScope == null || candidateScope.IsAncestorFlowScopeOf(topMostScope))
                    topMostScope = candidateScope;

            return topMostScope;
        }

        /// <summary>
        ///     This is relevant for <seealso cref="ActivityInstanceCmd" /> where in case of legacy multi-instance execution
        ///     trees, the default
        ///     algorithm omits multi-instance activity instances.
        /// </summary>
        /// <summary>
        ///     When deploying an async job definition for an activity wrapped in an miBody, set the activity id to the
        ///     miBody except the wrapped activity is marked as async.
        ///     Background: in <= 7.2 async job definitions were created for the inner activity, although the
        ///     semantics are that they are executed before the miBody is entered
        /// </summary>
        public static void MigrateMultiInstanceJobDefinitions(ProcessDefinitionEntity processDefinition,
            IList<JobDefinitionEntity> jobDefinitions)
        {
            foreach (var jobDefinition in jobDefinitions)
            {
                var activityId = jobDefinition.ActivityId;
                if (!ReferenceEquals(activityId, null))
                {
                    //ActivityImpl activity = processDefinition.findActivity(jobDefinition.ActivityId);

                    //if (!isAsync(activity) && isActivityWrappedInMultiInstanceBody(activity) && isAsyncJobDefinition(jobDefinition))
                    //{
                    //    jobDefinition.ActivityId = activity.FlowScope.Id;
                    //}
                }
            }
        }

        protected internal static bool IsAsync(ActivityImpl activity)
        {
            return activity.AsyncBefore || activity.AsyncAfter;
        }

        protected internal static bool IsAsyncJobDefinition(JobDefinitionEntity jobDefinition)
        {
            return AsyncContinuationJobHandler.TYPE.Equals(jobDefinition.JobType);
        }

        protected internal static bool IsActivityWrappedInMultiInstanceBody(ActivityImpl activity)
        {
            var flowScope = activity.FlowScope;

            if (flowScope != activity.ProcessDefinition)
            {
                var flowScopeActivity = (ActivityImpl) flowScope;

                return flowScopeActivity.ActivityBehavior is MultiInstanceActivityBehavior;
            }
            return false;
        }

        /// <summary>
        ///     When executing an async job for an activity wrapped in an miBody, set the execution to the
        ///     miBody except the wrapped activity is marked as async.
        ///     Background: in <= 7.2 async jobs were created for the inner activity, although the
        ///     semantics are that they are executed before the miBody is entered
        /// </summary>
        public static void RepairMultiInstanceAsyncJob(ExecutionEntity execution)
        {
            //ActivityImpl activity = execution.Activity();

            //if (!isAsync(activity) && isActivityWrappedInMultiInstanceBody(activity))
            //{
            //    execution.setActivity((ActivityImpl)activity.FlowScope);
            //}
        }

        /// <summary>
        ///     With prior versions, the boundary event was already executed when compensation was performed; Thus, after
        ///     compensation completes, the execution is signalled waiting at the boundary event.
        /// </summary>
        public static bool SignalCancelBoundaryEvent(string signalName)
        {
            return CompensationUtil.SignalCompensationDone.Equals(signalName);
        }

        /// <seealso cref= # signalCancelBoundaryEvent( String
        /// )
        /// </seealso>
        public static void ParseCancelBoundaryEvent(ActivityImpl activity)
        {
            activity.SetProperty(BpmnParse.PropertynameThrowsCompensation, true);
        }

        /// <summary>
        ///     <para>In general, only leaf executions have activity ids.</para>
        ///     <para>Exception to that rule: compensation throwing executions.</para>
        ///     <para>
        ///         Legacy exception (<= 7.2) to that rule: miBody executions and parallel gateway executions</para>
        /// </summary>
        /// <returns> true, if the argument is not a leaf and has an invalid (i.e. legacy) non-null activity id </returns>
        public static bool HasInvalidIntermediaryActivityId(PvmExecutionImpl execution)
        {
            return (execution.NonEventScopeExecutions.Count > 0) &&
                   !CompensationBehavior.IsCompensationThrowing(execution);
        }

        /// <summary>
        ///     Returns true if the given execution is in a compensation-throwing activity but there is no dedicated scope
        ///     execution
        ///     in the given mapping.
        /// </summary>
        public static bool IsCompensationThrowing(PvmExecutionImpl execution,
            IDictionary<ScopeImpl, PvmExecutionImpl> activityExecutionMapping)
        {
            if (CompensationBehavior.IsCompensationThrowing(execution))
            {
                ScopeImpl compensationThrowingActivity = (ScopeImpl)execution.Activity;

                if (compensationThrowingActivity.IsScope)
                    return activityExecutionMapping[compensationThrowingActivity] ==
                           activityExecutionMapping[compensationThrowingActivity.FlowScope];
                return compensationThrowingActivity.ActivityBehavior is CancelBoundaryEventActivityBehavior;
            }
            return false;
        }

        public static bool IsCompensationThrowing(PvmExecutionImpl execution)
        {
            return IsCompensationThrowing(execution, execution.CreateActivityExecutionMapping());
        }

        protected internal static PvmExecutionImpl FindCompensationThrowingAncestorExecution(PvmExecutionImpl execution)
        {
            var walker = new ExecutionWalker(execution);
            //walker.walkUntil(new WalkConditionAnonymousInnerClass());

            return walker.CurrentElement;
        }

        //private class WalkConditionAnonymousInnerClass : WalkCondition<PvmExecutionImpl>
        //{
        //    public WalkConditionAnonymousInnerClass()
        //    {
        //    }

        //    public virtual bool isFulfilled(PvmExecutionImpl element)
        //    {
        //        return element == null || CompensationBehavior.isCompensationThrowing(element);
        //    }

        //}
    }
}