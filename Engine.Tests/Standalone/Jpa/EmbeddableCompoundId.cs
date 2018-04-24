using System;

namespace Engine.Tests.Standalone.Jpa
{


	/// <summary>
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Embeddable @SuppressWarnings("serial") public class EmbeddableCompoundId implements java.io.Serializable
	[Serializable]
	public class EmbeddableCompoundId
	{

	  private long idPart1;

	  private string idPart2;

	  public virtual long IdPart1
	  {
		  get
		  {
			return idPart1;
		  }
		  set
		  {
			this.idPart1 = value;
		  }
	  }


	  public virtual string IdPart2
	  {
		  get
		  {
			return idPart2;
		  }
		  set
		  {
			this.idPart2 = value;
		  }
	  }


//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: @Override public boolean equals(final Object obj)
	  public override bool Equals(object obj)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final EmbeddableCompoundId other = (EmbeddableCompoundId) obj;
		EmbeddableCompoundId other = (EmbeddableCompoundId) obj;
		return idPart1 == other.idPart1 && idPart2.Equals(idPart2);
	  }

	  public override int GetHashCode()
	  {
		return (idPart1 + idPart2).GetHashCode();
	  }

	}

}