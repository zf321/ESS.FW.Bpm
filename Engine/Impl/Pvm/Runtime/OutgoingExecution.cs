using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;

namespace ESS.FW.Bpm.Engine.Impl.Pvm.Runtime
{
    /// <summary>
    ///      
    ///     
    /// </summary>
    public class OutgoingExecution
    {
        private static readonly PvmLogger Log = ProcessEngineLogger.PvmLogger;

        protected internal IActivityExecution outgoingExecution;
        protected internal IPvmTransition OutgoingTransition;

        public OutgoingExecution(IActivityExecution outgoingExecution, IPvmTransition outgoingTransition)
        {
            this.outgoingExecution = outgoingExecution;
            this.OutgoingTransition = outgoingTransition;
            outgoingExecution.Transition=(outgoingTransition);
        }

        public virtual void Take()
        {
            if (outgoingExecution.ReplacedBy != null)
                outgoingExecution = outgoingExecution.ReplacedBy;
            if (!outgoingExecution.IsEnded)
                outgoingExecution.Take();
            else
                Log.NotTakingTranistion(OutgoingTransition);
        }
    }
}