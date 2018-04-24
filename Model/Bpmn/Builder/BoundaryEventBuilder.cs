

using ESS.FW.Bpm.Model.Bpmn.instance;

namespace ESS.FW.Bpm.Model.Bpmn.builder
{
    /// <summary>
	/// 
	/// </summary>
	public class BoundaryEventBuilder : AbstractBoundaryEventBuilder/*<BoundaryEventBuilder>*/
	{

	  public BoundaryEventBuilder(IBpmnModelInstance modelInstance, IBoundaryEvent element) : base(modelInstance, element)
	  {
	  }

	}

}