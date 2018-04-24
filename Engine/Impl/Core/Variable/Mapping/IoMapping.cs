using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Core.Variable.Scope;

namespace ESS.FW.Bpm.Engine.Impl.Core.Variable.mapping
{
    /// <summary>
    ///     Maps variables in and out of a variable scope.
    ///     
    /// </summary>
    [System.Serializable]
    public class IoMapping
    {
        protected internal IList<InputParameter> inputParameters;

        protected internal IList<OutputParameter> outputParameters;

        public virtual IList<InputParameter> InputParameters
        {
            get
            {
                if (inputParameters == null)
                    return null;
                return inputParameters;
            }
            set { inputParameters = value; }
        }


        public virtual IList<OutputParameter> OutputParameters
        {
            get
            {
                if (outputParameters == null)
                    return null;
                return outputParameters;
            }
        }

        public virtual IList<OutputParameter> OuputParameters
        {
            set { outputParameters = value; }
        }

        public virtual void ExecuteInputParameters(AbstractVariableScope variableScope)
        {
            if (InputParameters != null)
            {
                foreach (var inputParameter in InputParameters)
                    inputParameter.Execute(variableScope);
            }
        }

        public virtual void ExecuteOutputParameters(AbstractVariableScope variableScope)
        {
            if (OutputParameters != null)
            {
                foreach (var outputParameter in OutputParameters)
                    outputParameter.Execute(variableScope);
            }
        }

        public virtual void AddInputParameter(InputParameter param)
        {
            if (inputParameters == null)
                inputParameters = new List<InputParameter>();
            inputParameters.Add(param);
        }

        public virtual void AddOutputParameter(OutputParameter param)
        {
            if (outputParameters == null)
                outputParameters = new List<OutputParameter>();
            outputParameters.Add(param);
        }
    }
}