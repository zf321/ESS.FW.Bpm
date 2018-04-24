

using System;

namespace ESS.FW.Bpm.Model.Xml.impl.type.attribute
{

    /// <summary>
    /// 
    /// </summary>
    public class IntegerAttributeBuilder : AttributeBuilderImpl<Int32?>
    {

        public IntegerAttributeBuilder(string attributeName, ModelElementTypeImpl modelType) 
            : base(attributeName, modelType, new IntegerAttribute(modelType))
        {
        }

        public new virtual IntegerAttributeBuilder Namespace(string namespaceUri)
        {
            return (IntegerAttributeBuilder)base.Namespace(namespaceUri);
        }

        public new virtual IntegerAttributeBuilder DefaultValue(Int32? defaultValue)
        {
            return (IntegerAttributeBuilder)base.DefaultValue(defaultValue);
        }

        public new virtual IntegerAttributeBuilder Required()
        {
            return (IntegerAttributeBuilder)base.Required();
        }

        public new virtual IntegerAttributeBuilder IdAttribute()
        {
            return (IntegerAttributeBuilder)base.IdAttribute();
        }
    }
}