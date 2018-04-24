using System;


namespace ESS.FW.Bpm.Model.Xml.impl.type.attribute
{

    public class NamedEnumAttributeBuilder<T> : AttributeBuilderImpl<T> where T:struct
    {

        public NamedEnumAttributeBuilder(string attributeName, ModelElementTypeImpl modelType) 
            : base(attributeName, modelType, new NamedEnumAttribute<T>(modelType))
        {
        }

        public new virtual NamedEnumAttributeBuilder<T> Namespace(string namespaceUri)
        {
            return (NamedEnumAttributeBuilder<T>)base.Namespace(namespaceUri);
        }

        public new virtual NamedEnumAttributeBuilder<T> DefaultValue(T defaultValue)
        {
            return (NamedEnumAttributeBuilder<T>)base.DefaultValue(defaultValue);
        }

        public new virtual NamedEnumAttributeBuilder<T> Required()
        {
            return (NamedEnumAttributeBuilder<T>)base.Required();
        }

        public new virtual NamedEnumAttributeBuilder<T> IdAttribute()
        {
            return (NamedEnumAttributeBuilder<T>)base.IdAttribute();
        }
    }
}