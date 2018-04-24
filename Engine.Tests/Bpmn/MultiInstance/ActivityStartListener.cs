using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Bpmn.MultiInstance
{

	/// <summary>
	/// 
	/// </summary>
	public class ActivityStartListener : IDelegateListener<IBaseDelegateExecution>
    {
        public void Notify(IBaseDelegateExecution instance)
        {
            int? counter = (int?)instance.GetVariable("executionListenerCounter");
            if (counter == null)
            {
                counter = 0;
            }
            instance.SetVariable("executionListenerCounter", ++counter);
        }
    }

}