using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;

namespace Engine.Tests.Standalone.Interceptor
{

    /// <summary>
	/// 
	/// 
	/// </summary>
    [TestFixture]
    public class MultiEngineCommandContextTest
	{

	  protected internal IProcessEngine Engine1;
	  protected internal IProcessEngine Engine2;

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void startEngines()
	  public virtual void StartEngines()
	  {
		Engine1 = CreateProcessEngine("engine1");
		Engine2 = CreateProcessEngine("engine2");
		StartProcessInstanceOnEngineDelegate.Engines["engine1"] = Engine1;
		StartProcessInstanceOnEngineDelegate.Engines["engine2"] = Engine2;
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void closeEngine1()
	  public virtual void CloseEngine1()
	  {
		try
		{
		  Engine1.Close();
		}
		finally
		{
		  Engine1 = null;
		}
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void closeEngine2()
	  public virtual void CloseEngine2()
	  {
		try
		{
		  Engine2.Close();
		}
		finally
		{
		  Engine2 = null;
		}
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void removeEngines()
	  public virtual void RemoveEngines()
	  {
		StartProcessInstanceOnEngineDelegate.Engines.Clear();
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldOpenNewCommandContextWhenInteractingAccrossEngines()
	  public virtual void ShouldOpenNewCommandContextWhenInteractingAccrossEngines()
	  {

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
		IBpmnModelInstance process1 = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process1").StartEvent().ServiceTask().CamundaInputParameter("engineName", "engine2").CamundaInputParameter("processKey", "process2").Func(m=>m.CamundaClass=typeof(StartProcessInstanceOnEngineDelegate).FullName)/*.CamundaClass(typeof(StartProcessInstanceOnEngineDelegate).FullName)*/.EndEvent().Done();

		IBpmnModelInstance process2 = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process2").StartEvent().EndEvent().Done();

		// given
		Engine1.RepositoryService.CreateDeployment().AddModelInstance("process1.bpmn", process1).Deploy();
		Engine2.RepositoryService.CreateDeployment().AddModelInstance("process2.bpmn", process2).Deploy();

		// if
		Engine1.RuntimeService.StartProcessInstanceByKey("process1");
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldOpenNewCommandContextWhenInteractingWithOtherEngineAndBack()
	  public virtual void ShouldOpenNewCommandContextWhenInteractingWithOtherEngineAndBack()
	  {

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
		IBpmnModelInstance process1 = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process1").StartEvent().ServiceTask().CamundaInputParameter("engineName", "engine2").CamundaInputParameter("processKey", "process2").Func(m=>m.CamundaClass=(typeof(StartProcessInstanceOnEngineDelegate).FullName)).EndEvent().Done();

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
		IBpmnModelInstance process2 = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process2").StartEvent().ServiceTask().CamundaInputParameter("engineName", "engine1").CamundaInputParameter("processKey", "process3").Func(m=>m.CamundaClass=(typeof(StartProcessInstanceOnEngineDelegate).FullName)).Done();

		IBpmnModelInstance process3 = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process3").StartEvent().EndEvent().Done();

		// given
		Engine1.RepositoryService.CreateDeployment().AddModelInstance("process1.bpmn", process1).Deploy();
		Engine2.RepositoryService.CreateDeployment().AddModelInstance("process2.bpmn", process2).Deploy();
		Engine1.RepositoryService.CreateDeployment().AddModelInstance("process3.bpmn", process3).Deploy();

		// if
		Engine1.RuntimeService.StartProcessInstanceByKey("process1");
	  }

	  private IProcessEngine CreateProcessEngine(string name)
	  {
		StandaloneInMemProcessEngineConfiguration processEngineConfiguration = new StandaloneInMemProcessEngineConfiguration();
		processEngineConfiguration.SetProcessEngineName(name);
		processEngineConfiguration.JdbcUrl = string.Format("jdbc:h2:mem:{0}", name);
		return processEngineConfiguration.BuildProcessEngine();
	  }

	}

}