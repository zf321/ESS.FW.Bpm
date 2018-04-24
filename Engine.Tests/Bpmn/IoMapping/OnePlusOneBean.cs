using System;

namespace Engine.Tests.Bpmn.IoMapping
{

	/// <summary>
	/// 
	/// </summary>
	[Serializable]
	public class OnePlusOneBean
	{

	  private const long serialVersionUID = 1L;

	  public virtual string Source
	  {
		  get
		  {
			return "return 1 + 1";
		  }
	  }

	  public virtual string Resource
	  {
		  get
		  {
			return "resources/bpmn/iomapping/oneplusone.groovy";
		  }
	  }

	  public virtual string ClasspathResource
	  {
		  get
		  {
			return "classpath://" + Resource;
		  }
	  }

	  public virtual string DeploymentResource
	  {
		  get
		  {
			return "deployment://" + Resource;
		  }
	  }

	}

}