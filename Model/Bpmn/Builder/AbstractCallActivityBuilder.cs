using System;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Bpmn.instance.camunda;



namespace ESS.FW.Bpm.Model.Bpmn.builder
{
    /// <summary>
	/// 
	/// </summary>
	public class AbstractCallActivityBuilder : AbstractActivityBuilder<ICallActivity>, ICallActivityBuilder
	{

	  protected internal AbstractCallActivityBuilder(IBpmnModelInstance modelInstance, ICallActivity element) : base(modelInstance, element)
	  {
	  }

	  /// <summary>
	  /// Sets the called element
	  /// </summary>
	  /// <param name="calledElement">  the process to call </param>
	  /// <returns> the builder object </returns>
	  public virtual ICallActivityBuilder CalledElement(string calledElement)
	  {
		element.CalledElement = calledElement;
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
	  public virtual ICallActivityBuilder CamundaAsync()
	  {
		element.CamundaAsyncBefore = true;
		return this;
	  }

	  /// @deprecated use camundaAsyncBefore(isCamundaAsyncBefore) instead
	  /// 
	  /// Sets the camunda async attribute.
	  /// 
	  /// <param name="isCamundaAsync">  the async state of the task </param>
	  /// <returns> the builder object </returns>
	  [Obsolete("use camundaAsyncBefore(isCamundaAsyncBefore) instead")]
	  public virtual ICallActivityBuilder CamundaAsync(bool isCamundaAsync)
	  {
		element.CamundaAsyncBefore = isCamundaAsync;
		return this;
	  }

	  /// <summary>
	  /// Sets the camunda calledElementBinding attribute
	  /// </summary>
	  /// <param name="camundaCalledElementBinding">  the element binding to use </param>
	  /// <returns> the builder object </returns>
	  public virtual ICallActivityBuilder CamundaCalledElementBinding(string camundaCalledElementBinding)
	  {
		element.CamundaCalledElementBinding = camundaCalledElementBinding;
		return this;
	  }

	  /// <summary>
	  /// Sets the camunda calledElementVersion attribute
	  /// </summary>
	  /// <param name="camundaCalledElementVersion">  the element version to use </param>
	  /// <returns> the builder object </returns>
	  public virtual ICallActivityBuilder CamundaCalledElementVersion(string camundaCalledElementVersion)
	  {
		element.CamundaCalledElementVersion = camundaCalledElementVersion;
		return this;
	  }

	  /// <summary>
	  /// Sets the camunda calledElementTenantId attribute </summary>
	  /// <param name="camundaCalledElementTenantId"> the called element tenant id </param>
	  /// <returns> the builder object </returns>
	  public virtual ICallActivityBuilder CamundaCalledElementTenantId(string camundaCalledElementTenantId)
	  {
		element.CamundaCalledElementTenantId = camundaCalledElementTenantId;
		return this;
	  }

	  /// <summary>
	  /// Sets the camunda caseRef attribute
	  /// </summary>
	  /// <param name="caseRef"> the case to call </param>
	  /// <returns> the builder object </returns>
	  public virtual ICallActivityBuilder CamundaCaseRef(string caseRef)
	  {
		element.CamundaCaseRef = caseRef;
		return this;
	  }

	  /// <summary>
	  /// Sets the camunda caseBinding attribute
	  /// </summary>
	  /// <param name="camundaCaseBinding">  the case binding to use </param>
	  /// <returns> the builder object </returns>
	  public virtual ICallActivityBuilder CamundaCaseBinding(string camundaCaseBinding)
	  {
		element.CamundaCaseBinding = camundaCaseBinding;
		return this;
	  }

	  /// <summary>
	  /// Sets the camunda caseVersion attribute
	  /// </summary>
	  /// <param name="camundaCaseVersion">  the case version to use </param>
	  /// <returns> the builder object </returns>
	  public virtual ICallActivityBuilder CamundaCaseVersion(string camundaCaseVersion)
	  {
		element.CamundaCaseVersion = camundaCaseVersion;
		return this;
	  }

	  /// <summary>
	  /// Sets the caseTenantId </summary>
	  /// <param name="tenantId"> the tenant id to set </param>
	  /// <returns> the builder object </returns>
	  public virtual ICallActivityBuilder CamundaCaseTenantId(string tenantId)
	  {
		element.CamundaCaseTenantId = tenantId;
		return this;
	  }

	  /// <summary>
	  /// Sets a "camunda in" parameter to pass a variable from the super process instance to the sub process instance
	  /// </summary>
	  /// <param name="source"> the name of variable in the super process instance </param>
	  /// <param name="target"> the name of the variable in the sub process instance </param>
	  /// <returns> the builder object </returns>
	  public virtual ICallActivityBuilder CamundaIn(string source, string target)
	  {
		ICamundaIn param = modelInstance.NewInstance<ICamundaIn>(typeof(ICamundaIn));
		param.CamundaSource = source;
		param.CamundaTarget = target;
		AddExtensionElement(param);
		return this;
	  }

	  /// <summary>
	  /// Sets a "camunda out" parameter to pass a variable from a sub process instance to the super process instance
	  /// </summary>
	  /// <param name="source"> the name of variable in the sub process instance </param>
	  /// <param name="target"> the name of the variable in the super process instance </param>
	  /// <returns> the builder object </returns>
	  public virtual ICallActivityBuilder CamundaOut(string source, string target)
	  {
		ICamundaOut param = modelInstance.NewInstance<ICamundaOut>(typeof(ICamundaOut));
		param.CamundaSource = source;
		param.CamundaTarget = target;
		AddExtensionElement(param);
		return this;
	  }


	  /// <summary>
	  /// Sets the camunda variableMappingClass attribute. It references on a class which implements the
	  /// <seealso cref="DelegateVariableMapping"/> interface.
	  /// Is used to delegate the variable in- and output mapping to the given class.
	  /// </summary>
	  /// <param name="camundaVariableMappingClass">                  the class name to set </param>
	  /// <returns>                              the builder object </returns>
	  public virtual ICallActivityBuilder CamundaVariableMappingClass(string camundaVariableMappingClass)
	  {
		element.CamundaVariableMappingClass = camundaVariableMappingClass;
		return this;
	  }

	  /// <summary>
	  /// Sets the camunda variableMappingDelegateExpression attribute. The expression when is resolved
	  /// references to an object of a class, which implements the <seealso cref="DelegateVariableMapping"/> interface.
	  /// Is used to delegate the variable in- and output mapping to the given class.
	  /// </summary>
	  /// <param name="camundaVariableMappingDelegateExpression">     the expression which references a delegate object </param>
	  /// <returns>                              the builder object </returns>
	  public virtual ICallActivityBuilder CamundaVariableMappingDelegateExpression(string camundaVariableMappingDelegateExpression)
	  {
		element.CamundaVariableMappingDelegateExpression = camundaVariableMappingDelegateExpression;
		return this;
	  }
	}

}