using System;
using ESS.FW.Bpm.Engine.Variable.Type;
using ESS.FW.Bpm.Engine.Variable.Value;

namespace ESS.FW.Bpm.Engine.Form.Impl.Type
{
    /// <summary>
    ///     The SimpleFormFieldType can be used when the form value and the model value are equal.
    ///     
    /// </summary>
    public abstract class SimpleFormFieldType : AbstractFormFieldType
    {
        public override ITypedValue ConvertToFormValue(ITypedValue propertyValue)
        {
            return ConvertValue(propertyValue);
        }

        public override ITypedValue ConvertToModelValue(ITypedValue propertyValue)
        {
            return ConvertValue(propertyValue);
        }

        protected internal abstract ITypedValue ConvertValue(ITypedValue propertyValue);
    }
}