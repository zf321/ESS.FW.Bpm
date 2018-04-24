using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Bpmn.ExecutionListener
{

    public class CurrentActivityExecutionListener : IDelegateListener<IBaseDelegateExecution>
    {

        private static IList<CurrentActivity> currentActivities = new List<CurrentActivity>();

        public class CurrentActivity
        {
            internal readonly string activityId;
            internal readonly string activityName;

            public CurrentActivity(string activityId, string activityName)
            {
                this.activityId = activityId;
                this.activityName = activityName;
            }

            public virtual string ActivityId
            {
                get
                {
                    return activityId;
                }
            }

            public virtual string ActivityName
            {
                get
                {
                    return activityName;
                }
            }
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public void notify(org.Camunda.bpm.engine.delegate.IDelegateExecution execution) throws Exception
        //public virtual void notify(IDelegateExecution execution)
        //{
        //    currentActivities.Add(new CurrentActivity(execution.CurrentActivityId, execution.CurrentActivityName));
        //}


        public void Notify(IBaseDelegateExecution execution)
        {
            currentActivities.Add(new CurrentActivity((execution as IDelegateExecution)?.CurrentActivityId, (execution as IDelegateExecution)?.CurrentActivityName));
        }

        public static IList<CurrentActivity> CurrentActivities
        {
            get
            {
                return currentActivities;
            }
        }

        public static void clear()
        {
            currentActivities.Clear();
        }

    }

}