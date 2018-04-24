using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Bpmn.MultiInstance
{
    /// <summary>
    /// </summary>
    public class RecordInvocationListener : IDelegateListener<IBaseDelegateExecution>
    {
        public static readonly IDictionary<string, int?> INVOCATIONS = new Dictionary<string, int?>();

        public void Notify(IBaseDelegateExecution execution)
        {
            var counter = INVOCATIONS[execution.EventName];
            if (counter == null)
                counter = 0;

            INVOCATIONS[execution.EventName] = ++counter;
        }

        public static void reset()
        {
            INVOCATIONS.Clear();
        }
    }
}