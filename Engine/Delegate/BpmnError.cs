using ESS.FW.Bpm.Engine.Impl.Bpmn.Parser;
using ESS.FW.Bpm.Engine.Impl.Util;

namespace ESS.FW.Bpm.Engine.Delegate
{
    /// <summary>
    ///     Special exception that can be used to throw a BPMN Error from
    ///     <seealso cref="IJavaDelegate" />s and expressions.
    ///     This should only be used for business faults, which shall be handled by a
    ///     Boundary Error Event or Error Event Sub-Process modeled in the process
    ///     definition. Technical errors should be represented by other exception types.
    ///     This class represents an actual instance of a BPMN Error, whereas
    ///     <seealso cref="Error" /> represents an Error definition.
    ///     
    /// </summary>
    public class BpmnError : ProcessEngineException
    {
        private const long SerialVersionUid = 1L;

        private string _errorCode;
        private string _errorMessage;

        public BpmnError(string errorCode) : base("")
        {
            ErrorCode = errorCode;
        }

        public BpmnError(string errorCode, string message) : base(message + " (errorCode='" + errorCode + "')")
        {
            ErrorCode = errorCode;
            Message = message;
        }

        protected internal virtual string ErrorCode
        {
            set
            {
                EnsureUtil.EnsureNotEmpty("Error Code", value);
                _errorCode = value;
            }
            get { return _errorCode; }
        }

        protected internal virtual string Message
        {
            set
            {
                EnsureUtil.EnsureNotEmpty("Error Message", value);
                _errorMessage = value;
            }
            get { return _errorMessage; }
        }


        public override string ToString()
        {
            return base.ToString() + " (errorCode='" + _errorCode + "')";
        }
    }
}