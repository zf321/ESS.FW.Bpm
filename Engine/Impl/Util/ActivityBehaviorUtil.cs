using ESS.FW.Bpm.Engine.Impl.Pvm;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;
using ESS.FW.Bpm.Engine.Impl.Pvm.Runtime;
using ESS.FW.Bpm.Engine.Impl.Util;

namespace ESS.FW.Bpm.Engine.Impl.Util
{
    /// <summary>
    ///      
    /// </summary>
    public class ActivityBehaviorUtil
    {
        //public static ICmmnActivityBehavior GetActivityBehavior(CmmnExecution execution)
        //{
        //    string id = execution.Id;

        //    CmmnActivity activity = execution.Activity;
        //    EnsureUtil.EnsureNotNull(typeof(PvmException), "Case execution '" + id + "' has no current activity.", "activity", activity);

        //    ICmmnActivityBehavior behavior = (ICmmnActivityBehavior)activity.ActivityBehavior;
        //    EnsureUtil.EnsureNotNull(typeof(PvmException), "There is no behavior specified in " + activity + " for case execution '" + id + "'.", "behavior", behavior);

        //    return behavior;
        //}

        public static IActivityBehavior GetActivityBehavior(PvmExecutionImpl execution)
        {
            var id = execution.Id;

            IPvmActivity activity = execution.Activity;
            EnsureUtil.EnsureNotNull(typeof(PvmException), "Execution '" + id + "' has no current activity.",
                "activity", activity);

            var behavior = activity.ActivityBehavior;
            EnsureUtil.EnsureNotNull(typeof(PvmException),
                "There is no behavior specified in " + activity + " for execution '" + id + "'.", "behavior", behavior);

            return behavior;
        }
    }
}