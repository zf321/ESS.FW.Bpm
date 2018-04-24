

namespace Engine.Tests.Standalone.Jpa
{



	/// <summary>
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Entity(name="BYTE_ID_ENTITY") public class ByteIdJPAEntity
	public class ByteIdJPAEntity
	{
//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Id @Column(name="ID_") private byte byteId;
		private sbyte byteId;


	  public virtual sbyte ByteId
	  {
		  get
		  {
			return byteId;
		  }
		  set
		  {
			this.ByteId = value;
		  }
	  }


	}

}