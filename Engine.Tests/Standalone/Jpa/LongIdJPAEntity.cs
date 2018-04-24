

namespace Engine.Tests.Standalone.Jpa
{


	/// <summary>
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Entity(name = "LONG_ID_ENTITY") public class LongIdJPAEntity
	public class LongIdJPAEntity
	{
//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Id @Column(name = "ID_") private long longId;
		private long longId;

	  public virtual long LongId
	  {
		  get
		  {
			return longId;
		  }
		  set
		  {
			this.longId = value;
		  }
	  }

	}

}