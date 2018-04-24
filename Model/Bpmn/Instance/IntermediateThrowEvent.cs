

using ESS.FW.Bpm.Model.Bpmn.builder;

namespace ESS.FW.Bpm.Model.Bpmn.instance
{

	using IntermediateThrowEventBuilder = IntermediateThrowEventBuilder;

	/// <summary>
	/// The BPMN intermediateThrowEvent element
	/// 
	/// 
	/// </summary>
	public interface INtermediateThrowEvent : IThrowEvent
	{

	  IntermediateThrowEventBuilder Builder();
	}

}