namespace ESS.FW.Bpm.Engine.Application
{
    /// <summary>
    ///     <para>Object holding information about a deployment made by a process application.</para>
    ///     
    /// </summary>
    public interface IProcessApplicationDeploymentInfo
    {
        /// <returns> the name of the process engine the deployment was made to </returns>
        string ProcessEngineName { get; }

        /// <returns> the id of the deployment that was performed. </returns>
        string DeploymentId { get; }
    }
}