using System.Collections.Generic;


namespace ESS.FW.Bpm.Model.Bpmn.instance
{

	/// <summary>
	/// The BPMN callableElement element
	/// 
	/// 
	/// 
	/// </summary>
	public interface ICallableElement : IRootElement
	{

	  string Name {get;set;}


	  ICollection<INterface> SupportedInterfaces {get;}

	  IOSpecification IoSpecification {get;set;}


	  ICollection<IOBinding> IoBindings {get;}

	}

}