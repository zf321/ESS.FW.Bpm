using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace ESS.FW.Bpm.Engine.Common.Utils
{
    /// <summary>
    /// </summary>
    public class IoUtil
    {
        private static readonly IoUtilLogger Log = UtilsLogger.IoUtilLogger;
        public static readonly string EncodingCharset = "UTF8";

        /// <summary>
        ///     Returns the input stream as <seealso cref="string" />.
        /// </summary>
        /// <param name="inputStream"> the input stream </param>
        /// <returns> the input stream as <seealso cref="string" />. </returns>
        public static string InputStreamAsString(Stream inputStream)
        {
            return StringHelperClass.NewString(InputStreamAsByteArray(inputStream), EncodingCharset);
        }

        /// <summary>
        ///     Returns the input stream as <seealso cref="byte[]" />.
        /// </summary>
        /// <param name="inputStream"> the input stream </param>
        /// <returns> the input stream as <seealso cref="byte[]" />. </returns>
        public static byte[] InputStreamAsByteArray(Stream inputStream)
        {
            var os = new MemoryStream();
            try
            {
                var buffer = new byte[16*1024];
                int read;
                while ((read = inputStream.Read(buffer, 0, buffer.Length)) > 0)
                    os.Write(buffer, 0, read);
                return os.ToArray();
            }
            catch (IOException e)
            {
                throw Log.UnableToReadInputStream(e);
            }
            finally
            {
                CloseSilently(inputStream);
            }
        }

        ///// <summary>
        ///// Returns the <seealso cref="Reader"/> content as <seealso cref="String"/>.
        ///// </summary>
        ///// <param name="reader"> the <seealso cref="Reader"/> </param>
        ///// <returns> the <seealso cref="Reader"/> content as <seealso cref="String"/> </returns>
        //public static string ReaderAsString(Reader reader)
        //{
        //    StringBuilder buffer = new StringBuilder();
        //    char[] chars = new char[16 * 1024];
        //    int numCharsRead;
        //    try
        //    {
        //        while ((numCharsRead = reader.read(chars, 0, chars.Length)) != -1)
        //        {
        //            buffer.Append(chars, 0, numCharsRead);
        //        }
        //        return buffer.ToString();
        //    }
        //    catch (IOException e)
        //    {
        //        throw Log.UnableToReadFromReader(e);
        //    }
        //    finally
        //    {
        //        CloseSilently(reader);
        //    }
        //}

        /// <summary>
        ///     Returns the <seealso cref="string" /> as <seealso cref="InputStream" />.
        /// </summary>
        /// <param name="string"> the <seealso cref="string" /> to convert </param>
        /// <returns> the <seealso cref="InputStream" /> containing the <seealso cref="string" /> </returns>
        public static Stream StringAsInputStream(string @string)
        {
            return new MemoryStream(@string.GetBytes(EncodingCharset));
        }

        /// <summary>
        ///     Close a closable ignoring any IO exception.
        /// </summary>
        /// <param name="closeable"> the closable to close </param>
        public static void CloseSilently(IDisposable closeable)
        {
            try
            {
                if (closeable != null)
                    closeable.Dispose();
            }
            catch (IOException)
            {
                // ignore
            }
        }

        /// <summary>
        ///     Returns the content of a file with specified filename
        /// </summary>
        /// <param name="filename"> name of the file to load </param>
        /// <returns> Content of the file as <seealso cref="String" /> </returns>
        public static string FileAsString(string filename)
        {
            var classpathFile = GetClasspathFile(filename);
            return FileAsString(classpathFile);
        }

        /// <summary>
        ///     Returns the content of a <seealso cref="File" />.
        /// </summary>
        /// <param name="file"> the file to load </param>
        /// <returns> Content of the file as <seealso cref="String" /> </returns>
        public static string FileAsString(FileInfo file)
        {
            try
            {
                return InputStreamAsString(new FileStream(file.Name, FileMode.Open, FileAccess.Read));
            }
            catch (FileNotFoundException e)
            {
                throw Log.FileNotFoundException(file.Name, e);
            }
        }

        /// <summary>
        ///     Returns the content of a <seealso cref="File" />.
        /// </summary>
        /// <param name="file"> the file to load </param>
        /// <returns> Content of the file as <seealso cref="String" /> </returns>
        public static byte[] FileAsByteArray(FileInfo file)
        {
            try
            {
                return InputStreamAsByteArray(new FileStream(file.Name, FileMode.Open, FileAccess.Read));
            }
            catch (FileNotFoundException e)
            {
                throw Log.FileNotFoundException(file.Name, e);
            }
        }


        /// <summary>
        ///     Returns the input stream of a file with specified filename
        /// </summary>
        /// <param name="filename"> the name of a <seealso cref="File" /> to load </param>
        /// <returns> the file content as input stream </returns>
        /// <exception cref="IoUtilException"> if the file cannot be loaded </exception>
        public static Stream FileAsStream(string filename)
        {
            var classpathFile = GetClasspathFile(filename);
            return FileAsStream(classpathFile);
        }

        /// <summary>
        ///     Returns the input stream of a file.
        /// </summary>
        /// <param name="file"> the <seealso cref="File" /> to load </param>
        /// <returns> the file content as input stream </returns>
        /// <exception cref="IoUtilException"> if the file cannot be loaded </exception>
        public static Stream FileAsStream(FileInfo file)
        {
            try
            {
                return new FileStream(file.Name, FileMode.Open, FileAccess.Read);
            }
            catch (FileNotFoundException e)
            {
                throw Log.FileNotFoundException(file.Name, e);
            }
        }


        /// <summary>
        ///     Returns the <seealso cref="File" /> for a filename.
        /// </summary>
        /// <param name="filename"> the filename to load </param>
        /// <returns> the file object </returns>
        /// <exception cref="IoUtilException"> if the file cannot be loaded </exception>
        public static FileInfo GetClasspathFile(string filename)
        {
            if (ReferenceEquals(filename, null))
                throw Log.NullParameter("filename");

            Uri fileUrl = null;

            //if (fileUrl == null)
            //{
            //    // Try the current Thread context classloader
            //    classLoader = Thread.CurrentThread.ContextClassLoader;
            //    fileUrl = classLoader.getResource(filename);

            //    if (fileUrl == null)
            //    {
            //        // Finally, try the classloader for this class
            //        classLoader = typeof(IoUtil).ClassLoader;
            //        fileUrl = classLoader.getResource(filename);
            //    }
            //}

            //if (fileUrl == null)
            //{
            //    throw Log.fileNotFoundException(filename);
            //}

            //try
            //{
            //    return new File(fileUrl.toURI());
            //}
            //catch (URISyntaxException e)
            //{
            //    throw Log.fileNotFoundException(filename, e);
            //}
            return null;
        }
    }

    
}