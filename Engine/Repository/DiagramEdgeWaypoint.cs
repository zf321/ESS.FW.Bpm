using System;

namespace ESS.FW.Bpm.Engine.Repository
{
    /// <summary>
    ///     Stores the position of a waypoint of a diagram edge.
    ///     
    /// </summary>
    [Serializable]
    public class DiagramEdgeWaypoint
    {
        private const long SerialVersionUid = 1L;

        private double? _x;
        private double? _y;

        public virtual double? X
        {
            get { return _x; }
            set { _x = value; }
        }

        public virtual double? Y
        {
            get { return _y; }
            set { _y = value; }
        }
    }
}