using System;
using ESS.FW.Bpm.Model.Dmn;
using ESS.FW.Bpm.Model.Xml.type;
using System.ComponentModel;
using System.Collections.Generic;

namespace ESS.FW.Bpm.Model.Xml.impl.type.attribute
{
    public class NamedEnumAttribute<T> : AttributeImpl where T : struct
    {
        protected static IDictionary<string, string> changedNames;
        public NamedEnumAttribute(IModelElementType owningElementType) 
            : base(owningElementType)
        {
            if (changedNames == null)
            {
                changedNames = new Dictionary<string, string>();
                var type = typeof(HitPolicy);
                foreach (var item in type.GetFields())
                {
                    if (item.GetCustomAttributes(false).Length > 0)
                    {
                        var cus = (DescriptionAttribute)item.GetCustomAttributes(false)[0];
                        changedNames.Add(cus.Description, item.Name);
                    }
                }
            }
        }

        protected override T ConvertXmlValueToModelValue<T>(string rawValue)
        {
            var enumType = typeof(T);
            if (!enumType.IsEnum)
                return default(T);

            var fields = enumType.GetFields();
            foreach (var f in fields)
            {
                var objs = f.GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (objs == null || objs.Length == 0)
                    continue;
                var value = (objs[0] as DescriptionAttribute)?.Description;
                if (value.Equals(rawValue))//大写+空格
                {
                    return (T)Enum.Parse(enumType, changedNames[value]);
                }
                if(changedNames.Values.Contains(rawValue))//小写格式
                {
                    return (T)Enum.Parse(enumType, rawValue);
                }
            }
            return default(T);            
        }
        
        protected override string ConvertModelValueToXmlValue<T>(T modelValue)
        {
            var enumType = typeof(T);

            if (enumType.IsEnum)
                return null;

            string strValue = modelValue.ToString();

            var fieldinfo = enumType.GetField(strValue);
            Object[] objs = fieldinfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (objs == null || objs.Length == 0)
            {
                return strValue;
            }
            else
            {
                DescriptionAttribute da = (DescriptionAttribute)objs[0];
                return da.Description;
            }
        }
    }
}