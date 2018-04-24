

using ESS.FW.Bpm.Model.Bpmn.instance;

namespace ESS.FW.Bpm.Model.Bpmn.builder
{
    /// 
	/// <summary>
	/// @author Deivarayan Azhagappan
	/// </summary>
	public class ErrorEventDefinitionBuilder : AbstractErrorEventDefinitionBuilder/*<ErrorEventDefinitionBuilder>*/
	{

	  public ErrorEventDefinitionBuilder(IBpmnModelInstance modelInstance, IErrorEventDefinition element) : base(modelInstance, element)
	  {
	  }
	}

}