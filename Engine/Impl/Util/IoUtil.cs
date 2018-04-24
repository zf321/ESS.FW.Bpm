using System;
using System.IO;

namespace ESS.FW.Bpm.Engine.Impl.Util
{
    /// <summary>
    ///      
    /// </summary>
    public class IoUtil
    {
        private static  EngineUtilLogger _logger = ProcessEngineLogger.UtilLogger;
        public static byte[] ReadInputStream(Stream inputStream, string inputStreamName)
        {
            byte[] bts = new byte[inputStream.Length];
            inputStream.Read(bts, 0, bts.Length);
            inputStream.Flush();
            inputStream.Dispose();
            return bts;
            //var outputStream = new MemoryStream();
            //var buffer = new byte[16*1024];
            //try
            //{
            //    var bytesRead = inputStream.Read(buffer, 0, buffer.Length);
            //    while (bytesRead != -1)
            //    {
            //        outputStream.Write(buffer, 0, bytesRead);
            //        bytesRead = inputStream.Read(buffer, 0, buffer.Length);
            //    }
            //}
            //catch (Exception e)
            //{
            //    throw new Exception(e.Message,e);
            //}
            //return outputStream.ToArray();
        }

        public static string ReadFileAsString(string filePath)
        {
            var buffer = new byte[(int) GetFile(filePath).Length];
            var result = string.Empty;
            try
            {
                var stream = new FileStream(filePath, FileMode.Open);
                var reader = new StreamReader(stream);
                result = reader.ReadToEnd();
            }
            catch (System.Exception e)
            {
            }
            return result;
        }

        public static FileInfo GetFile(string filePath)
        {
            //try
            //{
            return new FileInfo(filePath);
            //}
            //catch (Exception e)
            //{
            //}
        }

        //public static void writeStringToFile(string content, string filePath)
        //{
        //    BufferedOutputStream outputStream = null;
        //    try
        //    {
        //        outputStream =
        //            new BufferedOutputStream(new FileStream(getFile(filePath), FileMode.Create, FileAccess.Write));
        //        outputStream.write(content.GetBytes());
        //        outputStream.flush();
        //    }
        //    catch (Exception e)
        //    {
        //        throw LOG.exceptionWhileWritingToFile(filePath, e);
        //    }
        //    finally
        //    {
        //        IoUtil.closeSilently(outputStream);
        //    }
        //}

        /// <summary>
        ///     Closes the given stream. The same as calling <seealso cref="Closeable#close()" />, but
        ///     errors while closing are silently ignored.
        /// </summary>
        public static void CloseSilently(IDisposable closeable)
        {
            try
            {
                if (closeable != null)
                    closeable.Dispose();
            }
            catch (IOException ignore)
            {
                _logger.DebugCloseException(ignore);
            }
        }

        /// <summary>
        ///     Flushes the given object. The same as calling <seealso cref="Flushable#flush()" />, but
        ///     errors while flushing are silently ignored.

        /// </summary>
        //public static void flushSilently(Flushable flushable)
        //{
        //    try
        //    {
        //        if (flushable != null)
        //        {
        //            flushable.flush();
        //        }
        //    }
        //    catch (IOException ignore)
        //    {
        //        LOG.debugCloseException(ignore);
        //    }
        //}
    }
}