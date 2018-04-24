using System;

namespace ESS.FW.Bpm.Engine
{
    /// <summary>
    ///     Runtime exception indicating the requested class was not found or an error occurred
    ///     while loading the class.
    ///     
    /// </summary>
    public class ClassLoadingException : ProcessEngineException
    {
        private const long SerialVersionUid = 1L;

        protected internal string className;

        public ClassLoadingException(string message, System.Exception cause) : base(message, cause)
        {
        }

        public ClassLoadingException(string message, string className, System.Exception cause) : this(message, cause)
        {
            this.className = className;
        }

        /// <summary>
        ///     Returns the name of the class this exception is related to.
        /// </summary>
        public virtual string ClassName
        {
            get { return className; }
        }
    }
}