using ESS.FW.Bpm.Engine.Impl.Delegate;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Repository.Impl;

namespace ESS.FW.Bpm.Engine.Form.Impl.Handler
{
    /// <summary>
    ///     
    /// </summary>
    public class CreateStartFormInvocation : DelegateInvocation
    {
        protected internal ProcessDefinitionEntity Definition;

        protected internal IStartFormHandler StartFormHandler;

        public CreateStartFormInvocation(IStartFormHandler startFormHandler, ProcessDefinitionEntity definition)
            : base(null, (IResourceDefinitionEntity) definition)
        {
            this.StartFormHandler = startFormHandler;
            this.Definition = definition;
        }

//JAVA TO C# CONVERTER WARNING: MethodInfo 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void invoke() throws Exception
        protected internal override void Invoke()
        {
            InvocationResult = StartFormHandler.CreateStartFormData(Definition);
        }
    }
}