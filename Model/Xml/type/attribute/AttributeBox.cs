//using System;
//using ESS.FW.Bpm.Model.Xml.impl.type.attribute;

//namespace ESS.FW.Bpm.Model.Xml.type.attribute
//{
//    /// <summary>
//    /// 管理Attribute<object>的类型转换黑盒
//    /// </summary>
//    public class AttributeBox : AttributeImpl<object>, IAttribute<object>
//    {
//        public Type valueType;
//        public AttributeBox(IModelElementType owningElementType,Type _valueType) : base(owningElementType)
//        {
//            valueType = _valueType;
//        }
//        protected override string ConvertModelValueToXmlValue(object modelValue)
//        {
//            throw new NotImplementedException();
//        }

//        protected override object ConvertXmlValueToModelValue(string rawValue)
//        {
//            throw new NotImplementedException();
//        }
//        /// <summary>
//        /// 内部封装泛型转换
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="attr"></param>
//        /// <returns></returns>
//        public static AttributeBox CreateAttributeT<T>(AttributeImpl<T> attr) 
//        {
//            //Type type = typeof(T);
//            //if(type == typeof(string))
//            //{
//                var r = new AttributeBox(attr.OwningElementType,typeof(T));
//                SetValue<T>(attr, r);
//                return r;
//           // }
//            //if(type)
//        }
//        public static implicit operator AttributeImpl<string>(AttributeBox attribute)
//        {
//            var strAtt = new StringAttribute(attribute.OwningElementType);
//            if (attribute.DefaultValue != null)
//            {
//                strAtt.DefaultValue = (string)attribute.DefaultValue;
//            }
//            strAtt.AttributeName = attribute.AttributeName;
//            if (attribute.IdAttribute)
//            {
//                strAtt.SetId();
//            }
//            strAtt.Required = attribute.Required;
//            strAtt.NamespaceUri = attribute.NamespaceUri;
//            return strAtt;
//        }
//        private static void SetValue<T>(AttributeImpl<T> source,AttributeBox result)
//        {
//            result.DefaultValue = source.DefaultValue;
//            result.AttributeName = source.AttributeName;
//            result.Required = source.Required;
//            result.NamespaceUri = source.NamespaceUri;
//            if (source.IdAttribute)
//            {
//                result.SetId();
//            }
//        }
//    }
//}
