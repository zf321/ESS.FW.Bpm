

//using ESS.FW.Bpm.Engine.Application;
//using ESS.FW.Bpm.Engine.Application.Impl;
//using ESS.FW.Bpm.Engine.context.Impl;
//using ESS.FW.Bpm.Engine.Common;
//using ESS.FW.Bpm.Engine.Impl.Interceptor;
//using NUnit.Framework;
//using PluggableProcessEngineTestCase = ESS.FW.Bpm.Engine.Tests.PluggableProcessEngineTestCase;

//namespace ESS.FW.Bpm.Engine.Tests.Standalone.Scripting
//{
    

//	/// <summary>
//	/// 
//	/// 
//	/// </summary>
//	public class ScriptEngineCachingTest : PluggableProcessEngineTestCase
//	{

//	  protected internal const string PROCESS_PATH = "resources/api/oneTaskProcess.bpmn20.xml";
//	  protected internal const string SCRIPT_LANGUAGE = "groovy";

//	  public virtual void testGlobalCachingOfScriptEngine()
//	  {
//		// when
//		ScriptEngine engine = getScriptEngine(SCRIPT_LANGUAGE);

//		// then
//		Assert.NotNull(engine);
//		Assert.AreEqual(engine, getScriptEngine(SCRIPT_LANGUAGE));
//	  }

//	  public virtual void testGlobalDisableCachingOfScriptEngine()
//	  {
//		// then
//		processEngineConfiguration.EnableScriptEngineCaching = false;
//		ScriptingEngines.EnableScriptEngineCaching = false;

//		// when
//		ScriptEngine engine = getScriptEngine(SCRIPT_LANGUAGE);

//		// then
//		Assert.NotNull(engine);
//		Assert.IsFalse(engine.Equals(getScriptEngine(SCRIPT_LANGUAGE)));

//		processEngineConfiguration.EnableScriptEngineCaching = true;
//		ScriptingEngines.EnableScriptEngineCaching = true;
//	  }

//	  public virtual void testCachingOfScriptEngineInProcessApplication()
//	  {
//		// given
//		EmbeddedProcessApplication processApplication = new EmbeddedProcessApplication();

//		// when
//		ScriptEngine engine = processApplication.GetScriptEngineForName(SCRIPT_LANGUAGE, true);

//		// then
//		Assert.NotNull(engine);
//		Assert.AreEqual(engine, processApplication.GetScriptEngineForName(SCRIPT_LANGUAGE, true));
//	  }

//	  public virtual void testDisableCachingOfScriptEngineInProcessApplication()
//	  {
//		// given
//		EmbeddedProcessApplication processApplication = new EmbeddedProcessApplication();

//		// when
//		ScriptEngine engine = processApplication.GetScriptEngineForName(SCRIPT_LANGUAGE, false);

//		// then
//		Assert.NotNull(engine);
//		Assert.IsFalse(engine.Equals(processApplication.GetScriptEngineForName(SCRIPT_LANGUAGE, false)));
//	  }

//	  public virtual void testFetchScriptEngineFromPaEnableCaching()
//	  {
//		// then
//		EmbeddedProcessApplication processApplication = new EmbeddedProcessApplication();

//		IProcessApplicationDeployment deployment = repositoryService.CreateDeployment(processApplication.Reference).AddClasspathResource(PROCESS_PATH).Deploy();

//		// when
//		ScriptEngine engine = getScriptEngineFromPa(SCRIPT_LANGUAGE, processApplication);

//		// then
//		Assert.NotNull(engine);
//		Assert.AreEqual(engine, getScriptEngineFromPa(SCRIPT_LANGUAGE, processApplication));

//		// cached in pa
//		Assert.AreEqual(engine, processApplication.GetScriptEngineForName(SCRIPT_LANGUAGE, true));

//		repositoryService.DeleteDeployment(deployment.Id, true);
//	  }

//	  public virtual void testFetchScriptEngineFromPaDisableCaching()
//	  {
//		// then
//		processEngineConfiguration.EnableScriptEngineCaching = false;
//		ScriptingEngines.EnableScriptEngineCaching = false;

//		EmbeddedProcessApplication processApplication = new EmbeddedProcessApplication();

//		IProcessApplicationDeployment deployment = repositoryService.CreateDeployment(processApplication.Reference).AddClasspathResource(PROCESS_PATH).Deploy();

//		// when
//		ScriptEngine engine = getScriptEngineFromPa(SCRIPT_LANGUAGE, processApplication);

//		// then
//		Assert.NotNull(engine);
//		Assert.IsFalse(engine.Equals(getScriptEngineFromPa(SCRIPT_LANGUAGE, processApplication)));

