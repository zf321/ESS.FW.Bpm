namespace ESS.FW.Bpm.Engine.Impl.Interceptor
{
    /// <summary>
    ///      
    /// </summary>
    public interface ICommand<out T>
    {
        T Execute(CommandContext commandContext);
    }
}