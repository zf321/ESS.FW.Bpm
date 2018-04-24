using System;
using System.IO;
using ESS.FW.Bpm.Engine.Impl.util;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Util
{
    /// <summary>
    ///     
    ///     
    /// </summary>
    public class ExceptionUtil
    {
        public static string GetExceptionStacktrace(System.Exception exception)
        {
            return exception.StackTrace.ToString();
        }

        public static string GetExceptionStacktrace(ResourceEntity byteArray)
        {
            string result = null;
            if (byteArray != null)
            {
                result = StringUtil.FromBytes(byteArray.Bytes);
            }
            return result;
        }

        public static ResourceEntity CreateJobExceptionByteArray(byte[] byteArray)
        {
            return CreateExceptionByteArray("job.exceptionByteArray", byteArray);
        }

        /// <summary>
        ///     create ResourceEntity with specified name and payload and make sure it's
        ///     persisted
        ///     used in Jobs and ExternalTasks
        /// </summary>
        /// <param name="name"> - type\source of the exception </param>
        /// <param name="byteArray"> - payload of the exception </param>
        /// <returns> persisted entity </returns>
        public static ResourceEntity CreateExceptionByteArray(string name, byte[] byteArray)
        {
            ResourceEntity result = null;

            if (byteArray != null)
                result = new ResourceEntity(name, byteArray);

            return result;
        }
    }
}