

namespace ESS.FW.Bpm.Model.Bpmn.instance
{

	/// <summary>
	/// The BPMN formalExpression element
	/// 
	/// 
	/// </summary>
	public interface IFormalExpression : IExpression
	{

	  string Language {get;set;}


	  IItemDefinition EvaluatesToType {get;set;}


	}

}