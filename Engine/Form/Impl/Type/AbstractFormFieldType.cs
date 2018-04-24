using System;
using ESS.FW.Bpm.Engine.Variable.Type;
using ESS.FW.Bpm.Engine.Variable.Value;

namespace ESS.FW.Bpm.Engine.Form.Impl.Type
{
    /// <summary>
    ///      
    ///     
    /// </summary>
    public abstract class AbstractFormFieldType : IFormType
    {
        public abstract string Name { get; }

        public virtual object GetInformation(string key)
        {
            return null;
        }

        public abstract ITypedValue ConvertToFormValue(ITypedValue propertyValue);

        public abstract ITypedValue ConvertToModelValue(ITypedValue propertyValue);

        [Obsolete]
        public abstract object ConvertFormValueToModelValue(object propertyValue);

        [Obsolete]
        public abstract string ConvertModelValueToFormValue(object modelValue);
    }
}