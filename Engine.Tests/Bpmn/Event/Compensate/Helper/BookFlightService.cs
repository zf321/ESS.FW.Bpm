using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Bpmn.Event.Compensate.Helper
{
    public class BookFlightService : IJavaDelegate
    {

        public static IList<string> bookedFlights = new List<string>();

        public void Execute(IBaseDelegateExecution execution)
        {
            string flight = (string)execution.GetVariable("flight");

            if (!string.ReferenceEquals(flight, null))
            {
                bookedFlights.Add(flight);
            }
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: @Override public void execute(org.Camunda.bpm.Engine.Delegate.IDelegateExecution execution) throws Exception
        //public void Execute(IBaseDelegateExecution execution)
        //{
        //    string flight = (string)execution.GetVariable("flight");

        //    if (!string.ReferenceEquals(flight, null))
        //    {
        //        bookedFlights.Add(flight);
        //    }
        //}

    }

}