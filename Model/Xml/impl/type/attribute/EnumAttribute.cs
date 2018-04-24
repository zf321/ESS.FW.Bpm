using System;
using ESS.FW.Bpm.Model.Xml.type;


namespace ESS.FW.Bpm.Model.Xml.impl.type.attribute
{
	public class EnumAttribute<T> : AttributeImpl  where T:struct
    {
        
        public EnumAttribute(IModelElementType owningElementType) 
            : base(owningElementType)
        {
        }

        protected override T ConvertXmlValueToModelValue<T>(string rawValue)
        {
            if (!string.IsNullOrEmpty(rawValue))
            {
                return (T)Enum.Parse(typeof(T), rawValue);
            }
            return default(T);
        }

        protected override string ConvertModelValueToXmlValue<T>(T modelValue)
        {
            return modelValue.ToString();
        }

    }
}