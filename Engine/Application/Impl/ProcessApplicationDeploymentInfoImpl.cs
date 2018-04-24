namespace ESS.FW.Bpm.Engine.Application.Impl
{
    /// <summary>
    ///     
    /// </summary>
    public class ProcessApplicationDeploymentInfoImpl : IProcessApplicationDeploymentInfo
    {
        protected internal string deploymentId;

        protected internal string processEngineName;

        public virtual string ProcessEngineName
        {
            get { return processEngineName; }
            set { processEngineName = value; }
        }


        public virtual string DeploymentId
        {
            get { return deploymentId; }
            set { deploymentId = value; }
        }
    }
}