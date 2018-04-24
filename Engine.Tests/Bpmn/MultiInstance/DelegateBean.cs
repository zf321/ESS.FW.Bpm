using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Bpmn.MultiInstance
{
    /// <summary>
    /// </summary>
    [Serializable]
    public class DelegateBean
    {
        private const long serialVersionUID = 1L;

        protected internal static IList<DelegateEvent> RECORDED_EVENTS = new List<DelegateEvent>();

        public virtual IList<string> resolveCollection(IDelegateExecution delegateExecution)
        {
            DelegateEvent.RecordEventFor(delegateExecution);
            return new List<string> {"1"};
        }
    }
}