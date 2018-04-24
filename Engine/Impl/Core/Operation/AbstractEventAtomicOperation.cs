using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Core.Instance;
using ESS.FW.Bpm.Engine.Impl.Core.Model;
using ESS.FW.Bpm.Engine.Impl.Pvm;

namespace ESS.FW.Bpm.Engine.Impl.Core.Operation
{
    /// <summary>
    ///  Event执行器 同步
    /// </summary>
    public abstract class AbstractEventAtomicOperation<T> : ICoreAtomicOperation<T> where T : CoreExecution
    {
        protected internal abstract string EventName { get; }
        public abstract string CanonicalName { get; }

        public virtual bool IsAsync(T execution)
        {
            return false;
        }

        public virtual void Execute(T execution)//execution:ProcessInstance
        {
            
            var scope = GetScope(execution);//获取其scope(ProcessInstance.FlowScope(type:ProcessDefinitionEntity),包含activitys)
            CoreLogger.DecisionLogger.LogDebug("Execute scope:", scope.ToString(), scope);
            var listeners = GetListeners(scope, execution);//TODO ProcessInstanceEndListener获取不到
            var listenerIndex = execution.ListenerIndex;

            if (listenerIndex == 0)//Start
                execution = EventNotificationsStarted(execution);

            if (!IsSkipNotifyListeners(execution))//进入了抽象类 直接return false;
                if (listeners.Count > listenerIndex)
                {
                    execution.EventName = EventName;
                    execution.EventSource = scope;
                    var listener = listeners[listenerIndex];
                    try
                    {
                        execution.ListenerIndex = listenerIndex + 1;
                        execution.InvokeListener(listener);
                    }
                    catch (System.Exception e)
                    {
                        throw new PvmException("couldn't execute event listener : " + e.Message, e);
                    }
                    execution.PerformOperationSync(this);
                }
                else
                {
                    execution.ListenerIndex = 0;
                    execution.EventName = null;
                    execution.EventSource = null;
                    //事件通知完成
                    EventNotificationsCompleted(execution);
                }
            else
                EventNotificationsCompleted(execution);//
        }
        
        protected internal virtual IList<IDelegateListener<IBaseDelegateExecution>> GetListeners(CoreModelElement scope,
            T execution)
        {
            if (execution.SkipCustomListeners)
                return scope.GetBuiltInListeners(EventName);
            return scope.GetListeners(EventName);
        }

        protected internal virtual bool IsSkipNotifyListeners(T execution)
        {
            return false;
        }

        protected internal virtual T EventNotificationsStarted(T execution)
        {
            // do nothing
            return execution;
        }

        protected internal abstract CoreModelElement GetScope(T execution);
        protected internal abstract void EventNotificationsCompleted(T execution);
    }
}