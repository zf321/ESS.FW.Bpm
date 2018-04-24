

using System;
using ESS.FW.Bpm.Model.Xml.instance;
using ESS.FW.Bpm.Model.Xml.type.attribute;
using ESS.FW.Bpm.Model.Xml.type.reference;

namespace ESS.FW.Bpm.Model.Xml.impl.type.attribute
{
    /// 
    /// <summary>
    /// 
    /// 
    /// </summary>
    public abstract class AttributeBuilderImpl<T> : IAttributeBuilder<T>, IModelBuildOperation
    {

        private readonly AttributeImpl _attribute;
        private readonly ModelElementTypeImpl _modelType;

        internal AttributeBuilderImpl(string attributeName, ModelElementTypeImpl modelType, AttributeImpl/*<T>*/ attribute)
        {
            this._modelType = modelType;
            this._attribute = attribute;
            attribute.AttributeName = attributeName;
        }

        public virtual IAttributeBuilder<T> Namespace(string namespaceUri)
        {
            _attribute.NamespaceUri = namespaceUri;
            return this;
        }

        public virtual IAttributeBuilder<T> IdAttribute()
        {
            _attribute.SetId();
            return this;
        }


        public virtual IAttributeBuilder<T> DefaultValue(T defaultValue)
        {
            _attribute.SetDefaultValue(defaultValue);
            return this;
        }

        public virtual IAttributeBuilder<T> Required()
        {
            _attribute.Required = true;
            return this;
        }

        public virtual IAttribute Build()
        {
            _modelType.RegisterAttribute(_attribute);
            return _attribute;
        }        

        public virtual void PerformModelBuild(IModel model)
        {
            // do nothing
        }
    }
}