using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Repository;

namespace ESS.FW.Bpm.Engine.Application
{
    /// <summary>
    ///     <para>Represents a registration of a process application with a process engine</para>
    ///     
    /// </summary>
    /// <seealso cref= ManagementService# registerProcessApplication( String, ProcessApplicationReference
    /// )
    /// </seealso>
    public interface IProcessApplicationRegistration
    {
        /// <returns> the id of the <seealso cref="IDeployment" /> for which the registration was created </returns>
        IList<string> DeploymentIds { get; }

        /// <returns> the name of the process engine to which the deployment was made </returns>
        string ProcessEngineName { get; }
    }
}