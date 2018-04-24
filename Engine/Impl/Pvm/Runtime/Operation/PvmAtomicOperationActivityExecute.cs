using System;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Impl.Util;

namespace ESS.FW.Bpm.Engine.Impl.Pvm.Runtime.Operation
{
    /// <summary>
    ///     
    /// </summary>
    public class PvmAtomicOperationActivityExecute : IPvmAtomicOperation
    {
        private static readonly PvmLogger Log = ProcessEngineLogger.PvmLogger;

        public virtual bool IsAsync(PvmExecutionImpl execution)
        {
            return false;
        }

        public virtual void Execute(PvmExecutionImpl execution)
        {
            execution.ActivityInstanceStarted();
            execution.ContinueIfExecutionDoesNotAffectNextOperation((e)=>{
                if (execution.Activity.IsScope)
                    execution.DispatchEvent(null);
            }, (e) =>
            {
                var activityBehavior = ActivityBehaviorUtil.GetActivityBehavior(e);

                var activity = (ActivityImpl)e.Activity;
                Log.DebugExecutesActivity(e, activity, activityBehavior.GetType().FullName);
                //try
                //{
                Log.LogDebug("activityBehavior类型：", activityBehavior.GetType().Name);
                if (activity.IsTransaction)
                {
                    Log.LogDebug("进入SubProcess,并新建事务", activity.ActivityId);
                    context.Impl.Context.CommandContext.AddTransaction();
                }
                if (activity.IsInTransaction)
                {
                    Log.LogDebug("进入SubProcess子Activity：", activity.ActivityId);
                }
                activityBehavior.Execute(e);
                //}
                //catch (Exception e)
                //{
                //    throw new PvmException(
                //        "couldn't execute activity <" + activity.GetProperty("type") + " id=\"" + activity.Id +
                //        "\" ...>: " + e.Message, e);
                //}
            }, execution);
        }

        public virtual string CanonicalName
        {
            get { return "activity-execute"; }
        }

        public virtual bool AsyncCapable
        {
            get { return false; }
        }

    }
}