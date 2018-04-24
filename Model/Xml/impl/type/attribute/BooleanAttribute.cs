

using System;
using ESS.FW.Bpm.Model.Xml.impl.util;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;

namespace ESS.FW.Bpm.Model.Xml.impl.type.attribute
{
	public class BooleanAttribute : AttributeImpl
    {

        public BooleanAttribute(IModelElementType owningElementType) : base(owningElementType)
        {
        }
        
        protected override T ConvertXmlValueToModelValue<T>(string rawValue)
        {
            if (string.IsNullOrWhiteSpace(rawValue))
                return default(T);
            return (T) Convert.ChangeType(rawValue, typeof(T));
        }

        protected override string ConvertModelValueToXmlValue<T>(T modelValue)
        {
            return modelValue.ToString();
        }
    }
}