//		// not cached in pa
//		Assert.IsFalse(engine.Equals(processApplication.GetScriptEngineForName(SCRIPT_LANGUAGE, false)));

//		repositoryService.DeleteDeployment(deployment.Id, true);

//		processEngineConfiguration.EnableScriptEngineCaching = true;
//		ScriptingEngines.EnableScriptEngineCaching = true;
//	  }

//	  public virtual void testDisableFetchScriptEngineFromProcessApplication()
//	  {
//		// when
//		processEngineConfiguration.EnableFetchScriptEngineFromProcessApplication = false;

//		EmbeddedProcessApplication processApplication = new EmbeddedProcessApplication();

//		IProcessApplicationDeployment deployment = repositoryService.CreateDeployment(processApplication.Reference).AddClasspathResource(PROCESS_PATH).Deploy();

//		// when
//		ScriptEngine engine = getScriptEngineFromPa(SCRIPT_LANGUAGE, processApplication);

//		// then
//		Assert.NotNull(engine);
//		Assert.AreEqual(engine, getScriptEngineFromPa(SCRIPT_LANGUAGE, processApplication));

//		// not cached in pa
//		Assert.IsFalse(engine.Equals(processApplication.GetScriptEngineForName(SCRIPT_LANGUAGE, true)));

//		repositoryService.DeleteDeployment(deployment.Id, true);

//		processEngineConfiguration.EnableFetchScriptEngineFromProcessApplication = true;
//	  }

//	  protected internal virtual ScriptingEngines ScriptingEngines
//	  {
//		  get
//		  {
//			return processEngineConfiguration.ScriptingEngines;
//		  }
//	  }

////JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
////ORIGINAL LINE: protected javax.script.ScriptEngine getScriptEngine(final String name)
//	  protected internal virtual ScriptEngine getScriptEngine(string name)
//	  {
////JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
////ORIGINAL LINE: final impl.scripting.Engine.ScriptingEngines scriptingEngines = getScriptingEngines();
//		ScriptingEngines scriptingEngines = ScriptingEngines;
//		return processEngineConfiguration.CommandExecutorTxRequired.Execute(new CommandAnonymousInnerClass(this, name, scriptingEngines));
//	  }

//	  private class CommandAnonymousInnerClass : ICommand<ScriptEngine>
//	  {
//		  private readonly ScriptEngineCachingTest outerInstance;

//		  private string name;
//		  private ScriptingEngines scriptingEngines;

//		  public CommandAnonymousInnerClass(ScriptEngineCachingTest outerInstance, string name, ScriptingEngines scriptingEngines)
//		  {
//			  this.outerInstance = outerInstance;
//			  this.Name = name;
//			  this.scriptingEngines = scriptingEngines;
//		  }

//		  public virtual ScriptEngine execute(CommandContext commandContext)
//		  {
//			return scriptingEngines.GetScriptEngineForLanguage(name);
//		  }
//	  }

////JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
////ORIGINAL LINE: protected javax.script.ScriptEngine getScriptEngineFromPa(final String name, final org.Camunda.bpm.application.IProcessApplicationInterface processApplication)
//	  protected internal virtual ScriptEngine getScriptEngineFromPa(string name, IProcessApplicationInterface processApplication)
//	  {
//		return processEngineConfiguration.CommandExecutorTxRequired.Execute(new CommandAnonymousInnerClass2(this, name, processApplication));
//	  }

//	  private class CommandAnonymousInnerClass2 : ICommand<ScriptEngine>
//	  {
//		  private readonly ScriptEngineCachingTest outerInstance;

//		  private string name;
//		  private IProcessApplicationInterface processApplication;

//		  public CommandAnonymousInnerClass2(ScriptEngineCachingTest outerInstance, string name, IProcessApplicationInterface processApplication)
//		  {
//			  this.outerInstance = outerInstance;
//			  this.Name = name;
//			  this.ProcessApplication = processApplication;
//		  }

//		  public virtual ScriptEngine execute(CommandContext commandContext)
//		  {
//			return Context.ExecuteWithinProcessApplication(new CallableAnonymousInnerClass(this), processApplication.Reference);
//		  }

//		  private class CallableAnonymousInnerClass : ICallable<ScriptEngine>
//		  {
//			  private readonly CommandAnonymousInnerClass2 outerInstance;

//			  public CallableAnonymousInnerClass(CommandAnonymousInnerClass2 outerInstance)
//			  {
//				  this.outerInstance = outerInstance;
//			  }


////JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
////ORIGINAL LINE: public javax.script.ScriptEngine Call() throws Exception
//			  public virtual ScriptEngine Call()
//			  {
//				return outerInstance.outerInstance.GetScriptEngine(outerInstance.Name);
//			  }
//		  }
//	  }

//	}

//}