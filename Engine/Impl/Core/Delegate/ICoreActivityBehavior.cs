using ESS.FW.Bpm.Engine.Delegate;

namespace ESS.FW.Bpm.Engine.Impl.Core.Delegate
{
    /// <summary>
    ///     
    ///     
    ///     
    /// </summary>
    public interface ICoreActivityBehavior<in T> where T : IBaseDelegateExecution
    {
        void Execute(T execution);
    }
}