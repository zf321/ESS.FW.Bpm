using System;

namespace Engine.Tests.Bpmn.ScriptTask
{

	/// <summary>
	/// 
	/// </summary>
	[Serializable]
	public class MySerializable
	{

	  private const long serialVersionUID = 1L;

	  protected internal string name;

	  public MySerializable(string name)
	  {
		this.Name = name;
	  }

	  public virtual string Name
	  {
		  get
		  {
			return name;
		  }
		  set
		  {
			this.Name = value;
		  }
	  }


	}

}