

using ESS.FW.Bpm.Model.Bpmn.builder;

namespace ESS.FW.Bpm.Model.Bpmn.instance
{

	using ScriptTaskBuilder = ScriptTaskBuilder;

	/// <summary>
	/// The BPMN scriptTask element
	/// 
	/// 
	/// </summary>
	public interface IScriptTask : ITask
	{

	  ScriptTaskBuilder Builder();

	  string ScriptFormat {get;set;}


	  IScript Script {get;set;}


	  /// <summary>
	  /// camunda extensions </summary>

	  string CamundaResultVariable {get;set;}


	  string CamundaResource {get;set;}


	}

}