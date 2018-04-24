

using ESS.FW.Bpm.Model.Bpmn.instance;

namespace ESS.FW.Bpm.Model.Bpmn.builder
{
    /// <summary>
	/// 
	/// </summary>
	public class SequenceFlowBuilder : AbstractSequenceFlowBuilder/*<SequenceFlowBuilder>*/
	{

	  public SequenceFlowBuilder(IBpmnModelInstance modelInstance, ISequenceFlow element) : base(modelInstance, element)
	  {
	  }
	}

}