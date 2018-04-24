using ESS.FW.Bpm.Engine.Impl.Pvm.Runtime;

namespace ESS.FW.Bpm.Engine.Impl.tree
{
    /// <summary>
    ///     
    /// </summary>
    public class ExecutionWalker : SingleReferenceWalker<PvmExecutionImpl>
    {
        public ExecutionWalker(PvmExecutionImpl initialElement) : base(initialElement)
        {
        }

        protected internal override PvmExecutionImpl NextElement()
        {
            return (PvmExecutionImpl) CurrentElement.Parent;
        }
    }
}