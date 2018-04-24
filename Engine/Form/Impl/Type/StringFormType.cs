using System;
using ESS.FW.Bpm.Engine.Variable.Type;
using ESS.FW.Bpm.Engine.Variable.Value;

namespace ESS.FW.Bpm.Engine.Form.Impl.Type
{
    /// <summary>
    ///      
    /// </summary>
    public class StringFormType : SimpleFormFieldType
    {
        public const string TypeName = "string";

        public override string Name
        {
            get { return TypeName; }
        }

        //public override ITypedValue convertValue(ITypedValue propertyValue)
        //{
        //    if (propertyValue is StringValue)
        //    {
        //        return propertyValue;
        //    }
        //    object value = propertyValue.Value;
        //    if (value == null)
        //    {
        //        return Variables.stringValue(null);
        //    }
        //    return Variables.stringValue(value.ToString());
        //}

        // deprecated ////////////////////////////////////////////////////////////

        public override object ConvertFormValueToModelValue(object propertyValue)
        {
            return propertyValue.ToString();
        }

        public override string ConvertModelValueToFormValue(object modelValue)
        {
            return (string) modelValue;
        }

        protected internal override ITypedValue ConvertValue(ITypedValue propertyValue)
        {
            throw new NotImplementedException();
        }
    }
}