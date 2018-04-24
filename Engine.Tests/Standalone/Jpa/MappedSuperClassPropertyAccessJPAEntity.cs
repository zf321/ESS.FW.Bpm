

namespace Engine.Tests.Standalone.Jpa
{


	/// <summary>
	/// Mapped superclass containing an
	/// 
	/// </summary>

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @MappedSuperclass public class MappedSuperClassPropertyAccessJPAEntity
	public class MappedSuperClassPropertyAccessJPAEntity
	{

	  private long? id;

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Id @Column(name = "ID_") public Nullable<long> getId()
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