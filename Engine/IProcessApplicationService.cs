using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Application;

namespace ESS.FW.Bpm.Engine
{
    /// <summary>
    ///     <para>The process application service provides access to all deployed process applications.</para>
    ///     
    /// </summary>
    public interface IProcessApplicationService
    {
        /// <summary>
        ///     @returns the names of all deployed process applications
        /// </summary>
        ISet<string> ProcessApplicationNames { get; }

        /// <summary>
        ///     <para>Provides information about a deployed process application</para>
        /// </summary>
        /// <param name="processApplicationName">
        /// </param>
        /// <returns> the <seealso cref="IProcessApplicationInfo" /> object or null if no such process application is deployed.  </returns>
        IProcessApplicationInfo GetProcessApplicationInfo(string processApplicationName);
    }
}