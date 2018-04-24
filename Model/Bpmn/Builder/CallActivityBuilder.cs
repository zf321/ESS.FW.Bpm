

using ESS.FW.Bpm.Model.Bpmn.instance;

namespace ESS.FW.Bpm.Model.Bpmn.builder
{
    /// <summary>
	/// 
	/// </summary>
	public class CallActivityBuilder : AbstractCallActivityBuilder/*<CallActivityBuilder>*/
	{

	  public CallActivityBuilder(IBpmnModelInstance modelInstance, ICallActivity element) : base(modelInstance, element)
	  {
	  }
	}

}