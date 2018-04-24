

using ESS.FW.Bpm.Model.Bpmn.builder;

namespace ESS.FW.Bpm.Model.Bpmn.instance
{

	using ReceiveTaskBuilder = ReceiveTaskBuilder;

	/// <summary>
	/// The BPMN receiveTask element
	/// 
	/// 
	/// </summary>
	public interface IReceiveTask : ITask
	{

	  ReceiveTaskBuilder Builder();

	  string Implementation {get;set;}
        

	  bool Instantiate { get;set;}

	  IMessage Message {get;set;}


	  IOperation Operation {get;set;}


	}

}