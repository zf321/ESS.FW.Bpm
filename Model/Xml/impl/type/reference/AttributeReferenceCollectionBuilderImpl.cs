using System;
using ESS.FW.Bpm.Model.Xml.impl.type.attribute;
using ESS.FW.Bpm.Model.Xml.instance;
using ESS.FW.Bpm.Model.Xml.type.reference;

namespace ESS.FW.Bpm.Model.Xml.impl.type.reference
{
    /// <summary>
    /// </summary>
    public class AttributeReferenceCollectionBuilderImpl<T, V> : IAttributeReferenceCollectionBuilder<T>, IModelBuildOperation where T : IModelElementInstance where V : IModelElementInstance
    {
        private readonly AttributeReferenceCollection<T> _attributeReferenceCollection;

        private readonly AttributeImpl _referenceSourceAttribute;


        public AttributeReferenceCollectionBuilderImpl(AttributeImpl attribute, AttributeReferenceCollection<V> attributeReferenceCollection)
        {
            _referenceSourceAttribute = attribute;
            try
            {
                _attributeReferenceCollection = (AttributeReferenceCollection<T>)attributeReferenceCollection.GetType()
                    .GetConstructor(new[] { typeof(AttributeImpl) })
                    .Invoke(new[] { _referenceSourceAttribute });
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public virtual AttributeReferenceCollection<T> Build()
        {
            _referenceSourceAttribute.RegisterOutgoingReference(_attributeReferenceCollection);
            return _attributeReferenceCollection;
        }

        IAttributeReference IAttributeReferenceBuilder.Build()
        {
            return Build();
        }

        IReference IReferenceBuilder.Build()
        {
            return Build();
        }


        public virtual void PerformModelBuild(IModel model)
        {
            // register declaring type as a referencing type of referenced type
            var referenceTargetType = (ModelElementTypeImpl)model.GetType(typeof(T));

            // the actual referenced type
            _attributeReferenceCollection.ReferenceTargetElementType = referenceTargetType;

            // the referenced attribute may be declared on a base type of the referenced type.
            AttributeImpl idAttribute = (AttributeImpl)referenceTargetType.GetAttribute("id");
            if (idAttribute != null)
            {
                idAttribute.RegisterIncoming(_attributeReferenceCollection);
                _attributeReferenceCollection.ReferenceTargetAttribute = idAttribute;
            }
            else
            {
                throw new ModelException("XmlElement type " + referenceTargetType.TypeNamespace + ":" + referenceTargetType.TypeName + " has no id attribute");
            }
        }

    }
}