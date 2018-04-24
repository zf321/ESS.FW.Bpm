

namespace Engine.Tests.Standalone.Jpa
{


	/// <summary>
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Entity(name = "BIGINT_ID_ENTITY") public class BigIntegerIdJPAEntity
	public class BigIntegerIdJPAEntity
	{
//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Id @Column(name = "ID_") private java.Math.BigInteger bigIntegerId;
		private long bigIntegerId;

	  public virtual long BigIntegerId
	  {
		  get
		  {
			return bigIntegerId;
		  }
		  set
		  {
			this.bigIntegerId = value;
		  }
	  }


	}

}