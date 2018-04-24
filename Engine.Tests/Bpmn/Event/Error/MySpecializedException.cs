
namespace Engine.Tests.Bpmn.Event.Error
{

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class MySpecializedException : MyBusinessException
	{

	  private const long serialVersionUID = 1L;

	  public MySpecializedException(string message) : base(message)
	  {
	  }

	}

}