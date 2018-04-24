using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Bpmn.ServiceTask.Util
{
    public class ToUpperCaseFieldInjected : IJavaDelegate
	{
	    public IExpression Text { get; set;}
         

        public void Execute(IBaseDelegateExecution execution)
        {
            execution.SetVariable("var", ((string)Text.GetValue(execution)).ToUpper());
        }
    }

}