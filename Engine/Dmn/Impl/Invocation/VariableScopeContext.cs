using System;
using System.Collections.Generic;

using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Variable.Context;
using ESS.FW.Bpm.Engine.Variable.Type;
using ESS.FW.Bpm.Engine.Variable.Value;

namespace ESS.FW.Bpm.Engine.Dmn.Impl.Invocation
{
    /// <summary>
    ///     Implementation of the <seealso cref="IVariableContext" /> interface backed
    ///     by a <seealso cref="IVariableScope" />.
    ///     
    /// </summary>
    public class VariableScopeContext : IVariableContext
    {
        protected internal readonly IVariableScope VariableScope;

        public VariableScopeContext(IVariableScope variableScope)
        {
            this.VariableScope = variableScope;
        }

        public virtual bool ContainsVariable(string variableName)
        {
            return VariableScope.HasVariable(variableName);
        }

        public virtual ICollection<string> KeySet()
        {
            return VariableScope.VariableNames;
        }

        public virtual ITypedValue Resolve(string variableName)
        {
            return VariableScope.GetVariableTyped<ITypedValue>(variableName);
        }

        public static VariableScopeContext Wrap(IVariableScope variableScope)
        {
            return new VariableScopeContext(variableScope);
        }
    }
}