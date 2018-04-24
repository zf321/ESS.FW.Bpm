using System;

namespace Engine.Tests.Standalone.Jpa
{


	/// <summary>
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Entity(name = "DATE_ID_ENTITY") public class DateIdJPAEntity
	public class DateIdJPAEntity
	{
//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Id @Column(name = "ID_") private java.util.Date dateId;
		private DateTime dateId;

	  public virtual DateTime DateId
	  {
		  get
		  {
			return dateId;
		  }
		  set
		  {
			this.DateId = value;
		  }
	  }


	}

}