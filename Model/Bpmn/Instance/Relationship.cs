using System.Collections.Generic;
using ESS.FW.Bpm.Model.Bpmn.impl.instance;



namespace ESS.FW.Bpm.Model.Bpmn.instance
{

	using Source = Source;
	using Target = Target;

	/// <summary>
	/// The BPMN relationship element
	/// 
	/// 
	/// </summary>
	public interface IRelationship : IBaseElement
	{

	  string Type {get;set;}


	  RelationshipDirection Direction {get;set;}


	  ICollection<Source> Sources {get;}

	  ICollection<Target> Targets {get;}
	}

}