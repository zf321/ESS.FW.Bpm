using System;
using System.IO;
using System.Text;
using ESS.FW.Bpm.Engine.Variable.Type;

namespace ESS.FW.Bpm.Engine.Variable.Value.Impl
{
    /// <summary>
    /// </summary>
    [Serializable]
    public class FileValueImpl : IFileValue
    {
        private const long SerialVersionUid = 1L;
        protected internal string encoding;
        protected internal string filename;
        protected internal string mimeType;
        protected internal IFileValueType type;
        protected internal byte[] value;

        public FileValueImpl(byte[] value, IFileValueType type, string filename, string mimeType, string encoding)
        {
            this.value = value;
            this.type = type;
            this.filename = filename;
            this.mimeType = mimeType;
            this.encoding = encoding;
        }

        public FileValueImpl(IFileValueType type, string filename) : this(null, type, filename, null, null)
        {
        }


        /// <summary>
        ///     Get the byte array directly without wrapping it inside a stream to evade
        ///     not needed wrapping. This method is intended for the internal API, which
        ///     needs the byte array anyways.
        /// </summary>
        public virtual byte[] ByteArray
        {
            get { return value; }
        }

        public virtual string Filename
        {
            get { return filename; }
        }

        public virtual string MimeType
        {
            get { return mimeType; }
            set { mimeType = value; }
        }

        public object Value { get; }

        public virtual IValueType Type
        {
            get { return type; }
        }

        public virtual string Encoding
        {
            set { encoding = value; }
            get { return encoding; }
        }

        Stream IFileValue.GetValue
        {
            get { return GetValue(); }
        }


        public virtual Encoding EncodingAsCharset
        {
            get
            {
                if (ReferenceEquals(encoding, null))
                    return null;
                //return System.Text.Encoding.ForName(encoding);
                return null;
            }
        }


        public virtual void SetValue(byte[] bytes)
        {
            value = bytes;
        }

        public virtual Stream GetValue()
        {
            if (Value == null)
                return null;
            return new MemoryStream(value);
        }

        public override string ToString()
        {
            return "FileValueImpl [mimeType=" + mimeType + ", filename=" + filename + ", type=" + type + "]";
        }
    }
}