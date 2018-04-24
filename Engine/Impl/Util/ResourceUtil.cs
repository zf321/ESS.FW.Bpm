using System.IO;
using System.Text;
using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Impl.Util;

namespace ESS.FW.Bpm.Engine.Impl.Util
{
    /// <summary>
    ///     
    /// </summary>
    public sealed class ResourceUtil
    {
        private static readonly EngineUtilLogger Log = ProcessEngineLogger.UtilLogger;

        /// <summary>
        ///     Parse a camunda:resource attribute and loads the resource depending on the url scheme.
        ///     Supported Uri schemes are <code>classpath://</code> and <code>deployment://</code>.
        ///     If the scheme is omitted <code>classpath://</code> is assumed.
        /// </summary>
        /// <param name="resourcePath"> the path to the resource to load </param>
        /// <param name="deployment"> the deployment to load resources from </param>
        /// <returns> the resource content as <seealso cref="string" /> </returns>
        public static string LoadResourceContent(string resourcePath, DeploymentEntity deployment)
        {
            var pathSplit = resourcePath.Split("://", true);

            string resourceType;
            if (pathSplit.Length == 1)
                resourceType = "classpath";
            else
                resourceType = pathSplit[0];

            var resourceLocation = pathSplit[pathSplit.Length - 1];

            byte[] resourceBytes = null;

            if (resourceType.Equals("classpath"))
            {
                Stream resourceAsStream = null;
                try
                {
                    resourceAsStream = ReflectUtil.GetResourceAsStream(resourceLocation);
                    if (resourceAsStream != null)
                    {
                        resourceBytes = IoUtil.ReadInputStream(resourceAsStream, resourcePath);
                    }
                }
                finally
                {
                    IoUtil.CloseSilently(resourceAsStream);
                }
            }
            else if (resourceType.Equals("deployment"))
            {
                ResourceEntity resourceEntity = deployment.GetResource(resourceLocation);
                if (resourceEntity != null)
                {
                    resourceBytes = resourceEntity.Bytes;
                }
            }

            if (resourceBytes != null)
                return StringHelperClass.NewString(resourceBytes, Encoding.UTF8.ToString());
            throw Log.CannotFindResource(resourcePath);
        }
    }
}