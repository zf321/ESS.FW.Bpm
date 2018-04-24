using System.Collections.Generic;



namespace ESS.FW.Bpm.Model.Bpmn.instance
{

	/// <summary>
	/// The BPMN correlationSubscription element
	/// 
	/// 
	/// </summary>
	public interface ICorrelationSubscription : IBaseElement
	{

	  ICorrelationKey CorrelationKey {get;set;}


	  ICollection<ICorrelationPropertyBinding> CorrelationPropertyBindings {get;}
	}

}