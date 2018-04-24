using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Variable.Value;

namespace ESS.FW.Bpm.Engine.Variable.Context.Impl
{
    /// <summary>
    ///     An empty variable context implementation which does
    ///     not allow to resolve any variables.
    /// </summary>
    public class EmptyVariableContext : IVariableContext
    {
        public static readonly EmptyVariableContext Instance = new EmptyVariableContext();

        internal EmptyVariableContext()
        {
            // hidden
        }

        public virtual ITypedValue Resolve(string variableName)
        {
            return null;
        }

        public virtual bool ContainsVariable(string variableName)
        {
            return false;
        }

        public virtual ICollection<string> KeySet()
        {
            return new HashSet<string>();
        }
    }
}