using System.Collections.Generic;


namespace ESS.FW.Bpm.Model.Bpmn.instance
{

	/// <summary>
	/// The BPMN flowElement element
	/// 
	/// 
	/// 
	/// </summary>
	public interface IFlowElement : IBaseElement
	{

	  string Name {get;set;}


	  IAuditing Auditing {get;set;}


	  IMonitoring Monitoring {get;set;}


	  ICollection<ICategoryValue> CategoryValueRefs {get;}
	}

}