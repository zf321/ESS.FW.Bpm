using System.Text;

namespace Engine.Tests.Api.Authorization.Util
{


	/// <summary>
	/// 
	/// 
	/// </summary>
	public class AuthorizationScenario
	{

	  protected internal const string Indentation = "   ";

	  protected internal AuthorizationSpec[] givenAuthorizations = new AuthorizationSpec[]{};
	  protected internal AuthorizationSpec[] missingAuthorizations = new AuthorizationSpec[]{};

	  public static AuthorizationScenario Scenario()
	  {
		return new AuthorizationScenario();
	  }

	  public virtual AuthorizationScenario WithoutAuthorizations()
	  {
		return this;
	  }

	  public virtual AuthorizationScenario WithAuthorizations(params AuthorizationSpec[] givenAuthorizations)
	  {
		this.givenAuthorizations = givenAuthorizations;
		return this;
	  }

	  public virtual AuthorizationScenario Succeeds()
	  {
		return this;
	  }

	  public virtual AuthorizationScenario FailsDueToRequired(params AuthorizationSpec[] expectedMissingAuthorizations)
	  {
		this.missingAuthorizations = expectedMissingAuthorizations;
		return this;
	  }

	  public virtual AuthorizationSpec[] GivenAuthorizations
	  {
		  get
		  {
			return givenAuthorizations;
		  }
	  }

	  public virtual AuthorizationSpec[] MissingAuthorizations
	  {
		  get
		  {
			return missingAuthorizations;
		  }
	      set { missingAuthorizations = value; }
	  }

	  public override string ToString()
	  {
		StringBuilder sb = new StringBuilder();
		sb.Append("Given Authorizations: \n");
		foreach (AuthorizationSpec spec in givenAuthorizations)
		{
		  sb.Append(Indentation);
		  sb.Append(spec);
		  sb.Append("\n");
		}

		sb.Append("Expected missing authorizations: \n");
		foreach (AuthorizationSpec spec in missingAuthorizations)
		{
		  sb.Append(Indentation);
		  sb.Append(spec);
		  sb.Append("\n");
		}

		return sb.ToString();
	  }


	}

}