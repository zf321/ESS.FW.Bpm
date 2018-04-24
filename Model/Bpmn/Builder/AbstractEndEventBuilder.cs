using System;
using ESS.FW.Bpm.Model.Bpmn.instance;



namespace ESS.FW.Bpm.Model.Bpmn.builder
{
    /// <summary>
	/// 
	/// </summary>
	public abstract class AbstractEndEventBuilder : AbstractThrowEventBuilder<IEndEvent>, IEndEventBuilder
    {

	  protected internal AbstractEndEventBuilder(IBpmnModelInstance modelInstance, IEndEvent element) : base(modelInstance, element)
	  {
	  }

	  /// <summary>
	  /// Sets an error definition for the given error code. If already an error
	  /// with this code exists it will be used, otherwise a new error is created.
	  /// </summary>
	  /// <param name="errorCode"> the code of the error </param>
	  /// <returns> the builder object </returns>
	  public virtual IEndEventBuilder Error(string errorCode)
	  {
		IErrorEventDefinition errorEventDefinition = CreateErrorEventDefinition(errorCode);
		element.EventDefinitions.Add(errorEventDefinition);

		return this;
	  }
	}

}