using ESS.FW.Bpm.Model.Bpmn.builder;
using System.Collections.Generic;



namespace ESS.FW.Bpm.Model.Bpmn.instance
{

	/// <summary>
	/// The BPMN activity element
	/// 
	/// 
	/// </summary>
	public interface IActivity : IFlowNode, IInteractionNode
	{

	  bool ForCompensation {get;set;}


	  int StartQuantity {get;set;}


	  int CompletionQuantity {get;set;}


	  ISequenceFlow Default {get;set;}


	  IOSpecification IoSpecification {get;set;}


	  ICollection<IProperty> Properties {get;}

	  ICollection<IDataInputAssociation> DataInputAssociations {get;}

	  ICollection<IDataOutputAssociation> DataOutputAssociations {get;}

	  ICollection<IResourceRole> ResourceRoles {get;}

	  ILoopCharacteristics LoopCharacteristics {get;set;}

	}

}