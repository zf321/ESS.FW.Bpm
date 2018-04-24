using System.Linq;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Task;

namespace Engine.Tests.Bpmn.Event.Conditional
{

    public class SetVariableOnConcurrentExecutionDelegate : IJavaDelegate
    {
        public void Execute(IBaseDelegateExecution execution)
        {
            ITask task = (execution as IDelegateExecution).ProcessEngineServices.TaskService.CreateTaskQuery(c=>c.Name ==AbstractConditionalEventTestCase.TaskWithCondition).First();
            ((TaskEntity)task).Execution.SetVariableLocal(AbstractConditionalEventTestCase.VariableName, 1);
            execution.SetVariableLocal(AbstractConditionalEventTestCase.VariableName + 1, 1);
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: @Override public void execute(org.Camunda.bpm.Engine.Delegate.IDelegateExecution execution) throws Exception
        //public void Execute(IBaseDelegateExecution execution)
        //{
        //    ITask task = execution.ProcessEngineServices.TaskService.CreateTaskQuery(c=>c.Name ==TaskWithCondition).First();
        //    ((TaskEntity)task).Execution.SetVariableLocal(VARIABLE_NAME, 1);
        //    execution.SetVariableLocal(VARIABLE_NAME + 1, 1);
        //}
    }

}