using System;
using ESS.FW.Bpm.Engine.Impl.Core.Operation;
using ESS.FW.Bpm.Engine.Impl.Pvm.Runtime.Operation;

namespace ESS.FW.Bpm.Engine.Impl.Pvm.Runtime
{

    /// <summary>
    /// </summary>
    [Obsolete]
    public interface IAtomicOperation : ICoreAtomicOperation<PvmExecutionImpl>
    {
        bool AsyncCapable { get; }
    }

    public static class AtomicOperationFields
    {
        public static readonly IAtomicOperation ProcessStart = PvmAtomicOperationFields.ProcessStart;
        //public static readonly AtomicOperation PROCESS_START_INITIAL = IPvmAtomicOperation.PROCESS_START_INITIAL;
        public static readonly IAtomicOperation ProcessEnd = PvmAtomicOperationFields.ProcessEnd;
        public static readonly IAtomicOperation ActivityStart = PvmAtomicOperationFields.ActivityStart;

        public static readonly IAtomicOperation ActivityStartConcurrent =
            PvmAtomicOperationFields.ActivityStartConcurrent;

        public static readonly IAtomicOperation ActivityStartCancelScope =
            PvmAtomicOperationFields.ActivityStartCancelScope;

        public static readonly IAtomicOperation ActivityExecute = PvmAtomicOperationFields.ActivityExecute;
        public static readonly IAtomicOperation ActivityEnd = PvmAtomicOperationFields.ActivityEnd;
        public static readonly IAtomicOperation FireActivityEnd = PvmAtomicOperationFields.FireActivityEnd;

        public static readonly IAtomicOperation TransitionNotifyListenerEnd =
            PvmAtomicOperationFields.TransitionNotifyListenerEnd;

        public static readonly IAtomicOperation TransitionDestroyScope =
            PvmAtomicOperationFields.TransitionDestroyScope;

        public static readonly IAtomicOperation TransitionNotifyListenerTake =
            PvmAtomicOperationFields.TransitionNotifyListenerTake;

        public static readonly IAtomicOperation TransitionCreateScope =
            PvmAtomicOperationFields.TransitionCreateScope;

        public static readonly IAtomicOperation TransitionNotifyListenerStart =
            PvmAtomicOperationFields.TransitionNotifyListenerStart;

        public static readonly IAtomicOperation DeleteCascade = PvmAtomicOperationFields.DeleteCascade;

        public static readonly IAtomicOperation DeleteCascadeFireActivityEnd =
            PvmAtomicOperationFields.DeleteCascadeFireActivityEnd;
    }
}