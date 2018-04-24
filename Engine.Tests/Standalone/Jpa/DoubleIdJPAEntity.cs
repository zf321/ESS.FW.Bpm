

namespace Engine.Tests.Standalone.Jpa
{


	/// <summary>
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Entity(name = "DOUBLE_ID_ENTITY") public class DoubleIdJPAEntity
	public class DoubleIdJPAEntity
	{
//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Id @Column(name = "ID_") private double doubleId;
		private double doubleId;

	  public virtual double DoubleId
	  {
		  get
		  {
			return doubleId;
		  }
		  set
		  {
			this.DoubleId = value;
		  }
	  }

	}

}