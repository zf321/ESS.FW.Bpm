using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine
{
    /// <summary>
    ///     <para>The <seealso cref="IProcessEngineService" /> provides access to the list of Managed Process Engines.</para>
    ///     <para>
    ///         Users of this class may look up an instance of the service through a lookup strategy
    ///         appropriate for the platform they are using (Examples: Jndi, OSGi Service Registry ...)
    ///     </para>
    ///     
    /// </summary>
    public interface IProcessEngineService
    {
        /// <returns> the default process engine. </returns>
        IProcessEngine DefaultProcessEngine { get; }

        /// <returns> all <seealso cref="IProcessEngine" /> managed by the camunda BPM platform. </returns>
        IList<IProcessEngine> ProcessEngines { get; }

        /// <returns> the names of all <seealso cref="IProcessEngine" /> managed by the camunda BPM platform. </returns>
        ISet<string> ProcessEngineNames { get; }

        /// <returns> the <seealso cref="IProcessEngine" /> for the given name or null if no such process engine exists. </returns>
        IProcessEngine GetProcessEngine(string name);
    }
}