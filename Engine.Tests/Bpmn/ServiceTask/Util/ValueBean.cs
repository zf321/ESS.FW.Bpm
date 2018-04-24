using System;

namespace Engine.Tests.Bpmn.ServiceTask.Util
{
    
	[Serializable]
	public class ValueBean
	{

	  private const long serialVersionUID = 1L;

	  private readonly string value;

	  public ValueBean(string value)
	  {
		this.value = value;
	  }

	  public virtual string Value
	  {
		  get
		  {
			return value;
		  }
	  }

	}
}