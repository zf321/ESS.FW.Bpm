using System;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Parser;
using ESS.FW.Bpm.Engine.Impl.Util.xml;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Variable;


namespace ESS.FW.Bpm.Engine.Form.Impl.Handler
{
    /// <summary>
    ///     
    /// </summary>
    public class DelegateStartFormHandler : DelegateFormHandler, IStartFormHandler
    {
        public DelegateStartFormHandler(IStartFormHandler formHandler, DeploymentEntity deployment)
            : base(formHandler, deployment.Id)
        {
        }

        public override IFormHandler FormHandler
        {
            get { return formHandler; }
        }

        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: public org.camunda.bpm.engine.form.StartFormData createStartFormData(final org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity processDefinition)
        public virtual IStartFormData CreateStartFormData(Persistence.Entity.ProcessDefinitionEntity processDefinition)
        {
            return null;
            //return performContextSwitch(new CallableAnonymousInnerClass(this, processDefinition));
        }


        public void SubmitFormVariables(IVariableMap properties, IVariableScope variableScope)
        {
            PerformContextSwitch<object>(() =>
            {
                Context.ProcessEngineConfiguration
                .DelegateInterceptor
                .HandleInvocation(new SubmitFormVariablesInvocation(formHandler, properties, variableScope));
                return null;
            });
        }

        //private class CallableAnonymousInnerClass //: Callable<StartFormData>
        //{
        //    private readonly DelegateStartFormHandler _outerInstance;

        //    private readonly Persistence.Entity.ProcessDefinitionEntity _processDefinition;

        //    public CallableAnonymousInnerClass(DelegateStartFormHandler outerInstance,
        //        Persistence.Entity.ProcessDefinitionEntity processDefinition)
        //    {
        //        this._outerInstance = outerInstance;
        //        this._processDefinition = processDefinition;
        //    }

        //    //JAVA TO C# CONVERTER WARNING: MethodInfo 'throws' clauses are not available in .NET:
        //    //ORIGINAL LINE: public org.camunda.bpm.engine.form.StartFormData call() throws Exception
        //    public virtual IStartFormData Call()
        //    {
        //        var invocation = new CreateStartFormInvocation((IStartFormHandler)_outerInstance.formHandler,
        //            _processDefinition);
        //        Context.ProcessEngineConfiguration.DelegateInterceptor.HandleInvocation(invocation);
        //        return (IStartFormData)invocation.InvocationResult;
        //    }
        //}
    }
}