using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.Application.Impl
{
    /// <summary>
    ///     
    /// </summary>
    public class DefaultProcessApplicationRegistration : IProcessApplicationRegistration
    {
        protected internal IList<string> deploymentIds;
        protected internal string processEngineName;
        protected internal IProcessApplicationReference reference;

        /// <param name="reference"> </param>
        public DefaultProcessApplicationRegistration(IProcessApplicationReference reference, IList<string> deploymentIds,
            string processEnginenName)
        {
            this.reference = reference;
            this.deploymentIds = deploymentIds;
            processEngineName = processEnginenName;
        }

        public virtual IProcessApplicationReference Reference
        {
            get { return reference; }
        }

        public virtual IList<string> DeploymentIds
        {
            get { return deploymentIds; }
        }

        public virtual string ProcessEngineName
        {
            get { return processEngineName; }
        }
    }
}