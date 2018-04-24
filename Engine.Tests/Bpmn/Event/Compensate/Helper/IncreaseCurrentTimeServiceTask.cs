using System;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Util;

namespace Engine.Tests.Bpmn.Event.Compensate.Helper
{
    public class IncreaseCurrentTimeServiceTask : IJavaDelegate
    {
        public void Execute(IBaseDelegateExecution execution)
        {
            DateTime currentTime = (DateTime)execution.GetVariable("currentTime");
            //currentTime = DateUtils.AddSeconds(currentTime, 1);
            currentTime = currentTime.AddSeconds(1);
            ClockUtil.CurrentTime = currentTime;
            execution.SetVariable("currentTime", currentTime);
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public void execute(org.Camunda.bpm.Engine.Delegate.IDelegateExecution execution) throws Exception
        //public virtual void execute(IDelegateExecution execution)
        //{
        //    DateTime currentTime = (DateTime)execution.GetVariable("currentTime");
        //    currentTime = DateUtils.AddSeconds(currentTime, 1);
        //    ClockUtil.CurrentTime = currentTime;
        //    execution.SetVariable("currentTime", currentTime);
        //}

    }

}