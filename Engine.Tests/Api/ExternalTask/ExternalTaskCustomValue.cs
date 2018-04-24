using System;

namespace Engine.Tests.Api.ExternalTask
{

	[Serializable]
	public class ExternalTaskCustomValue
	{

	  private const long serialVersionUID = 1L;

	  protected internal string testValue;

	  public virtual string TestValue
	  {
		  get
		  {
			return testValue;
		  }
		  set
		  {
			this.TestValue = value;
		  }
	  }


	}

}