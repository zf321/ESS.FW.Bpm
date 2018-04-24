
namespace Engine.Tests.Api.Authorization.Util
{


	/// <summary>
	/// 
	/// 
	/// </summary>
	public class AuthorizationScenarioWithCount : AuthorizationScenario
	{
	  protected internal long count;

	  public static AuthorizationScenarioWithCount scenario()
	  {
		return new AuthorizationScenarioWithCount();
	  }

	  public virtual long Count
	  {
		  get
		  {
			return count;
		  }
		  set
		  {
			this.count = value;
		  }
	  }


	  public virtual AuthorizationScenarioWithCount withCount(long instances)
	  {
		this.count = instances;
		return this;
	  }
	}

}