

namespace ESS.FW.Bpm.Model.Dmn.instance
{

	public interface IAssociation : IArtifact
	{

	  AssociationDirection AssociationDirection {get;set;}


	  IDmnElement Source {get;set;}


	  IDmnElement Target {get;set;}


	}

}