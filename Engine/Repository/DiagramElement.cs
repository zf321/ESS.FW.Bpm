using System;

namespace ESS.FW.Bpm.Engine.Repository
{
    /// <summary>
    ///     Represents a diagram node.
    ///     
    /// </summary>
    [Serializable]
    public abstract class DiagramElement
    {
        private const long SerialVersionUid = 1L;

        protected internal string id;

        public DiagramElement()
        {
        }

        public DiagramElement(string id)
        {
            this.id = id;
        }

        /// <summary>
        ///     Id of the diagram element.
        /// </summary>
        public virtual string Id
        {
            get { return id; }
            set { id = value; }
        }

        public abstract bool Node { get; }
        public abstract bool Edge { get; }


        public override string ToString()
        {
            return "id=" + Id;
        }
    }
}