using System;

namespace ESS.FW.Bpm.Model.Xml
{
   
	public class ModelReferenceException : ModelException
	{

	  private const long SerialVersionUid = 1L;

	  public ModelReferenceException()
	  {
	  }

	  public ModelReferenceException(string message, Exception cause) : base(message, cause)
	  {
	  }

	  public ModelReferenceException(string message) : base(message)
	  {
	  }

	  public ModelReferenceException(Exception cause) : base(cause)
	  {
	  }

	}

}