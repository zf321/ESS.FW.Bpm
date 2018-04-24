using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Application;
using ESS.FW.Bpm.Engine.Repository;



namespace ESS.FW.Bpm.Engine.Persistence.Entity
{



    /// <summary>
	/// 
	/// 
	/// </summary>
	public class ProcessApplicationDeploymentImpl : IProcessApplicationDeployment
	{

	  protected internal IDeploymentWithDefinitions Deployment;
	  protected internal IProcessApplicationRegistration Registration;

	  public ProcessApplicationDeploymentImpl(IDeploymentWithDefinitions deployment, IProcessApplicationRegistration registration)
	  {
		this.Deployment = deployment;
		this.Registration = registration;
	  }

	  public virtual string Id
	  {
		  get
		  {
			return Deployment.Id;
		  }
	  }

	  public virtual string Name
	  {
		  get
		  {
			return Deployment.Name;
		  }
	  }

	  public virtual DateTime DeploymentTime
	  {
		  get
		  {
			return Deployment.DeploymentTime;
		  }
	  }

	  public virtual string Source
	  {
		  get
		  {
			return Deployment.Source;
		  }
	  }

	  public virtual string TenantId
	  {
		  get
		  {
			return Deployment.TenantId;
		  }
	  }

	  public virtual IProcessApplicationRegistration ProcessApplicationRegistration
	  {
		  get
		  {
			return Registration;
		  }
	  }

	  public  IList<IProcessDefinition> DeployedProcessDefinitions
	  {
		  get
		  {
			return Deployment.DeployedProcessDefinitions;
		  }
	  }

	  public  IList<ICaseDefinition> DeployedCaseDefinitions
	  {
		  get
		  {
			return Deployment.DeployedCaseDefinitions;
		  }
	  }

	  public  IList<IDecisionDefinition> DeployedDecisionDefinitions
	  {
		  get
		  {
			return Deployment.DeployedDecisionDefinitions;
		  }
	  }

	  public  IList<IDecisionRequirementsDefinition> DeployedDecisionRequirementsDefinitions
	  {
		  get
		  {
			return Deployment.DeployedDecisionRequirementsDefinitions;
		  }
	  }
	}

}