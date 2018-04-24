
using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Bpmn.TaskListener.Util
{

    /// <summary>
    /// 
    /// </summary>
    public class TaskDeleteListener : ITaskListener
    {

        public static int EventCounter = 0;
        public static string LastTaskDefinitionKey = null;
        public static string LastDeleteReason = null;

        public virtual void Notify(IDelegateTask delegateTask)
        {
            TaskDeleteListener.EventCounter++;
            TaskDeleteListener.LastTaskDefinitionKey = delegateTask.TaskDefinitionKey;
            TaskDeleteListener.LastDeleteReason = delegateTask.DeleteReason;
        }

        public static void Clear()
        {
            EventCounter = 0;
            LastTaskDefinitionKey = null;
            LastDeleteReason = null;
        }

    }

}