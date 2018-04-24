using ESS.FW.Bpm.Engine.Impl.Interceptor;

namespace ESS.FW.Bpm.Engine.Impl.Cfg
{
    /// <summary>
    ///     Implementations may not assume that this is executed by the same thread as the command itself.
    ///     Especially thread locals such as those available in <seealso cref="context" /> should not be accessed
    ///     in an implementation of this interface. See CAM-3684 for details.
    /// </summary>
    public interface ITransactionListener
    {
        void Execute(CommandContext commandContext);
    }
}