using System;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Impl.Core.Variable.mapping.value;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl
{

    /// <summary>
    ///     Represents a default priority provider, which contains some functionality to evaluate the priority.
    ///     Can be used as base class for other priority providers.
    ///      
    /// </summary>
    /// @param
    /// <T> the type of the param to determine the priority </param>
    public abstract class DefaultPriorityProvider<T> : IPriorityProvider<T>
    {
        /// <summary>
        ///     The default priority.
        /// </summary>
        public static long DEFAULT_PRIORITY = 0;

        /// <summary>
        ///     The default priority in case of resolution failure.
        /// </summary>
        public static long DEFAULT_PRIORITY_ON_RESOLUTION_FAILURE = 0;

        /// <summary>
        ///     Returns the default priority.
        /// </summary>
        /// <returns> the default priority </returns>
        public virtual long DefaultPriority
        {
            get { return DEFAULT_PRIORITY; }
        }

        /// <summary>
        ///     Returns the default priority in case of resolution failure.
        /// </summary>
        /// <returns> the default priority </returns>
        public virtual long DefaultPriorityOnResolutionFailure
        {
            get { return DEFAULT_PRIORITY_ON_RESOLUTION_FAILURE; }
        }

        public virtual long DeterminePriority(ExecutionEntity execution, T param, string jobDefinitionId)
        {
            if ((param != null) || (execution != null))
            {
                var specificPriority = GetSpecificPriority(execution, param, jobDefinitionId);
                if (specificPriority != null)
                    return specificPriority.Value;

                var processDefinitionPriority = GetProcessDefinitionPriority(execution, param);
                if (processDefinitionPriority != null)
                    return processDefinitionPriority.Value;
            }
            return DefaultPriority;
        }

        /// <summary>
        ///     Evaluates a given value provider with the given execution entity to determine
        ///     the correct value. The error message heading is used for the error message
        ///     if the validation fails because the value is no valid priority.
        /// </summary>
        /// <param name="valueProvider"> the provider which contains the value </param>
        /// <param name="execution"> the execution entity </param>
        /// <param name="errorMessageHeading"> the heading which is used for the error message </param>
        /// <returns> the valid priority value </returns>
        protected internal virtual long EvaluateValueProvider(IParameterValueProvider valueProvider,
            ExecutionEntity execution, string errorMessageHeading)
        {
            object value;
            try
            {
                value = valueProvider.GetValue(execution);
            }
            catch (ProcessEngineException e)
            {
                if (Context.ProcessEngineConfiguration.EnableGracefulDegradationOnContextSwitchFailure &&
                    IsSymptomOfContextSwitchFailure(e, execution))
                {
                    value = DefaultPriorityOnResolutionFailure;
                    LogNotDeterminingPriority(execution, value, e);
                }
                else
                {
                    throw e;
                }
            }

            long numberValue;
            try
            {
                numberValue = long.Parse(value.ToString());
                return numberValue;                
            }
            catch (System.Exception ex)
            {
                throw new ProcessEngineException(errorMessageHeading + " " + ex.Message + ": Priority value is not an Integer," + value);
            }



            //long numberValue;
            ////if (!(value is int||value is decimal))
            //try
            //{
            //    numberValue = long.Parse(value.ToString());
            //    return numberValue;
            //}
            //catch (Exception e)
            //{
            //    throw new ProcessEngineException(errorMessageHeading + " " + e.Message + ": Priority value is not an Integer," + value);
            //}
            //else
            //{
            //decimal numberValue = (decimal)value;
            //if (IsValidLongValue(numberValue))
            //{
            //    return (long?)numberValue;
            //}
            //else
            //{
            //    throw new ProcessEngineException(errorMessageHeading + ": Priority value must be either Short, Integer, or Long");
            //}
            //}
        }

        /// <summary>
        ///     Returns the priority defined in the specific entity. Like a job definition priority or
        ///     an activity priority. The result can also be null in that case the process
        ///     priority will be used.
        /// </summary>
        /// <param name="execution"> the current execution </param>
        /// <param name="param"> the generic param </param>
        /// <param name="jobDefinitionId"> the job definition id if related to a job </param>
        /// <returns> the specific priority </returns>
        protected internal abstract long? GetSpecificPriority(ExecutionEntity execution, T param, string jobDefinitionId);

        /// <summary>
        ///     Returns the priority defined in the process definition. Can also be null
        ///     in that case the fallback is the default priority.
        /// </summary>
        /// <param name="execution"> the current execution </param>
        /// <param name="param"> the generic param </param>
        /// <returns> the priority defined in the process definition </returns>
        protected internal abstract long? GetProcessDefinitionPriority(ExecutionEntity execution, T param);

        /// <summary>
        ///     Returns the priority which is defined in the given process definition.
        ///     The priority value is identified with the given propertyKey.
        ///     Returns null if the process definition is null or no priority was defined.
        /// </summary>
        /// <param name="processDefinition"> the process definition that should contains the priority </param>
        /// <param name="propertyKey"> the key which identifies the property </param>
        /// <param name="execution"> the current execution </param>
        /// <param name="errorMsgHead"> the error message header which is used if the evaluation fails </param>
        /// <returns> the priority defined in the given process </returns>
        protected internal virtual long? GetProcessDefinedPriority(ProcessDefinitionImpl processDefinition,
            string propertyKey, ExecutionEntity execution, string errorMsgHead)
        {
            if (processDefinition != null)
            {
                var priorityProvider = (IParameterValueProvider)processDefinition.GetProperty(propertyKey);
                if (priorityProvider != null)
                    return EvaluateValueProvider(priorityProvider, execution, errorMsgHead);
            }
            return null;
        }

        /// <summary>
        ///     Logs the exception which was thrown if the priority can not be determined.
        /// </summary>
        /// <param name="execution"> the current execution entity </param>
        /// <param name="value"> the current value </param>
        /// <param name="e"> the exception which was catched </param>
        protected internal abstract void LogNotDeterminingPriority(ExecutionEntity execution, object value,
            ProcessEngineException e);


        protected internal virtual bool IsSymptomOfContextSwitchFailure(System.Exception t, ExecutionEntity contextExecution)
        {
            //throw new NotImplementedException();
            // a context switch failure can occur, if the current engine has no PA registration for the deployment
            // subclasses may assert the actual throwable to narrow down the diagnose
            return ProcessApplicationContextUtil.GetTargetProcessApplication(contextExecution) == null;
        }

        /// <summary>
        ///     Checks if the given number is a valid long value.
        /// </summary>
        /// <param name="value"> the number which should be checked </param>
        /// <returns> true if is a valid long value, false otherwise </returns>
        protected internal virtual bool IsValidLongValue(object value)
        {
            return value is short? || value is int? || value is long?;
        }
    }
}