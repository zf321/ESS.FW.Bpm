using System;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Bpmn.instance.camunda;



namespace ESS.FW.Bpm.Model.Bpmn.builder
{
    /// <summary>
	/// 
	/// </summary>
	public abstract class AbstractEventBuilder<TE> : AbstractFlowNodeBuilder<TE>, IEventBuilder<TE> where TE : IEvent
	{

	  protected internal AbstractEventBuilder(IBpmnModelInstance modelInstance, TE element) : base(modelInstance, element)
	  {
	  }

	  /// <summary>
	  /// Creates a new camunda input parameter extension element with the
	  /// given name and value.
	  /// </summary>
	  /// <param name="name"> the name of the input parameter </param>
	  /// <param name="value"> the value of the input parameter </param>
	  /// <returns> the builder object </returns>
	  public virtual IEventBuilder<TE> CamundaInputParameter(string name, string value)
	  {
		ICamundaInputOutput camundaInputOutput = GetCreateSingleExtensionElement< ICamundaInputOutput>(typeof(ICamundaInputOutput));

		ICamundaInputParameter camundaInputParameter = CreateChild<ICamundaInputParameter>(camundaInputOutput, typeof(ICamundaInputParameter));
		camundaInputParameter.CamundaName = name;
		camundaInputParameter.TextContent = value;

		return this;
	  }

	  /// <summary>
	  /// Creates a new camunda output parameter extension element with the
	  /// given name and value.
	  /// </summary>
	  /// <param name="name"> the name of the output parameter </param>
	  /// <param name="value"> the value of the output parameter </param>
	  /// <returns> the builder object </returns>
	  public virtual IEventBuilder<TE> CamundaOutputParameter(string name, string value)
	  {
		ICamundaInputOutput camundaInputOutput = GetCreateSingleExtensionElement< ICamundaInputOutput>(typeof(ICamundaInputOutput));

		ICamundaOutputParameter camundaOutputParameter = CreateChild<ICamundaOutputParameter>(camundaInputOutput, typeof(ICamundaOutputParameter));
		camundaOutputParameter.CamundaName = name;
		camundaOutputParameter.TextContent = value;

		return this;
	  }

	}

}