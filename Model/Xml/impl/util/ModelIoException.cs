using System;



namespace ESS.FW.Bpm.Model.Xml.impl.util
{

    /// <summary>
    /// An exception during IO operations.
    /// 
    /// 
    /// </summary>
    public class ModelIoException : Exception
    {

        public ModelIoException()
        {
        }

        public ModelIoException(string message) : base(message)
        {
        }

        public ModelIoException(string message, Exception cause) : base(message, cause)
        {
        }

        public ModelIoException(Exception cause) : base("", cause)
        {
        }
    }
}