using System;

namespace ESS.FW.Bpm.Engine.Impl.Bpmn.Parser
{
    /// <summary>
    ///     
    /// </summary>
    [Serializable]
    public class CompensateEventDefinition
    {
        private const long SerialVersionUid = 1L;

        protected internal string activityRef;
        protected internal bool waitForCompletion;

        public virtual string ActivityRef
        {
            get { return activityRef; }
            set { activityRef = value; }
        }


        public virtual bool WaitForCompletion
        {
            get { return waitForCompletion; }
            set { waitForCompletion = value; }
        }
    }
}