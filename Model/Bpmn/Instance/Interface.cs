using System.Collections.Generic;



namespace ESS.FW.Bpm.Model.Bpmn.instance
{

	/// <summary>
	/// The BPMN interface element
	/// 
	/// 
	/// </summary>
	public interface INterface : IRootElement
	{

	  string Name {get;set;}


	  string ImplementationRef {get;set;}


	  ICollection<IOperation> Operations {get;}

	}

}