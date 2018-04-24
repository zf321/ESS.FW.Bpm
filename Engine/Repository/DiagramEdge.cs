using System;
using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.Repository
{
    /// <summary>
    ///     Stores waypoints of a diagram edge.
    ///     
    /// </summary>
    [Serializable]
    public class DiagramEdge : DiagramElement
    {
        private const long SerialVersionUid = 1L;

        private IList<DiagramEdgeWaypoint> _waypoints;

        public DiagramEdge()
        {
        }

        public DiagramEdge(string id, IList<DiagramEdgeWaypoint> waypoints) : base(id)
        {
            this._waypoints = waypoints;
        }

        public override bool Node
        {
            get { return false; }
        }

        public override bool Edge
        {
            get { return true; }
        }

        public virtual IList<DiagramEdgeWaypoint> Waypoints
        {
            get { return _waypoints; }
            set { _waypoints = value; }
        }
    }
}