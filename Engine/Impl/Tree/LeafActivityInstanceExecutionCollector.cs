using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Pvm.Runtime;

namespace ESS.FW.Bpm.Engine.Impl.tree
{
    /// <summary>
    ///     Collects executions that execute an activity instance that is a leaf in the activity instance tree.
    ///     Typically, such executions are also leaves in the execution tree. The exception to this are compensation-throwing
    ///     executions: Their activities are leaves but they have child executions responsible for compensation handling.
    ///     
    /// </summary>
    public class LeafActivityInstanceExecutionCollector : ITreeVisitor<PvmExecutionImpl>
    {
        protected internal IList<PvmExecutionImpl> leaves = new List<PvmExecutionImpl>();

        public virtual IList<PvmExecutionImpl> Leaves
        {
            get { return leaves; }
        }

        public virtual void Visit(PvmExecutionImpl obj)
        {
            if ((obj.NonEventScopeExecutions.Count == 0) ||
                ((obj.Activity != null) && !LegacyBehavior.HasInvalidIntermediaryActivityId(obj)))
                leaves.Add(obj);
        }
    }
}