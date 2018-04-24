namespace ESS.FW.Bpm.Engine.Repository
{
    /// <summary>
    ///     Contains the constants for the possible values the property
    ///     <seealso cref="IProcessApplicationDeploymentBuilder#resumePreviousVersionsBy(String)" />.
    /// </summary>
    public sealed class ResumePreviousBy
    {
        /// <summary>
        ///     Resume previous deployments that contain processes with the same key as in the new deployment
        /// </summary>
        public const string ResumeByProcessDefinitionKey = "process-definition-key";

        /// <summary>
        ///     Resume previous deployments that have the same name as the new deployment
        /// </summary>
        public const string ResumeByDeploymentName = "deployment-name";
    }
}