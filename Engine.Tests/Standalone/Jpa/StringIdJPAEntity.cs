

namespace Engine.Tests.Standalone.Jpa
{


	/// <summary>
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Entity(name = "STRING_ID_ENTITY") public class StringIdJPAEntity
	public class StringIdJPAEntity
	{
//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Id @Column(name = "ID_") private String stringId;
		private string stringId;

	  public virtual string StringId
	  {
		  get
		  {
			return stringId;
		  }
		  set
		  {
			this.stringId = value;
		  }
	  }


	}

}