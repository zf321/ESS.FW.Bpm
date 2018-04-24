namespace Engine.Tests.Bpmn.Event.Error
{

	public class MyBusinessException : System.Exception
	{

	  private const long serialVersionUID = -8849430031097301135L;

	  public MyBusinessException(string message) : base(message)
	  {
	  }
	}

}