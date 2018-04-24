using System.Collections.Generic;


namespace ESS.FW.Bpm.Model.Bpmn.instance
{

	/// <summary>
	/// The BPMN throwEvent element
	/// 
	/// 
	/// 
	/// </summary>
	public interface IThrowEvent : IEvent
	{

	  ICollection<IDataInput> DataInputs {get;}

	  ICollection<IDataInputAssociation> DataInputAssociations {get;}

	  INputSet InputSet {get;set;}


	  ICollection<IEventDefinition> EventDefinitions {get;}

	  ICollection<IEventDefinition> EventDefinitionRefs {get;}

	}

}