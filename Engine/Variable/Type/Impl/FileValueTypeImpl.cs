using System;
using System.Collections.Generic;
using System.IO;
using ESS.FW.Bpm.Engine.Variable.Value;

namespace ESS.FW.Bpm.Engine.Variable.Type.Impl
{
    /// <summary>
    ///     Valuetype to save files from byte arrays, inputstreams or just files as
    ///     process variables and retrieve them via an <seealso cref="InputStream" />.
    /// </summary>
    [Serializable]
    public class FileValueTypeImpl : AbstractValueTypeImpl, IFileValueType
    {
        private const long SerialVersionUid = 1L;

        public FileValueTypeImpl() : base("file")
        {
        }

        public override ITypedValue CreateValue(object value, IDictionary<string, object> valueInfo)
        {
            var filename = valueInfo[FileValueTypeFields.ValueInfoFileName];
            if (filename == null)
                throw new ArgumentException(
                    "Cannot create file without filename! Please set a name into ValueInfo with key " +
                    FileValueTypeFields.ValueInfoFileName);
            var builder = Variables.FileValue(filename.ToString());
            if (value is FileInfo)
                builder.File((FileInfo) value);
            else if (value is Stream)
                builder.File((Stream) value);
            else if (value is byte[])
                builder.File((byte[]) value);
            else
                throw new ArgumentException("Provided value is not of File, InputStream or byte[] type.");

            if (valueInfo.ContainsKey(FileValueTypeFields.ValueInfoFileMimeType))
            {
                var mimeType = valueInfo[FileValueTypeFields.ValueInfoFileMimeType];

                if (mimeType == null)
                    throw new ArgumentException(
                        "The provided mime type is null. Set a non-null value info property with key '" +
                        FileValueTypeFields.ValueInfoFileName + "'");

                builder.MimeType(mimeType.ToString());
            }
            if (valueInfo.ContainsKey(FileValueTypeFields.ValueInfoFileEncoding))
            {
                var encoding = valueInfo[FileValueTypeFields.ValueInfoFileEncoding];

                if (encoding == null)
                    throw new ArgumentException(
                        "The provided encoding is null. Set a non-null value info property with key '" +
                        FileValueTypeFields.ValueInfoFileEncoding + "'");

                builder.Encoding(encoding.ToString());
            }
            return builder.Create();
        }

        public override IDictionary<string, object> GetValueInfo(ITypedValue typedValue)
        {
            if (!(typedValue is IFileValue))
                throw new ArgumentException("Value not of type FileValue");
            var fileValue = (IFileValue) typedValue;
            IDictionary<string, object> result = new Dictionary<string, object>(2);
            result[FileValueTypeFields.ValueInfoFileName] = fileValue.Filename;
            if (!ReferenceEquals(fileValue.MimeType, null))
                result[FileValueTypeFields.ValueInfoFileMimeType] = fileValue.MimeType;
            if (!ReferenceEquals(fileValue.Encoding, null))
                result[FileValueTypeFields.ValueInfoFileEncoding] = fileValue.Encoding;
            return result;
        }

        public override bool IsPrimitiveValueType
        {
            get { return true; }
        }
    }
}