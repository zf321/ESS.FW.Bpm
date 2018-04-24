
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Form.Impl.Type;
using ESS.FW.Bpm.Engine.Impl.EL;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Variable;
using IExpression = ESS.FW.Bpm.Engine.Impl.EL.IExpression;

namespace ESS.FW.Bpm.Engine.Form.Impl.Handler
{
    /// <summary>
    ///      
    /// </summary>
    public class FormPropertyHandler
    {
        protected internal IExpression defaultExpression;

        protected internal string id;
        protected internal bool IsReadable;
        protected internal bool IsRequired;
        protected internal bool IsWritable;
        protected internal string name;
        protected internal AbstractFormFieldType Type;
        protected internal IExpression variableExpression;
        protected internal string variableName;

        // getters and setters //////////////////////////////////////////////////////

        public virtual string Id
        {
            get { return id; }
            set { id = value; }
        }


        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }

        public virtual bool Readable
        {
            get { return IsReadable; }
            set { IsReadable = value; }
        }


        public virtual bool Required
        {
            get { return IsRequired; }
            set { IsRequired = value; }
        }


        public virtual string VariableName
        {
            get { return variableName; }
            set { variableName = value; }
        }


        public virtual IExpression VariableExpression
        {
            get { return variableExpression; }
            set { variableExpression = value; }
        }


        public virtual IExpression DefaultExpression
        {
            get { return defaultExpression; }
            set { defaultExpression = value; }
        }


        public virtual bool Writable
        {
            get { return IsWritable; }
            set { IsWritable = value; }
        }

        public virtual IFormProperty CreateFormProperty(ExecutionEntity execution)
        {
            var formProperty = new FormPropertyImpl(this);
            object modelValue = null;

            if (execution != null)
            {
                if (!ReferenceEquals(variableName, null) || (variableExpression == null))
                {
                    //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                    //ORIGINAL LINE: final String varName = variableName != null ? variableName : id;
                    var varName = !ReferenceEquals(variableName, null) ? variableName : id;
                    //if (execution.hasVariable(varName))
                    //{
                    //    modelValue = execution.getVariable(varName);
                    //}
                    //else if (defaultExpression != null)
                    //{
                    //    modelValue = defaultExpression.getValue((IVariableScope)execution);
                    //}
                }
                else
                {
                    //modelValue = variableExpression.GetValue((IVariableScope) execution);
                }
            }
            else
            {
                // Execution is null, the form-property is used in a start-form. Default value
                // should be available (ACT-1028) even though no execution is available.
                //if (defaultExpression != null)
                    //modelValue = defaultExpression.GetValue(StartProcessVariableScope.SharedInstance);
            }

            if (modelValue is string)
            {
                formProperty.Value = (string) modelValue;
            }
            else if (Type != null)
            {
                var formValue = Type.ConvertModelValueToFormValue(modelValue);
                formProperty.Value = formValue;
            }
            else if (modelValue != null)
            {
                formProperty.Value = modelValue.ToString();
            }

            return formProperty;
        }

        public virtual void SubmitFormProperty(IVariableScope variableScope, IVariableMap variables)
        {
            if (!IsWritable && variables.ContainsKey(id))
                throw new ProcessEngineException("form property '" + id + "' is not writable");

            if (IsRequired && !variables.ContainsKey(id) && (defaultExpression == null))
                throw new ProcessEngineException("form property '" + id + "' is required");

            object modelValue = null;
            if (variables.ContainsKey(id))
            {
                //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                //ORIGINAL LINE: final Object propertyValue = variables.remove(id);
                object propertyValue = variables.Remove(id);
                if (Type != null)
                    modelValue = Type.ConvertFormValueToModelValue(propertyValue);
                else
                    modelValue = propertyValue;
            }
            else if (defaultExpression != null)
            {
                //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                //ORIGINAL LINE: final Object expressionValue = defaultExpression.getValue(variableScope);
                //var expressionValue = defaultExpression.GetValue(variableScope);
                //if ((Type != null) && (expressionValue != null))
                //    modelValue = Type.ConvertFormValueToModelValue(expressionValue.ToString());
                //else if (expressionValue != null)
                //    modelValue = expressionValue.ToString();
                //else if (IsRequired)
                //    throw new ProcessEngineException("form property '" + id + "' is required");
            }

            //if (modelValue != null)
            //    if (!ReferenceEquals(variableName, null))
            //        variableScope.SetVariable(variableName, modelValue);
            //    else if (variableExpression != null)
            //        variableExpression.SetValue(modelValue, variableScope);
            //    else
            //        variableScope.SetVariable(id, modelValue);
        }


        public virtual IFormType getType()
        {
            return Type;
        }

        public virtual void SetType(AbstractFormFieldType type)
        {
            this.Type = type;
        }
    }
}