using System.Collections.Generic;
using ESS.FW.Bpm.Model.Bpmn.builder;



namespace ESS.FW.Bpm.Model.Bpmn.instance
{


	using UserTaskBuilder = UserTaskBuilder;

	/// <summary>
	/// The BPMN userTask element
	/// 
	/// 
	/// </summary>
	public interface IUserTask : ITask
	{

	  UserTaskBuilder Builder();

	  string Implementation {get;set;}


	  ICollection<IRendering> Renderings {get;}

	  /// <summary>
	  /// camunda extensions </summary>

	  string CamundaAssignee {get;set;}


	  string CamundaCandidateGroups {get;set;}


	  IList<string> CamundaCandidateGroupsList {get;set;}


	  string CamundaCandidateUsers {get;set;}


	  IList<string> CamundaCandidateUsersList {get;set;}


	  string CamundaDueDate {get;set;}


	  string CamundaFollowUpDate {get;set;}


	  string CamundaFormHandlerClass {get;set;}


	  string CamundaFormKey {get;set;}


	  string CamundaPriority {get;set;}

	}

}