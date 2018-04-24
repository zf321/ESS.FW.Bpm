

namespace Engine.Tests.Standalone.Jpa
{


	/// <summary>
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Entity(name = "BIGD_ID_ENTITY") public class BigDecimalIdJPAEntity
	public class BigDecimalIdJPAEntity
	{
//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Id @Column(name = "ID_") private java.Math.BigDecimal bigDecimalId;
		private decimal bigDecimalId;

	  public virtual decimal BigDecimalId
	  {
		  get
		  {
			return bigDecimalId;
		  }
		  set
		  {
			this.bigDecimalId = value;
		  }
	  }


	}

}