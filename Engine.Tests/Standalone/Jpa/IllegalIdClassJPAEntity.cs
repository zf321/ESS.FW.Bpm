using System;

namespace Engine.Tests.Standalone.Jpa
{


	/// 
	/// <summary>
	/// WARNING: This class cannot be used in JPA-context, since it has an illegal
	/// type of ID.
	/// 
	/// For testing purposes only.
	/// 
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Entity public class IllegalIdClassJPAEntity
	public class IllegalIdClassJPAEntity
	{
//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Id private java.util.Calendar id;
		private DateTime id;

	  public virtual DateTime Id
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