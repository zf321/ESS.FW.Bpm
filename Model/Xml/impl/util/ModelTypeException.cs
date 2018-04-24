using System;


namespace ESS.FW.Bpm.Model.Xml.impl.util
{


	/// <summary>
	/// <para>Thrown in case a value cannot be converted to or from the requested type</para>
	/// 
	/// 
	/// 
	/// </summary>
	public class ModelTypeException : ModelException
	{

	  private const long SerialVersionUid = 1L;

	  public ModelTypeException(string message) : base(message)
	  {
	  }

	  public ModelTypeException(string value, Type type) : base("Illegal value " + value + " for type " + type)
	  {
	  }

	}

}