using System;

namespace Engine.Tests.Api.Variables
{

	/// <summary>
	/// 
	/// 
	/// </summary>
	[Serializable]
	public class JavaSerializable
	{

	  private const long serialVersionUID = 1L;

	  private string property;

	  public JavaSerializable(string property)
	  {
		this.property = property;
	  }

	  public virtual string Property
	  {
		  get
		  {
			return property;
		  }
		  set
		  {
			this.property = value;
		  }
	  }


	  public override int GetHashCode()
	  {
		const int prime = 31;
		int result = 1;
		result = prime * result + ((string.ReferenceEquals(property, null)) ? 0 : property.GetHashCode());
		return result;
	  }

	  public override bool Equals(object obj)
	  {
		if (this == obj)
		{
		  return true;
		}
		if (obj == null)
		{
		  return false;
		}
		if (this.GetType() != obj.GetType())
		{
		  return false;
		}
		JavaSerializable other = (JavaSerializable) obj;
		if (string.ReferenceEquals(property, null))
		{
		  if (!string.ReferenceEquals(other.property, null))
		  {
			return false;
		  }
		}
		else if (!property.Equals(other.property))
		{
		  return false;
		}
		return true;
	  }

	  public override string ToString()
	  {
		return "JavaSerializable [property=" + property + "]";
	  }


	}

}