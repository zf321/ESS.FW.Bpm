



using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Cmd;
using ESS.FW.Bpm.Engine.Impl.Interceptor;

namespace ESS.FW.Bpm.Engine.Persistence.Deploy
{
	public class DeploymentFailListener : Impl.Cfg.ITransactionListener
	{

	  protected internal IList<string> DeploymentIds;

	  public DeploymentFailListener(string deploymentId)
	  {
	      this.DeploymentIds = new List<string>() {deploymentId};// Collections.singleton(deploymentId);
	  }

	  public DeploymentFailListener(IList<string> deploymentIds)
	  {
		this.DeploymentIds = deploymentIds;
	  }

	  public virtual void Execute(CommandContext commandContext)
	  {
		(new UnregisterDeploymentCmd(DeploymentIds)).Execute(commandContext);
	  }

	}

}