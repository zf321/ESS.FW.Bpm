using System;
using ESS.FW.Bpm.Engine.Impl.Pvm.Runtime.Operation;

namespace ESS.FW.Bpm.Engine.Impl.JobExecutor
{
    /// <summary>
    ///     
    /// </summary>
    [Serializable]
    public class AsyncBeforeMessageJobDeclaration : MessageJobDeclaration
    {
        private const long SerialVersionUid = 1L;

        public static readonly string[] AsyncBeforeOperations =
        {
            PvmAtomicOperationFields.TransitionCreateScope.CanonicalName,
            PvmAtomicOperationFields.ProcessStart.CanonicalName,
            PvmAtomicOperationFields.ActivityStartCreateScope.CanonicalName
        };

        public AsyncBeforeMessageJobDeclaration() : base(AsyncBeforeOperations)
        {
            JobConfiguration = AsyncBefore;
        }
    }
}