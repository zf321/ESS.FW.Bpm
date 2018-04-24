using System;

namespace Engine.Tests.History
{


	/// <summary>
	/// 
	/// </summary>
	[Serializable]
	public class SerializableVariable
	{

	  private const long serialVersionUID = 1L;

	  public string Text;

	  public SerializableVariable(string text) : base()
	  {
		this.Text = text;
	  }

	  public override int GetHashCode()
	  {
		const int prime = 31;
		int result = 1;
		result = prime * result + ((string.ReferenceEquals(Text, null)) ? 0 : Text.GetHashCode());
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
		SerializableVariable other = (SerializableVariable) obj;
		if (string.ReferenceEquals(Text, null))
		{
		  if (!string.ReferenceEquals(other.Text, null))
		  {
			return false;
		  }
		}
		else if (!Text.Equals(other.Text))
		{
		  return false;
		}
		return true;
	  }
	}

}