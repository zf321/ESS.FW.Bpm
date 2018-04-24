using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Bpmn.MultiInstance
{
    /// <summary>
    /// </summary>
    public class TaskCompletionListener : ITaskListener
    {
        public virtual void Notify(IDelegateTask delegateTask)
        {
            var counter = (int?) delegateTask.GetVariable("taskListenerCounter");
            if (counter == null)
                counter = 0;
            delegateTask.SetVariable("taskListenerCounter", ++counter);
        }
    }
}