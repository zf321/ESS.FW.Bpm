using System.Collections.Generic;



namespace ESS.FW.Bpm.Model.Bpmn.instance.di
{

    /// <summary>
    /// The DI Edge element
    /// 
    /// 
    /// </summary>
    public interface IEdge : IDiagramElement
    {
        ICollection<IWaypoint> Waypoints { get; }
    }
}