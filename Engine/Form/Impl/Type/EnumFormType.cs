using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Variable;
using ESS.FW.Bpm.Engine.Variable.Value;

namespace ESS.FW.Bpm.Engine.Form.Impl.Type
{
    /// <summary>
	///  
	/// </summary>
	public class EnumFormType : SimpleFormFieldType
    {

        public const string TYPE_NAME = "enum";

        protected internal IDictionary<string, string> values;

        public EnumFormType(IDictionary<string, string> values)
        {
            this.values = values;
        }

        public override string Name
        {
            get
            {
                return TYPE_NAME;
            }
        }

        public override object GetInformation(string key)
        {
            if (key.Equals("values"))
            {
                return values;
            }
            return null;
        }

        protected internal override ITypedValue ConvertValue(ITypedValue propertyValue)
        {
            object value = propertyValue;
            if (value == null || typeof(string).IsInstanceOfType(value))
            {
                ValidateValue(value);
                return Variables.StringValue((string)value);
            }
            else
            {
                throw new ProcessEngineException("Value '" + value + "' is not of type String.");
            }
            return null;
        }

        protected internal virtual void ValidateValue(object value)
        {
            if (value != null)
            {
                if (values != null && !values.ContainsKey((string)value))
                {
                    throw new ProcessEngineException("Invalid value for enum form property: " + value);
                }
            }
        }

        public virtual IDictionary<string, string> Values
        {
            get
            {
                return values;
            }
        }

        //////////////////// deprecated ////////////////////////////////////////

        [Obsolete]
        public override object ConvertFormValueToModelValue(object propertyValue)
        {
            ValidateValue(propertyValue);
            return propertyValue;
        }

        [Obsolete]
        public override string ConvertModelValueToFormValue(object modelValue)
        {
            if (modelValue != null)
            {
                if (!(modelValue is string))
                {
                    throw new ProcessEngineException("Model value should be a String");
                }
                ValidateValue(modelValue);
            }
            return (string)modelValue;
        }

    }

}

