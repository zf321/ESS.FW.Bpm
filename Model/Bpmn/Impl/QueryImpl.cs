using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Model.Xml.instance;
using ESS.FW.Bpm.Model.Xml.type;



namespace ESS.FW.Bpm.Model.Bpmn.impl
{
    /// <summary>
	/// 
	/// </summary>
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

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <V extends org.camunda.bpm.model.xml.instance.ModelElementInstance> org.camunda.bpm.model.bpmn.Query<V> filterByType(org.camunda.bpm.model.xml.type.ModelElementType elementType)
	  public virtual IQuery<TV> FilterByType<TV>(IModelElementType elementType) where TV : IModelElementInstance
	  {
		Type elementClass = (Type) elementType.InstanceType;
		return FilterByType<TV>(elementClass);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <V extends org.camunda.bpm.model.xml.instance.ModelElementInstance> org.camunda.bpm.model.bpmn.Query<V> filterByType(Class<V> elementClass)
	  public virtual IQuery<T> FilterByType<T>(Type elementClass) where T : IModelElementInstance
        {
		IList<T> filtered = new List<T>();
		//foreach (T instance in collection)
		//{
		//  if (elementClass.IsAssignableFrom(instance.GetType()))
		//  {
		//	filtered.Add(instance);
		//  }
		//}
		return new QueryImpl<T>(filtered);
	  }

	  public virtual T SingleResult()
	  {
		if (_collection.Count == 1)
		{
		  return _collection.GetEnumerator().Current;
		}
		else
		{
		  throw new BpmnModelException("Collection expected to have <1> entry but has <" + _collection.Count + ">");
		}
	  }
	}

}