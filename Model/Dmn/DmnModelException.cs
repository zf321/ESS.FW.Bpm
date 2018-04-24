using System;
using ModelException = ESS.FW.Bpm.Model.Xml.ModelException;



namespace ESS.FW.Bpm.Model.Dmn
{

	using ModelException = ModelException;

	public class DmnModelException : ModelException
	{

	  private const long SerialVersionUid = 1L;

	  public DmnModelException()
	  {
	  }

	  public DmnModelException(string message, Exception cause) : base(message, cause)
	  {
	  }

	  public DmnModelException(string message) : base(message)
	  {
	  }

	  public DmnModelException(Exception cause) : base(cause)
	  {
	  }


	}

}