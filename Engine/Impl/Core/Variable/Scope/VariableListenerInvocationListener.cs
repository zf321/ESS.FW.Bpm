using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Core.Variable.Event;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Runtime;

namespace ESS.FW.Bpm.Engine.Impl.Core.Variable.Scope
{

    /// <summary>
    /// 
    /// 
    /// </summary>
    public class VariableListenerInvocationListener : IVariableInstanceLifecycleListener<ICoreVariableInstance>
    {
        protected internal readonly AbstractVariableScope TargetScope;

        public VariableListenerInvocationListener(AbstractVariableScope targetScope)
        {
            this.TargetScope = targetScope;
        }

        public virtual void OnCreate(ICoreVariableInstance variable, AbstractVariableScope sourceScope)
        {
            HandleEvent(new VariableEvent((ICoreVariableInstance) variable, VariableListenerFields.Create, sourceScope));
        }

        public virtual void OnUpdate(ICoreVariableInstance variable, AbstractVariableScope sourceScope)
        {
            HandleEvent(new VariableEvent((ICoreVariableInstance) variable, VariableListenerFields.Update, sourceScope));
        }

        public virtual void OnDelete(ICoreVariableInstance variable, AbstractVariableScope sourceScope)
        {
            HandleEvent(new VariableEvent((ICoreVariableInstance) variable, VariableListenerFields.Delete, sourceScope));
        }

        protected internal virtual void HandleEvent(VariableEvent @event)
        {
            var sourceScope = @event.SourceScope;

            if (sourceScope is ExecutionEntity)
            {
                AddEventToScopeExecution((ExecutionEntity)sourceScope, @event);
            }
            else if (sourceScope is TaskEntity)
            {
                TaskEntity task = (TaskEntity)sourceScope;
                ExecutionEntity execution = task.GetExecution();
                if (execution != null)
                {
                    AddEventToScopeExecution(execution, @event);
                }
            }
            else if (sourceScope.ParentVariableScope is ExecutionEntity)
            {
                AddEventToScopeExecution((ExecutionEntity)sourceScope.ParentVariableScope, @event);
            }
            else
            {
                throw new ProcessEngineException("BPMN execution scope expected");
            }
        }

        protected internal virtual void AddEventToScopeExecution(ExecutionEntity sourceScope, VariableEvent @event)
        {
            // ignore events of variables that are not set in an execution
            ExecutionEntity sourceExecution = sourceScope;
            ExecutionEntity scopeExecution = (ExecutionEntity) (sourceExecution.IsScope ? sourceExecution : sourceExecution.Parent);
            scopeExecution.DelayEvent((ExecutionEntity)TargetScope, @event);
        }
    }
}