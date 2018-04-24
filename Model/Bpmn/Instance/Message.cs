

namespace ESS.FW.Bpm.Model.Bpmn.instance
{

	/// <summary>
	/// The BPMN message element
	/// 
	/// 
	/// </summary>
	public interface IMessage : IRootElement
	{

	  string Name {get;set;}


	  IItemDefinition Item {get;set;}


	}

}