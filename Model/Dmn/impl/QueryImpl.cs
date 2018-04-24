using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Model.Xml.instance;
using ESS.FW.Bpm.Model.Xml.type;



namespace ESS.FW.Bpm.Model.Dmn.impl
{
    public class QueryImpl<T> : IQuery<T> where T : IModelElementInstance
    {

        private readonly ICollection<T> _collection;

        public QueryImpl(ICollection<T> collection)
        {
            this._collection = collection;
        }

        public virtual IList<T> List()
        {
            return new List<T>(_collection);
        }

        public virtual int Count()
        {
            return _collection.Count;
        }

        public virtual T SingleResult()
        {
            if (_collection.Count == 1)
            {
                return _collection.GetEnumerator().Current;
            }
            else
            {
                throw new DmnModelException("Collection expected to have <1> entry but has <" + _collection.Count + ">");
            }
        }

        public IQuery<T> FilterByType<T>(IModelElementType elementClass) where T : IModelElementInstance
        {
            IList<T> filtered = new List<T>();
            foreach (var instance in _collection)
            {
                if (elementClass.InstanceType == instance.GetType())
                {
                    //filtered.Add((T)instance);
                }
            }
            return new QueryImpl<T>(filtered);
        }

        public IQuery<TV> FilterByType<TV>(Type elementClass) where TV : IModelElementInstance
        {
            throw new NotImplementedException();
            //return filterByType(elementClass);
        }
    }
}