using System;
using ModelException = ESS.FW.Bpm.Model.Xml.ModelException;


namespace ESS.FW.Bpm.Model.Bpmn
{

	using ModelException = ModelException;

	/// <summary>
	/// <para>A <seealso cref="RuntimeException"/> in the Bpmn Model.</para>
	/// 
	/// 
	/// 
	/// </summary>
	public class BpmnModelException : ModelException
	{

	  private const long SerialVersionUid = 1L;

	  public BpmnModelException()
	  {
	  }

	  public BpmnModelException(string message, Exception cause) : base(message, cause)
	  {
	  }

	  public BpmnModelException(string message) : base(message)
	  {
	  }

	  public BpmnModelException(Exception cause) : base(cause)
	  {
	  }

	}

}