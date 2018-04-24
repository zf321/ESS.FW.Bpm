using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Model.Xml.impl;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.impl.type.attribute;
using ESS.FW.Bpm.Model.Xml.impl.type.reference;
using ESS.FW.Bpm.Model.Xml.impl.util;
using ESS.FW.Bpm.Model.Xml.instance;
using ESS.FW.Bpm.Model.Xml.type.attribute;


namespace ESS.FW.Bpm.Model.Xml.type.reference
{

    /// <summary>
    /// 
    /// 
    /// </summary>
    public abstract class AttributeReferenceCollection<T> : AttributeReferenceImpl<T> where T : IModelElementInstance
    {

        protected internal char Separator = ' ';

        public AttributeReferenceCollection(AttributeImpl referenceSourceAttribute) : base(referenceSourceAttribute)
        {
        }

        protected internal override void UpdateReference(IModelElementInstance referenceSourceElement, string oldIdentifier, string newIdentifier)
        {
            string referencingIdentifier = GetReferenceIdentifier(referenceSourceElement);
            IList<string> references = StringUtil.SplitListBySeparator(referencingIdentifier, Separator);
            if (!string.IsNullOrEmpty(oldIdentifier) && references.Contains(oldIdentifier))
            {
                referencingIdentifier = referencingIdentifier.Replace(oldIdentifier, newIdentifier);
                SetReferenceIdentifier(referenceSourceElement, newIdentifier);
            }
        }

        protected internal override void RemoveReference(IModelElementInstance referenceSourceElement, IModelElementInstance referenceTargetElement)
        {
            string identifier = GetReferenceIdentifier(referenceSourceElement);
            IList<string> references = StringUtil.SplitListBySeparator(identifier, Separator);
            string identifierToRemove = GetTargetElementIdentifier(referenceTargetElement);
            references.Remove(identifierToRemove);
            identifier = StringUtil.JoinList(references, Separator.ToString());
            SetReferenceIdentifier(referenceSourceElement, identifier);
        }

        protected internal abstract string GetTargetElementIdentifier(IModelElementInstance referenceTargetElement);

