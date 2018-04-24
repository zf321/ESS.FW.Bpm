

using ESS.FW.Bpm.Model.Bpmn.builder;

namespace ESS.FW.Bpm.Model.Bpmn.instance
{

	using ManualTaskBuilder = ManualTaskBuilder;

	/// <summary>
	/// The BPMN manualTask element
	/// 
	/// 
	/// </summary>
	public interface IManualTask : ITask
	{

	  ManualTaskBuilder Builder();

	}

}