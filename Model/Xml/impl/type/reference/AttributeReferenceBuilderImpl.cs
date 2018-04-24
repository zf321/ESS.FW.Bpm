using System;
using ESS.FW.Bpm.Model.Xml.impl.type.attribute;
using ESS.FW.Bpm.Model.Xml.instance;
using ESS.FW.Bpm.Model.Xml.type.reference;


namespace ESS.FW.Bpm.Model.Xml.impl.type.reference
{
    public class AttributeReferenceBuilderImpl<T> : IAttributeReferenceBuilder, IModelBuildOperation 
        where T : IModelElementInstance
    {

        private readonly AttributeImpl _referenceSourceAttribute;
        protected internal AttributeReferenceImpl<T> AttributeReferenceImpl;

        
        public AttributeReferenceBuilderImpl(AttributeImpl referenceSourceAttribute)
        {
            this._referenceSourceAttribute = referenceSourceAttribute;
            this.AttributeReferenceImpl = new AttributeReferenceImpl<T>(referenceSourceAttribute);
        }

        public virtual IAttributeReference Build()
        {
            _referenceSourceAttribute.RegisterOutgoingReference(AttributeReferenceImpl);
            return AttributeReferenceImpl;
        }

        public virtual void PerformModelBuild(IModel model)
        {
            // register declaring type as a referencing type of referenced type
            ModelElementTypeImpl referenceTargetType = (ModelElementTypeImpl)model.GetType(typeof(T));

            // the actual referenced type
            AttributeReferenceImpl.ReferenceTargetElementType = referenceTargetType;

            // the referenced attribute may be declared on a base type of the referenced type.

            AttributeImpl idAttribute = (AttributeImpl) referenceTargetType.GetAttribute("id");
            if (idAttribute != null)
            {
                idAttribute.RegisterIncoming(AttributeReferenceImpl);
                AttributeReferenceImpl.ReferenceTargetAttribute = idAttribute;
            }
            else
            {
                throw new ModelException("XmlElement type " + referenceTargetType.TypeNamespace + ":" + referenceTargetType.TypeName + " has no id attribute");
            }
        }

        IReference IReferenceBuilder.Build()
        {
            return Build();
        }
        
    }
}