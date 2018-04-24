using System;
using ESS.FW.Bpm.Engine.Application;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Form.Impl.Validator
{
    /// <summary>
    ///     <seealso cref="IFormFieldValidator" /> delegating to a custom, user-provided validator implementation.
    ///     The implementation is resolved either using a fully qualified classname of a Java Class
    ///     or using a java delegate implementation.
    ///     
    /// </summary>
    public class DelegateFormFieldValidator : IFormFieldValidator
    {
        protected internal string Clazz;
        protected internal IExpression DelegateExpression;

        public DelegateFormFieldValidator(IExpression expression)
        {
            DelegateExpression = expression;
        }

        public DelegateFormFieldValidator(string clazz)
        {
            this.Clazz = clazz;
        }

        public DelegateFormFieldValidator()
        {
        }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: @Override public boolean validate(final Object submittedValue, final FormFieldValidatorContext validatorContext)
        public virtual bool Validate(object submittedValue, IFormFieldValidatorContext validatorContext)
        {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.delegate.DelegateExecution execution = validatorContext.getExecution();
            var execution = validatorContext.Execution;

            if (ShouldPerformPaContextSwitch(validatorContext.Execution))
            {
                var processApplicationReference =
                    ProcessApplicationContextUtil.GetTargetProcessApplication((ExecutionEntity)execution);

                return
                    Context.ExecuteWithinProcessApplication(()=> DoValidate(submittedValue, validatorContext),
                        processApplicationReference, new InvocationContext(execution));
            }
            return DoValidate(submittedValue, validatorContext);
        }

        protected internal virtual bool ShouldPerformPaContextSwitch(IDelegateExecution execution)
        {
            throw new NotImplementedException();
            //if (execution == null)
            //    return false;
            //var targetPa =
            //    ProcessApplicationContextUtil.getTargetProcessApplication((ExecutionEntity) execution);
            //return (targetPa != null) && !targetPa.Equals(Context.CurrentProcessApplication);
        }

        protected internal virtual bool DoValidate(object submittedValue, IFormFieldValidatorContext validatorContext)
        {
            IFormFieldValidator validator;

            if (!ReferenceEquals(Clazz, null))
            {
                // resolve validator using Fully Qualified Classname
                var validatorObject = ReflectUtil.Instantiate(Clazz);
                if (validatorObject is IFormFieldValidator)
                    validator = (IFormFieldValidator) validatorObject;
                else
                    throw new ProcessEngineException("Validator class '" + Clazz + "' is not an instance of " +
                                                     typeof(IFormFieldValidator).FullName);
            }
            else
            {
                //resolve validator using expression
                var validatorObject = DelegateExpression.GetValue(validatorContext.Execution);
                if (validatorObject is IFormFieldValidator)
                    validator = (IFormFieldValidator) validatorObject;
                else
                    throw new ProcessEngineException("Validator expression '" + DelegateExpression +
                                                     "' does not resolve to instance of " +
                                                     typeof(IFormFieldValidator).FullName);
            }

            var invocation = new FormFieldValidatorInvocation(validator, submittedValue, validatorContext);
            try
            {
                Context.ProcessEngineConfiguration.DelegateInterceptor.HandleInvocation(invocation);
            }
            catch (System.Exception e)
            {
                throw new ProcessEngineException(e);
            }

            //return invocation.InvocationResult.Value;
            return true;
        }
        
    }
}