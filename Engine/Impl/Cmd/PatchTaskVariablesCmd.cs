using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///     Patches ITask variables: First, applies modifications to existing variables and then deletes
    ///     specified variables.
    ///     
    /// </summary>
    [Serializable]
    public class PatchTaskVariablesCmd : AbstractPatchVariablesCmd
    {
        private const long SerialVersionUid = 1L;

        public PatchTaskVariablesCmd(string taskId, IDictionary<string, object> modifications,
            ICollection<string> deletions, bool isLocal) : base(taskId, modifications, deletions, isLocal)
        {
        }

        protected internal override AbstractSetVariableCmd SetVariableCmd
        {
            get { return new SetTaskVariablesCmd(EntityId, Variables, IsLocal); }
        }

        protected internal override AbstractRemoveVariableCmd RemoveVariableCmd
        {
            get { return new RemoveTaskVariablesCmd(EntityId, Deletions, IsLocal); }
        }


        protected internal override void LogVariableOperation(CommandContext commandContext)
        {
            commandContext.OperationLogManager.LogVariableOperation(LogEntryOperation, null, EntityId, PropertyChange.EmptyChange);
        }
    }
}