using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Form.Impl.Type;
using ESS.FW.Bpm.Engine.Impl.EL;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Variable;
using ESS.FW.Bpm.Engine.Variable.Type;
using ESS.FW.Bpm.Engine.Variable.Value;

namespace ESS.FW.Bpm.Engine.Form.Impl.Handler
{
    /// <summary>
    ///     
    /// </summary>
    public class FormFieldHandler
    {
        protected internal bool businessKey;
        protected internal IExpression defaultValueExpression;

        protected internal string id;
        protected internal IExpression label;
        protected internal IDictionary<string, string> properties = new Dictionary<string, string>();
        protected internal AbstractFormFieldType Type;

        protected internal IList<FormFieldValidationConstraintHandler> validationHandlers =
            new List<FormFieldValidationConstraintHandler>();

        // getters / setters //////////////////////////////////

        public virtual string Id
        {
            get { return id; }
            set { id = value; }
        }


        public virtual IExpression Label
        {
            get { return label; }
            set { label = value; }
        }

        public virtual IDictionary<string, string> Properties
        {
            set { properties = value; }
            get { return properties; }
        }

        public virtual IExpression DefaultValueExpression
        {
            get { return defaultValueExpression; }
            set { defaultValueExpression = value; }
        }


        public virtual IList<FormFieldValidationConstraintHandler> ValidationHandlers
        {
            get { return validationHandlers; }
            set { validationHandlers = value; }
        }


        public virtual bool BusinessKey
        {
            set { businessKey = value; }
            get { return businessKey; }
        }

        public virtual IFormField CreateFormField(ExecutionEntity executionEntity)
        {
            var formField = new FormFieldImpl();

            // set id
            formField.Id = id;

            // set label (evaluate expression)
            //IVariableScope variableScope = executionEntity != null
            //    ? executionEntity
            //    : StartProcessVariableScope.SharedInstance;
            if (label != null)
            {
                //var labelValueObject = label.getValue(variableScope);
                //if (labelValueObject != null)
                //{
                //    formField.Label = labelValueObject.ToString();
                //}
            }

            formField.BusinessKey = businessKey;

            // set type
            formField.Type = Type;

            // set default value (evaluate expression)
            object defaultValue = null;
            if (defaultValueExpression != null)
            {
                //defaultValue = defaultValueExpression.getValue(variableScope);

                //if (defaultValue != null)
                //{
                //    formField.DefaultValue = type.convertFormValueToModelValue(defaultValue);
                //}
                //else
                //{
                //    formField.DefaultValue = null;
                //}
            }

            // value
            //ITypedValue value = variableScope.getVariableTyped(id);
            //if (value != null)
            //{
            //    formField.Value = type.convertToFormValue(value);
            //}

            // properties
            formField.Properties = properties;

            // validation
            var validationConstraints = formField.ValidationConstraints;
            foreach (var validationHandler in validationHandlers)
                if (!"validator".Equals(validationHandler.name))
                    validationConstraints.Add(validationHandler.CreateValidationConstraint(executionEntity));

            return formField;
        }

        // submit /////////////////////////////////////////////

        public virtual void HandleSubmit(Delegate.IVariableScope variableScope, IVariableMap values, IVariableMap allValues)
        {
            var submittedValue = values.GetValueTyped<ITypedValue>(id);
            //values.remove(id);

            // perform validation
            foreach (var validationHandler in validationHandlers)
            {
                object value = null;
                if (submittedValue != null)
                {
                    //value = submittedValue.Value;
                }
                validationHandler.Validate(value, allValues, this, variableScope);
            }

            // update variable(s)
            ITypedValue modelValue = null;
            if (submittedValue != null)
                if (Type != null)
                    modelValue = Type.ConvertToModelValue(submittedValue);
                else
                    modelValue = submittedValue;
            else if (defaultValueExpression != null)
                if (Type != null)
                {
                    // first, need to convert to model value since the default value may be a String Constant specified in the model xml.
                    //modelValue = type.convertToModelValue(Variables.untypedValue(expressionValue));
                }

            if (modelValue != null)
                if (!ReferenceEquals(id, null))
                    variableScope.SetVariable(id, modelValue);
        }


        public virtual void SetType(AbstractFormFieldType formType)
        {
            Type = formType;
        }


        public virtual IFormType getType()
        {
            return Type;
        }
    }
}