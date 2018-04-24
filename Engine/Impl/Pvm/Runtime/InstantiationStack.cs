using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Util;

namespace ESS.FW.Bpm.Engine.Impl.Pvm.Runtime
{
    /// <summary>
    ///     Expresses that the list of activities should be instantiated and optionally a transition
    ///     afterwards.
    ///     
    /// </summary>
    public class InstantiationStack
    {
        public InstantiationStack(IList<IPvmActivity> activities)
        {
            this.Activities = activities;
        }

        public InstantiationStack(IList<IPvmActivity> activities, IPvmActivity targetActivity,
            IPvmTransition targetTransition)
        {
            EnsureUtil.EnsureOnlyOneNotNull("target must be either a transition or an activity", targetActivity,
                targetTransition);
            this.Activities = activities;
            // TODO: make this a subclass that contains targetActivity and targetTransition?!
            this.TargetActivity = targetActivity;
            this.TargetTransition = targetTransition;
        }

        public virtual IList<IPvmActivity> Activities { get; }

        public virtual IPvmTransition TargetTransition { get; }

        public virtual IPvmActivity TargetActivity { get; }
    }
}