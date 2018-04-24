//using System.Collections.Generic;
//using ESS.FW.Bpm.Engine.Authorization;
//using ESS.FW.Bpm.Engine.Impl.Digest._apacheCommonsCodec;

//namespace ESS.FW.Bpm.Engine.Tests.Api.Authorization.Util
//{

//	/// <summary>
//	/// @author Filip Hrisafov
//	/// </summary>
//	public class MissingAuthorizationMatcher : TypeSafeDiagnosingMatcher<MissingAuthorization>
//	{

//	  private MissingAuthorization missing;

//	  private MissingAuthorizationMatcher(MissingAuthorization authorization)
//	  {
//		this.Missing = authorization;
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: There is no .NET equivalent to the Java 'super' constraint:
////ORIGINAL LINE: public static java.util.Collection<org.hamcrest.Matcher<? super org.camunda.bpm.Engine.authorization.MissingAuthorization>> asMatchers(java.util.List<org.camunda.bpm.Engine.authorization.MissingAuthorization> missingAuthorizations)
//	  public static ICollection<Matcher> asMatchers(IList<MissingAuthorization> missingAuthorizations)
//	  {
////JAVA TO C# CONVERTER TODO Resources.Task: There is no .NET equivalent to the Java 'super' constraint:
////ORIGINAL LINE: java.util.Collection<org.hamcrest.Matcher<? super org.camunda.bpm.Engine.authorization.MissingAuthorization>> matchers = new java.util.ArrayList<org.hamcrest.Matcher<? super org.camunda.bpm.Engine.authorization.MissingAuthorization>>(missingAuthorizations.Count());
////JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//		ICollection<Matcher> matchers = new List<Matcher>(missingAuthorizations.Count);
//		foreach (MissingAuthorization authorization in missingAuthorizations)
//		{
//		  matchers.Add(new MissingAuthorizationMatcher(authorization));
//		}
//		return matchers;
//	  }

//	  protected internal static MissingAuthorization asMissingAuthorization(IAuthorization authorization)
//	  {
//		string permissionName = null;
//		string resourceId = null;
//		string resourceName = null;

//		foreach (Permission permission in authorization.GetPermissions(Permissions.values()))
//		{
//		  if (permission != Permissions.NONE)
//		  {
//			permissionName = permission.Name;
//			break;
//		  }
//		}

//		if (!AuthorizationFields.Any.Equals(authorization.ResourceId))
//		{
//		  // missing AuthorizationFields.Any authorizations are not explicitly represented in the error message
//		  resourceId = authorization.ResourceId;
//		}

//		Resource resource = AuthorizationTestUtil.GetResourceByType(authorization.ResourceType);
//		resourceName = resource.ToString()/*.ResourceName()*/;
//		return new MissingAuthorization(permissionName, resourceName, resourceId);
//	  }

//	  public static IList<MissingAuthorization> asMissingAuthorizations(IList<Authorization> authorizations)
//	  {
//		IList<MissingAuthorization> missingAuthorizations = new List<MissingAuthorization>();
//		foreach (IAuthorization authorization in authorizations)
//		{
//		  missingAuthorizations.Add(asMissingAuthorization(authorization));
//		}
//		return missingAuthorizations;
//	  }

//	  protected internal override bool matchesSafely(MissingAuthorization item, Description mismatchDescription)
//	  {
//		if (StringUtils.Equals(missing.ResourceId, item.ResourceId) && StringUtils.Equals(missing.ResourceType, item.ResourceType) && StringUtils.Equals(missing.ViolatedPermissionName, item.ViolatedPermissionName))
//		{
//		  return true;
//		}
//		mismatchDescription.appendText("expected missing authorization: ").appendValue(missing).appendValue(" received: ").appendValue(item);
//		return false;
//	  }

//	  public override void describeTo(Description description)
//	  {
//		description.appendValue(missing);
//	  }
//	}

//}