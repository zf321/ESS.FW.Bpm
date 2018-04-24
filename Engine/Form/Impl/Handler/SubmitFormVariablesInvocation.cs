
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Delegate;
using ESS.FW.Bpm.Engine.Variable;

namespace ESS.FW.Bpm.Engine.Form.Impl.Handler
{
    /// <summary>
    ///     
    /// </summary>
    public class SubmitFormVariablesInvocation : DelegateInvocation
    {
        protected internal IFormHandler FormHandler;
        protected internal IVariableMap Properties;
        protected internal IVariableScope VariableScope;


        public SubmitFormVariablesInvocation(IFormHandler formHandler, IVariableMap properties,
            IVariableScope variableScope) : base(null, null)
        {
            this.FormHandler = formHandler;
            this.Properties = properties;
            this.VariableScope = variableScope;
        }

//JAVA TO C# CONVERTER WARNING: MethodInfo 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void invoke() throws Exception
        protected internal override void Invoke()
        {
            FormHandler.SubmitFormVariables(Properties, VariableScope);
        }
    }
}