using System;
using System.Collections.Generic;

using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Core.Variable.mapping.value;
using ESS.FW.Bpm.Engine.Variable;


namespace ESS.FW.Bpm.Engine.Impl.Core.Model
{
    /// <summary>
    ///     
    /// </summary>
    public class CallableElement : BaseCallableElement
    {
        protected internal IParameterValueProvider businessKeyValueProvider;
        protected internal IList<CallableElementParameter> inputs;
        protected internal IList<CallableElementParameter> outputs;
        protected internal IList<CallableElementParameter> outputsLocal;

        public CallableElement()
        {
            inputs = new List<CallableElementParameter>();
            outputs = new List<CallableElementParameter>();
            outputsLocal = new List<CallableElementParameter>();
        }

        public virtual IParameterValueProvider BusinessKeyValueProvider
        {
            get { return businessKeyValueProvider; }
            set { businessKeyValueProvider = value; }
        }


        // inputs //////////////////////////////////////////////////////////////////////

        public virtual IList<CallableElementParameter> Inputs
        {
            get { return inputs; }
        }

        // outputs /////////////////////////////////////////////////////////////////////

        public virtual IList<CallableElementParameter> Outputs
        {
            get { return outputs; }
        }

        public virtual IList<CallableElementParameter> OutputsLocal
        {
            get { return outputsLocal; }
        }

        // definitionKey ////////////////////////////////////////////////////////////////

        // binding /////////////////////////////////////////////////////////////////////

        // version //////////////////////////////////////////////////////////////////////

        // businessKey /////////////////////////////////////////////////////////////////

        public virtual string GetBusinessKey(IVariableScope variableScope)
        {
            if (businessKeyValueProvider == null)
                return null;

            var result = businessKeyValueProvider.GetValue(variableScope);

            if ((result != null) && !(result is string))
                throw new InvalidCastException("Cannot cast '" + result + "' to String");

            return (string) result;
        }

        public virtual void AddInput(CallableElementParameter input)
        {
            inputs.Add(input);
        }

        public virtual void AddInputs(IList<CallableElementParameter> inputs)
        {
            ((List<CallableElementParameter>) this.inputs).AddRange(inputs);
        }

        public virtual IVariableMap GetInputVariables(IVariableScope variableScope)
        {
            var inputs = Inputs;
            return GetVariables(inputs, variableScope);
        }

        public virtual void AddOutput(CallableElementParameter output)
        {
            outputs.Add(output);
        }

        public virtual void AddOutputLocal(CallableElementParameter output)
        {
            outputsLocal.Add(output);
        }

        public virtual void AddOutputs(IList<CallableElementParameter> outputs)
        {
            ((List<CallableElementParameter>) this.outputs).AddRange(outputs);
        }

        public virtual IVariableMap GetOutputVariables(IVariableScope calledElementScope)
        {
            var outputs = Outputs;
            return GetVariables(outputs, calledElementScope);
        }

        public virtual IVariableMap GetOutputVariablesLocal(IVariableScope calledElementScope)
        {
            var outputs = OutputsLocal;
            return GetVariables(outputs, calledElementScope);
        }

        // variables //////////////////////////////////////////////////////////////////

        protected internal virtual IVariableMap GetVariables(IList<CallableElementParameter> @params,
            IVariableScope variableScope)
        {
            var result = Variables.CreateVariables();

            foreach (var param in @params)
                param.ApplyTo(variableScope, result);

            return result;
        }

        // deployment id //////////////////////////////////////////////////////////////
    }
}