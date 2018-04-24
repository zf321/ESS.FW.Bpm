using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Model.Dmn.instance;
using NUnit.Framework;

namespace Engine.Tests.Api.Authorization.Util
{


    /// <summary>
    /// 
    /// 
    /// </summary>
    public class AuthorizationTestRule : AuthorizationTestBaseRule
    {

	  protected internal AuthorizationExceptionInterceptor Interceptor;
	  protected internal ICommandExecutor ReplacedCommandExecutor;

	  protected internal AuthorizationScenarioInstance ScenarioInstance;

	  public AuthorizationTestRule(ProcessEngineRule engineRule) : base(engineRule)
	  {
		this.Interceptor = new AuthorizationExceptionInterceptor();
	  }

	  public virtual void Start(AuthorizationScenario scenario)
	  {
		Start(scenario, null, new Dictionary<string, string>());
	  }

	  public virtual void Start(AuthorizationScenario scenario, string userId, IDictionary<string, string> resourceBindings)
	  {
		Assert.IsNull(Interceptor.LastException);
		ScenarioInstance = new AuthorizationScenarioInstance(scenario, EngineRule.IAuthorizationService, resourceBindings);
		EnableAuthorization(userId);
		Interceptor.Activate();
	  }

        /// <summary>
        /// Assert the scenario conditions. If no exception or the expected one was thrown.
        /// </summary>
        /// <param name="scenario"> the scenario to Assert on </param>
        /// <returns> true if no exception was thrown, false otherwise </returns>
        public virtual bool AssertScenario(AuthorizationScenario scenario)
        {

		Interceptor.Deactivate();
		DisableAuthorization();
		ScenarioInstance.tearDown(EngineRule.IAuthorizationService);
		ScenarioInstance.AssertAuthorizationException(Interceptor.LastException);
		ScenarioInstance = null;

		return ScenarioSucceeded();
	  }

	  /// <summary>
	  /// No exception was expected and no was thrown
	  /// </summary>
	  public virtual bool ScenarioSucceeded()
	  {
		return Interceptor.LastException == null;
	  }

	  public virtual bool ScenarioFailed()
	  {
		return Interceptor.LastException != null;
	  }

	  protected internal virtual void Starting(IDescription description)
	  {
		ProcessEngineConfigurationImpl engineConfiguration = (ProcessEngineConfigurationImpl) EngineRule.ProcessEngine.ProcessEngineConfiguration;

		Interceptor.Reset();
		engineConfiguration.CommandInterceptorsTxRequired[0].Next = Interceptor;
		Interceptor.Next = engineConfiguration.CommandInterceptorsTxRequired[1];

		//base.Starting(description);
	  }

	    protected internal override void Finished(IDescription description)
	  {
		base.Finished(description);

            ProcessEngineConfigurationImpl engineConfiguration = (ProcessEngineConfigurationImpl)EngineRule.ProcessEngine.ProcessEngineConfiguration;

		engineConfiguration.CommandInterceptorsTxRequired[0].Next = Interceptor.Next;
		Interceptor.Next = null;
	  }

	  public static ICollection<AuthorizationScenario[]> AsParameters(params AuthorizationScenario[] scenarios)
	  {
		IList<AuthorizationScenario[]> scenarioList = new List<AuthorizationScenario[]>();
		foreach (AuthorizationScenario scenario in scenarios)
		{
		  scenarioList.Add(new AuthorizationScenario[]{scenario});
		}

            return scenarioList;
        }

	  public virtual AuthorizationScenarioInstanceBuilder Init(AuthorizationScenario scenario)
	  {
		AuthorizationScenarioInstanceBuilder builder = new AuthorizationScenarioInstanceBuilder();
		builder.Scenario = scenario;
		builder.Rule = this;
		return builder;
	  }

	  public class AuthorizationScenarioInstanceBuilder
	  {
		protected internal AuthorizationScenario Scenario;
		protected internal AuthorizationTestRule Rule;
		protected internal string UserId;
		protected internal IDictionary<string, string> ResourceBindings = new Dictionary<string, string>();

		public virtual AuthorizationScenarioInstanceBuilder WithUser(string userId)
		{
		  this.UserId = userId;
		  return this;
		}

		public virtual AuthorizationScenarioInstanceBuilder BindResource(string key, string value)
		{
		  ResourceBindings[key] = value;
		  return this;
		}

		public virtual void Start()
		{
		  Rule.Start(Scenario, UserId, ResourceBindings);
		}
	  }

    }

}