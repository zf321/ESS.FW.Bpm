

using System;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Xml.impl.type.attribute
{
   

    public class StringAttribute : AttributeImpl
    {

        public StringAttribute(IModelElementType owningElementType) 
            : base(owningElementType)
        {
        }       

        protected override T ConvertXmlValueToModelValue<T>(string rawValue)
        {
            if (string.IsNullOrEmpty(rawValue))
                return default(T);
            return (T)Convert.ChangeType(rawValue,typeof(T));
        }

        protected override string ConvertModelValueToXmlValue<T>(T modelValue)
        {
            return modelValue.ToString();
        }
    }
}