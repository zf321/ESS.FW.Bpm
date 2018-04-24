using System.Collections.Generic;



namespace ESS.FW.Bpm.Model.Bpmn.instance
{

	/// <summary>
	/// The BPMN correlationKey element
	/// 
	/// 
	/// </summary>
	public interface ICorrelationKey : IBaseElement
	{

	  string Name {get;set;}


	  ICollection<ICorrelationProperty> CorrelationProperties {get;}
	}

}