using System.Collections.Generic;



namespace ESS.FW.Bpm.Model.Bpmn.instance
{

	/// <summary>
	/// The BPMN resource element
	/// 
	/// 
	/// </summary>
	public interface Resources : IRootElement
	{

	  string Name {get;set;}


	  ICollection<IResourceParameter> ResourceParameters {get;}
	}

}