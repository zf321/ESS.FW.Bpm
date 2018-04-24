using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Bpmn.ServiceTask.Util
{
    public class ToUppercase : IJavaDelegate
	{

	  private const string VariableName = "input";
         

        public void Execute(IBaseDelegateExecution execution)
        {
            string @var = (string)execution.GetVariable(VariableName);
            @var = @var.ToUpper();
            execution.SetVariable(VariableName, @var);
        }
    }

}