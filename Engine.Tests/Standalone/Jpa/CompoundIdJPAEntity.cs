

namespace Engine.Tests.Standalone.Jpa
{



	/// <summary>
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Entity public class CompoundIdJPAEntity
	public class CompoundIdJPAEntity
	{
//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @EmbeddedId private EmbeddableCompoundId id;
		private EmbeddableCompoundId id;

	  public virtual EmbeddableCompoundId Id
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


	  public override bool Equals(object obj)
	  {
		return id.Equals(((CompoundIdJPAEntity)obj).Id);
	  }
	}

}