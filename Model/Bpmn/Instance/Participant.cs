using System.Collections.Generic;



namespace ESS.FW.Bpm.Model.Bpmn.instance
{

	/// <summary>
	/// The BPMN participant element
	/// 
	/// 
	/// </summary>
	public interface IParticipant : IBaseElement, IInteractionNode
	{

	  string Name {get;set;}


	  IProcess Process {get;set;}


	  ICollection<INterface> Interfaces {get;}

	  ICollection<IEndPoint> EndPoints {get;}

	  IParticipantMultiplicity ParticipantMultiplicity {get;set;}


	}

}