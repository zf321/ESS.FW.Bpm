

using ESS.FW.Bpm.Model.Bpmn;

namespace Engine.Tests.Util
{

	/// <summary>
	///
	/// </summary>
	public interface IBpmnEventTrigger
	{
	  void Trigger(ProcessEngineTestRule rule);
	  IBpmnModelInstance ProcessModel {get;}
	}

}