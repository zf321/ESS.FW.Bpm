using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///     Patches execution variables: First, applies modifications to existing variables and then deletes
    ///     specified variables.
    /// </summary>
    [Serializable]
    public class PatchExecutionVariablesCmd : AbstractPatchVariablesCmd
    {
        private const long SerialVersionUid = 1L;

        public PatchExecutionVariablesCmd(string executionId, IDictionary<string, object> modifications,
            ICollection<string> deletions, bool isLocal) : base(executionId, modifications, deletions, isLocal)
        {
        }

        protected internal override AbstractSetVariableCmd SetVariableCmd
        {
            get
            {
                return new SetExecutionVariablesCmd(EntityId, Variables, IsLocal);
            }
        }

        protected internal override AbstractRemoveVariableCmd RemoveVariableCmd
        {
            get { return new RemoveExecutionVariablesCmd(EntityId, Deletions, IsLocal); }
        }

        protected internal override void LogVariableOperation(CommandContext commandContext)
        {
            commandContext.OperationLogManager.LogVariableOperation(LogEntryOperation, EntityId, null,
                PropertyChange.EmptyChange);
        }
    }
}