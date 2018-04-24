using ESS.FW.Bpm.Engine.Application;

namespace ESS.FW.Bpm.Engine.Repository
{
    /// <summary>
    ///     
    /// </summary>
    public interface IProcessApplicationDeployment : IDeploymentWithDefinitions
    {
        /// <returns> the <seealso cref="ProcessApplicationRegistration" /> performed for this process application deployment. </returns>
        IProcessApplicationRegistration ProcessApplicationRegistration { get; }
    }

    public static class ProcessApplicationDeploymentFields
    {
        public const string ProcessApplicationDeploymentSource = "process application";
    }
}