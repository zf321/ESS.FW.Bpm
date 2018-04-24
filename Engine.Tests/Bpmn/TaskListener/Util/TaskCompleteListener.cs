

using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Bpmn.TaskListener.Util
{

    /// <summary>
    /// 
    /// </summary>
    public class TaskCompleteListener : ITaskListener
    {

        private IExpression greeter;
        private IExpression shortName;

        public virtual void Notify(IDelegateTask delegateTask)
        {
            delegateTask.Execution.SetVariable("greeting", "Hello from " + greeter.GetValue(delegateTask.Execution));
            delegateTask.Execution.SetVariable("shortName", shortName.GetValue(delegateTask.Execution));

            delegateTask.SetVariableLocal("myTaskVariable", "test");
        }

    }

}