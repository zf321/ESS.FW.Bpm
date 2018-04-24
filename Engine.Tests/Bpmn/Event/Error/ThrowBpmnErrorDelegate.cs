using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Bpmn.Event.Error
{

    public class ThrowBpmnErrorDelegate : IJavaDelegate
    {
        public void Execute(IBaseDelegateExecution execution)
        {
            int? executionsBeforeError = (int?)execution.GetVariable("executionsBeforeError");
            int? executions = (int?)execution.GetVariable("executions");
            bool? exceptionType = (bool?)execution.GetVariable("exceptionType");
            if (executions == null)
            {
                executions = 0;
            }
            executions++;
            if (executionsBeforeError == null || executionsBeforeError < executions)
            {
                if (exceptionType.HasValue && exceptionType.Value)
                {
                    throw new MyBusinessException("This is a business exception, which can be caught by a BPMN Error Event.");
                }
                else
                {
                    throw new BpmnError("23", "This is a business fault, which can be caught by a BPMN Error Event.");
                }
            }
            else
            {
                execution.SetVariable("executions", executions);
            }
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public void execute(org.Camunda.bpm.engine.delegate.IDelegateExecution execution) throws Exception
        //public virtual void execute(IDelegateExecution execution)
        //{
        //    int? executionsBeforeError = (int?)execution.GetVariable("executionsBeforeError");
        //    int? executions = (int?)execution.GetVariable("executions");
        //    bool? exceptionType = (bool?)execution.GetVariable("exceptionType");
        //    if (executions == null)
        //    {
        //        executions = 0;
        //    }
        //    executions++;
        //    if (executionsBeforeError == null || executionsBeforeError < executions)
        //    {
        //        if (exceptionType != null && exceptionType)
        //        {
        //            throw new MyBusinessException("This is a business exception, which can be caught by a BPMN Error Event.");
        //        }
        //        else
        //        {
        //            throw new BpmnError("23", "This is a business fault, which can be caught by a BPMN Error Event.");
        //        }
        //    }
        //    else
        //    {
        //        execution.SetVariable("executions", executions);
        //    }
        //}

    }

}