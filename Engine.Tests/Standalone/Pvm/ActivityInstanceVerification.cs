using System.Collections.Generic;
using System.Diagnostics;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Pvm;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;
using NUnit.Framework;

namespace Engine.Tests.Standalone.Pvm
{
    public class ActivityInstanceVerification : IDelegateListener<IBaseDelegateExecution>
    {
        public IDictionary<string, IList<ActivityInstance>> EndedActivityInstances =
            new Dictionary<string, IList<ActivityInstance>>();

        public IDictionary<string, IList<ActivityInstance>> StartedActivityInstances =
            new Dictionary<string, IList<ActivityInstance>>();

        public virtual void Notify(IBaseDelegateExecution e)
        {
            var execution = (IActivityExecution) e;

            if (execution.ActivityInstanceId == null)
                return;

            if (execution.EventName.Equals(ExecutionListenerFields.EventNameStart))
                AddActivityInstanceId(execution, StartedActivityInstances);
            else if (execution.EventName.Equals(ExecutionListenerFields.EventNameEnd))
                AddActivityInstanceId(execution, EndedActivityInstances);
        }

        private void AddActivityInstanceId(IActivityExecution execution,
            IDictionary<string, IList<ActivityInstance>> instanceMap)
        {
            var actId = execution.Activity.Id;
            var actInstanceId = execution.ActivityInstanceId;
            var parentActInstanceId = execution.ParentActivityInstanceId;
            var executionId = execution.GetHashCode().ToString();

            // add to instance map
            IList<ActivityInstance> instancesForThisAct;
            instanceMap.TryGetValue(actId, out instancesForThisAct);
            if (instancesForThisAct == null)
            {
                instancesForThisAct = new List<ActivityInstance>();
                instanceMap[actId] = instancesForThisAct;
            }
            var activityInstance = new ActivityInstance(this, executionId, actInstanceId, parentActInstanceId,
                execution.IsCompleteScope);
            instancesForThisAct.Add(activityInstance);
        }

        // Assertions //////////////////////////////

        public virtual void AssertStartInstanceCount(int Count, string actId)
        {
            IList<ActivityInstance> startInstancesForThisAct;
            StartedActivityInstances.TryGetValue(actId, out startInstancesForThisAct);
            if ((Count == 0) && (startInstancesForThisAct == null))
                return;

            Assert.NotNull(startInstancesForThisAct);
            Assert.AreEqual(Count, startInstancesForThisAct.Count);

            var endInstancesForThisAct = EndedActivityInstances[actId];
            Assert.NotNull(endInstancesForThisAct);

            foreach (var startedActInstance in startInstancesForThisAct)
            {
                Assert.NotNull("activityInstanceId cannot be null for " + startedActInstance, startedActInstance.Id);
                Assert.NotNull("executionId cannot be null for " + startedActInstance, startedActInstance.ExecutionId);
                Assert.NotNull("parentId cannot be null for " + startedActInstance, startedActInstance.ParentId);

                var foundMatchingEnd = false;
                foreach (var endedActInstance in endInstancesForThisAct)
                    if (startedActInstance.Id.Equals(endedActInstance.Id))
                    {
                        Assert.AreEqual(startedActInstance.ParentId, endedActInstance.ParentId);
                        foundMatchingEnd = true;
                    }
                if (!foundMatchingEnd)
                    Debug.WriteLine("cannot find matching end activity instance for start activity instance " +
                                    startedActInstance.Id);
            }
        }

        public virtual void AssertProcessInstanceParent(string actId, IPvmProcessInstance processInstance)
        {
            AssertParentActInstance(actId, processInstance.GetHashCode().ToString());
        }

        public virtual void AssertParentActInstance(string actId, string actInstId)
        {
            var actInstanceList = StartedActivityInstances[actId];

            foreach (var activityInstance in actInstanceList)
                Assert.AreEqual(actInstId, activityInstance.ParentId);

            actInstanceList = EndedActivityInstances[actId];
            foreach (var activityInstance in actInstanceList)
                Assert.AreEqual(actInstId, activityInstance.ParentId);
        }

        public virtual void AssertParent(string actId, string parentId)
        {
            var actInstanceList = StartedActivityInstances[actId];
            var parentInstances = StartedActivityInstances[parentId];

            foreach (var activityInstance in actInstanceList)
            {
                var found = false;
                foreach (var parentIntance in parentInstances)
                    if (activityInstance.ParentId.Equals(parentIntance.Id))
                        found = true;
                if (!found)
                    Debug.WriteLine("every instance of '" + actId + "' must have a parent which is an instance of '" +
                                    parentId);
            }
        }

        public virtual void AssertIsCompletingActivityInstance(string activityId)
        {
            AssertIsCompletingActivityInstance(activityId, -1);
        }

        public virtual void AssertIsCompletingActivityInstance(string activityId, int Count)
        {
            AssertCorrectCompletingState(activityId, Count, true);
        }

        public virtual void AssertNonCompletingActivityInstance(string activityId)
        {
            AssertNonCompletingActivityInstance(activityId, -1);
        }

        public virtual void AssertNonCompletingActivityInstance(string activityId, int Count)
        {
            AssertCorrectCompletingState(activityId, Count, false);
        }

        private void AssertCorrectCompletingState(string activityId, int expectedCount, bool completing)
        {
            var endActivityInstances = EndedActivityInstances[activityId];
            Assert.NotNull(endActivityInstances);

            foreach (var instance in endActivityInstances)
                Assert.AreEqual(completing, instance.IsCompleteScope);

            if (expectedCount != -1)
                Assert.AreEqual(expectedCount, endActivityInstances.Count);
        }

        public class ActivityInstance
        {
            private readonly ActivityInstanceVerification _outerInstance;
            internal string ExecutionId;


            internal string Id;
            internal bool IsCompleteScope;
            internal string ParentId;

            public ActivityInstance(ActivityInstanceVerification outerInstance, string executionId, string actInstanceId,
                string parentId, bool isCompleteScope)
            {
                _outerInstance = outerInstance;
                Id = actInstanceId;
                ExecutionId = executionId;
                ParentId = parentId;
                IsCompleteScope = isCompleteScope;
            }

            public override string ToString()
            {
                return Id + " by " + ExecutionId + " parent: " + ParentId;
            }
        }
    }
}