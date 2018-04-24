using ESS.FW.Bpm.Engine.Impl.Pvm;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;

namespace ESS.FW.Bpm.Engine.Impl.tree
{
    /// <summary>
    ///     Walks the flow scope hierarchy upwards
    /// </summary>
    public class FlowScopeWalker : SingleReferenceWalker<ScopeImpl>
    {
        public FlowScopeWalker(ScopeImpl startActivity) : base(startActivity)
        {
        }

        protected internal override ScopeImpl NextElement()
        {
            var currentElement = CurrentElement;
            if ((currentElement != null) && currentElement.GetType().IsAssignableFrom(typeof(ActivityImpl)))
                return ((IPvmActivity) currentElement).FlowScope;
            return null;
        }
    }
}