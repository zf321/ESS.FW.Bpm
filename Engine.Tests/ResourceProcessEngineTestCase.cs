

namespace Engine.Tests
{


	/// <summary>
	/// 
	/// @author Joram Barrez
	/// </summary>
	public abstract class ResourceProcessEngineTestCase : AbstractProcessEngineTestCase
	{

	  protected internal string EngineConfigurationResource;

	  public ResourceProcessEngineTestCase(string configurationResource)
	  {
		this.EngineConfigurationResource = configurationResource;
	  }

	  protected internal override void CloseDownProcessEngine()
	  {
		base.CloseDownProcessEngine();
		ProcessEngine.Close();
		ProcessEngine = null;
	  }

	  protected internal override void InitializeProcessEngine()
	  {
		ProcessEngine = ESS.FW.Bpm.Engine.ProcessEngineConfiguration.CreateProcessEngineConfigurationFromResource(EngineConfigurationResource).BuildProcessEngine();
	  }

	}

}