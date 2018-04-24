using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Api.Runtime.Util
{
    /// <summary>
    /// </summary>
    public class IncrementCounterListener : IDelegateListener<IBaseDelegateExecution>
    {
        public static int Counter;

        public void Notify(IBaseDelegateExecution execution)
        {
            Counter++;
        }
    }
}