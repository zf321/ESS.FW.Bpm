namespace ESS.FW.Bpm.Engine.Impl.Core.Variable.Scope
{
    /// <summary>
    ///     
    /// </summary>
    public interface IVariableInstanceLifecycleListener<T> where T : ICoreVariableInstance
    {
        void OnCreate(T variableInstance, AbstractVariableScope sourceScope);

        void OnDelete(T variableInstance, AbstractVariableScope sourceScope);

        void OnUpdate(T variableInstance, AbstractVariableScope sourceScope);
    }
}