using System.Collections.Generic;


namespace ESS.FW.Bpm.Model.Bpmn.instance
{

	/// <summary>
	/// The BPMN catchEvent element
	/// 
	/// 
	/// 
	/// </summary>
	public interface ICatchEvent : IEvent
	{

	  bool ParallelMultiple {get;set;}


	  ICollection<IDataOutput> DataOutputs {get;}

	  ICollection<IDataOutputAssociation> DataOutputAssociations {get;}

	  IOutputSet OutputSet {get;set;}


	  ICollection<IEventDefinition> EventDefinitions {get;}

	  ICollection<IEventDefinition> EventDefinitionRefs {get;}

	}

}