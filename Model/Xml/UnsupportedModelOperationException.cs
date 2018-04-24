namespace ESS.FW.Bpm.Model.Xml
{

    public class UnsupportedModelOperationException : System.NotSupportedException
    {

        private const long SerialVersionUid = 1L;

        public UnsupportedModelOperationException(string operationName, string reason) : base("The operation " + operationName + " is unsupported: " + reason + ".")
        {
        }

    }

}