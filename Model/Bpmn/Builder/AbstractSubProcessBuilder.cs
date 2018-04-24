using System;
using ESS.FW.Bpm.Model.Bpmn.instance;



namespace ESS.FW.Bpm.Model.Bpmn.builder
{
    /// <summary>
	/// 
	/// </summary>
	public class AbstractSubProcessBuilder : AbstractActivityBuilder<ISubProcess>
	{

	  protected internal AbstractSubProcessBuilder(IBpmnModelInstance modelInstance, ISubProcess element) : base(modelInstance, element)
	  {
	  }

        public virtual EmbeddedSubProcessBuilder EmbeddedSubProcess()
        {
            return new EmbeddedSubProcessBuilder(this);
        }

        /// <summary>
        /// Sets the sub process to be triggered by an event.
        /// </summary>
        /// <returns>  the builder object </returns>
        public virtual AbstractSubProcessBuilder TriggerByEvent()
	  {
		element.TriggeredByEvent = true;
		return this;
	  }

	  /// <summary>
	  /// camunda extensions </summary>

	  /// @deprecated use camundaAsyncBefore() instead.
	  /// 
	  /// Sets the camunda async attribute to true.
	  /// 
	  /// <returns> the builder object </returns>
	  [Obsolete("use camundaAsyncBefore() instead.")]
	  public virtual AbstractSubProcessBuilder CamundaAsync()
	  {
		element.CamundaAsyncBefore = true;
		return this;
	  }

	  /// @deprecated use camundaAsyncBefore(isCamundaAsyncBefore) instead.
	  /// 
	  /// Sets the camunda async attribute.
	  /// 
	  /// <param name="isCamundaAsync">  the async state of the task </param>
	  /// <returns> the builder object </returns>
	  [Obsolete("use camundaAsyncBefore(isCamundaAsyncBefore) instead.")]
	  public virtual AbstractSubProcessBuilder CamundaAsync(bool isCamundaAsync)
	  {
		element.CamundaAsyncBefore = isCamundaAsync;
		return this;
	  }

	}

}