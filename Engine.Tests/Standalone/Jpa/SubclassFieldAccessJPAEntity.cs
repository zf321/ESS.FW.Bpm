

namespace Engine.Tests.Standalone.Jpa
{


	/// <summary>
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Entity(name = "JPA_SUBCLASS_FIELD") public class SubclassFieldAccessJPAEntity extends MappedSuperClassFieldAccessJPAEntity
	public class SubclassFieldAccessJPAEntity : MappedSuperClassFieldAccessJPAEntity
	{
//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Column(name = "VALUE_") private String value;
		private string value;

	  public virtual string Value
	  {
		  get
		  {
			return value;
		  }
		  set
		  {
			this.value = value;
		  }
	  }


	}

}