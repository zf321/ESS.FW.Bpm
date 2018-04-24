using ESS.FW.Bpm.Engine.Impl.Core.Instance;
using ESS.FW.Bpm.Engine.Impl.Core.Operation;
using ESS.FW.Bpm.Engine.Impl.Core.Variable;
using ESS.FW.Bpm.Engine.Impl.Core.Variable.Scope;

namespace ESS.FW.Bpm.Engine.Impl.Core
{
    //using CoreAtomicOperation = ICoreAtomicOperation<>;

    /// <summary>
    ///     
    /// </summary>
    [System.Serializable]
    public class CoreLogger : ProcessEngineLogger
    {
        public virtual void DebugMappingValueFromOuterScopeToInnerScope(object value, AbstractVariableScope outerScope,
            string name, AbstractVariableScope innerScope)
        {
            LogDebug("001", "Mapping value '{0} from outer scope '{1}' to variable '{2}' in inner scope '{3}'.", value,
                outerScope, name, innerScope);
        }

        public virtual void DebugMappingValuefromInnerScopeToOuterScope(object value, AbstractVariableScope innerScope,
            string name, AbstractVariableScope outerScope)
        {
            LogDebug("002", "Mapping value '{0}' from inner scope '{1}' to variable '{2}' in outer scope '{3}'.", value,
                innerScope, name, outerScope);
        }

        public virtual void DebugPerformingAtomicOperation<T1>(ICoreAtomicOperation<T1> atomicOperation, CoreExecution e) where T1:CoreExecution
        {
            LogDebug("003", "Performing atomic operation {0} on {1}", atomicOperation, e);
        }

        public virtual ProcessEngineException DuplicateVariableInstanceException(ICoreVariableInstance variableInstance)
        {
            return
                new ProcessEngineException(ExceptionMessage("004",
                    "Cannot add variable instance with name {0}. Variable already exists", variableInstance.Name));
        }

        public virtual ProcessEngineException MissingVariableInstanceException(ICoreVariableInstance variableInstance)
        {
            return
                new ProcessEngineException(ExceptionMessage("004",
                    "Cannot update variable instance with name {0}. Variable does not exist", variableInstance.Name));
        }
    }
}