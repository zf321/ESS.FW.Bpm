using System;
using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.Impl.Externaltask
{
    /// <summary>
    ///     
    /// </summary>
    [Serializable]
    public class TopicFetchInstruction
    {
        private const long SerialVersionUid = 1L;
        protected internal bool deserializeVariables;
        protected internal long lockDuration;

        protected internal string topicName;
        protected internal IList<string> variablesToFetch;

        public TopicFetchInstruction(string topicName, long lockDuration)
        {
            this.topicName = topicName;
            this.lockDuration = lockDuration;
        }

        public virtual IList<string> VariablesToFetch
        {
            get { return variablesToFetch; }
            set { variablesToFetch = value; }
        }


        public virtual long? LockDuration
        {
            get { return lockDuration; }
        }

        public virtual string TopicName
        {
            get { return topicName; }
        }

        public virtual bool DeserializeVariables
        {
            get { return deserializeVariables; }
            set { deserializeVariables = value; }
        }
    }
}