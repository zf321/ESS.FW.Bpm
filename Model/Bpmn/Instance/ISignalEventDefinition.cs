

namespace ESS.FW.Bpm.Model.Bpmn.instance
{

	/// <summary>
	/// The BPMN signalEventDefinition element
	/// 
	/// 
	/// </summary>
	public interface ISignalEventDefinition : IEventDefinition
	{

	  ISignal Signal {get;set;}


	  bool CamundaAsync {get;set;}


	}

}