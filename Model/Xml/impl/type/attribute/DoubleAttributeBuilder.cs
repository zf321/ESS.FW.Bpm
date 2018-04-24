

using ESS.FW.Bpm.Model.Xml.type.attribute;
using System;

namespace ESS.FW.Bpm.Model.Xml.impl.type.attribute
{

    /// <summary>
    /// 
    /// </summary>
    public class DoubleAttributeBuilder : AttributeBuilderImpl<Double?>
    {

        public DoubleAttributeBuilder(string attributeName, ModelElementTypeImpl modelType) 
            : base(attributeName, modelType, new DoubleAttribute(modelType))
        {
        }

        public new virtual  DoubleAttributeBuilder Namespace(string namespaceUri)
        {
            return (DoubleAttributeBuilder)base.Namespace(namespaceUri);
        }

        public new virtual DoubleAttributeBuilder DefaultValue(Double? defaultValue)
        {
            return (DoubleAttributeBuilder)base.DefaultValue(defaultValue);
        }

        public new virtual DoubleAttributeBuilder Required()
        {
            return (DoubleAttributeBuilder)base.Required();
        }

        public new virtual DoubleAttributeBuilder IdAttribute()
        {
            return (DoubleAttributeBuilder)base.IdAttribute();
        }
    }
}