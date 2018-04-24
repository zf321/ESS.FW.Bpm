using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;

namespace ESS.FW.Bpm.Engine.Impl.tree
{
    /// <summary>
    ///     
    /// </summary>
    public class ScopeCollector : ITreeVisitor<ScopeImpl>
    {
        protected internal IList<ScopeImpl> scopes = new List<ScopeImpl>();

        public virtual IList<ScopeImpl> Scopes
        {
            get { return scopes; }
        }

        public virtual void Visit(ScopeImpl obj)
        {
            if ((obj != null) && obj.IsScope)
                scopes.Add(obj);
        }
    }
}