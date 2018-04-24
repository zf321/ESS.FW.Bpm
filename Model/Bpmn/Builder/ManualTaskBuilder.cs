

using ESS.FW.Bpm.Model.Bpmn.instance;

namespace ESS.FW.Bpm.Model.Bpmn.builder
{
    /// <summary>
	/// 
	/// </summary>
	public class ManualTaskBuilder : AbstractManualTaskBuilder/*<ManualTaskBuilder>*/
	{

	  public ManualTaskBuilder(IBpmnModelInstance modelInstance, IManualTask element) : base(modelInstance, element)
	  {
	  }

	}

}