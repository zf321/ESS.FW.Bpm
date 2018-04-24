using System.Collections.Generic;



namespace ESS.FW.Bpm.Model.Bpmn.instance
{

	/// <summary>
	/// The BPMN extension element
	/// 
	/// 
	/// </summary>
	public interface IExtension : IBpmnModelElementInstance
	{

	  string Definition {get;set;}        

	  bool MustUnderstand { get;set;}

	  ICollection<IDocumentation> Documentations {get;}

	}

}