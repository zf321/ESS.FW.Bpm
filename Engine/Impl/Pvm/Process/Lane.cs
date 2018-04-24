using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.Impl.Pvm.Process
{
    /// <summary>
    ///     A single lane in a BPMN 2.0 LaneSet, currently only used internally for rendering the
    ///     diagram. The PVM doesn't actually use the laneSets/lanes.
    ///     
    /// </summary>
    public class Lane : IHasDiBounds
    {
        protected internal IList<string> flowNodeIds;

        public virtual string Id { set; get; }


        public virtual string Name { get; set; }


        public virtual IList<string> FlowNodeIds
        {
            get
            {
                if (flowNodeIds == null)
                    flowNodeIds = new List<string>();
                return flowNodeIds;
            }
        }


        public virtual int X { get; set; } = -1;


        public virtual int Y { get; set; } = -1;


        public virtual int Width { get; set; } = -1;


        public virtual int Height { get; set; } = -1;
    }
}