using System;
using ESS.FW.Bpm.Model.Bpmn.instance;



namespace ESS.FW.Bpm.Model.Bpmn.builder
{
    /// <summary>
	/// 
	/// </summary>
	public abstract class AbstractCallableElementBuilder<TE> : AbstractRootElementBuilder<TE>, ICallableElementBuilder<TE> where TE : ICallableElement
	{

	  protected internal AbstractCallableElementBuilder(IBpmnModelInstance modelInstance, TE element) : base(modelInstance, element)
	  {
	  }

	  /// <summary>
	  /// Sets the element name attribute.
	  /// </summary>
	  /// <param name="name">  the name to set </param>
	  /// <returns> the builder object </returns>
	  public virtual ICallableElementBuilder<TE> Name(string name)
	  {
		element.Name = name;
		return this;
	  }
	}

}