using System.Collections.Generic;
using System.Text;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Authorization;

namespace Engine.Tests.Api.Authorization.Util
{
    

	/// <summary>
	/// 
	/// 
	/// </summary>
	public class AuthorizationSpec
	{
	  protected internal int type;
	  protected internal Resources resource;
	  protected internal string resourceId;
	  protected internal string userId;
	  protected internal Permissions[] permissions;

	  public static AuthorizationSpec Auth(int type, Resources resource, string resourceId, string userId, params Permissions[] permissions)
	  {
		AuthorizationSpec spec = new AuthorizationSpec();
		spec.type = type;
		spec.resource = resource;
		spec.resourceId = resourceId;
		spec.userId = userId;
		spec.permissions = permissions;
		return spec;
	  }

	  public static AuthorizationSpec Global(Resources resource, string resourceId, string userId, params Permissions[] permissions)
	  {
		return Auth(AuthorizationFields.AuthTypeGlobal, resource, resourceId, userId, permissions);
	  }

	  public static AuthorizationSpec Grant(Resources resource, string resourceId, string userId, params Permissions[] permissions)
	  {
		return Auth(AuthorizationFields.AuthTypeGrant, resource, resourceId, userId, permissions);
	  }

	  public static AuthorizationSpec Revoke(Resources resource, string resourceId, string userId, params Permissions[] permissions)
	  {
		return Auth(AuthorizationFields.AuthTypeRevoke, resource, resourceId, userId, permissions);
	  }

	  public virtual IAuthorization Instantiate(IAuthorizationService authorizationService, IDictionary<string, string> replacements)
	  {
		IAuthorization authorization = authorizationService.CreateNewAuthorization(type);

		// TODO: group id is missing
		authorization.Resource = resource;

		if (replacements.ContainsKey(resourceId))
		{
		  authorization.ResourceId = replacements[resourceId];
		}
		else
		{
		  authorization.ResourceId = resourceId;
		}
		authorization.UserId = userId;
		//authorization.SetPermissions( permissions);

		return authorization;
	  }

	  public override string ToString()
	  {
		StringBuilder sb = new StringBuilder();
		sb.Append("[Resource: ");
		sb.Append(resource);
		sb.Append(", Resource Id: ");
		sb.Append(resourceId);
		sb.Append(", Type: ");
		sb.Append(type);
		sb.Append(", IUser Id: ");
		sb.Append(userId);
		sb.Append(", Permissions: [");

		foreach (Permissions permission in permissions)
		{
		  sb.Append(permission.ToString());
		  sb.Append(", ");
		}

		sb.Append("]]");

		return sb.ToString();

	  }
	}

}