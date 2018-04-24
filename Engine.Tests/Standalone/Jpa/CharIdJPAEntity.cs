

namespace Engine.Tests.Standalone.Jpa
{


	/// <summary>
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Entity(name = "CHAR_ID_ENTITY") public class CharIdJPAEntity
	public class CharIdJPAEntity
	{
//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Id @Column(name = "ID_") private char charId;
		private char charId;

	  public virtual char CharId
	  {
		  get
		  {
			return charId;
		  }
		  set
		  {
			this.charId = value;
		  }
	  }


	}

}