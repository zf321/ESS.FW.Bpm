
using System;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;
using ESS.FW.Bpm.Engine.Impl.Util;

namespace ESS.FW.Bpm.Engine.Impl.Pvm.Runtime.Operation
{
    /// <summary>
    ///     
    /// </summary>
    public class PvmAtomicOperationActivityLeave : IPvmAtomicOperation
    {
        private static readonly PvmLogger Log = ProcessEngineLogger.PvmLogger;

        public virtual bool IsAsync(PvmExecutionImpl execution)
        {
            return false;
        }

        public virtual void Execute(PvmExecutionImpl execution)
        {
            execution.ActivityInstanceDone();

            IActivityBehavior activityBehavior = ActivityBehaviorUtil.GetActivityBehavior(execution);

            if (activityBehavior is FlowNodeActivityBehavior)
            {
                FlowNodeActivityBehavior behavior = (FlowNodeActivityBehavior)activityBehavior;

                var activity = execution.Activity;
                var activityInstanceId = execution.ActivityInstanceId;
                if (!ReferenceEquals(activityInstanceId, null))
                {
                    Log.DebugLeavesActivityInstance(execution, activityInstanceId);
                }

                try
                {
                    behavior.DoLeave(execution);
                }
                //catch (RuntimeException e)
                //{
                //    throw e;
                //}
                catch (System.Exception e)
                {
                    throw new PvmException(
                        "couldn't leave activity <" + activity.GetProperty("type") + " id=\"" + activity.Id +
                        "\" ...>: " + e.Message, e);
                }
            }
            else
            {
                throw new PvmException("Behavior of current activity is not an instance of " +
                                       typeof(FlowNodeActivityBehavior).Name + ". Execution " + execution);
            }
        }

        public virtual string CanonicalName
        {
            get { return "activity-leave"; }
        }

        public virtual bool AsyncCapable
        {
            get { return false; }
        }
    }
}