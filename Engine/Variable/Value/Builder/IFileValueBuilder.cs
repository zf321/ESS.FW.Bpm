using System.IO;
using System.Text;

namespace ESS.FW.Bpm.Engine.Variable.Value.Builder
{
    /// <summary>
    /// </summary>
    public interface IFileValueBuilder : ITypedValueBuilder<IFileValue>
    {
        /// <summary>
        ///     Saves the MIME type of a file in the value infos.
        /// </summary>
        /// <param name="type">
        ///     the MIME type as string
        /// </param>
        IFileValueBuilder MimeType(string mimeType);

        /// <summary>
        ///     Sets the value to the specified <seealso cref="System.IO.File" />.
        /// </summary>
        /// <seealso cref= # file( byte
        /// [
        /// ]
        /// )
        /// </seealso>
        /// <seealso cref= # file( InputStream
        /// )
        /// </seealso>
        IFileValueBuilder File(FileInfo file);

        /// <summary>
        ///     Sets the value to the specified <seealso cref="InputStream" />.
        /// </summary>
        /// <seealso cref= # file( byte
        /// [
        /// ]
        /// )
        /// </seealso>
        /// <seealso cref= # file( File
        /// )
        /// </seealso>
        IFileValueBuilder File(Stream stream);

        /// <summary>
        ///     Sets the value to the specified <seealso cref="byte" /> array
        /// </summary>
        /// <seealso cref= # file( File
        /// )
        /// </seealso>
        /// <seealso cref= # file( InputStream
        /// )
        /// </seealso>
        IFileValueBuilder File(byte[] bytes);

        /// <summary>
        ///     Sets the encoding for the file in the value infos (optional).
        /// </summary>
        /// <param name="encoding">
        ///     @return
        /// </param>
        IFileValueBuilder Encoding(Encoding encoding);

        /// <summary>
        ///     Sets the encoding for the file in the value infos (optional).
        /// </summary>
        /// <param name="encoding">
        ///     @return
        /// </param>
        IFileValueBuilder Encoding(string encoding);
    }
}