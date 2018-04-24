using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESS.FW.Bpm.Engine.Impl.Scripting.Engine
{
    /// <summary>
    /// Bindings implementation using an <seealso cref="ExecutionImpl"/> as 'back-end'.
    /// </summary>
    public class VariableScopeResolver : IResolver
    {

        protected internal IVariableScope variableScope;
        protected internal string variableScopeKey;

        public VariableScopeResolver(IVariableScope variableScope)
        {
            EnsureUtil.EnsureNotNull("variableScope", variableScope);
            variableScopeKey = variableScope.VariableScopeKey;
            this.variableScope = variableScope;
        }

        public virtual bool ContainsKey(object key)
        {
            return variableScopeKey.Equals(key) || variableScope.HasVariable((string)key);
        }

        public virtual object Get(object key)
        {
            if (variableScopeKey.Equals(key))
            {
                return variableScope;
            }
            return variableScope.GetVariable((string)key);
        }

        public virtual ICollection<string> KeySet()
        {
            // get variable names will return a new set instance
            ISet<string> variableNames = variableScope.VariableNames;
            variableNames.Add(variableScopeKey);
            return variableNames;
        }
    }

}
