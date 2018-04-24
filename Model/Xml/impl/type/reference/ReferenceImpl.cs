using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Model.Xml.impl.type.attribute;
using ESS.FW.Bpm.Model.Xml.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;
using ESS.FW.Bpm.Model.Xml.type.reference;


namespace ESS.FW.Bpm.Model.Xml.impl.type.reference
{
    public abstract class ReferenceImpl : IReference
    {
        public IAttribute ReferenceTargetAttribute { get; set; }
        private ModelElementTypeImpl _referenceTargetElementType;


        protected internal abstract void SetReferenceIdentifier(IModelElementInstance referenceSourceElement, string referenceIdentifier);
        public abstract string GetReferenceIdentifier(IModelElementInstance referenceSourceElement);
        public abstract IModelElementType ReferenceSourceElementType { get; }

        public virtual T GetReferenceTargetElement<T>(IModelElementInstance referenceSourceElement) 
            where T : IModelElementInstance
        {
            string identifier = GetReferenceIdentifier(referenceSourceElement);
            IModelElementInstance referenceTargetElement = referenceSourceElement.ModelInstance.GetModelElementById(identifier);
            if (referenceTargetElement != null)
            {
                try
                {
                    return (T)referenceTargetElement;

                }
                catch (System.InvalidCastException)
                {
                    throw new ModelReferenceException("XmlElement " + referenceSourceElement + " references element " + referenceTargetElement + " of wrong type. " + "Expecting " + ReferenceTargetAttribute.OwningElementType + " got " + referenceTargetElement.ElementType);
                }
            }
            return default(T);
        }
        
        public virtual void SetReferenceTargetElement<T>(IModelElementInstance referenceSourceElement, T referenceTargetElement) 
            where T : IModelElementInstance
        {
            IModelInstance modelInstance = referenceSourceElement.ModelInstance;
            string referenceTargetIdentifier = ReferenceTargetAttribute.GetValue<String>(referenceTargetElement);
            IModelElementInstance existingElement = modelInstance.GetModelElementById(referenceTargetIdentifier);

            if (existingElement == null || existingElement.Id != referenceTargetElement.Id)
            {
                throw new ModelReferenceException("Cannot create reference to model element " + referenceTargetElement + ": element is not part of model. Please connect element to the model first.");
            }
            SetReferenceIdentifier(referenceSourceElement, referenceTargetIdentifier);
        }

        public virtual ModelElementTypeImpl ReferenceTargetElementType
        {
            set => _referenceTargetElementType = value;
        }

        public virtual IEnumerable<IModelElementInstance> FindReferenceSourceElements(IModelElementInstance referenceTargetElement)
        {
            if (_referenceTargetElementType.IsBaseTypeOf(referenceTargetElement.ElementType))
            {
                IModelElementType owningElementType = ReferenceSourceElementType;
                return referenceTargetElement.ModelInstance.GetModelElementsByType(owningElementType);
            }
            return new List<IModelElementInstance>();
        }

        protected internal abstract void UpdateReference(IModelElementInstance referenceSourceElement, string oldIdentifier, string newIdentifier);

        public virtual void ReferencedElementUpdated(IModelElementInstance referenceTargetElement, string oldIdentifier, string newIdentifier)
        {
            foreach (IModelElementInstance referenceSourceElement in FindReferenceSourceElements(referenceTargetElement))
            {
                UpdateReference(referenceSourceElement, oldIdentifier, newIdentifier);
            }
        }

        protected internal abstract void RemoveReference(IModelElementInstance referenceSourceElement, IModelElementInstance referenceTargetElement);

        public virtual void ReferencedElementRemoved(IModelElementInstance referenceTargetElement, object referenceIdentifier)
        {
            foreach (IModelElementInstance referenceSourceElement in FindReferenceSourceElements(referenceTargetElement))
            {
                if (referenceIdentifier.Equals(GetReferenceIdentifier(referenceSourceElement)))
                {
                    RemoveReference(referenceSourceElement, referenceTargetElement);
                }
            }
        }

    }
}