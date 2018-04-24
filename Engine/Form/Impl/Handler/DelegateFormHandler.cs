using ESS.FW.Bpm.Engine.Application;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Parser;
using ESS.FW.Bpm.Engine.Impl.Util.xml;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using System;

namespace ESS.FW.Bpm.Engine.Form.Impl.Handler
{
    /// <summary>
    ///     
    /// </summary>
    public abstract class DelegateFormHandler
    {
        protected internal string deploymentId;
        protected internal IFormHandler formHandler;

        public DelegateFormHandler(IFormHandler formHandler, string deploymentId)
        {
            this.formHandler = formHandler;
            this.deploymentId = deploymentId;
        }

        public abstract IFormHandler FormHandler { get; }

        public virtual void ParseConfiguration(Element activityElement, DeploymentEntity deployment,
            ProcessDefinitionEntity processDefinition, BpmnParse bpmnParse)
        {
            // should not be called!
        }

        //protected internal virtual T doCall<T>(Callable<T> callable)
        //{
        //    try
        //    {

        //        return callable.call();
        //    }
        //    catch (Exception e)
        //    {
        //        throw new ProcessEngineException(e);
        //    }
        //}

        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: public void submitFormVariables(final org.camunda.bpm.engine.Variable.VariableMap properties, final org.camunda.bpm.engine.delegate.VariableScope variableScope)
        //public virtual void submitFormVariables(IVariableMap properties, IVariableScope variableScope)
        //{
        //    performContextSwitch(new CallableAnonymousInnerClass2(this, properties, variableScope));
        //}

        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: protected <T> T performContextSwitch(final java.Util.concurrent.Callable<T> callable)
        protected internal virtual T PerformContextSwitch<T>(Func<T> callable)
        {
            IProcessApplicationReference targetProcessApplication = ProcessApplicationContextUtil.GetTargetProcessApplication(deploymentId);

            if (targetProcessApplication != null)
            {

                return Context.ExecuteWithinProcessApplication(/*new CallableAnonymousInnerClass(this, callable),*/
                    callable,targetProcessApplication);

            }
            else
            {
                return callable.Invoke();
            }
        }

        //private class CallableAnonymousInnerClass : ICallable<T>
        //{
        //    private readonly DelegateFormHandler outerInstance;

        //    //private readonly Callable<T> callable;

        //    public CallableAnonymousInnerClass(DelegateFormHandler outerInstance, Callable<T> callable)
        //    {
        //        this.outerInstance = outerInstance;
        //        this.callable = callable;
        //    }

        //    //JAVA TO C# CONVERTER WARNING: MethodInfo 'throws' clauses are not available in .NET:
        //    //ORIGINAL LINE: public T call() throws Exception
        //    public virtual T call()
        //    {
        //        return outerInstance.doCall(callable);
        //    }
        //}

        //        private class CallableAnonymousInnerClass2 /*: Callable<object>
        //        {
        //            private readonly DelegateFormHandler outerInstance;

        //            private readonly IVariableMap properties;
        //            private readonly IVariableScope variableScope;

        //            public CallableAnonymousInnerClass2(DelegateFormHandler outerInstance, IVariableMap properties,
        //                IVariableScope variableScope)
        //            {
        //                this.outerInstance = outerInstance;
        //                this.properties = properties;
        //                this.variableScope = variableScope;
        //            }

        ////JAVA TO C# CONVERTER WARNING: MethodInfo 'throws' clauses are not available in .NET:
        ////ORIGINAL LINE: public void call() throws Exception
        //            public virtual void call()
        //            {
        //                Context.ProcessEngineConfiguration.DelegateInterceptor.handleInvocation(
        //                    new SubmitFormVariablesInvocation(outerInstance.formHandler, properties, variableScope));

        //                return null;
        //            }
    }
}