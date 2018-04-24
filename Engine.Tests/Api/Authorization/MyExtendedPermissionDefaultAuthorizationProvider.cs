using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Task;

namespace Engine.Tests.Api.Authorization
{

    // Todo: DefaultAuthorizationProvider
    public class MyExtendedPermissionDefaultAuthorizationProvider //: DefaultAuthorizationProvider
	{

	  public virtual AuthorizationEntity[] newTaskAssignee(ITask task, string oldAssignee, string newAssignee)
	  {
		//AuthorizationEntity[] authorizations = base.NewTaskAssignee(task, oldAssignee, newAssignee);
		//authorizations[0].AddPermission(Permissions.DeleteHistory);
		//return authorizations;
	      return null;
	  }
	}

}