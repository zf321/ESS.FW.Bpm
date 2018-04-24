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
    public abstract class AbstractSetVariableCmd : AbstractVariableCmd
    {
        
        protected internal IDictionary<string, object> Variables;

        public AbstractSetVariableCmd(string entityId, IDictionary<string, object> variables, bool isLocal)
            : base(entityId, isLocal)
        {
            this.Variables = variables;
        }

        protected internal override string LogEntryOperation
        {
            get { return UserOperationLogEntryFields.OperationTypeSetVariable; }
        }

        protected internal override void ExecuteOperation(AbstractVariableScope scope)
        {
            if (IsLocal)
                scope.SetVariablesLocal(Variables);
            else
                scope.SetVariables(Variables);
        }
    }
}