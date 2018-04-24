using System.Linq;
using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Bpmn.ServiceTask.Util
{

    public class ReverseStringsFieldInjected : IJavaDelegate
	{

	  public IExpression Text1 { get; set; }
        public IExpression Text2 { get; set; }
        public IExpression Text3 { get; set; }


        public void Execute(IBaseDelegateExecution execution)
        {
            string value1 = (string)Text1.GetValue(execution);
            
            execution.SetVariable("var1", string.Join("",value1.Reverse()));
            
            string value2 = (string)Text2.GetValue(execution);
            execution.SetVariable("var2", string.Join("",value2.Reverse()));

            if (Text3 != null)
            {
                string value3 = (string)Text3.GetValue(execution);
                execution.SetVariable("var3", value3);
            }
        }
    }

}