using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Parser;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.Util
{

    /// <summary>
	/// 
	/// 
	/// </summary>
	public class ActivityInstanceAssert
    {

        public class ActivityInstanceAssertThatClause
        {

            protected internal IActivityInstance Actual;

            public ActivityInstanceAssertThatClause(IActivityInstance actual)
            {
                this.Actual = actual;
            }

            public virtual void HasStructure(IActivityInstance expected)
            {
                AssertTreeMatch(expected, Actual);
            }

            protected internal virtual void AssertTreeMatch(IActivityInstance expected, IActivityInstance actual)
            {
                bool treesMatch = IsTreeMatched(expected, actual);
                if (!treesMatch)
                {
                    Assert.Fail("Could not match expected tree \n" + expected + " \n\n with actual tree \n\n " + actual);
                }

            }


            /// <summary>
            /// if anyone wants to improve this algorithm, feel welcome! </summary>
            protected internal virtual bool IsTreeMatched(IActivityInstance expectedInstance, IActivityInstance actualInstance)
            {
                if (expectedInstance.ActivityId!=actualInstance.ActivityId || (expectedInstance.Id != null && expectedInstance.Id!=actualInstance.Id))
                {
                    return false;
                }
                else
                {
                    if (expectedInstance.ChildActivityInstances.Length != actualInstance.ChildActivityInstances.Length)
                    {
                        return false;
                    }
                    else
                    {

                        IList<IActivityInstance> unmatchedInstances = new List<IActivityInstance>((expectedInstance.ChildActivityInstances));
                        foreach (IActivityInstance actualChild in actualInstance.ChildActivityInstances)
                        {
                            bool matchFound = false;
                            foreach (IActivityInstance expectedChild in unmatchedInstances)
                            {
                                if (IsTreeMatched(expectedChild, actualChild))
                                {
                                    unmatchedInstances.Remove(actualChild);
                                    matchFound = true;
                                    break;
                                }
                            }
                            if (!matchFound)
                            {
                                return false;
                            }
                        }

                        if (expectedInstance.ChildTransitionInstances.Length != actualInstance.ChildTransitionInstances.Length)
                        {
                            return false;
                        }

                        IList<ITransitionInstance> unmatchedTransitionInstances = new List<ITransitionInstance>((expectedInstance.ChildTransitionInstances));
                        foreach (ITransitionInstance child in actualInstance.ChildTransitionInstances)
                        {
                            IEnumerator<ITransitionInstance> expectedTransitionInstanceIt = unmatchedTransitionInstances.GetEnumerator();

                            bool matchFound = false;
                            while (expectedTransitionInstanceIt.MoveNext() && !matchFound)
                            {
                                ITransitionInstance expectedChild = expectedTransitionInstanceIt.Current;
                                if (expectedChild.ActivityId.Equals(child.ActivityId))
                                {
                                    matchFound = true;
                                    //JAVA TO C# CONVERTER TODO Resources.Task: .NET enumerators are read-only:
                                    //expectedTransitionInstanceIt.Remove();
                                }
                            }

                            if (!matchFound)
                            {
                                return false;
                            }
                        }

                    }
                    return true;

                }
            }

        }

        public class ActivityInstanceTreeBuilder
        {

            protected internal ActivityInstanceImpl RootInstance = null;
            protected internal Stack<ActivityInstanceImpl> ActivityInstanceStack = new Stack<ActivityInstanceImpl>();

            public ActivityInstanceTreeBuilder() : this(null)
            {
            }

            public ActivityInstanceTreeBuilder(string rootActivityId)
            {
                RootInstance = new ActivityInstanceImpl();
                RootInstance.ActivityId = rootActivityId;
                ActivityInstanceStack.Push(RootInstance);
            }

            public virtual ActivityInstanceTreeBuilder BeginScope(string activityId)
            {
                return BeginScope(activityId, null);
            }

            public virtual ActivityInstanceTreeBuilder BeginScope(string activityId, string activityInstanceId)
            {
                ActivityInstanceImpl newInstance = new ActivityInstanceImpl();
                newInstance.ActivityId = activityId;
                newInstance.Id = activityInstanceId;

                ActivityInstanceImpl parentInstance = ActivityInstanceStack.Peek();
                IList<IActivityInstance> childInstances = new List<IActivityInstance>((parentInstance.ChildActivityInstances));
                childInstances.Add(newInstance);
                parentInstance.ChildActivityInstances = childInstances.ToArray();

                ActivityInstanceStack.Push(newInstance);

                return this;
            }

            public virtual ActivityInstanceTreeBuilder BeginMiBody(string activityId)
            {
                return BeginScope(activityId + BpmnParse.MultiInstanceBodyIdSuffix, null);
            }

            public virtual ActivityInstanceTreeBuilder BeginMiBody(string activityId, string activityInstanceId)
            {
                return BeginScope(activityId + BpmnParse.MultiInstanceBodyIdSuffix, activityInstanceId);
            }

            public virtual ActivityInstanceTreeBuilder Activity(string activityId)
            {

                return Activity(activityId, null);
            }

            public virtual ActivityInstanceTreeBuilder Activity(string activityId, string activityInstanceId)
            {

                BeginScope(activityId);
                Id(activityInstanceId);
                EndScope();

                return this;
            }

            public virtual ActivityInstanceTreeBuilder Transition(string activityId)
            {

                TransitionInstanceImpl newInstance = new TransitionInstanceImpl();
                newInstance.ActivityId = activityId;
                ActivityInstanceImpl parentInstance = ActivityInstanceStack.Peek();

                IList<ITransitionInstance> childInstances = new List<ITransitionInstance>((parentInstance.ChildTransitionInstances));
                childInstances.Add(newInstance);
                parentInstance.ChildTransitionInstances = childInstances.ToArray();

                return this;
            }

            public virtual ActivityInstanceTreeBuilder EndScope()
            {
                ActivityInstanceStack.Pop();
                return this;
            }

            public virtual IActivityInstance Done()
            {
                return RootInstance;
            }

            protected internal virtual ActivityInstanceTreeBuilder Id(string expectedActivityInstanceId)
            {
                ActivityInstanceImpl activityInstanceImpl = ActivityInstanceStack.Peek();
                activityInstanceImpl.Id = expectedActivityInstanceId;
                return this;
            }
        }

        public static ActivityInstanceTreeBuilder DescribeActivityInstanceTree()
        {
            return new ActivityInstanceTreeBuilder();
        }

        public static ActivityInstanceTreeBuilder DescribeActivityInstanceTree(string rootActivityId)
        {
            return new ActivityInstanceTreeBuilder(rootActivityId);
        }

        public static ActivityInstanceAssertThatClause That(IActivityInstance actual)
        {
            return new ActivityInstanceAssertThatClause(actual);
        }

    }

}