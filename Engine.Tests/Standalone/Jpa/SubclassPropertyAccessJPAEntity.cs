

namespace Engine.Tests.Standalone.Jpa
{


	/// <summary>
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Entity(name = "JPA_SUBCLASS_PROPERTY") public class SubclassPropertyAccessJPAEntity extends MappedSuperClassPropertyAccessJPAEntity
	public class SubclassPropertyAccessJPAEntity : MappedSuperClassPropertyAccessJPAEntity
	{

	  private string value;

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Column(name = "VALUE_") public String getValue()
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