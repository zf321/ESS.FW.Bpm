

using ESS.FW.Bpm.Model.Bpmn.instance;

namespace ESS.FW.Bpm.Model.Bpmn.builder
{
    /// <summary>
	/// 
	/// </summary>
	public class SubProcessBuilder : AbstractSubProcessBuilder//<SubProcessBuilder>
	{

	  public SubProcessBuilder(IBpmnModelInstance modelInstance, ISubProcess element)
            : base(modelInstance, element)
        {
	  }
	}

}