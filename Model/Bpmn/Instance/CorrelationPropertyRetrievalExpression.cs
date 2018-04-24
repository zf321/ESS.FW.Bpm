

using ESS.FW.Bpm.Model.Bpmn.impl.instance;

namespace ESS.FW.Bpm.Model.Bpmn.instance
{

	using MessagePath = MessagePath;

	/// <summary>
	/// The BPMN correlationPropertyRetrievalExpression element
	/// 
	/// 
	/// </summary>
	public interface ICorrelationPropertyRetrievalExpression : IBaseElement
	{

	  IMessage Message {get;set;}


	  MessagePath MessagePath {get;set;}

	}

}