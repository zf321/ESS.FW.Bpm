using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Impl.Core.Variable.Scope;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///     .
    /// </summary>
    [Serializable]
    public abstract class AbstractRemoveVariableCmd : AbstractVariableCmd
    {

        protected internal readonly ICollection<string> VariableNames;

        public AbstractRemoveVariableCmd(string entityId, ICollection<string> variableNames, bool isLocal)
            : base(entityId, isLocal)
        {
            this.VariableNames = variableNames;
        }

        protected internal override string LogEntryOperation
        {
            get { return UserOperationLogEntryFields.OperationTypeRemoveVariable; }
        }

        protected internal override void ExecuteOperation(AbstractVariableScope scope)
        {
            if (IsLocal)
                scope.RemoveVariablesLocal(VariableNames);
            else
                scope.RemoveVariables(VariableNames);
        }
    }
}