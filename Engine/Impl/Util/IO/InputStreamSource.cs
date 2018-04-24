using System.IO;

namespace ESS.FW.Bpm.Engine.Impl.Util.IO
{
    /// <summary>
    ///      
    /// </summary>
    public class InputStreamSource : IStreamSource
    {
        internal Stream inputStream;

        public InputStreamSource(Stream inputStream)
        {
            this.inputStream = inputStream;
        }

        public virtual Stream InputStream
        {
            get { return inputStream; }
        }

        public override string ToString()
        {
            return "InputStream";
        }
    }
}