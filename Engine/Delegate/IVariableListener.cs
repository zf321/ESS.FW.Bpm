namespace ESS.FW.Bpm.Engine.Delegate
{
    /// <summary>
    ///     
    /// </summary>
    public interface IVariableListener<T> where T: IDelegateVariableInstance<IBaseDelegateExecution>
    {
        void Notify(T variableInstance);
    }

    public static class VariableListenerFields
    {
        public const string Create = "create";
        public const string Update = "update";
        public const string Delete = "delete";
    }
}