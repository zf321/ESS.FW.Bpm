

using ESS.FW.Bpm.Model.Bpmn.impl.instance;

namespace ESS.FW.Bpm.Model.Bpmn.instance
{

	using DataPath = DataPath;

	/// <summary>
	/// The BPMN correlationPropertyBinding element
	/// 
	/// 
	/// </summary>
	public interface ICorrelationPropertyBinding : IBaseElement
	{

	  ICorrelationProperty CorrelationProperty {get;set;}


	  DataPath DataPath {get;set;}

	}

}