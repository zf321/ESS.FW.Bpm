using System;

namespace ESS.FW.Bpm.Model.Xml
{

    public class ModelException : Exception
    {

        private const long SerialVersionUid = 1L;

        public ModelException()
        {
        }

        public ModelException(string message, Exception cause) : base(message, cause)
        {
        }

        public ModelException(string message) : base(message)
        {
        }

        public ModelException(Exception cause) : base("", cause)
        {
        }
    }
}