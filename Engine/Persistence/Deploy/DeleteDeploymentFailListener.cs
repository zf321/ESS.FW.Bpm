


using ESS.FW.Bpm.Engine.Application;
using ESS.FW.Bpm.Engine.Impl.Cmd;
using ESS.FW.Bpm.Engine.Impl.Interceptor;

namespace ESS.FW.Bpm.Engine.Persistence.Deploy
{
	public class DeleteDeploymentFailListener : Impl.Cfg.ITransactionListener
	{

	  protected internal string DeploymentId;
	  protected internal IProcessApplicationReference processApplicationReference;

	  public DeleteDeploymentFailListener(string deploymentId, IProcessApplicationReference processApplicationReference)
	  {
		this.DeploymentId = deploymentId;
		this.processApplicationReference = processApplicationReference;
	  }

	  public virtual void Execute(CommandContext commandContext)
	  {
		(new RegisterDeploymentCmd(DeploymentId)).Execute(commandContext);
		if (processApplicationReference != null)
		{
		  (new RegisterProcessApplicationCmd(DeploymentId, processApplicationReference)).Execute(commandContext);
		}
	  }

	}

}