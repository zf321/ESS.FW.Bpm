using System;

namespace Engine.Tests.Bpmn.ScriptTask
{

	/// <summary>
	/// 
	/// </summary>
	[Serializable]
	public class ScriptResourceBean
	{

	  private const long serialVersionUID = 1L;

	  public virtual string Source
	  {
		  get
		  {
			return "execution.SetVariable('foo', 'bar')";
		  }
	  }

	  public virtual string Resource
	  {
		  get
		  {
			return "resources/bpmn/scripttask/greeting.py";
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