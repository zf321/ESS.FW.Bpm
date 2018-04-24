using System.Collections.Generic;



namespace ESS.FW.Bpm.Model.Dmn.instance
{

	public interface IDefinitions : INamedElement
	{

	  string ExpressionLanguage {get;set;}


	  string TypeLanguage {get;set;}


	  string Namespace {get;set;}


	  string Exporter {get;set;}


	  string ExporterVersion {get;set;}


	  ICollection<IMport> Imports {get;}

	  ICollection<ITemDefinition> ItemDefinitions {get;}

	  ICollection<IArtifact> Artifacts {get;}

	  ICollection<IDrgElement> DrgElements {get;}

	  ICollection<IElementCollection> ElementCollections {get;}

	  ICollection<IBusinessContextElement> BusinessContextElements {get;}

	}

}