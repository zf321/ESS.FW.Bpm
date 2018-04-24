using System;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.impl.util;
using ESS.FW.Bpm.Model.Xml.instance;
using ESS.FW.Bpm.Model.Xml.type.child;


namespace ESS.FW.Bpm.Model.Xml.impl.type.child
{
    
    public class ChildElementImpl<T> : ChildElementCollectionImpl<T>, IChildElement where T : IModelElementInstance
    {

        public ChildElementImpl(ModelElementTypeImpl parentElementType) : base(parentElementType)
        {
            this.maxOccurs = 1;
        }
        
        private void PerformAddOperation(ModelElementInstanceImpl modelElement, T e)
        {
            modelElement.UniqueChildElementByNameNs = e;
        }

        public virtual void SetChild(IModelElementInstance element, IModelElementInstance newChildElement)
        {
            PerformAddOperation((ModelElementInstanceImpl)element, (T)newChildElement);
        }
        
        public virtual IModelElementInstance GetChild(IModelElementInstance element)
        {
            ModelElementInstanceImpl elementInstanceImpl = (ModelElementInstanceImpl)element;

            IModelElementInstance childElement = elementInstanceImpl.GetUniqueChildElementByType(typeof(T));
            if (childElement != null)
            {
                ModelUtil.EnsureInstanceOf(childElement, typeof(T));
                return (T)childElement;
            }
            return default(T);
        }

        public virtual bool RemoveChild(IModelElementInstance element)
        {
            IModelElementInstance childElement = GetChild(element);
            ModelElementInstanceImpl elementInstanceImpl = (ModelElementInstanceImpl)element;
            return elementInstanceImpl.RemoveChildElement(childElement);
        }
        
    }
}