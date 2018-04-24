using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.Application
{
    /// <summary>
    ///     <para>Object holding information about a deployed Process Application</para>
    ///     
    /// </summary>
    /// <seealso cref= ProcessApplicationService# getProcessApplicationInfo( String
    /// )
    /// </seealso>
    public interface IProcessApplicationInfo
    {
        /// <summary>
        ///     constant for the servlet context path property
        /// </summary>
        /// <returns> the name of the process application </returns>
        string Name { get; }

        /// <returns>
        ///     a list of <seealso cref="IProcessApplicationDeploymentInfo" /> objects that
        ///     provide information about the deployments made by the process
        ///     application to the process engine(s).
        /// </returns>
        IList<IProcessApplicationDeploymentInfo> DeploymentInfo { get; }

        /// <summary>
        ///     <para>Provides access to a list of process application-provided properties.</para>
        ///     <para>This class provides a set of constants for commonly-used properties</para>
        /// </summary>
        /// <seealso cref= ProcessApplicationInfo# PROP_SERVLET_CONTEXT_PATH
        /// </seealso>
        IDictionary<string, string> Properties { get; }
    }

    public static class ProcessApplicationInfoFields
    {
        public const string PropServletContextPath = "servletContextPath";
    }
}