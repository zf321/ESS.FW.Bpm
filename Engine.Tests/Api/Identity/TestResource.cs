

namespace Engine.Tests.Api.Identity
{

	/// <summary>
	/// 
	/// 
	/// </summary>
	public class TestResource //: IResources
	{

	  protected internal int id;
	  protected internal string Name;

	  public TestResource(string name, int id)
	  {
		this.Name = name;
		this.id = id;
	  }

	  public virtual string resourceName()
	  {
		return Name;
	  }

	  public virtual int resourceType()
	  {
		return id;
	  }

	}

}