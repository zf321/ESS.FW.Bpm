using System;
using ESS.FW.Bpm.Engine.Variable.Type;
using ESS.FW.Bpm.Engine.Variable.Value;

namespace ESS.FW.Bpm.Engine.Form.Impl.Type
{
    /// <summary>
    ///      
    /// </summary>
    public class LongFormType : SimpleFormFieldType
    {
        public const string TypeName = "long";

        public override string Name
        {
            get { return TypeName; }
        }

        //public override ITypedValue convertValue(ITypedValue propertyValue)
        //{
        //    if (propertyValue is LongValue)
        //    {
        //        return propertyValue;
        //    }
        //    object value = propertyValue.Value;
        //    if (value == null)
        //    {
        //        return Variables.longValue(null);
        //    }
        //    if ((value is decimal) || (value is string))
        //    {
        //        return Variables.longValue(Convert.ToInt64(value.ToString()));
        //    }
        //    throw new ProcessEngineException("Value '" + value + "' is not of type Long.");
        //}

        // deprecated ////////////////////////////////////////////

        public override object ConvertFormValueToModelValue(object propertyValue)
        {
            if ((propertyValue == null) || "".Equals(propertyValue))
                return null;
            return Convert.ToInt64(propertyValue.ToString());
        }

        public override string ConvertModelValueToFormValue(object modelValue)
        {
            if (modelValue == null)
                return null;
            return modelValue.ToString();
        }

        protected internal override ITypedValue ConvertValue(ITypedValue propertyValue)
        {
            throw new NotImplementedException();
        }
    }
}