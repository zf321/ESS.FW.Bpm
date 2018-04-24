using System;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.instance;
using ESS.FW.Bpm.Model.Xml.type.child;
using ESS.FW.Bpm.Model.Xml.type.reference;

namespace ESS.FW.Bpm.Model.Xml.impl.type.reference
{

    using ModelElementInstanceImpl = ModelElementInstanceImpl;

    /// <summary>
    /// 
    /// </summary>
    public class ElementReferenceImpl<TTarget, TSource> : ElementReferenceCollectionImpl<TTarget, TSource>, IElementReference 
        where TTarget : IModelElementInstance where TSource : IModelElementInstance
    {


        public ElementReferenceImpl(IChildElement referenceSourceCollection) : base(referenceSourceCollection)
        {
        }

        private IChildElement ReferenceSourceChild => (IChildElement)GetReferenceSourceCollection();

        public virtual IModelElementInstance GetReferenceSource(IModelElementInstance referenceSourceParent)
        {
            return ReferenceSourceChild.GetChild(referenceSourceParent);
        }

        private void SetReferenceSource(IModelElementInstance referenceSourceParent, TSource referenceSource)
        {
            ReferenceSourceChild.SetChild(referenceSourceParent, referenceSource);
        }

        public virtual IModelElementInstance GetReferenceTargetElement(ModelElementInstanceImpl referenceSourceParentElement)
        {
            IModelElementInstance referenceSource = GetReferenceSource(referenceSourceParentElement);
            if (referenceSource != null)
            {
                string identifier = GetReferenceIdentifier(referenceSource);
                IModelElementInstance referenceTargetElement = referenceSourceParentElement.ModelInstance.GetModelElementById(identifier);
                if (referenceTargetElement != null)
                {
                    return referenceTargetElement;
                }
                throw new ModelException("Unable to find a model element instance for id " + identifier);
            }
            return null;
        }

        public virtual void SetReferenceTargetElement<TTarget>(ModelElementInstanceImpl referenceSourceParentElement, TTarget referenceTargetElement) where TTarget : IModelElementInstance
        {
            ModelInstanceImpl modelInstance = (ModelInstanceImpl)referenceSourceParentElement.ModelInstance;
            string identifier = ReferenceTargetAttribute.GetValue<String>(referenceTargetElement);
            IModelElementInstance existingElement = modelInstance.GetModelElementById(identifier);

            if (existingElement == null || !existingElement.Equals(referenceTargetElement))
            {
                throw new ModelReferenceException("Cannot create reference to model element " + referenceTargetElement + ": element is not part of model. Please connect element to the model first.");
            }
            TSource referenceSourceElement = modelInstance.NewInstance<TSource>(ReferenceSourceElementType);
            SetReferenceSource(referenceSourceParentElement, referenceSourceElement);
            SetReferenceIdentifier(referenceSourceElement, identifier);
        }

        public virtual void ClearReferenceTargetElement(ModelElementInstanceImpl referenceSourceParentElement)
        {
            ReferenceSourceChild.RemoveChild(referenceSourceParentElement);
        }
    }
}