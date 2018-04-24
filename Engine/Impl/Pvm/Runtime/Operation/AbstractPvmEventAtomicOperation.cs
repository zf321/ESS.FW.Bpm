using ESS.FW.Bpm.Engine.Impl.Core.Model;
using ESS.FW.Bpm.Engine.Impl.Core.Operation;

namespace ESS.FW.Bpm.Engine.Impl.Pvm.Runtime.Operation
{

    /// <summary>
    ///     
    ///     
    ///     
    /// </summary>
    public abstract class AbstractPvmEventAtomicOperation : AbstractEventAtomicOperation<PvmExecutionImpl>,
        IPvmAtomicOperation
    {

        public virtual bool AsyncCapable
        {
            get { return false; }
        }

        protected internal abstract override CoreModelElement GetScope(PvmExecutionImpl execution);
    }
}