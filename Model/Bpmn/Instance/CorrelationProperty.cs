using System.Collections.Generic;



namespace ESS.FW.Bpm.Model.Bpmn.instance
{

	/// <summary>
	/// The BPMN correlationProperty element
	/// 
	/// 
	/// </summary>
	public interface ICorrelationProperty : IRootElement
	{

	  string Name {get;set;}


	  IItemDefinition Type {get;set;}


	  ICollection<ICorrelationPropertyRetrievalExpression> CorrelationPropertyRetrievalExpressions {get;}
	}

}