using System;


namespace ESS.FW.Bpm.Model.Xml
{
    
	public class ModelParseException : ModelException
	{

	  private const long SerialVersionUid = 1L;

	  public ModelParseException()
	  {
	  }

	  public ModelParseException(string message, Exception cause) : base(message, cause)
	  {
	  }

	  public ModelParseException(string message) : base(message)
	  {
	  }

	  public ModelParseException(Exception cause) : base(cause)
	  {
	  }

	}

}