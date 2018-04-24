using ESS.FW.Bpm.Engine.Impl.Interceptor;

namespace ESS.FW.Bpm.Engine.Impl
{
    /// <summary>
    ///     
    /// </summary>
    public interface IProcessInstanceModificationInstruction
    {
        void Execute(CommandContext commandContext);
    }
}