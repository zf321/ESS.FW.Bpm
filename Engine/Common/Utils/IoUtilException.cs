namespace ESS.FW.Bpm.Engine.Common.Utils
{
    /// <summary>
    ///     
    /// </summary>
    public class IoUtilException : System.Exception
    {
        public IoUtilException(string message) : base(message)
        {
        }

        public IoUtilException(string message, System.Exception cause) : base(message, cause)
        {
        }
    }
}