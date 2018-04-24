using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.Impl.Pvm.Process
{
    /// <summary>
    ///     A BPMN 2.0 LaneSet, containg <seealso cref="Lane" />s, currently only used for
    ///     rendering the DI info.
    ///     
    /// </summary>
    public class LaneSet
    {
        protected internal IList<Lane> lanes;

        public virtual string Id { set; get; }


        public virtual string Name { get; set; }


        public virtual IList<Lane> Lanes
        {
            get
            {
                if (lanes == null)
                    lanes = new List<Lane>();
                return lanes;
            }
        }

        public virtual void AddLane(Lane laneToAdd)
        {
            Lanes.Add(laneToAdd);
        }

        public virtual Lane GetLaneForId(string id)
        {
            if ((lanes != null) && (lanes.Count > 0))
                foreach (var lane in lanes)
                    if (id.Equals(lane.Id))
                        return lane;
            return null;
        }
    }
}