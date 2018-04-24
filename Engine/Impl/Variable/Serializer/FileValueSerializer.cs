using System;
using ESS.FW.Bpm.Engine.Variable;
using ESS.FW.Bpm.Engine.Variable.Serializer;
using ESS.FW.Bpm.Engine.Variable.Type;
using ESS.FW.Bpm.Engine.Variable.Value;
using ESS.FW.Bpm.Engine.Variable.Value.Impl;

namespace ESS.FW.Bpm.Engine.Impl.Variable.Serializer
{
    /// <summary>
    /// </summary>
    public class FileValueSerializer : AbstractTypedValueSerializer<IFileValue>
    {
        /// <summary>
        ///     The numbers values we encoded in textfield two.
        /// </summary>
        protected internal const int NR_OF_VALUES_IN_TEXTFIELD2 = 2;

        /// <summary>
        ///     The separator to be able to store encoding and mimetype inside the same
        ///     text field. Please be aware that the separator only works when it is a
        ///     character that is not allowed in the first component.
        /// </summary>
        protected internal const char MIMETYPE_ENCODING_SEPARATOR = '#';

        public FileValueSerializer() : base(ValueTypeFields.File)
        {
        }

        public override string Name
        {
            get { return valueType.Name; }
        }

        public override void WriteValue(ITypedValue value, IValueFields valueFields)
        {
            var v = (FileValueImpl) value;
            var data = ((FileValueImpl) value).ByteArray;
            valueFields.ByteArrayValue = data;
            valueFields.TextValue = v.Filename;
            if (v.MimeType == null && v.Encoding != null)
                valueFields.TextValue2 = MIMETYPE_ENCODING_SEPARATOR + v.Encoding;
            else if (v.MimeType != null && v.Encoding == null)
                valueFields.TextValue2 = v.MimeType + MIMETYPE_ENCODING_SEPARATOR;
            else if (v.MimeType != null && v.Encoding != null)
                valueFields.TextValue2 = v.MimeType + MIMETYPE_ENCODING_SEPARATOR + v.Encoding;
        }

        public override ITypedValue ConvertToTypedValue(UntypedValueImpl untypedValue)
        {
            throw new NotSupportedException("Currently no automatic conversation from UntypedValue to FileValue");
        }

        public override ITypedValue ReadValue(IValueFields valueFields, bool deserializeValue)
        {
            var builder = Variables.FileValue(valueFields.TextValue);
            if (valueFields.ByteArrayValue != null)
                builder.File(valueFields.ByteArrayValue);
            // to ensure the same array size all the time
            if (!ReferenceEquals(valueFields.TextValue2, null))
            {
                var split = valueFields.TextValue2.Split(MIMETYPE_ENCODING_SEPARATOR);

                var mimeType = ReturnNullIfEmptyString(split[0]);
                var encoding = ReturnNullIfEmptyString(split[1]);

                builder.MimeType(mimeType);
                builder.Encoding(encoding);
            }
            return builder.Create();
        }

        protected internal virtual string ReturnNullIfEmptyString(string s)
        {
            if (s.Length == 0)
                return null;
            return s;
        }

        protected internal override bool CanWriteValue(ITypedValue value)
        {
            if (value == null || value.Type == null)
                return false;
            return value.Type.Name.Equals(Name);
        }
    }
}