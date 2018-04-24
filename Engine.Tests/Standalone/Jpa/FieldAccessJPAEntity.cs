

namespace Engine.Tests.Standalone.Jpa
{


	/// <summary>
	/// Simple JPA entity, id is set on a field.
	/// 
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Entity(name = "JPA_ENTITY_FIELD") public class FieldAccessJPAEntity
	public class FieldAccessJPAEntity
	{
//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Id @Column(name = "ID_") private Nullable<long> id;
		private long? id;

	  private string value;

	  public FieldAccessJPAEntity()
	  {
		// Empty constructor needed for JPA
	  }

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


	  public virtual string Value
	  {
		  get
		  {
			return value;
		  }
		  set
		  {
			this.value = value;
		  }
	  }


	}

}