using System.IO;

namespace ESS.FW.Bpm.Engine.Common.Utils
{
    /// <summary>
    ///     
    /// </summary>
    public class IoUtilLogger : UtilsLogger
    {
        public virtual IoUtilException UnableToReadInputStream(IOException cause)
        {
            return new IoUtilException(ExceptionMessage("001", "Unable to read input stream"), cause);
        }

        public virtual IoUtilException FileNotFoundException(string filename, System.Exception cause)
        {
            return new IoUtilException(ExceptionMessage("002", "Unable to find file with path '{}'", filename), cause);
        }

        public virtual IoUtilException FileNotFoundException(string filename)
        {
            return FileNotFoundException(filename, null);
        }

        public virtual IoUtilException NullParameter(string parameter)
        {
            return new IoUtilException(ExceptionMessage("003", "Parameter '{}' can not be null", parameter));
        }

        public virtual IoUtilException UnableToReadFromReader(System.Exception cause)
        {
            return new IoUtilException(ExceptionMessage("004", "Unable to read from reader"), cause);
        }
    }
}