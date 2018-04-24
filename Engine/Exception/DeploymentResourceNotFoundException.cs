using System;

namespace ESS.FW.Bpm.Engine.exception
{
    /// <summary>
    ///     
    /// </summary>
    public class DeploymentResourceNotFoundException : ProcessEngineException
    {
        

        public DeploymentResourceNotFoundException()
        {
        }

        public DeploymentResourceNotFoundException(string message, System.Exception cause) : base(message, cause)
        {
        }

        public DeploymentResourceNotFoundException(string message) : base(message)
        {
        }

        public DeploymentResourceNotFoundException(System.Exception cause) : base( cause)
        {
        }
    }
}