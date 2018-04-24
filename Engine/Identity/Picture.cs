using System.IO;

namespace ESS.FW.Bpm.Engine.Identity
{
    /// <summary>
    ///      
    /// </summary>
    public class Picture
    {
        public Picture(byte[] bytes, string mimeType)
        {
            this.Bytes = bytes;
            this.MimeType = mimeType;
        }

        public virtual byte[] Bytes { get; }

        public virtual Stream InputStream
        {
            get { return new MemoryStream(Bytes); }
        }

        public virtual string MimeType { get; }
    }
}