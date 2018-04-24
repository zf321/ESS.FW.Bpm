

namespace Engine.Tests.Standalone.Jpa
{


	/// <summary>
	/// Mapped superclass containing an
	/// 
	/// </summary>

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @MappedSuperclass public class MappedSuperClassFieldAccessJPAEntity
	public class MappedSuperClassFieldAccessJPAEntity
	{
//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Id @Column(name = "ID_") private Nullable<long> id;
		private long? id;

	  public virtual long? Id
	  {
		  get
		  {
			return id;
		  }
		  set
		  {
			this.Id = value;
		  }
	  }


	}

}