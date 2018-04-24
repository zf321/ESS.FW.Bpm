using ESS.FW.Bpm.Engine.Filter;
using ESS.FW.Bpm.Engine.Identity;
using ESS.FW.Bpm.Engine.Impl.Cfg.Auth;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Task;

namespace Engine.Tests.Api.Authorization
{
    

	/// <summary>
	/// 
	/// 
	/// </summary>
	public class MyResourceAuthorizationProvider : IResourceAuthorizationProvider
	{

	  // assignee
	  public static string OldAssignee;
	  public static string NewAssignee;

	  // owner
	  public static string OldOwner;
	  public static string NewOwner;

	  // add user identity link
	  public static string AddUserIdentityLinkType;
	  public static string AddUserIdentityLinkUser;

	  // Delete user identity link
	  public static string DeleteUserIdentityLinkType = null;
	  public static string DeleteUserIdentityLinkUser = null;

	  // add group identity link
	  public static string AddGroupIdentityLinkType;
	  public static string AddGroupIdentityLinkGroup;

	  // Delete group identity link
	  public static string DeleteGroupIdentityLinkType = null;
	  public static string DeleteGroupIdentityLinkGroup = null;

	  public virtual AuthorizationEntity[] NewUser(IUser user)
	  {
		return null;
	  }

	  public virtual AuthorizationEntity[] NewGroup(IGroup group)
	  {
		return null;
	  }

	  public virtual AuthorizationEntity[] NewTenant(ITenant tenant)
	  {
		return null;
	  }

	  public virtual AuthorizationEntity[] GroupMembershipCreated(string groupId, string userId)
	  {
		return null;
	  }

	  public virtual AuthorizationEntity[] TenantMembershipCreated(ITenant tenant, IUser user)
	  {
		return null;
	  }

	  public virtual AuthorizationEntity[] TenantMembershipCreated(ITenant tenant, IGroup group)
	  {
		return null;
	  }

	  public virtual AuthorizationEntity[] NewFilter(IFilter filter)
	  {
		return null;
	  }

	  public virtual AuthorizationEntity[] NewDeployment(IDeployment deployment)
	  {
		return null;
	  }

	  public virtual AuthorizationEntity[] NewProcessDefinition(IProcessDefinition processDefinition)
	  {
		return null;
	  }

	  public virtual AuthorizationEntity[] NewProcessInstance(IProcessInstance processInstance)
	  {
		return null;
	  }

	  public virtual AuthorizationEntity[] NewTask(ITask task)
	  {
		return null;
	  }

	  public virtual AuthorizationEntity[] NewTaskAssignee(ITask task, string oldAssignee, string newAssignee)
	  {
		OldAssignee = oldAssignee;
		NewAssignee = newAssignee;
		return null;
	  }

	  public virtual AuthorizationEntity[] NewTaskOwner(ITask task, string oldOwner, string newOwner)
	  {
		OldOwner = oldOwner;
		NewOwner = newOwner;
		return null;
	  }

	  public virtual AuthorizationEntity[] NewTaskUserIdentityLink(ITask task, string userId, string type)
	  {
		AddUserIdentityLinkType = type;
		AddUserIdentityLinkUser = userId;
		return null;
	  }

	  public virtual AuthorizationEntity[] NewTaskGroupIdentityLink(ITask task, string groupId, string type)
	  {
		AddGroupIdentityLinkType = type;
		AddGroupIdentityLinkGroup = groupId;
		return null;
	  }

	  public virtual AuthorizationEntity[] DeleteTaskUserIdentityLink(ITask task, string userId, string type)
	  {
		DeleteUserIdentityLinkType = type;
		DeleteUserIdentityLinkUser = userId;
		return null;
	  }

	  public virtual AuthorizationEntity[] DeleteTaskGroupIdentityLink(ITask task, string groupId, string type)
	  {
		DeleteGroupIdentityLinkType = type;
		DeleteGroupIdentityLinkGroup = groupId;
		return null;
	  }

	  public static void ClearProperties()
	  {
		OldAssignee = null;
		NewAssignee = null;
		OldOwner = null;
		NewOwner = null;
		AddUserIdentityLinkType = null;
		AddUserIdentityLinkUser = null;
		AddGroupIdentityLinkType = null;
		AddGroupIdentityLinkGroup = null;
		DeleteUserIdentityLinkType = null;
		DeleteUserIdentityLinkUser = null;
		DeleteGroupIdentityLinkType = null;
		DeleteGroupIdentityLinkGroup = null;
	  }

	  public virtual AuthorizationEntity[] NewDecisionDefinition(IDecisionDefinition decisionDefinition)
	  {
		return null;
	  }

	  public virtual AuthorizationEntity[] NewDecisionRequirementsDefinition(IDecisionRequirementsDefinition decisionRequirementsDefinition)
	  {
		return null;
	  }

	}

}