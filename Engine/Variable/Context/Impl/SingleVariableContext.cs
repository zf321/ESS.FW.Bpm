using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Variable.Value;

namespace ESS.FW.Bpm.Engine.Variable.Context.Impl
{
    /// <summary>
    ///     An <seealso cref="IVariableContext" /> allowing to resolve a single variable only.
    /// </summary>
    public class SingleVariableContext : IVariableContext
    {
        protected internal readonly ITypedValue TypedValue;
        protected internal string Name;

        public SingleVariableContext(string name, ITypedValue typedValue)
        {
            Name = name;
            TypedValue = typedValue;
        }

        public virtual ITypedValue Resolve(string variableName)
        {
            if (ContainsVariable(variableName))
                return TypedValue;
            return null;
        }

        public virtual bool ContainsVariable(string name)
        {
            if (ReferenceEquals(Name, null))
                return ReferenceEquals(name, null);
            return Name.Equals(name);
        }

        public virtual ICollection<string> KeySet()
        {
            return new HashSet<string> {Name};
        }

        public static SingleVariableContext SingleVariable(string name, ITypedValue value)
        {
            return new SingleVariableContext(name, value);
        }
    }
}