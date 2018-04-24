using System;
using System.Collections;
using System.Collections.Generic;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Bpmn.instance.camunda;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.impl.util;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.impl;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance.camunda
{
    
    public class CamundaListImpl : BpmnModelElementInstanceImpl, ICamundaList
    {

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<ICamundaList>(/*typeof(ICamundaList),*/ BpmnModelConstants.CamundaElementList)
                .NamespaceUri(BpmnModelConstants.CamundaNs)
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<ICamundaList>
        {
            public virtual ICamundaList NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new CamundaListImpl(instanceContext);
            }
        }

        public CamundaListImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unchecked") public <T extends org.camunda.bpm.model.bpmn.instance.BpmnModelElementInstance> java.util.Collection<T> getValues()
        public virtual ICollection<IBpmnModelElementInstance> GetValues() //where T : IBpmnModelElementInstance
        {
            return new CollectionAnonymousInnerClass(this);
        }

        private class CollectionAnonymousInnerClass : ICollection<IBpmnModelElementInstance>
        {
            private readonly CamundaListImpl _outerInstance;

            public CollectionAnonymousInnerClass(CamundaListImpl outerInstance)
            {
                this._outerInstance = outerInstance;
            }


            protected internal virtual ICollection<IBpmnModelElementInstance> Elements
            {
                get
                {
                    return ModelUtil.GetModelElementCollection<IBpmnModelElementInstance>(_outerInstance.DomElement.ChildElements, (ModelInstanceImpl)_outerInstance.ModelInstance);
                }
            }

            public virtual int Size()
            {
                return Elements.Count;
            }

            public virtual bool Empty
            {
                get
                {
                    return Elements.Count == 0;
                }
            }

            public int Count
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public bool IsReadOnly
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public virtual bool contains(IBpmnModelElementInstance o)
            {
                return Elements.Contains(o);
            }

            public virtual IEnumerator<IBpmnModelElementInstance> Iterator()
            {
                return (IEnumerator<IBpmnModelElementInstance>)Elements.GetEnumerator();
            }

            public virtual T1[] ToArray<T1>(T1[] a)
            {
                return a;
            }

            public virtual bool add(IBpmnModelElementInstance t)
            {
                //XmlElement.AppendChild(t.DomElement);
                return true;
            }

            public virtual bool remove(object o)
            {
                ModelUtil.EnsureInstanceOf(o, typeof(IBpmnModelElementInstance));
                //return DomElement.removeChild(((IBpmnModelElementInstance)o).DomElement);
                return false;
            }

            public virtual bool AddAll<T1>(ICollection<T1> c) where T1 : IBpmnModelElementInstance
            {
                foreach (IBpmnModelElementInstance element in c)
                {
                    add(element);
                }
                return true;
            }

            public virtual bool RemoveAll<T1>(ICollection<T1> c)
            {
                bool result = false;
                foreach (object o in c)
                {
                    result |= remove(o);
                }
                return result;
            }

            public virtual bool RetainAll<T1>(ICollection<T1> c)
            {
                throw new UnsupportedModelOperationException("retainAll()", "not implemented");
            }

            public virtual void clear()
            {
                //IDomElement domElement = DomElement;
                //IList<IDomElement> childElements = domElement.ChildElements;
                //foreach (IDomElement childElement in childElements)
                //{
                //    domElement.removeChild(childElement);
                //}
            }

            public void Add(IBpmnModelElementInstance item)
            {
                throw new NotImplementedException();
            }

            public void Clear()
            {
                throw new NotImplementedException();
            }

            public bool Contains(IBpmnModelElementInstance item)
            {
                throw new NotImplementedException();
            }

            public void CopyTo(IBpmnModelElementInstance[] array, int arrayIndex)
            {
                throw new NotImplementedException();
            }

            public bool Remove(IBpmnModelElementInstance item)
            {
                throw new NotImplementedException();
            }

            public IEnumerator<IBpmnModelElementInstance> GetEnumerator()
            {
                throw new NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }

    }
}