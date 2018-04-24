using System;
using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.Repository
{
    /// <summary>
    ///     Stores a two-dimensional graph layout.
    ///     
    /// </summary>
    [Serializable]
    public class DiagramLayout
    {
        private const long SerialVersionUid = 1L;

        private IDictionary<string, DiagramElement> _elements;

        public DiagramLayout(IDictionary<string, DiagramElement> elements)
        {
            Elements = elements;
        }

        public virtual IDictionary<string, DiagramElement> Elements
        {
            get { return _elements; }
            set { _elements = value; }
        }


        public virtual IList<DiagramNode> Nodes
        {
            get
            {
                IList<DiagramNode> nodes = new List<DiagramNode>();
                foreach (var entry in Elements)
                {
                    var element = entry.Value;
                    if (element is DiagramNode)
                        nodes.Add((DiagramNode) element);
                }
                return nodes;
            }
        }

        public virtual DiagramNode GetNode(string id)
        {
            var element = Elements[id];
            if (element is DiagramNode)
                return (DiagramNode) element;
            return null;
        }

        public virtual DiagramEdge GetEdge(string id)
        {
            var element = Elements[id];
            if (element is DiagramEdge)
                return (DiagramEdge) element;
            return null;
        }
    }
}