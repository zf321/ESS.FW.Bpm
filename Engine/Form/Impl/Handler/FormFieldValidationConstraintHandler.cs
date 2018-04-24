
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Form.Impl.Validator;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Variable;

namespace ESS.FW.Bpm.Engine.Form.Impl.Handler
{
    /// <summary>
    ///     <para>Wrapper for a validation constraint</para>
    ///     
    /// </summary>
    public class FormFieldValidationConstraintHandler
    {
        protected internal string config;

        protected internal string name;
        protected internal IFormFieldValidator validator;

        // getter / setter ////////////////////////

        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }


        public virtual string Config
        {
            set { config = value; }
            get { return config; }
        }


        public virtual IFormFieldValidator Validator
        {
            set { validator = value; }
            get { return validator; }
        }

        public virtual IFormFieldValidationConstraint CreateValidationConstraint(ExecutionEntity execution)
        {
            return new FormFieldValidationConstraintImpl(name, config);
        }

        // submit /////////////////////////////////

        public virtual void Validate(object submittedValue, IVariableMap submittedValues,
            FormFieldHandler formFieldHandler, IVariableScope variableScope)
        {
            try
            {
                IFormFieldValidatorContext context = new DefaultFormFieldValidatorContext(variableScope, config,
                    submittedValues, formFieldHandler);
                if (!validator.Validate(submittedValue, context))
                    throw new FormFieldValidatorException(formFieldHandler.Id, name, config, submittedValue,
                        "Invalid value submitted for form field '" + formFieldHandler.Id + "': validation of " + this +
                        " failed.");
            }
            catch (FormFieldValidationException e)
            {
                throw new FormFieldValidatorException(formFieldHandler.Id, name, config, submittedValue,
                    "Invalid value submitted for form field '" + formFieldHandler.Id + "': validation of " + this +
                    " failed.", e);
            }
        }


        public override string ToString()
        {
            return name + (!ReferenceEquals(config, null) ? "(" + config + ")" : "");
        }
    }
}