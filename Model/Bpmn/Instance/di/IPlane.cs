using System.Collections.Generic;



namespace ESS.FW.Bpm.Model.Bpmn.instance.di
{

	/// <summary>
	/// The DI Plane element
	/// 
	/// 
	/// </summary>
	public interface IPlane : INode
	{

	  ICollection<IDiagramElement> DiagramElements {get;}

	}

}