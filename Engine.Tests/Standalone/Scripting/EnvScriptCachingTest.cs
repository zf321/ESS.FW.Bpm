//using System;
//using System.Collections.Generic;
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
//	public class EnvScriptCachingTest : PluggableProcessEngineTestCase
//	{

//	  protected internal const string PROCESS_PATH = "resources/api/oneTaskProcess.bpmn20.xml";
//	  protected internal const string SCRIPT_LANGUAGE = "groovy";
//	  protected internal const string SCRIPT = "println 'hello world'";
//	  protected internal const string ENV_SCRIPT = "println 'hello world from env script'";
//	  protected internal static readonly ScriptEnvResolver RESOLVER;

//	  static EnvScriptCachingTest()
//	  {
//		RESOLVER = new ScriptEnvResolverAnonymousInnerClass();
//	  }

//	  private class ScriptEnvResolverAnonymousInnerClass : ScriptEnvResolver
//	  {
//		  public ScriptEnvResolverAnonymousInnerClass()
//		  {
//		  }

//		  public virtual string[] resolve(string language)
//		  {
//			return new string[] {ENV_SCRIPT};
//		  }
//	  }

//	  protected internal ScriptFactory scriptFactory;

////JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
////ORIGINAL LINE: public void setUp() throws Exception
//	  public virtual void setUp()
//	  {
//		base.SetUp();
//		scriptFactory = processEngineConfiguration.ScriptFactory;
//		processEngineConfiguration.EnvScriptResolvers.Add(RESOLVER);
//	  }

////JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
////ORIGINAL LINE: protected void tearDown() throws Exception
//	  protected internal virtual void tearDown()
//	  {
//		base.TearDown();
//		processEngineConfiguration.EnvScriptResolvers.Remove(RESOLVER);
//	  }

//	  public virtual void testEnabledPaEnvScriptCaching()
//	  {
//		// given
//		EmbeddedProcessApplication processApplication = new EmbeddedProcessApplication();

//		IProcessApplicationDeployment deployment = repositoryService.CreateDeployment(processApplication.Reference).AddClasspathResource(PROCESS_PATH).Deploy();

//		// when
//		executeScript(processApplication);

//		// then
//		IDictionary<string, IList<ExecutableScript>> environmentScripts = processApplication.EnvironmentScripts;
//		Assert.NotNull(environmentScripts);

//		IList<ExecutableScript> groovyEnvScripts = environmentScripts[SCRIPT_LANGUAGE];

//		Assert.NotNull(groovyEnvScripts);
//		Assert.IsFalse(groovyEnvScripts.Count == 0);
//		Assert.AreEqual(processEngineConfiguration.EnvScriptResolvers.Count(), groovyEnvScripts.Count);

//		repositoryService.DeleteDeployment(deployment.Id, true);
//	  }

//	  public virtual void testDisabledPaEnvScriptCaching()
//	  {
//		// given
//		processEngineConfiguration.EnableFetchScriptEngineFromProcessApplication = false;

//		EmbeddedProcessApplication processApplication = new EmbeddedProcessApplication();

//		IProcessApplicationDeployment deployment = repositoryService.CreateDeployment(processApplication.Reference).AddClasspathResource(PROCESS_PATH).Deploy();

//		// when
//		executeScript(processApplication);

//		// then
//		IDictionary<string, IList<ExecutableScript>> environmentScripts = processApplication.EnvironmentScripts;
//		Assert.NotNull(environmentScripts);
//		Assert.IsNull(environmentScripts[SCRIPT_LANGUAGE]);

//		repositoryService.DeleteDeployment(deployment.Id, true);

//		processEngineConfiguration.EnableFetchScriptEngineFromProcessApplication = true;
//	  }

//	  protected internal virtual SourceExecutableScript createScript(string language, string source)
//	  {
//		return (SourceExecutableScript) scriptFactory.CreateScriptFromSource(language, source);
//	  }

////JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
////ORIGINAL LINE: protected void executeScript(final org.Camunda.bpm.application.IProcessApplicationInterface processApplication)
//	  protected internal virtual void executeScript(IProcessApplicationInterface processApplication)
//	  {
//		processEngineConfiguration.CommandExecutorTxRequired.Execute(new CommandAnonymousInnerClass(this, processApplication));
//	  }

//	  private class CommandAnonymousInnerClass : ICommand<object>
//	  {
//		  private readonly EnvScriptCachingTest outerInstance;

//		  private IProcessApplicationInterface processApplication;

//		  public CommandAnonymousInnerClass(EnvScriptCachingTest outerInstance, IProcessApplicationInterface processApplication)
//		  {
//			  this.outerInstance = outerInstance;
//			  this.ProcessApplication = processApplication;
//		  }

//		  public virtual object Execute(CommandContext commandContext)
//		  {
//			return Context.ExecuteWithinProcessApplication(new CallableAnonymousInnerClass(this), processApplication.Reference);
//		  }

//		  private class CallableAnonymousInnerClass : ICallable<object>
//		  {
//			  private readonly CommandAnonymousInnerClass outerInstance;

//			  public CallableAnonymousInnerClass(CommandAnonymousInnerClass outerInstance)
//			  {
//				  this.outerInstance = outerInstance;
//			  }


////JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
////ORIGINAL LINE: public object Call() throws Exception
//			  public virtual object Call()
//			  {
//				ScriptingEngines scriptingEngines = processEngineConfiguration.ScriptingEngines;
//				ScriptEngine scriptEngine = scriptingEngines.GetScriptEngineForLanguage(SCRIPT_LANGUAGE);

//				SourceExecutableScript script = outerInstance.outerInstance.CreateScript(SCRIPT_LANGUAGE, SCRIPT);

//				ScriptingEnvironment scriptingEnvironment = processEngineConfiguration.ScriptingEnvironment;
//				scriptingEnvironment.Execute(script, null, null, scriptEngine);

//				return null;
//			  }
//		  }
//	  }

//	}

//}