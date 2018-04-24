using Engine.Tests.History;
using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Api.Runtime.Util
{
    /// <summary>
    /// </summary>
    public class ChangeVariablesDelegate : IJavaDelegate
    {
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void execute(org.Camunda.bpm.Engine.Delegate.IDelegateExecution execution) throws Exception
        public virtual void Execute(IBaseDelegateExecution execution)
        {
            // first set variable to some string
            execution.SetVariable("VariableName", "test");

            // now set to serializable
            execution.SetVariable("VariableName", new SerializableVariable("foo"));
        }
    }
}