using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.Application.Impl
{
    /// <summary>
    ///     
    /// </summary>
    public class ProcessApplicationInfoImpl : IProcessApplicationInfo
    {
        protected internal IList<IProcessApplicationDeploymentInfo> deploymentInfo;

        protected internal string name;
        protected internal IDictionary<string, string> properties;

        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }


        public virtual IList<IProcessApplicationDeploymentInfo> DeploymentInfo
        {
            get { return deploymentInfo; }
            set { deploymentInfo = value; }
        }


        public virtual IDictionary<string, string> Properties
        {
            get { return properties; }
            set { properties = value; }
        }
    }
}