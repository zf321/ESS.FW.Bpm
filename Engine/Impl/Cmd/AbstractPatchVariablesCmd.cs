using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Impl.Interceptor;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///     .
    /// </summary>
    [Serializable]
    public abstract class AbstractPatchVariablesCmd : ICommand<object>
    {
        private const long SerialVersionUid = 1L;

        protected internal string _entityId;
        protected internal bool _isLocal;
        protected internal ICollection<string> Deletions;
        protected internal IDictionary<string, object> Variables;

        public AbstractPatchVariablesCmd(string entityId, IDictionary<string, object> variables,
            ICollection<string> deletions, bool isLocal)
        {
            _entityId = entityId;
            this.Variables = variables;
            this.Deletions = deletions;
            _isLocal = isLocal;
        }

        public string EntityId
        {
            get { return _entityId; }
            set { _entityId = value; }
        }

        public bool IsLocal
        {
            get { return _isLocal; }
            set { _isLocal = value; }
        }

        protected internal virtual string LogEntryOperation
        {
            get { return UserOperationLogEntryFields.OperationTypeModifyVariable; }
        }

        protected internal abstract AbstractSetVariableCmd SetVariableCmd { get; }

        protected internal abstract AbstractRemoveVariableCmd RemoveVariableCmd { get; }

        public virtual object Execute(CommandContext commandContext)
        {
            SetVariableCmd.DisableLogUserOperation().Execute(commandContext);
            RemoveVariableCmd.DisableLogUserOperation().Execute(commandContext);
            LogVariableOperation(commandContext);
            return null;
        }

        protected internal abstract void LogVariableOperation(CommandContext commandContext);
    }
}