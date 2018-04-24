using System;
using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Util;

namespace Engine.Tests.Standalone.History
{
    /// <summary>
    /// </summary>
    public class VariableSetter : IJavaDelegate
    {
        public virtual void Execute(IBaseDelegateExecution execution)
        {
            //string sdf = new string("dd/MM/yyyy hh:mm:ss SSS");
            // We set the time to check of the updated time is picked up in the history
            var updatedDate = DateTime.Parse("01/01/2001 01:23:46 000");
            ClockUtil.CurrentTime = updatedDate;


            execution.SetVariable("aVariable", "updated value");
            execution.SetVariable("bVariable", 123);
            execution.SetVariable("cVariable", 12345L);
            execution.SetVariable("dVariable", 1234.567);
            execution.SetVariable("eVariable", (short) 12);

            var theDate = DateTime.Parse("01/01/2001 01:23:45 678");
            execution.SetVariable("fVariable", theDate);

            //execution.SetVariable("gVariable", new SerializableVariable("hello hello"));
            execution.SetVariable("hVariable", ";-)".GetBytes());
        }
    }
}