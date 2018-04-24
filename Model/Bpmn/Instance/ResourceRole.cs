using System.Collections.Generic;



namespace ESS.FW.Bpm.Model.Bpmn.instance
{

	/// <summary>
	/// The BPMN resourceRole element
	/// 
	/// 
	/// </summary>
	public interface IResourceRole : IBaseElement
	{

	  string Name {get;set;}


	  Resources Resource {get;set;}


	  ICollection<IResourceParameterBinding> ResourceParameterBinding {get;}

	  IResourceAssignmentExpression ResourceAssignmentExpression {get;}

	}

}