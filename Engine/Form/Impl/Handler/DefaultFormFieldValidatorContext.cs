using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Form.Impl.Validator;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Variable;


namespace ESS.FW.Bpm.Engine.Form.Impl.Handler
{
    /// <summary>
    ///     
    /// </summary>
    public class DefaultFormFieldValidatorContext : IFormFieldValidatorContext
    {
        protected internal string configuration;
        protected internal FormFieldHandler formFieldHandler;
        protected internal IVariableMap submittedValues;

        protected internal IVariableScope variableScope;

        public DefaultFormFieldValidatorContext(IVariableScope variableScope, string configuration,
            IVariableMap submittedValues, FormFieldHandler formFieldHandler)
        {
            this.variableScope = variableScope;
            this.configuration = configuration;
            this.submittedValues = submittedValues;
            this.formFieldHandler = formFieldHandler;
        }

        public virtual FormFieldHandler FormFieldHandler
        {
            get { return formFieldHandler; }
        }

        public virtual IDelegateExecution Execution
        {
            get
            {
                if (variableScope is IDelegateExecution)
                    return (IDelegateExecution) variableScope;
                if (variableScope is TaskEntity)
                    return null;
                return null;
            }
        }

        public virtual IVariableScope VariableScope
        {
            get { return variableScope; }
        }

        public virtual string Configuration
        {
            get { return configuration; }
            set { configuration = value; }
        }


        public virtual IDictionary<string, object> SubmittedValues
        {
            get { return submittedValues; }
        }
    }
}