        private ICollection<IDomElement> GetView(IModelElementInstance referenceSourceElement)
        {
            IDomDocument document = referenceSourceElement.ModelInstance.Document;

            string identifier = GetReferenceIdentifier(referenceSourceElement);
            IList<string> references = StringUtil.SplitListBySeparator(identifier, Separator);

            ICollection<IDomElement> referenceTargetElements = new List<IDomElement>();
            foreach (string reference in references)
            {
                //TODO document.getElementById可能获取不到值
                IDomElement referenceTargetElement = document.GetElementById(reference);
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

        public virtual ICollection<T> GetReferenceTargetElements(IModelElementInstance referenceSourceElement)
        {
            return new CollectionAnonymousInnerClass(this, referenceSourceElement);
        }

        private class CollectionAnonymousInnerClass : ICollection<T>
        {
            private readonly AttributeReferenceCollection<T> _outerInstance;

            private IModelElementInstance referenceSourceElement;

            public CollectionAnonymousInnerClass(AttributeReferenceCollection<T> outerInstance, IModelElementInstance referenceSourceElement)
            {
                this._outerInstance = outerInstance;
                this.referenceSourceElement = referenceSourceElement;
            }


            public virtual int size()
            {
                return _outerInstance.GetView(referenceSourceElement).Count;
            }

            public virtual bool Empty
            {
                get
                {
                    return _outerInstance.GetView(referenceSourceElement).Count == 0;
                }
            }

            public int Count => throw new NotImplementedException();

            public bool IsReadOnly => throw new NotImplementedException();

            public virtual bool contains(object o)
            {
                if (o == null)
                {
                    return false;
                }
                else if (!(o is ModelElementInstanceImpl))
                {
                    return false;
                }
                else
                {
                    return _outerInstance.GetView(referenceSourceElement).Contains(((ModelElementInstanceImpl)o).DomElement);
                }
            }

            public bool Contains(T item)
            {
                return contains(item);
            }

            public virtual IEnumerator<T> iterator()
            {
                ICollection<T> modelElementCollection = ModelUtil.GetModelElementCollection<T>(_outerInstance.GetView(referenceSourceElement).ToList(), (ModelInstanceImpl)referenceSourceElement.ModelInstance);
                return modelElementCollection.GetEnumerator();
            }

            public IEnumerator<T> GetEnumerator()
            {
                return iterator();
            }

            public virtual object[] toArray()
            {
                ICollection<T> modelElementCollection = ModelUtil.GetModelElementCollection<T>(_outerInstance.GetView(referenceSourceElement).ToList(), (ModelInstanceImpl)referenceSourceElement.ModelInstance);
                return modelElementCollection.Select(t => (object) t).ToArray();
                //return modelElementCollection.ToArray();
            }

            public virtual T1[] toArray<T1>(T1[] a)
            {
                throw new NotImplementedException();
                //ICollection<T> modelElementCollection = ModelUtil.GetModelElementCollection<T>(_outerInstance.GetView(referenceSourceElement).ToList(), (ModelInstanceImpl)referenceSourceElement.ModelInstance);
                //return modelElementCollection.toArray(a);
            }

            public virtual bool add(T t)
            {
                if (!contains(t))
                {
                    _outerInstance.PerformAddOperation(referenceSourceElement, t);
                }
                return true;
            }

            public void Add(T item)
            {
                add(item);
            }

            public virtual bool remove(object o)
            {
                ModelUtil.EnsureInstanceOf(o, typeof(ModelElementInstanceImpl));
                _outerInstance.PerformRemoveOperation(referenceSourceElement, o);
                return true;
            }

            public bool Remove(T item)
            {
                return remove(item);
            }

            public virtual bool containsAll<T1>(ICollection<T1> c)
            {
                throw new NotImplementedException();
                //ICollection<T> modelElementCollection = ModelUtil.GetModelElementCollection<T>(_outerInstance.GetView(referenceSourceElement).ToList(), (ModelInstanceImpl)referenceSourceElement.ModelInstance);
                ////JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'containsAll' method:
                //return modelElementCollection.containsAll(c);
            }

            public virtual bool addAll<T1>(ICollection<T1> c) where T1 : T
            {
                bool result = false;
                foreach (T o in c)
                {
                    result |= add(o);
                }
                return result;
            }

            public virtual bool removeAll<T1>(ICollection<T1> c)
            {
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
                _outerInstance.PerformClearOperation(referenceSourceElement);
            }

            public void Clear()
            {
                clear();
            }


            public void CopyTo(T[] array, int arrayIndex)
            {
                throw new NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }        

        protected internal virtual void PerformClearOperation(IModelElementInstance referenceSourceElement)
        {
            SetReferenceIdentifier(referenceSourceElement, "");
        }

        protected internal override void SetReferenceIdentifier(IModelElementInstance referenceSourceElement, string referenceIdentifier)
        {
            //if (!string.ReferenceEquals(referenceIdentifier, null) && referenceIdentifier.Length > 0)
            if(!string.IsNullOrWhiteSpace(referenceIdentifier))
            {
                base.SetReferenceIdentifier(referenceSourceElement, referenceIdentifier);
            }
            else
            {
                referenceSourceAttribute.RemoveAttribute(referenceSourceElement);
            }
        }
        
        protected internal virtual void PerformRemoveOperation(IModelElementInstance referenceSourceElement, object o)
        {
            RemoveReference(referenceSourceElement, (IModelElementInstance)o);
        }

        protected internal virtual void PerformAddOperation(IModelElementInstance referenceSourceElement, IModelElementInstance referenceTargetElement)
        {
            string identifier = GetReferenceIdentifier(referenceSourceElement);
            IList<string> references = StringUtil.SplitListBySeparator(identifier, Separator);

            string targetIdentifier = GetTargetElementIdentifier(referenceTargetElement);
            references.Add(targetIdentifier);

            identifier = StringUtil.JoinList(references, Separator.ToString());

            SetReferenceIdentifier(referenceSourceElement, identifier);
        }
    }
}