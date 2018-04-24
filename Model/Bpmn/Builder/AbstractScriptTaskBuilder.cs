using System;
using ESS.FW.Bpm.Model.Bpmn.instance;



namespace ESS.FW.Bpm.Model.Bpmn.builder
{
    /// <summary>
	/// 
	/// </summary>
	public abstract class AbstractScriptTaskBuilder : AbstractTaskBuilder<IScriptTask>, IScriptTaskBuilder
    {

	  protected internal AbstractScriptTaskBuilder(IBpmnModelInstance modelInstance, IScriptTask element) : base(modelInstance, element)
	  {
	  }

	  /// <summary>
	  /// Sets the script format of the build script task.
	  /// </summary>
	  /// <param name="scriptFormat">  the script format to set </param>
	  /// <returns> the builder object </returns>
	  public virtual IScriptTaskBuilder ScriptFormat(string scriptFormat)
	  {
		element.ScriptFormat = scriptFormat;
		return this;
	  }

	  /// <summary>
	  /// Sets the script of the build script task.
	  /// </summary>
	  /// <param name="script">  the script to set </param>
	  /// <returns> the builder object </returns>
	  public virtual IScriptTaskBuilder Script(IScript script)
	  {
		element.Script = script;
		return this;
	  }

	  public virtual IScriptTaskBuilder ScriptText(string scriptText)
	  {
		IScript script = CreateChild<IScript>(typeof(IScript));
		script.TextContent = scriptText;
		return this;
	  }

	  /// <summary>
	  /// camunda extensions </summary>

	  /// <summary>
	  /// Sets the camunda result variable of the build script task.
	  /// </summary>
	  /// <param name="camundaResultVariable">  the result variable to set </param>
	  /// <returns> the builder object </returns>
	  public virtual IScriptTaskBuilder CamundaResultVariable(string camundaResultVariable)
	  {
		element.CamundaResultVariable = camundaResultVariable;
		return this;
	  }

	  /// <summary>
	  /// Sets the camunda resource of the build script task.
	  /// </summary>
	  /// <param name="camundaResource">  the resource to set </param>
	  /// <returns> the builder object </returns>
	  public virtual IScriptTaskBuilder CamundaResource(string camundaResource)
	  {
		element.CamundaResource = camundaResource;
		return this;
	  }

	}

}