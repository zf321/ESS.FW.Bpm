namespace ESS.FW.Bpm.Engine.Delegate
{
    /// <summary>
    ///     
    ///     
    ///     
    /// </summary>
    public interface IDelegateListener<in T> where T : IBaseDelegateExecution
    {
        void Notify(T instance);
    }
}