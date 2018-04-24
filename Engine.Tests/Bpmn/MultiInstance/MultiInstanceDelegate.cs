using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Bpmn.MultiInstance
{
    /// <summary>
    /// </summary>
    public class MultiInstanceDelegate : IJavaDelegate
    {
        public virtual void Execute(IBaseDelegateExecution execution)
        {
            var result = (int?) execution.GetVariable("result");

            var item = (int?) execution.GetVariable("item");
            if (item != null)
                result = result * item;
            else
                result = result * 2;
            execution.SetVariable("result", result);
        }
    }
}