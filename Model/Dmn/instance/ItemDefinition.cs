using System.Collections.Generic;



namespace ESS.FW.Bpm.Model.Dmn.instance
{

	public interface ITemDefinition : INamedElement
	{

	  string TypeLanguage {get;set;}


	  bool Collection {get;set;}


	  ITypeRef TypeRef {get;set;}


	  IAllowedValues AllowedValues {get;set;}


	  ICollection<ITemComponent> ItemComponents {get;}

	}

}