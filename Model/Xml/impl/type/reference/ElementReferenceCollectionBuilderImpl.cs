using System;
using ESS.FW.Bpm.Model.Xml.impl.type.attribute;
using ESS.FW.Bpm.Model.Xml.impl.type.child;
using ESS.FW.Bpm.Model.Xml.instance;
using ESS.FW.Bpm.Model.Xml.type.reference;

namespace ESS.FW.Bpm.Model.Xml.impl.type.reference
{
    /// <summary>
    /// 
    /// </summary>
    public class ElementReferenceCollectionBuilderImpl<TTarget, TSource> : IElementReferenceCollectionBuilder 
        where TTarget : IModelElementInstance where TSource : IModelElementInstance
    {
        
        protected internal ElementReferenceCollectionImpl<TTarget, TSource> ElementReferenceCollectionImpl;

        public ElementReferenceCollectionBuilderImpl(ChildElementCollectionImpl<TSource> collection)
        {
            this.ElementReferenceCollectionImpl = new ElementReferenceCollectionImpl<TTarget, TSource>(collection);
        }

        public virtual IElementReferenceCollection Build()
        {
            return ElementReferenceCollectionImpl;
        }
        
        public virtual void PerformModelBuild(IModel model)
        {
            ModelElementTypeImpl referenceTargetType = (ModelElementTypeImpl)model.GetType(typeof(TTarget));
            ModelElementTypeImpl referenceSourceType = (ModelElementTypeImpl)model.GetType(typeof(TSource));
            ElementReferenceCollectionImpl.ReferenceTargetElementType = referenceTargetType;
            ElementReferenceCollectionImpl.SetReferenceSourceElementType(referenceSourceType);

            // the referenced attribute may be declared on a base type of the referenced type.
            AttributeImpl idAttribute = (AttributeImpl)referenceTargetType.GetAttribute("id");
            if (idAttribute != null)
            {
                idAttribute.RegisterIncoming(ElementReferenceCollectionImpl);
                ElementReferenceCollectionImpl.ReferenceTargetAttribute = idAttribute;
            }
            else
            {
                throw new ModelException("Unable to find id attribute of " + typeof(TTarget));
            }
        }
    }
}