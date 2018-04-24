using System.IO;

namespace ESS.FW.Bpm.Engine.Impl.Util.IO
{
    /// <summary>
    ///      
    /// </summary>
    public class StringStreamSource : IStreamSource
    {
        internal string String;

        public StringStreamSource(string @string)
        {
            this.String = @string;
        }

        public virtual Stream InputStream
        {
            //get { return new MemoryStream(@string.GetBytes()); }
            get { return null; }
        }

        public override string ToString()
        {
            return "String";
        }
    }
}