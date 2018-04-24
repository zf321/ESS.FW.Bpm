using System.Threading;

namespace ESS.FW.Bpm.Engine.Application.Impl
{
    /// <summary>
    ///     
    /// </summary>
    public class ProcessApplicationContextImpl
    {
        protected internal static ThreadLocal<ProcessApplicationIdentifier> CurrentProcessApplication =
            new ThreadLocal<ProcessApplicationIdentifier>();

        public static ProcessApplicationIdentifier Get()
        {
            return CurrentProcessApplication.Value;
        }

        public static void Set(ProcessApplicationIdentifier identifier)
        {
            CurrentProcessApplication.Value = identifier;
        }

        public static void Clear()
        {
            CurrentProcessApplication.Values.Remove(CurrentProcessApplication.Value);
        }
    }
}