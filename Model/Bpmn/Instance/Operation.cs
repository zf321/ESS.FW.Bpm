using System.Collections.Generic;



namespace ESS.FW.Bpm.Model.Bpmn.instance
{

	/// <summary>
	/// The BPMN operation element
	/// 
	/// 
	/// </summary>
	public interface IOperation : IBaseElement
	{

	  string Name {get;set;}


	  string ImplementationRef {get;set;}


	  IMessage InMessage {get;set;}


	  IMessage OutMessage {get;set;}


	  ICollection<IError> Errors {get;}

	}

}