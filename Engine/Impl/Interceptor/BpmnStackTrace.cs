using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ESS.FW.Bpm.Engine.Impl.Interceptor
{
    /// <summary>
    ///     
    /// </summary>
    public class BpmnStackTrace
    {
        private static readonly ContextLogger Log = ProcessEngineLogger.ContextLogger;

        protected internal IList<AtomicOperationInvocation> PerfromedInvocations = new List<AtomicOperationInvocation>();

        public virtual void PrintStackTrace(bool verbose)
        {
            if (PerfromedInvocations.Count == 0)
                return;

            var writer = new StringWriter();
            writer.Write("BPMN Stack Trace:\n");

            if (!verbose)
                LogNonVerbose(writer);
            else
                LogVerbose(writer);

            Log.BpmnStackTrace(writer.ToString());

            PerfromedInvocations.Clear();
        }

        protected internal virtual void LogNonVerbose(StringWriter writer)
        {
            // log the failed operation verbosely
            WriteInvocation(PerfromedInvocations[PerfromedInvocations.Count - 1], writer);

            // log human consumable trace of activity ids only
            var activities = CollectActivityTrace();
            LogActivityTrace(writer, activities);
        }

        protected internal virtual void LogVerbose(StringWriter writer)
        {
            // log process engine developer consumable trace
            var arr = PerfromedInvocations.ToArray();
            Array.Reverse(arr);
            foreach (var invocation in arr)
                WriteInvocation(invocation, writer);
        }

        protected internal virtual void LogActivityTrace(StringWriter writer, IList<string> activities)
        {
            for (var i = 0; i < activities.Count; i++)
            {
                if (i != 0)
                {
                    writer.Write("\t  ^\n");
                    writer.Write("\t  |\n");
                }
                writer.Write("\t");
                writer.Write(activities[i]);
                writer.Write("\n");
            }
        }

        protected internal virtual IList<string> CollectActivityTrace()
        {
            IList<string> activities = new List<string>();
            foreach (var atomicOperationInvocation in PerfromedInvocations)
            {
                var activityId = atomicOperationInvocation.ActivityId;
                if (ReferenceEquals(activityId, null))
                    continue;
                if ((activities.Count == 0) || !activityId.Equals(activities[0]))
                    activities.Insert(0, activityId);
            }
            return activities;
        }

        public virtual void Add(AtomicOperationInvocation atomicOperationInvocation)
        {
            PerfromedInvocations.Add(atomicOperationInvocation);
        }

        protected internal virtual void WriteInvocation(AtomicOperationInvocation invocation, StringWriter writer)
        {
            writer.Write("\t");
            writer.Write(invocation.ActivityId);
            writer.Write(" (");
            writer.Write(invocation.Operation.CanonicalName);
            writer.Write(", ");
            writer.Write(invocation.Execution.ToString());

            if (invocation.PerformAsync)
                writer.Write(", ASYNC");

            if (!ReferenceEquals(invocation.ApplicationContextName, null))
            {
                writer.Write(", pa=");
                writer.Write(invocation.ApplicationContextName);
            }

            writer.Write(")\n");
        }
    }
}