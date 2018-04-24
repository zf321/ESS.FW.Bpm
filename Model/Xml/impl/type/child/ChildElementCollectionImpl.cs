using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.impl.util;
using ESS.FW.Bpm.Model.Xml.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.child;


namespace ESS.FW.Bpm.Model.Xml.impl.type.child
{
    using System.Collections;
    using System.Linq;
    using ModelElementInstanceImpl = ModelElementInstanceImpl;
    using ModelUtil = ModelUtil;


    /// <summary>
    /// <para>This collection is a view on an the children of a Model XmlElement.</para>
    /// 
    /// 
    /// 
    /// </summary>
    public class ChildElementCollectionImpl<T> : IChildElementCollection
        where T : IModelElementInstance
    {
        #region members

        private readonly IModelElementType _parentElementType;
        private int _minOccurs = 0;
        protected internal int maxOccurs = -1;
        private bool _isMutable = true;

        #endregion

        #region constructor
        public ChildElementCollectionImpl(ModelElementTypeImpl parentElementType)
        {
            //this.childElementTypeClass = childElementTypeClass;
            this._parentElementType = parentElementType;
        }

        #endregion


        public virtual void SetImmutable() => Mutable = false;

        public virtual bool Mutable
        {
            set => _isMutable = value;
        }

        public virtual bool Immutable => !_isMutable;

        // view /////////////////////////////////////////////////////////

        /// <summary>
        /// Internal method providing access to the view represented by this collection.
        /// </summary>
        /// <returns> the view represented by this collection </returns>
        private IList<IDomElement> GetView(ModelElementInstanceImpl modelElement)
        {
            return modelElement.DomElement.GetChildElementsByType(modelElement.ModelInstance, typeof(T));
        }

        public virtual int MinOccurs
        {
            get => _minOccurs;
            set => _minOccurs = value;
        }


        public virtual int MaxOccurs
        {
            get => maxOccurs;
            set => maxOccurs = value;
        }

        public virtual IModelElementType GetChildElementType(IModel model)
        {
            return model.GetType(typeof(T));
        }

        public virtual T GetChildElementTypeClass<T>(T typeClass) where T : IModelElementInstance
        {
            return typeClass;
        }

        public virtual IModelElementType ParentElementType => _parentElementType;


        /// <summary>
        /// the "add" operation used by the collection </summary>
        private void PerformAddOperation(ModelElementInstanceImpl modelElement, T e)
        {
            modelElement.AddChildElement((IModelElementInstance)e);
        }

        /// <summary>
        /// the "remove" operation used by this collection </summary>
        private bool PerformRemoveOperation(ModelElementInstanceImpl modelElement, object e)
        {
            return modelElement.RemoveChildElement((ModelElementInstanceImpl)e);
        }

        /// <summary>
        /// the "clear" operation used by this collection </summary>
        private void PerformClearOperation(ModelElementInstanceImpl modelElement, IList<IDomElement> elementsToRemove)
        {
            ICollection<IModelElementInstance> modelElements = ModelUtil.GetModelElementCollection<IModelElementInstance>(elementsToRemove, modelElement.ModelInstance);
            foreach (IModelElementInstance element in modelElements)
            {
                modelElement.RemoveChildElement(element);
            }
        }

        public virtual ICollection<T> Get<T>(IModelElementInstance element) where T : IModelElementInstance
        {
            ModelElementInstanceImpl modelElement = (ModelElementInstanceImpl)element;
            return new CollectionAnonymousInnerClass(this, modelElement) as ICollection<T>;

        }

        private class CollectionAnonymousInnerClass :ICollection<T>
        {
            private readonly ChildElementCollectionImpl<T> _outerInstance;

            private ModelElementInstanceImpl modelElement;

            public CollectionAnonymousInnerClass(ChildElementCollectionImpl<T> outerInstance, ModelElementInstanceImpl modelElement)
            {
                this._outerInstance = outerInstance;
                this.modelElement = modelElement;
            }


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
                return _outerInstance.GetView(modelElement).Contains(((ModelElementInstanceImpl)o).DomElement);
            }


            public bool Contains(T item)
            {
                return contains(item);
            }


            public virtual bool containsAll<T1>(ICollection<T1> c)
            {
                foreach (object elementToCheck in c)
                {
                    if (!contains(elementToCheck))
                    {
                        return false;
                    }
                }
                return true;
            }

            public virtual bool Empty
            {
                get
                {
                    return _outerInstance.GetView(modelElement).Count == 0;
                }
            }

            public int Count => size();

            public bool IsReadOnly => throw new NotImplementedException();

            public virtual IEnumerator<T> iterator()
            {
                var modelElementCollection = ModelUtil.GetModelElementCollection<T>(_outerInstance.GetView(modelElement), modelElement.ModelInstance);
                return modelElementCollection.GetEnumerator();
            }

            public virtual object[] toArray()
            {
                var modelElementCollection = ModelUtil.GetModelElementCollection<T>(_outerInstance.GetView(modelElement), modelElement.ModelInstance);
                var result = modelElementCollection.Select(d => (object)d);
                return result.ToArray();
            }

            public virtual U[] toArray<U>(U[] a) 
            {
                throw new NotImplementedException();
                //var modelElementCollection = ModelUtil.GetModelElementCollection<U>(_outerInstance.GetView(modelElement), modelElement.ModelInstance);
                //return modelElementCollection.ToArray();
            }

            public virtual int size()
            {
                return _outerInstance.GetView(modelElement).Count;
            }

            public virtual bool add(T e)
            {
                if (_outerInstance.Immutable)
                {
                    throw new UnsupportedModelOperationException("add()", "collection is immutable");
                }
                _outerInstance.PerformAddOperation(modelElement, e);
                return true;
            }

            public void Add(T item)
            {
                this.add(item);
            }

            public virtual bool addAll<T1>(ICollection<T1> c) where T1 : T
            {
                if (_outerInstance.Immutable)
                {
                    throw new UnsupportedModelOperationException("addAll()", "collection is immutable");
                }
                bool result = false;
                foreach (T t in c)
                {
                    result |= add(t);
                }
                return result;
            }

            public virtual void clear()
            {
                if (_outerInstance.Immutable)
                {
                    throw new UnsupportedModelOperationException("clear()", "collection is immutable");
                }
                IList<IDomElement> view = _outerInstance.GetView(modelElement);
                _outerInstance.PerformClearOperation(modelElement, view);
            }

            public virtual bool remove(object e)
            {
                if (_outerInstance.Immutable)
                {
                    throw new UnsupportedModelOperationException("remove()", "collection is immutable");
                }
                ModelUtil.EnsureInstanceOf(e, typeof(ModelElementInstanceImpl));
                return _outerInstance.PerformRemoveOperation(modelElement, e);
            }

            public virtual bool removeAll<T1>(ICollection<T1> c)
            {
                if (_outerInstance.Immutable)
                {
                    throw new UnsupportedModelOperationException("removeAll()", "collection is immutable");
                }
                bool result = false;
                foreach (object t in c)
                {
                    result |= remove(t);
                }
                return result;
            }

            public void Clear()
            {
                this.clear();
            }


            public virtual bool retainAll<T1>(ICollection<T1> c)
            {
                throw new UnsupportedModelOperationException("retainAll()", "not implemented");
            }


            public virtual void CopyTo(T[] array, int arrayIndex)
            {
                throw new NotImplementedException();
            }

            public bool Remove(T item)
            {
                return remove(item);
            }

            public IEnumerator<T> GetEnumerator()
            {
                return this.iterator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }

    }
}