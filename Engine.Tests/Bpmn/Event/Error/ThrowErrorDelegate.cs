using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;

namespace Engine.Tests.Bpmn.Event.Error
{

    [Serializable]
    public class ThrowErrorDelegate : AbstractBpmnActivityBehavior
    {

        public const long serialVersionUID = 1L;

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: @Override public void execute(org.Camunda.bpm.engine.impl.pvm.delegate.ActivityExecution execution) throws Exception
        //public override void execute(ActivityExecution execution)
        //{
        //    handle(execution, "executed");
        //}

        public override void Execute(IActivityExecution execution)
        {
            handle(execution, "executed");
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: @Override public void signal(org.Camunda.bpm.engine.impl.pvm.delegate.ActivityExecution execution, String signalName, Object signalData) throws Exception
        //public override void signal(ActivityExecution execution, string signalName, object signalData)
        //{
        //    handle(execution, "signaled");
        //}

        public override void Signal(IActivityExecution execution, string signalName, object signalData)
        {
            handle(execution, "signaled");
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: protected void handle(org.Camunda.bpm.engine.impl.pvm.delegate.ActivityExecution execution, String action) throws Exception
        protected internal virtual void handle(IActivityExecution execution, string action)
        {
            execution.SetVariable(action, true);
            string type = (string)execution.GetVariable("type");
            if ("error".Equals(type, StringComparison.CurrentCultureIgnoreCase))
            {
                throw new BpmnError("MyError");
            }
            else if ("exception".Equals(type, StringComparison.CurrentCultureIgnoreCase))
            {
                throw new MyBusinessException("MyException");
            }
            else if ("leave".Equals(type, StringComparison.CurrentCultureIgnoreCase))
            {
                execution.SetVariable("type", null);
                Leave(execution);
            }
        }

        public static IDictionary<string, object> throwError()
        {
            return Collections.SingletonMap("type", (object)"error");
        }

        public static IDictionary<string, object> throwException()
        {
            return Collections.SingletonMap("type", (object)"exception");
        }

        public static IDictionary<string, object> leaveExecution()
        {
            return Collections.SingletonMap("type", (object)"leave");
        }

    }

}