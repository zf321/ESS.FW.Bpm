using ESS.FW.Bpm.Engine.Impl.Bpmn.Parser;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;

namespace ESS.FW.Bpm.Engine.Impl.Pvm.Runtime
{
    /// <summary>
    ///     Contains the oddities required by compensation due to the execution structures it creates.
    ///     Anything that is a cross-cutting concern, but requires some extra compensation-specific conditions, should go here.
    ///     
    /// </summary>
    public class CompensationBehavior
    {
        /// <summary>
        ///     With compensation, we have a dedicated scope execution for every handler, even if the handler is not
        ///     a scope activity; this must be respected when invoking end listeners, etc.
        /// </summary>
        public static bool executesNonScopeCompensationHandler(IActivityExecution execution)
        {
            IPvmActivity activity = execution.Activity;

            return execution.IsScope && (activity != null) && activity.CompensationHandler && !activity.IsScope;
        }

        public static bool IsCompensationThrowing(IActivityExecution execution)
        {
            var currentActivity = execution.Activity;
            if (currentActivity != null)
            {
                var isCompensationThrowing =
                    (bool?) currentActivity.GetProperty(BpmnParse.PropertynameThrowsCompensation);
                if (isCompensationThrowing != null && isCompensationThrowing.Value)
                    return true;
            }

            return false;
        }

        /// <summary>
        ///     Determines whether an execution is responsible for default compensation handling.
        ///     This is the case if
        ///     <ul>
        ///         <li>
        ///             the execution has an activity
        ///             <li>
        ///                 the execution is a scope
        ///                 <li>
        ///                     the activity is a scope
        ///                     <li>
        ///                         the execution has children
        ///                         <li>the execution does not throw compensation
        ///     </ul>
        /// </summary>
        public static bool executesDefaultCompensationHandler(PvmExecutionImpl scopeExecution)
        {
            var currentActivity = scopeExecution.Activity;

            if (currentActivity != null)
                return scopeExecution.IsScope && currentActivity.IsScope &&
                       (scopeExecution.NonEventScopeExecutions.Count > 0) &&
                       !IsCompensationThrowing(scopeExecution);
            return false;
        }

        public static string GetParentActivityInstanceId(PvmExecutionImpl execution)
        {
            var activityExecutionMapping = execution.CreateActivityExecutionMapping();
            var parentScopeExecution = activityExecutionMapping[execution.Activity.FlowScope];

            return parentScopeExecution.ParentActivityInstanceId;
        }
    }
}