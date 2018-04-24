using System.Collections.Generic;
using System.Collections.ObjectModel;
using ESS.FW.Bpm.Engine.Variable.Value;

namespace ESS.FW.Bpm.Engine.Variable.Context.Impl
{
    /// <summary>
    /// </summary>
    public class CompositeVariableContext : IVariableContext
    {
        protected internal readonly IVariableContext[] DelegateContexts;

        public CompositeVariableContext(IVariableContext[] delegateContexts)
        {
            DelegateContexts = delegateContexts;
        }

        public virtual ITypedValue Resolve(string variableName)
        {
            foreach (var variableContext in DelegateContexts)
            {
                var resolvedValue = variableContext.Resolve(variableName);
                if (resolvedValue != null)
                    return resolvedValue;
            }

            return null;
        }

        public virtual bool ContainsVariable(string name)
        {
            foreach (var variableContext in DelegateContexts)
                if (variableContext.ContainsVariable(name))
                    return true;

            return false;
        }

        public virtual ICollection<string> KeySet()
        {
            ICollection<string> keySet = new Collection<string>();
            foreach (var variableContext in DelegateContexts)
            foreach (var s in variableContext.KeySet())
                keySet.Add(s);
            return keySet;
        }

        public static CompositeVariableContext Compose(params IVariableContext[] variableContexts)
        {
            return new CompositeVariableContext(variableContexts);
        }
    }
}