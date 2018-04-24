

namespace ESS.FW.Bpm.Model.Bpmn.instance
{

	/// <summary>
	/// The BPMN resourceParameterBinding element
	/// 
	/// 
	/// </summary>
	public interface IResourceParameterBinding : IBaseElement
	{

	  IResourceParameter Parameter {get;set;}


	  IExpression Expression {get;set;}


	}

}