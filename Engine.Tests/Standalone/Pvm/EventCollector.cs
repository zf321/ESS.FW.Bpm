using System.Collections.Generic;
using System.Diagnostics;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Pvm.Runtime;

namespace Engine.Tests.Standalone.Pvm
{
    public class EventCollector : IDelegateListener<IBaseDelegateExecution>
    {
        //private static Logger _log = ProcessEngineLogger.TEST_LOGGER.Logger;

        public IList<string> Events = new List<string>();


        public virtual void Notify(IBaseDelegateExecution execution)
        {
            var executionImpl = (PvmExecutionImpl) execution;
            Debug.WriteLine("collecting event: " + execution.EventName + " on " + executionImpl.EventSource);
            Events.Add(execution.EventName + " on " + executionImpl.EventSource);
        }

        //public override string ToString()
        //{
        //    var text = new StringBuilder();
        //    foreach (var @event in Events)
        //    {
        //        text.Append(@event);
        //        text.Append("\n");
        //    }
        //    return text.ToString();
        //}
    }
}