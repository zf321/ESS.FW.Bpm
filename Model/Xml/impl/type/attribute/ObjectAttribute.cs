using ESS.FW.Bpm.Model.Xml.type;
using System;

namespace ESS.FW.Bpm.Model.Xml.impl.type.attribute
{
    public class ObjectAttribute : AttributeImpl
    {
        public ObjectAttribute(IModelElementType owningElementType) : base(owningElementType)
        {
        }
        

        protected override T ConvertXmlValueToModelValue<T>(string rawValue)
        {
            if (string.IsNullOrEmpty(rawValue))
                return default(T);
            return (T) Convert.ChangeType(rawValue, typeof(T));
        }

        protected override string ConvertModelValueToXmlValue<T>(T modelValue)
        {
            return modelValue.ToString();
        }
    }
}
