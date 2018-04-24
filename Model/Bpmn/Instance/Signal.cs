

namespace ESS.FW.Bpm.Model.Bpmn.instance
{

	/// <summary>
	/// The BPMN signal element
	/// 
	/// 
	/// </summary>
	public interface ISignal : IRootElement
	{

	  string Name {get;set;}


	  IItemDefinition Structure {get;set;}


	}

}