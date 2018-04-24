

namespace Engine.Tests.Standalone.Jpa
{


	/// <summary>
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Entity(name = "SHORT_ID_ENTITY") public class ShortIdJPAEntity
	public class ShortIdJPAEntity
	{
//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Id @Column(name = "ID_") private short shortId;
		private short shortId;

	  public virtual short ShortId
	  {
		  get
		  {
			return shortId;
		  }
		  set
		  {
			this.shortId = value;
		  }
	  }


	}

}