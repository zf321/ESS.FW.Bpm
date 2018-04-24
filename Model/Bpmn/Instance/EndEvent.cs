

using ESS.FW.Bpm.Model.Bpmn.builder;

namespace ESS.FW.Bpm.Model.Bpmn.instance
{

	using EndEventBuilder = EndEventBuilder;

	/// <summary>
	/// The BPMN endEvent element
	/// 
	/// 
	/// </summary>
	public interface IEndEvent : IThrowEvent
	{

	  EndEventBuilder Builder();

	}

}