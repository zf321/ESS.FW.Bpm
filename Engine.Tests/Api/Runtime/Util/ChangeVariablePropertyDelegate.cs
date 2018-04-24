using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Api.Runtime.Util
{
    /// <summary>
    /// </summary>
    public class ChangeVariablePropertyDelegate : IJavaDelegate
    {
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void execute(org.Camunda.bpm.Engine.Delegate.IDelegateExecution execution) throws Exception
        public virtual void Execute(IBaseDelegateExecution execution)
        {
            execution.SetVariableLocal("var", new SimpleSerializableBean());

            var variable = (SimpleSerializableBean) execution.GetVariable("var");
            variable.IntProperty = variable.IntProperty + 1;

            var shouldExplicitlyUpdateVariable = (bool) execution.GetVariable("shouldExplicitlyUpdateVariable");

            if (shouldExplicitlyUpdateVariable)
                execution.SetVariableLocal("var", variable);
        }
    }
}