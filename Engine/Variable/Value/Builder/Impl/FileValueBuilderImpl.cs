using System.IO;
using System.Text;
using ESS.FW.Bpm.Engine.Common.Utils;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Variable.Type;
using ESS.FW.Bpm.Engine.Variable.Value.Impl;
using IoUtil = ESS.FW.Bpm.Engine.Common.Utils.IoUtil;

namespace ESS.FW.Bpm.Engine.Variable.Value.Builder.Impl
{
    /// <summary>
    /// </summary>
    public class FileValueBuilderImpl : IFileValueBuilder
    {
        protected internal FileValueImpl FileValue;

        public FileValueBuilderImpl(string filename)
        {
            EnsureUtil.EnsureNotNull("filename", filename);
            FileValue = new FileValueImpl(ValueTypeFields.File, filename);
        }

        public IFileValue Create()
        {
            return FileValue;
        }

        public virtual IFileValueBuilder MimeType(string mimeType)
        {
            FileValue.MimeType = mimeType;
            return this;
        }

        public virtual IFileValueBuilder File(FileInfo file)
        {
            try
            {
                return File(IoUtil.FileAsByteArray(file));
            }
            catch (IoUtilException e)
            {
                throw;
                //throw new System.ArgumentException(e);
            }
        }

        public virtual IFileValueBuilder File(Stream stream)
        {
            return File(IoUtil.InputStreamAsByteArray(stream));
        }


        public virtual IFileValueBuilder File(byte[] bytes)
        {
            FileValue.SetValue(bytes);
            return this;
        }

        public virtual IFileValueBuilder Encoding(Encoding encoding)
        {
            FileValue.Encoding = encoding.EncodingName;
            return this;
        }

        public virtual IFileValueBuilder Encoding(string encoding)
        {
            FileValue.Encoding = encoding;
            return this;
        }
    }
}