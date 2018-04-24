using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ESS.FW.Bpm.Engine.Application;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Parser;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Exceptions;

namespace ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior
{
    /// <summary>
    ///     Helper class for bpmn constructs that allow class delegation.
    ///     This class will lazily instantiate the referenced classes when needed at runtime.
    /// </summary>
    public class ClassMethodDelegateActivityBehavior : AbstractBpmnActivityBehavior
    {
        protected internal new static readonly BpmnBehaviorLogger Log = ProcessEngineLogger.BpmnBehaviorLogger;

        private readonly string _className;
        private readonly string _methodName;
        private readonly IList<FieldDeclaration> _fieldDeclarations;
        private readonly string _resultVariable;

        public ClassMethodDelegateActivityBehavior(string className, IList<FieldDeclaration> fieldDeclarations, string resultVariable)
        {
            //format  jzterp.service.classname-methodname,namespace
            _methodName = className.Split(',')[0].Split('-')[1];
            _className = className.Replace("-" + _methodName, "");
            _fieldDeclarations = fieldDeclarations;
            _resultVariable = resultVariable;
        }

        // Activity Behavior
        public override void Execute(IActivityExecution execution)
        {
            var scope = Context.CommandContext.Scope;
            var type = ReflectUtil.LoadClass(_className);
            object clazz;

            if (scope.IsRegistered(type))
            {
                clazz = scope.Resolve(type);
            }
            else
            {
                clazz = ReflectUtil.Instantiate(_className);
            }
            if (type != null)
            {
                //execution.SetVariableLocal(_className,clazz);
                MethodInfo m = ReflectUtil.GetMethod(type, _methodName);
                if (m == null)
                    throw Log.MissingClassException(_className);
                PerformExecution(execution, m, clazz);
                return;
            }
            throw Log.MissingClassException(_className);

        }
        protected internal virtual IActivityBehavior GetActivityBehaviorInstance(IActivityExecution execution)
        {
            IExpression expression =
                Context.CommandContext.ProcessEngineConfiguration.ExpressionManager.CreateExpression($"{_className}.{_methodName}()");
            return new ServiceTaskExpressionActivityBehavior(expression, _resultVariable);
        }
        protected void PerformExecution(IActivityExecution execution, MethodInfo method, object obj)
        {
            ExecuteWithErrorPropagation(execution, () =>
            {
                var args = new List<object>();
                foreach (var fieldDeclaration in _fieldDeclarations)
                {
                    var v = ((IExpression)fieldDeclaration.Value).GetValue(execution);
                    args.Add(v);
                }
                var parameters = method.GetParameters();
                if (parameters.Length > args.Count)
                {
                    var j = args.Count;
                    for (int i = 0; i < parameters.Length - j; i++)
                    {
                        if (parameters[j + i]
                            .HasDefaultValue)
                        {
                            args.Add(parameters[j + i].DefaultValue);
                        }
                    }
                }
                object value = null;
                var tr = Context.CommandContext.CurrentTransaction;
                var oldTr = System.Transactions.Transaction.Current;
                try
                {
                    if (tr != null)
                    {
                        Log.LogInfo("切换环境事务 Id:", tr.TransactionInformation.LocalIdentifier);
                        System.Transactions.Transaction.Current = tr;
                    }
                    value = method.Invoke(obj, args.ToArray());
                }
                catch (System.Exception ex)
                {
                    Log.LogInfo("外部代码异常:", ex.Message);
                    throw new ClassMethodDelegateException(string.Format("外部代码异常:{0}", ex.Message), ex);
                }
                finally
                {
                    if (tr != null)
                    {
                        System.Transactions.Transaction.Current = oldTr;
                        Log.LogInfo("还原环境事务 ", oldTr==null?"null":oldTr.TransactionInformation.LocalIdentifier);
                    }
                }
                if (!ReferenceEquals(_resultVariable, null))
                    execution.SetVariable(_resultVariable, value);
                Leave(execution);
            });
        }

    }
}