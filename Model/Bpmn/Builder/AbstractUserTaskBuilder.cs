using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Bpmn.instance.camunda;



namespace ESS.FW.Bpm.Model.Bpmn.builder
{
    /// <summary>
	/// 
	/// </summary>
	public abstract class AbstractUserTaskBuilder : AbstractTaskBuilder< IUserTask>
	{

	  protected internal AbstractUserTaskBuilder(IBpmnModelInstance modelInstance, IUserTask element) : base(modelInstance, element)
	  {
	  }

	  /// <summary>
	  /// Sets the implementation of the build user task.
	  /// </summary>
	  /// <param name="implementation">  the implementation to set </param>
	  /// <returns> the builder object </returns>
	  public virtual AbstractUserTaskBuilder Implementation(string implementation)
	  {
		element.Implementation = implementation;
		return this;
	  }

	  /// <summary>
	  /// camunda extensions </summary>

	  /// <summary>
	  /// Sets the camunda attribute assignee.
	  /// </summary>
	  /// <param name="camundaAssignee">  the assignee to set </param>
	  /// <returns> the builder object </returns>
	  public virtual AbstractUserTaskBuilder CamundaAssignee(string camundaAssignee)
	  {
		element.CamundaAssignee = camundaAssignee;
		return this;
	  }

	  /// <summary>
	  /// Sets the camunda candidate groups attribute.
	  /// </summary>
	  /// <param name="camundaCandidateGroups">  the candidate groups to set </param>
	  /// <returns> the builder object </returns>
	  public virtual AbstractUserTaskBuilder CamundaCandidateGroups(string camundaCandidateGroups)
	  {
		element.CamundaCandidateGroups = camundaCandidateGroups;
		return this;
	  }

	  /// <summary>
	  /// Sets the camunda candidate groups attribute.
	  /// </summary>
	  /// <param name="camundaCandidateGroups">  the candidate groups to set </param>
	  /// <returns> the builder object </returns>
	  public virtual AbstractUserTaskBuilder CamundaCandidateGroups(IList<string> camundaCandidateGroups)
	  {
		element.CamundaCandidateGroupsList = camundaCandidateGroups;
		return this;
	  }

	  /// <summary>
	  /// Sets the camunda candidate users attribute.
	  /// </summary>
	  /// <param name="camundaCandidateUsers">  the candidate users to set </param>
	  /// <returns> the builder object </returns>
	  public virtual AbstractUserTaskBuilder CamundaCandidateUsers(string camundaCandidateUsers)
	  {
		element.CamundaCandidateUsers = camundaCandidateUsers;
		return this;
	  }

	  /// <summary>
	  /// Sets the camunda candidate users attribute.
	  /// </summary>
	  /// <param name="camundaCandidateUsers">  the candidate users to set </param>
	  /// <returns> the builder object </returns>
	  public virtual AbstractUserTaskBuilder CamundaCandidateUsers(IList<string> camundaCandidateUsers)
	  {
		element.CamundaCandidateUsersList = camundaCandidateUsers;
		return this;
	  }

	  /// <summary>
	  /// Sets the camunda due date attribute.
	  /// </summary>
	  /// <param name="camundaDueDate">  the due date of the user task </param>
	  /// <returns> the builder object </returns>
	  public virtual AbstractUserTaskBuilder CamundaDueDate(string camundaDueDate)
	  {
		element.CamundaDueDate = camundaDueDate;
		return this;
	  }

	  /// <summary>
	  /// Sets the camunda follow up date attribute.
	  /// </summary>
	  /// <param name="camundaFollowUpDate">  the follow up date of the user task </param>
	  /// <returns> the builder object </returns>
	  public virtual AbstractUserTaskBuilder CamundaFollowUpDate(string camundaFollowUpDate)
	  {
		element.CamundaFollowUpDate = camundaFollowUpDate;
		return this;
	  }

	  /// <summary>
	  /// Sets the camunda form handler class attribute.
	  /// </summary>
	  /// <param name="camundaFormHandlerClass">  the class name of the form handler </param>
	  /// <returns> the builder object </returns>
	  public virtual AbstractUserTaskBuilder camundaFormHandlerClass(string camundaFormHandlerClass)
	  {
		element.CamundaFormHandlerClass = camundaFormHandlerClass;
		return this;
	  }

	  /// <summary>
	  /// Sets the camunda form key attribute.
	  /// </summary>
	  /// <param name="camundaFormKey">  the form key to set </param>
	  /// <returns> the builder object </returns>
	  public virtual AbstractUserTaskBuilder CamundaFormKey(string camundaFormKey)
	  {
		element.CamundaFormKey = camundaFormKey;
		return this;
	  }

	  /// <summary>
	  /// Sets the camunda priority attribute.
	  /// </summary>
	  /// <param name="camundaPriority">  the priority of the user task </param>
	  /// <returns> the builder object </returns>
	  public virtual AbstractUserTaskBuilder CamundaPriority(string camundaPriority)
	  {
		element.CamundaPriority = camundaPriority;
		return this;
	  }

	  /// <summary>
	  /// Creates a new camunda form field extension element.
	  /// </summary>
	  /// <returns> the builder object </returns>
	  public virtual CamundaUserTaskFormFieldBuilder camundaFormField()
	  {
		ICamundaFormData camundaFormData = GetCreateSingleExtensionElement<ICamundaFormData>(typeof(ICamundaFormData));
		ICamundaFormField camundaFormField = CreateChild<ICamundaFormField>(camundaFormData, typeof(ICamundaFormField));
		return new CamundaUserTaskFormFieldBuilder(modelInstance, element, camundaFormField);
	  }

	  /// <summary>
	  /// Add a class based task listener with specified event name
	  /// </summary>
	  /// <param name="eventName"> - event names to listen to </param>
	  /// <param name="fullQualifiedClassName"> - a string representing a class </param>
	  /// <returns> the builder object </returns>
	  public virtual AbstractUserTaskBuilder CamundaTaskListenerClass(string eventName, string fullQualifiedClassName)
	  {
		ICamundaTaskListener executionListener = CreateInstance<ICamundaTaskListener>(typeof(ICamundaTaskListener));
		executionListener.CamundaEvent = eventName;
		executionListener.CamundaClass = fullQualifiedClassName;

		AddExtensionElement(executionListener);

		return this;
	  }

	  public virtual AbstractUserTaskBuilder CamundaTaskListenerExpression(string eventName, string expression)
	  {
		ICamundaTaskListener executionListener = CreateInstance<ICamundaTaskListener>(typeof(ICamundaTaskListener));
		executionListener.CamundaEvent = eventName;
		executionListener.CamundaExpression = expression;

		AddExtensionElement(executionListener);

		return this;
	  }

	  public virtual AbstractUserTaskBuilder CamundaTaskListenerDelegateExpression(string eventName, string delegateExpression)
	  {
		ICamundaTaskListener executionListener = CreateInstance<ICamundaTaskListener>(typeof(ICamundaTaskListener));
		executionListener.CamundaEvent = eventName;
		executionListener.CamundaDelegateExpression = delegateExpression;

		AddExtensionElement(executionListener);

		return this;
	  }
	}

}