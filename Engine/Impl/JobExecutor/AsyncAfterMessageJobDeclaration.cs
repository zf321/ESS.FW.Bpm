using System;
using ESS.FW.Bpm.Engine.Impl.Pvm.Runtime.Operation;

namespace ESS.FW.Bpm.Engine.Impl.JobExecutor
{
    /// <summary>
    ///     
    /// </summary>
    [Serializable]
    public class AsyncAfterMessageJobDeclaration : MessageJobDeclaration
    {
        public static readonly string[] AsyncAfterOperations =
        {
            PvmAtomicOperationFields.TransitionNotifyListenerTake.CanonicalName,
            PvmAtomicOperationFields.ActivityEnd.CanonicalName
        };

        public AsyncAfterMessageJobDeclaration() : base(AsyncAfterOperations)
        {
            JobConfiguration = AsyncAfter;
        }
    }
}