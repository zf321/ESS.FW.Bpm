

using ESS.FW.Bpm.Model.Xml.impl.type.attribute;
using ESS.FW.Bpm.Model.Xml.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;
using ESS.FW.Bpm.Model.Xml.type.reference;

namespace ESS.FW.Bpm.Model.Xml.impl.type.reference
{
    /// <summary>
    /// 
    /// </summary>
    public class AttributeReferenceImpl<T> : ReferenceImpl, IAttributeReference
        where T : IModelElementInstance
    {

        protected internal readonly AttributeImpl referenceSourceAttribute;

        public AttributeReferenceImpl(AttributeImpl referenceSourceAttribute)
        {
            this.referenceSourceAttribute = referenceSourceAttribute;
        }

        public override string GetReferenceIdentifier(IModelElementInstance referenceSourceElement)
        {
            return referenceSourceAttribute.GetValue<string>(referenceSourceElement);
        }

        protected internal override void SetReferenceIdentifier(IModelElementInstance referenceSourceElement, string referenceIdentifier)
        {
            referenceSourceAttribute.SetValue(referenceSourceElement, referenceIdentifier);
        }

        /// <summary>
        /// Get the reference source attribute
        /// </summary>
        /// <returns> the reference source attribute </returns>
        public virtual IAttribute ReferenceSourceAttribute => referenceSourceAttribute;

        public override IModelElementType ReferenceSourceElementType => referenceSourceAttribute.OwningElementType;

        protected internal override void UpdateReference(IModelElementInstance referenceSourceElement, string oldIdentifier, string newIdentifier)
        {
            string referencingAttributeValue = GetReferenceIdentifier(referenceSourceElement);
            //if (!string.ReferenceEquals(oldIdentifier, null) && oldIdentifier.Equals(referencingAttributeValue))
            if (!string.IsNullOrEmpty(oldIdentifier) && oldIdentifier.Equals(referencingAttributeValue))
            {
                SetReferenceIdentifier(referenceSourceElement, newIdentifier);
            }
        }

        protected internal override void RemoveReference(IModelElementInstance referenceSourceElement, IModelElementInstance referenceTargetElement)
        {
            referenceSourceAttribute.RemoveAttribute(referenceSourceElement);
        }

    }

}