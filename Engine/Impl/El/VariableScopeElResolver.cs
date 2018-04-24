using System;
using System.Collections.Generic;
using System.Reflection;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Javax.EL;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.EL
{

    /// <summary>
    ///     Implementation of an <seealso cref="ELResolver" /> that resolves expressions
    ///     with the process variables of a given <seealso cref="IVariableScope" /> as context.
    ///     <br>
    ///         Also exposes the currently logged in username to be used in expressions (if any)
    ///         
    ///         
    /// </summary>
    public class VariableScopeElResolver : ELResolver
    {
        public const string ExecutionKey = "execution";
        public const string CaseExecutionKey = "caseExecution";
        public const string TaskKey = "task";
        public const string LoggedInUserKey = "authenticatedUserId";

        public override object GetValue(ELContext context, object @base, object property)
        {
            var @object = context.GetContext(typeof(IVariableScope));
            if (@object != null)
            {
                var variableScope = (IVariableScope) @object;
                if (@base == null)
                {
                    var variable = (string)property; // according to javadoc, can only be a String

                    if ((ExecutionKey.Equals(property) && variableScope is ExecutionEntity) ||
                        (TaskKey.Equals(property) && variableScope is TaskEntity) ||
                        (/*variableScope is CaseExecutionEntity &&*/
                         (CaseExecutionKey.Equals(property) || ExecutionKey.Equals(property))))
                    {
                        context.PropertyResolved = true;
                        return variableScope;
                    }
                    if (ExecutionKey.Equals(property) && variableScope is TaskEntity)
                    {
                        context.PropertyResolved = true;
                        return ((TaskEntity)variableScope).Execution;
                    }
                    if (LoggedInUserKey.Equals(property))
                    {
                        context.PropertyResolved = true;
                        return Context.CommandContext.AuthenticatedUserId;
                    }
                    if (variableScope.HasVariable(variable))
                    {
                        context.PropertyResolved = true;
                        // if not set, the next elResolver in the CompositeElResolver will be called
                        return variableScope.GetVariable(variable);
                    }
                }
            }

            // property resolution (eg. bean.value) will be done by the BeanElResolver (part of the CompositeElResolver)
            // It will use the bean resolved in this resolver as base.

            return null;
        }

        public override bool IsReadOnly(ELContext context, object @base, object property)
        {
            if (@base == null)
            {
                var variable = (string) property;
                var @object = context.GetContext(typeof(IVariableScope));
                return (@object != null) && !((IVariableScope) @object).HasVariable(variable);
            }
            return true;
        }

        public override void SetValue(ELContext context, object @base, object property, object value)
        {
            if (@base == null)
            {
                var variable = (string) property;
                var @object = context.GetContext(typeof(IVariableScope));
                if (@object != null)
                {
                    var variableScope = (IVariableScope) @object;
                    if (variableScope.HasVariable(variable))
                        variableScope.SetVariable(variable, value);
                }
            }
        }

        public override Type GetCommonPropertyType(ELContext arg0, object arg1)
        {
            return typeof(object);
        }

        public override IEnumerator<MemberInfo> GetMemberInfos(ELContext arg0, object arg1)
        {
            return null;
        }

        public override Type GetType(ELContext arg0, object arg1, object arg2)
        {
            return typeof(object);
        }
    }
}