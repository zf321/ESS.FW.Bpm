using System;
using System.Collections;
using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.Impl.Cfg
{
    /// <summary>
    ///      
    /// </summary>
    public class SpringBeanFactoryProxyMap : IDictionary<object, object>
    {
        //protected internal BeanFactory beanFactory;

        //public SpringBeanFactoryProxyMap(BeanFactory beanFactory)
        //{
        //    this.beanFactory = beanFactory;
        //}

        public virtual bool Empty
        {
            get { throw new ProcessEngineException("unsupported operation on configuration beans"); }
        }

        //public virtual bool ContainsKey(object key)
        //{
        //    if ((key == null) || (!key.GetType().IsSubclassOf(typeof (string))))
        //    {
        //        return false;
        //    }
        //    return beanFactory.containsBean((string) key);
        //}

        public virtual void Clear()
        {
            throw new ProcessEngineException("can't clear configuration beans");
        }

        public virtual int Count
        {
            get { throw new ProcessEngineException("unsupported operation on configuration beans"); }
        }

        public virtual ICollection<object> Values
        {
            get { throw new ProcessEngineException("unsupported operation on configuration beans"); }
        }

        public ICollection<object> Keys
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        public object this[object key]
        {
            get { throw new NotImplementedException(); }

            set { throw new NotImplementedException(); }
        }

        public void Add(object key, object value)
        {
            throw new NotImplementedException();
        }

        public bool Remove(object key)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(object key, out object value)
        {
            throw new NotImplementedException();
        }

        public void Add(KeyValuePair<object, object> item)
        {
            throw new NotImplementedException();
        }

        public bool Contains(KeyValuePair<object, object> item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(KeyValuePair<object, object>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<object, object> item)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<object, object>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public bool ContainsKey(object key)
        {
            throw new NotImplementedException();
        }

        //public virtual object get(object key)
        //{
        //    if ((key == null) || (!key.GetType().IsSubclassOf(typeof (string))))
        //    {
        //        return null;
        //    }
        //    return beanFactory.getBean((string) key);
        //}

        public virtual ISet<object> KeySet()
        {
            return null;
        }

        public virtual bool ContainsValue(object value)
        {
            throw new ProcessEngineException("can't search values in configuration beans");
        }

        public virtual ISet<KeyValuePair<object, object>> EntrySet()
        {
            throw new ProcessEngineException("unsupported operation on configuration beans");
        }

        public virtual object Put(object key, object value)
        {
            throw new ProcessEngineException("unsupported operation on configuration beans");
        }

        public virtual object remove(object key)
        {
            throw new ProcessEngineException("unsupported operation on configuration beans");
        }
    }
}