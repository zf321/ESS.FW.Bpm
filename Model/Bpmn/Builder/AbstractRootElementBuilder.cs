using System;
using ESS.FW.Bpm.Model.Bpmn.instance;



namespace ESS.FW.Bpm.Model.Bpmn.builder
{
    /// <summary>
	/// 
	/// </summary>
	public abstract class AbstractRootElementBuilder<TE> : AbstractBaseElementBuilder<TE>  where TE : IRootElement
	{

	  protected internal AbstractRootElementBuilder(IBpmnModelInstance modelInstance, TE element) : base(modelInstance,element)
	  {
	  }
	}

}