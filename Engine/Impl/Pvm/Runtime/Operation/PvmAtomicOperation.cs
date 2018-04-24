using ESS.FW.Bpm.Engine.Impl.Core.Operation;

namespace ESS.FW.Bpm.Engine.Impl.Pvm.Runtime.Operation
{
    /// <summary>
    ///     
    ///     
    ///     
    /// </summary>
    public interface IPvmAtomicOperation : ICoreAtomicOperation<PvmExecutionImpl>, IAtomicOperation
    {
    }

    public static class PvmAtomicOperationFields
    {
        public static readonly IPvmAtomicOperation ProcessStart = new PvmAtomicOperationProcessStart();
        public static readonly IPvmAtomicOperation FireProcessStart = new PvmAtomicOperationFireProcessStart();
        public static readonly IPvmAtomicOperation ProcessEnd = new PvmAtomicOperationProcessEnd();
        public static readonly IPvmAtomicOperation ActivityStart = new PvmAtomicOperationActivityStart();

        public static readonly IPvmAtomicOperation ActivityStartConcurrent =
            new PvmAtomicOperationActivityStartConcurrent();

        public static readonly IPvmAtomicOperation ActivityStartCancelScope =
            new PvmAtomicOperationActivityStartCancelScope();

        public static readonly IPvmAtomicOperation ActivityStartInterruptScope =
            new PvmAtomicOperationActivityStartInterruptEventScope();

        public static readonly IPvmAtomicOperation ActivityStartCreateScope =
            new PvmAtomicOperationActivityStartCreateScope();

        public static readonly IPvmAtomicOperation ActivityInitStackNotifyListenerStart =
            new PvmAtomicOperationActivityInitStackNotifyListenerStart();

        public static readonly IPvmAtomicOperation ActivityInitStackNotifyListenerReturn =
            new PvmAtomicOperationActivityInitStackNotifyListenerReturn();

        public static readonly IPvmAtomicOperation ActivityInitStack =
            new PvmAtomicOperationActivityInitStack(ActivityInitStackNotifyListenerStart);

        public static readonly IPvmAtomicOperation ActivityInitStackAndReturn =
            new PvmAtomicOperationActivityInitStack(ActivityInitStackNotifyListenerReturn);

        public static readonly IPvmAtomicOperation ActivityExecute = new PvmAtomicOperationActivityExecute();

        public static readonly IPvmAtomicOperation ActivityNotifyListenerEnd =
            new PvmAtomicOperationActivityNotifyListenerEnd();

        public static readonly IPvmAtomicOperation ActivityEnd = new PvmAtomicOperationActivityEnd();
        public static readonly IPvmAtomicOperation FireActivityEnd = new PvmAtomicOperationFireActivityEnd();

        public static readonly IPvmAtomicOperation TransitionNotifyListenerEnd =
            new PvmAtomicOperationTransitionNotifyListenerEnd();

        public static readonly IPvmAtomicOperation TransitionDestroyScope =
            new PvmAtomicOperationTransitionDestroyScope();

        public static readonly IPvmAtomicOperation TransitionNotifyListenerTake =
            new PvmAtomicOperationTransitionNotifyListenerTake();

        public static readonly IPvmAtomicOperation TransitionStartNotifyListenerTake =
            new PvmAtomicOperationStartTransitionNotifyListenerTake();

        public static readonly IPvmAtomicOperation TransitionCreateScope =
            new PvmAtomicOperationTransitionCreateScope();

        public static readonly IPvmAtomicOperation TransitionInterruptFlowScope =
            new PvmAtomicOperationsTransitionInterruptFlowScope();

        public static readonly IPvmAtomicOperation TransitionNotifyListenerStart =
            new PvmAtomicOperationTransitionNotifyListenerStart();

        public static readonly IPvmAtomicOperation DeleteCascade = new PvmAtomicOperationDeleteCascade();

        public static readonly IPvmAtomicOperation DeleteCascadeFireActivityEnd =
            new PvmAtomicOperationDeleteCascadeFireActivityEnd();

        public static readonly IPvmAtomicOperation ActivityLeave = new PvmAtomicOperationActivityLeave();
    }
}