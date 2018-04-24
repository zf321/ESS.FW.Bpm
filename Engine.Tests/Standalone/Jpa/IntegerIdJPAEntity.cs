

namespace Engine.Tests.Standalone.Jpa
{


	/// <summary>
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Entity(name = "INT_ID_ENTITY") public class IntegerIdJPAEntity
	public class IntegerIdJPAEntity
	{
//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Id @Column(name = "ID_") private int intId;
		private int intId;

	  public virtual int IntId
	  {
		  get
		  {
			return intId;
		  }
		  set
		  {
			this.IntId = value;
		  }
	  }

	}

}