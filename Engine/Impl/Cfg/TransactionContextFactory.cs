using ESS.FW.Bpm.Engine.Impl.Interceptor;

namespace ESS.FW.Bpm.Engine.Impl.Cfg
{
    /// <summary>
    ///      
    /// </summary>
    public interface ITransactionContextFactory
    {
        ITransactionContext OpenTransactionContext(CommandContext commandContext);
    }
}