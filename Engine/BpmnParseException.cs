using System;
using ESS.FW.Bpm.Engine.Impl.Util.xml;

namespace ESS.FW.Bpm.Engine
{
    /// <summary>
    ///     Exception during the parsing of an BPMN model.
    /// </summary>
    public class BpmnParseException : ProcessEngineException
    {
        private const long SerialVersionUid = 1L;
        protected internal Element element;

        public BpmnParseException(string message, Element element) : base(message)
        {
            this.element = element;
        }

        public BpmnParseException(string message, Element element, System.Exception cause) : base(message, cause)
        {
            this.element = element;
        }

        public virtual Element Element
        {
            get { return element; }
        }
    }
}