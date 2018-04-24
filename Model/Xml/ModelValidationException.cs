using System;


namespace ESS.FW.Bpm.Model.Xml
{
    
	public class ModelValidationException : ModelException
	{

	  private const long SerialVersionUid = 1L;

	  public ModelValidationException()
	  {
	  }

	  public ModelValidationException(string message, Exception cause) : base(message, cause)
	  {
	  }

	  public ModelValidationException(string message) : base(message)
	  {
	  }

	  public ModelValidationException(Exception cause) : base(cause)
	  {
	  }

	}

}