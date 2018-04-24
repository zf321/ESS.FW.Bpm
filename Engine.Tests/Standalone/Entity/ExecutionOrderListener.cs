using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace Engine.Tests.Standalone.Entity
{
    /// <summary>
    /// </summary>
    public class ExecutionOrderListener : IDelegateListener<IBaseDelegateExecution>
    {
        protected internal static IList<ActivitySequenceCounterMap> activityExecutionOrder =
            new List<ActivitySequenceCounterMap>();
        public virtual void Notify(IBaseDelegateExecution execution)
        {
            var executionEntity = (ExecutionEntity) execution;

            var sequenceCounter = executionEntity.SequenceCounter;
            var activityId = executionEntity.ActivityId;

            activityExecutionOrder.Add(new ActivitySequenceCounterMap(this, activityId, sequenceCounter));
        }

        public static void ClearActivityExecutionOrder()
        {
            activityExecutionOrder.Clear();
        }

        public static IList<ActivitySequenceCounterMap> ActivityExecutionOrder
        {
            get { return activityExecutionOrder; }
        }
    }

    public class ActivitySequenceCounterMap
    {
        private readonly ExecutionOrderListener _outerInstance;


        protected internal string activityId;
        protected internal long sequenceCounter;

        public ActivitySequenceCounterMap(ExecutionOrderListener outerInstance, string activityId, long sequenceCounter)
        {
            _outerInstance = outerInstance;
            this.activityId = activityId;
            this.sequenceCounter = sequenceCounter;
        }

        public virtual string ActivityId
        {
            get { return activityId; }
        }

        public virtual long SequenceCounter
        {
            get { return sequenceCounter; }
        }
    }
}