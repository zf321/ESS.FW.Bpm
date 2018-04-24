
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Core.Variable.mapping.value;
using ESS.FW.Bpm.Engine.Impl.Core.Variable.Scope;
using ESS.FW.Bpm.Engine.Variable;
using ESS.FW.Bpm.Engine.Variable.Value;

namespace ESS.FW.Bpm.Engine.Impl.Core.Model
{
    /// <summary>
    ///     
    /// </summary>
    public class CallableElementParameter
    {
        protected internal bool allVariables;
        protected internal bool readLocal;

        protected internal IParameterValueProvider sourceValueProvider;
        protected internal string target;

        public virtual IParameterValueProvider SourceValueProvider
        {
            get { return sourceValueProvider; }
            set { sourceValueProvider = value; }
        }


        // target //////////////////////////////////////////////////////////

        public virtual string Target
        {
            get { return target; }
            set { target = value; }
        }


        // all variables //////////////////////////////////////////////////

        public virtual bool AllVariables
        {
            get { return allVariables; }
            set { allVariables = value; }
        }


        // local

        public virtual bool ReadLocal
        {
            set { readLocal = value; }
            get { return readLocal; }
        }

        // source ////////////////////////////////////////////////////////

        public virtual object GetSource(IVariableScope variableScope)
        {
            if (sourceValueProvider is ConstantValueProvider)
            {
                var variableName = (string) sourceValueProvider.GetValue(variableScope);

                return variableScope.GetVariableTyped<ITypedValue>(variableName);
            }
            return sourceValueProvider.GetValue(variableScope);
        }

        public virtual void ApplyTo(IVariableScope variableScope, IVariableMap variables)
        {
            if (readLocal)
                variableScope = new VariableScopeLocalAdapter(variableScope);

            if (allVariables)
            {
                var _allVariables = variableScope.Variables;
                foreach (var it in _allVariables)
                    variables.PutValue(it.Key, it.Value);//.Add(it.Key, it.Value);
            }
            else
            {
                var value = GetSource(variableScope);
                variables.PutValue(target, value);
            }
        }
    }
}