using System;
using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Bpmn.ServiceTask
{
	/// <summary>
	/// 
	/// </summary>
	public class DummyServiceTask :IJavaDelegate
    {
       
	 
        public void Execute(IBaseDelegateExecution execution)
        {
            int? Count = (int?)execution.GetVariable("Count");
            Count = Count + 1;
            Console.WriteLine("Count = " + Count);
            execution.SetVariable("Count", Count);
        }
    }

}