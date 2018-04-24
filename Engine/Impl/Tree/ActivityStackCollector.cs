using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Pvm;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;

namespace ESS.FW.Bpm.Engine.Impl.tree
{
    /// <summary>
    ///     
    /// </summary>
    public class ActivityStackCollector : ITreeVisitor<ScopeImpl>
    {
        protected internal IList<IPvmActivity> activityStack = new List<IPvmActivity>();

        public virtual IList<IPvmActivity> ActivityStack
        {
            get { return activityStack; }
        }

        public virtual void Visit(ScopeImpl scope)
        {
            if ((scope != null) && scope.GetType().IsSubclassOf(typeof(IPvmActivity)))
                activityStack.Add((IPvmActivity) scope);
        }
    }
}