using System.IO;
using System.Text;

namespace ESS.FW.Bpm.Engine.Variable.Value
{
    /// <summary>
    /// </summary>
    public interface IFileValue : ITypedValue
    {
        string Filename { get; }

        string MimeType { get; }

        /// <summary>
        ///     Convenience method to save the transformation. This method will perform no
        ///     check if the saved encoding is known to the JVM and therefore could throw
        ///     every exception that <seealso cref="Encoding#forName(String)" /> lists.
        ///     <para>
        ///         If no encoding has been saved it will return null.
        ///     </para>
        /// </summary>
        Encoding EncodingAsCharset { get; }

        /// <returns> the saved encoding or null if none has been saved </returns>
        string Encoding { get; }

        Stream GetValue { get; }
    }
}