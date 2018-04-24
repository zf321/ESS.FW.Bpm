using System;
using ESS.FW.Bpm.Model.Xml.impl.type.reference;
using ESS.FW.Bpm.Model.Xml.instance;
using ESS.FW.Bpm.Model.Xml.type.attribute;
using ESS.FW.Bpm.Model.Xml.type.reference;


namespace ESS.FW.Bpm.Model.Xml.impl.type.attribute
{
    public class StringAttributeBuilderImpl : AttributeBuilderImpl<String>, IStringAttributeBuilder
    {

        private IAttributeReferenceBuilder _referenceBuilder;

        public StringAttributeBuilderImpl(string attributeName, ModelElementTypeImpl modelType) 
            : base(attributeName, modelType, new StringAttribute(modelType))
        {
        }

        public new virtual IStringAttributeBuilder Namespace(String namespaceUri)
        {
            return (IStringAttributeBuilder)base.Namespace(namespaceUri);
        }

        public new virtual IStringAttributeBuilder DefaultValue(string defaultValue)
        {
            return (IStringAttributeBuilder)base.DefaultValue(defaultValue);
        }

        public new virtual IStringAttributeBuilder Required()
        {
            return (IStringAttributeBuilder)base.Required();
        }

        public new virtual  IStringAttributeBuilder IdAttribute()
        {
            return (IStringAttributeBuilder)base.IdAttribute();
        }

        
        public IAttributeReferenceBuilder QNameAttributeReference<T>() where T : IModelElementInstance
        {
            AttributeImpl attribute = (AttributeImpl)Build();
            AttributeReferenceBuilderImpl<T> referenceBuilder = new QNameAttributeReferenceBuilderImpl<T>(attribute);
            _referenceBuilder = referenceBuilder;
            return referenceBuilder;
        }

        public IAttributeReferenceBuilder IdAttributeReference<T>() where T : IModelElementInstance
        {
            AttributeImpl attribute = (AttributeImpl)Build();
            AttributeReferenceBuilderImpl<T> referenceBuilder = new AttributeReferenceBuilderImpl<T>(attribute);
            _referenceBuilder = referenceBuilder;
            return referenceBuilder;
        }

        public IAttributeReferenceCollectionBuilder<TTarget> IdAttributeReferenceCollection<TTarget, TReference>(AttributeReferenceCollection<TReference> attributeReferenceCollection) where TTarget : IModelElementInstance where TReference : IModelElementInstance
        {
            AttributeImpl attribute = (AttributeImpl)Build();
            IAttributeReferenceCollectionBuilder<TTarget> referenceBuilder = new AttributeReferenceCollectionBuilderImpl<TTarget, TReference>(attribute, attributeReferenceCollection);
            _referenceBuilder = referenceBuilder;
            return referenceBuilder;
        }

        public override void PerformModelBuild(IModel model)
        {
            base.PerformModelBuild(model);
            if (_referenceBuilder != null)
            {
                ((IModelBuildOperation)_referenceBuilder).PerformModelBuild(model);
            }
        }

    }
}