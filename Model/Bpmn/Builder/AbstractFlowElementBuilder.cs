using System;
using ESS.FW.Bpm.Model.Bpmn.instance;



namespace ESS.FW.Bpm.Model.Bpmn.builder
{
    /// <summary>
	/// 
	/// </summary>
	public abstract class AbstractFlowElementBuilder<TE> : AbstractBaseElementBuilder<TE>, IFlowElementBuilder<TE> where TE : IFlowElement
	{

	  protected internal AbstractFlowElementBuilder(IBpmnModelInstance modelInstance, TE element) : base(modelInstance, element)
	  {
	  }

	  /// <summary>
	  /// Sets the element name attribute.
	  /// </summary>
	  /// <param name="name">  the name to set </param>
	  /// <returns> the builder object </returns>
	  public virtual TOut Name<TOut>(string name) where TOut : IFlowElementBuilder<TE>
        {
		element.Name = name;
		return (TOut)(IFlowElementBuilder<TE>)this;
	  }
	}

}