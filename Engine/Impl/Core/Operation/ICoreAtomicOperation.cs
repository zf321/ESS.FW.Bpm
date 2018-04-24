using ESS.FW.Bpm.Engine.Impl.Core.Instance;

namespace ESS.FW.Bpm.Engine.Impl.Core.Operation
{
    /// <summary>
    ///     
    ///     
    ///     
    /// </summary>
    /// @param
    /// <T> The execution type this atomic operation should work on. </param>
    public interface ICoreAtomicOperation<in T> where T : CoreExecution
    {
        string CanonicalName { get; }

        void Execute(T instance);

        bool IsAsync(T instance);
    }
}