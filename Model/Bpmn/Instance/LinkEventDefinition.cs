using System.Collections.Generic;



namespace ESS.FW.Bpm.Model.Bpmn.instance
{

	/// <summary>
	/// The BPMN linkEventDefinition element
	/// 
	/// 
	/// </summary>
	public interface ILinkEventDefinition : IEventDefinition
	{

	  string Name {get;set;}


	  ICollection<ILinkEventDefinition> Sources {get;}

	  ILinkEventDefinition Target {get;set;}


	}

}