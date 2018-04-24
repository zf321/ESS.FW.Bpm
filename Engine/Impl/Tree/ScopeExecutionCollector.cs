using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Pvm.Runtime;

namespace ESS.FW.Bpm.Engine.Impl.tree
{
    /// <summary>
    ///     
    /// </summary>
    public class ScopeExecutionCollector : ITreeVisitor<PvmExecutionImpl>
    {
        protected internal IList<PvmExecutionImpl> scopeExecutions = new List<PvmExecutionImpl>();

        public virtual IList<PvmExecutionImpl> ScopeExecutions
        {
            get { return scopeExecutions; }
        }

        public virtual void Visit(PvmExecutionImpl obj)
        {
            if (obj.IsScope)
                scopeExecutions.Add(obj);
        }
    }
}