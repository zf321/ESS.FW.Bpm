using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.impl.util;
using ESS.FW.Bpm.Model.Xml.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.child;
using ESS.FW.Bpm.Model.Xml.type.reference;



namespace ESS.FW.Bpm.Model.Xml.impl.type.reference
{

    /// <summary>
    /// 
    /// </summary>
    public class ElementReferenceCollectionImpl<TTarget, TSource> : ReferenceImpl, IElementReferenceCollection
        where TTarget : IModelElementInstance where TSource : IModelElementInstance
    {

        private readonly IChildElementCollection _referenceSourceCollection;

        private ModelElementTypeImpl _referenceSourceType;

        public ElementReferenceCollectionImpl(IChildElementCollection referenceSourceCollection)
        {
            this._referenceSourceCollection = referenceSourceCollection;
        }


        public virtual IChildElementCollection GetReferenceSourceCollection() => _referenceSourceCollection;

        protected internal override void SetReferenceIdentifier(IModelElementInstance referenceSourceElement, string referenceIdentifier)
        {
            referenceSourceElement.TextContent = referenceIdentifier;
        }

        protected internal virtual void PerformAddOperation(ModelElementInstanceImpl referenceSourceParentElement, IModelElementInstance referenceTargetElement)
        {
            ModelInstanceImpl modelInstance = (ModelInstanceImpl)referenceSourceParentElement.ModelInstance;
            string referenceTargetIdentifier = ReferenceTargetAttribute.GetValue<String>(referenceTargetElement);
            IModelElementInstance existingElement = modelInstance.GetModelElementById(referenceTargetIdentifier);

            if (existingElement == null || !existingElement.Equals(referenceTargetElement))
            {
                throw new ModelReferenceException("Cannot create reference to model element " + referenceTargetElement + ": element is not part of model. Please connect element to the model first.");
            }
            else
            {
                ICollection<TSource> referenceSourceElements = _referenceSourceCollection.Get<TSource>(referenceSourceParentElement);
                TSource referenceSourceElement = modelInstance.NewInstance<TSource>(_referenceSourceType);
                referenceSourceElements.Add(referenceSourceElement);
                SetReferenceIdentifier(referenceSourceElement, referenceTargetIdentifier);
            }
        }

        protected internal virtual void PerformRemoveOperation(ModelElementInstanceImpl referenceSourceParentElement, object referenceTargetElement)
        {
            ICollection<IModelElementInstance> referenceSourceChildElements = referenceSourceParentElement.GetChildElementsByType(_referenceSourceType);
            foreach (IModelElementInstance referenceSourceChildElement in referenceSourceChildElements)
            {
                if (GetReferenceTargetElement<TTarget>(referenceSourceChildElement).Equals(referenceTargetElement))
                {
                    referenceSourceParentElement.RemoveChildElement(referenceSourceChildElement);
                }
            }
        }

        protected internal virtual void PerformClearOperation(ModelElementInstanceImpl referenceSourceParentElement, ICollection<IDomElement> elementsToRemove)
        {
            foreach (IDomElement element in elementsToRemove)
            {
                referenceSourceParentElement.DomElement.RemoveChild(element);
            }
        }

        public override string GetReferenceIdentifier(IModelElementInstance referenceSourceElement)
        {
            return referenceSourceElement.TextContent;
        }

        protected internal override void UpdateReference(IModelElementInstance referenceSourceElement, string oldIdentifier, string newIdentifier)
        {
            string referencingTextContent = GetReferenceIdentifier(referenceSourceElement);
            if (!string.IsNullOrWhiteSpace(oldIdentifier) && oldIdentifier.Equals(referencingTextContent))
            {
                SetReferenceIdentifier(referenceSourceElement, newIdentifier);
            }
        }

        protected internal override void RemoveReference(IModelElementInstance referenceSourceElement, IModelElementInstance referenceTargetElement)
        {
            IModelElementInstance parentElement = referenceSourceElement.ParentElement;
            ICollection<TSource> childElementCollection = _referenceSourceCollection.Get<TSource>(parentElement);
            childElementCollection.Remove((TSource)referenceSourceElement);
        }


        public virtual void SetReferenceSourceElementType(ModelElementTypeImpl referenceSourceType)
        {
            this._referenceSourceType = referenceSourceType;
        }

        public override IModelElementType ReferenceSourceElementType => _referenceSourceType;

        protected internal virtual IList<IDomElement> GetView(ModelElementInstanceImpl referenceSourceParentElement)
        {
            IDomDocument document = referenceSourceParentElement.ModelInstance.Document;
            ICollection<TSource> referenceSourceElements = _referenceSourceCollection.Get<TSource>(referenceSourceParentElement);
            IList<IDomElement> referenceTargetElements = new List<IDomElement>();
            foreach (TSource referenceSourceElement in referenceSourceElements)
            {
                string identifier = GetReferenceIdentifier(referenceSourceElement);

                IDomElement referenceTargetElement = document.GetElementById(identifier);
                if (referenceTargetElement != null)
                {
                    referenceTargetElements.Add(referenceTargetElement);
                }
                else
                {
                    throw new ModelException("Unable to find a model element instance for id " + identifier);
                }
            }
            return referenceTargetElements;
        }

        public virtual ICollection<TTarget> GetReferenceTargetElements<TTarget>(ModelElementInstanceImpl referenceSourceParentElement) where TTarget:IModelElementInstance
        {
            return new CollectionAnonymousInnerClass(this, referenceSourceParentElement) as ICollection<TTarget>;
        }

        private class CollectionAnonymousInnerClass : ICollection<TTarget>
        {
            private readonly ElementReferenceCollectionImpl<TTarget,TSource> _outerInstance;

            private ModelElementInstanceImpl referenceSourceParentElement;

            public CollectionAnonymousInnerClass(ElementReferenceCollectionImpl<TTarget, TSource> outerInstance, ModelElementInstanceImpl referenceSourceParentElement)
            {
                this._outerInstance = outerInstance;
                this.referenceSourceParentElement = referenceSourceParentElement;
            }


            public virtual int size()
            {
                return _outerInstance.GetView(referenceSourceParentElement).Count;
            }

            public int Count => size();

            public virtual bool Empty => _outerInstance.GetView(referenceSourceParentElement).Count == 0;


            public bool IsReadOnly => throw new NotImplementedException();

            public virtual bool contains(object o)
            {
                if (o == null)
                {
                    return false;
                }
                if (!(o is ModelElementInstanceImpl))
                {
                    return false;
                }
                return _outerInstance.GetView(referenceSourceParentElement).Contains(((ModelElementInstanceImpl)o).DomElement);
            }

            public bool Contains(TTarget item)
            {
                return contains(item);
            }

            public virtual IEnumerator<TTarget> iterator()
            {
                ICollection<TTarget> modelElementCollection = ModelUtil.GetModelElementCollection<TTarget>(_outerInstance.GetView(referenceSourceParentElement), referenceSourceParentElement.ModelInstance);
                return modelElementCollection.GetEnumerator();
            }

            public virtual object[] toArray()
            {
                ICollection<TTarget> modelElementCollection = ModelUtil.GetModelElementCollection<TTarget>(_outerInstance.GetView(referenceSourceParentElement), referenceSourceParentElement.ModelInstance);
                return modelElementCollection.Select(t => (object) t).ToArray();
            }

            public virtual T1[] toArray<T1>(T1[] a)
            {
                throw new NotImplementedException();
                //ICollection<TTarget> modelElementCollection = ModelUtil.GetModelElementCollection<TTarget>(_outerInstance.GetView(referenceSourceParentElement), referenceSourceParentElement.ModelInstance);                
                //return modelElementCollection.toArray(a);
            }

            public virtual bool add(TTarget t)
            {
                if (_outerInstance.GetReferenceSourceCollection().Immutable)
                {
                    throw new UnsupportedModelOperationException("add()", "collection is immutable");
                }
                if (!contains(t))
                {
                    _outerInstance.PerformAddOperation(referenceSourceParentElement, t);
                }
                return true;
            }


            public void Add(TTarget item)
            {
                add(item);
            }

            public virtual bool remove(object o)
            {
                if (_outerInstance.GetReferenceSourceCollection().Immutable)
                {
                    throw new UnsupportedModelOperationException("remove()", "collection is immutable");
                }
                else
                {
                    ModelUtil.EnsureInstanceOf(o, typeof(ModelElementInstanceImpl));
                    _outerInstance.PerformRemoveOperation(referenceSourceParentElement, o);
                    return true;
                }
            }

            public bool Remove(TTarget item)
            {
                return remove(item);
            }


            public virtual bool containsAll<T1>(ICollection<T1> c)
            {
                throw new NotImplementedException();
                //ICollection<TTarget> modelElementCollection = ModelUtil.GetModelElementCollection<TTarget>(_outerInstance.GetView(referenceSourceParentElement), referenceSourceParentElement.ModelInstance);
                ////JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'containsAll' method:
                //return modelElementCollection.containsAll(c);
            }

            public virtual bool addAll<T1>(ICollection<T1> c) where T1 : TTarget
            {
                if (_outerInstance.GetReferenceSourceCollection().Immutable)
                {
                    throw new UnsupportedModelOperationException("addAll()", "collection is immutable");
                }
                bool result = false;
                foreach (TTarget o in c)
                {
                    result |= add(o);
                }
                return result;
            }

            public virtual bool removeAll<T1>(ICollection<T1> c)
            {
                if (_outerInstance.GetReferenceSourceCollection().Immutable)
                {
                    throw new UnsupportedModelOperationException("removeAll()", "collection is immutable");
                }
                bool result = false;
                foreach (object o in c)
                {
                    result |= remove(o);
                }
                return result;
            }

            public virtual bool retainAll<T1>(ICollection<T1> c)
            {
                throw new UnsupportedModelOperationException("retainAll()", "not implemented");
            }

            public virtual void clear()
            {
                if (_outerInstance.GetReferenceSourceCollection().Immutable)
                {
                    throw new UnsupportedModelOperationException("clear()", "collection is immutable");
                }
                ICollection<IDomElement> view = new List<IDomElement>();
                foreach (TSource referenceSourceElement in _outerInstance.GetReferenceSourceCollection().Get<TSource>(referenceSourceParentElement))
                {
                    view.Add(referenceSourceElement.DomElement);
                }
                _outerInstance.PerformClearOperation(referenceSourceParentElement, view);
            }


            public void Clear()
            {
                clear();
            }

            public void CopyTo(TTarget[] array, int arrayIndex)
            {
                throw new NotImplementedException();
            }

            public IEnumerator<TTarget> GetEnumerator()
            {
                return iterator();
            }
            IEnumerator IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }
    }
}