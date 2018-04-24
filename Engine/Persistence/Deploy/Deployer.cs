using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Persistence.Deploy
{
	/// <summary>
	///  
	/// </summary>
	public interface IDeployer
	{

	  void Deploy(DeploymentEntity deployment);
	}

}