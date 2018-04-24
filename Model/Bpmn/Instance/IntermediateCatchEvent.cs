

using ESS.FW.Bpm.Model.Bpmn.builder;

namespace ESS.FW.Bpm.Model.Bpmn.instance
{

	using IntermediateCatchEventBuilder = IntermediateCatchEventBuilder;

	/// <summary>
	/// The BPMN intermediateCatchEvent element
	/// 
	/// 
	/// </summary>
	public interface INtermediateCatchEvent : ICatchEvent
	{

	  IntermediateCatchEventBuilder Builder();

	}

}