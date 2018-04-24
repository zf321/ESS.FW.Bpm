using System.Linq;
using Engine.Tests.Api.Authorization.Util;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Task;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.TaskListener
{
    //API中AuthorizationTestRule 暂时没有修改完
	using ProcessEngineTestRule = ProcessEngineTestRule;
	using ProvidedProcessEngineRule = ProvidedProcessEngineRule;

    //using ExpectedException = org.junit.Rules.ExpectedException;
	//using RuleChain = org.junit.Rules.RuleChain;

	/// <summary>
	///
	/// </summary>
	[TestFixture]
    public class TaskListenerDelegateCompletionTest
	{
		private bool InstanceFieldsInitialized = false;

        public TaskListenerDelegateCompletionTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}
      //  [SetUp]
        private void InitializeInstanceFields()
		{
            authRule = new AuthorizationTestRule(engineRule);
            testHelper = new ProcessEngineTestRule(engineRule);
			//ruleChain = ruleChain.outerRule(engineRule).around(authRule).around(testHelper);
		}


	  protected internal const string COMPLETE_LISTENER = "bpmn.Tasklistener.util.CompletingTaskListener";
	  protected internal const string TASK_LISTENER_PROCESS = "taskListenerProcess";
	  protected internal const string ACTIVITY_ID = "UT";

	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  protected internal AuthorizationTestRule authRule;
	  protected internal ProcessEngineTestRule testHelper;


        //ORIGINAL LINE: @Rule public org.junit.Rules.ExpectedException thrown = org.junit.Rules.ExpectedException.None();
        //public NUnit.Framework.ExpectedException thrown = new NUnit.Framework.ExpectedException.None();
       

      //ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(engineRule).around(authRule).around(testHelper);
	  ////public RuleChain ruleChain; 

	  protected internal IRuntimeService runtimeService;
	  protected internal ITaskService taskService;
        
        [SetUp]
	  public virtual void setUp()
	  {
		taskService = engineRule.TaskService;
		runtimeService = engineRule.RuntimeService;
	  }
        
        [TearDown]
	  public virtual void cleanUp()
	  {
		if (runtimeService.CreateProcessInstanceQuery().Count() > 0)
		{
		  runtimeService.DeleteProcessInstance(runtimeService.CreateProcessInstanceQuery().First().Id,null,true);
		}
	  }


	  protected internal static IBpmnModelInstance setupProcess(string eventName)
	  {
	     return ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(TASK_LISTENER_PROCESS).StartEvent().UserTask(ACTIVITY_ID)
                .CamundaTaskListenerClass(eventName,COMPLETE_LISTENER).EndEvent().Done();
	  }

        [Test]
        [Deployment]
        public virtual void testCompletionIsPossibleOnCreation()
	  {
		//given
		createProcessWithListener(TaskListenerFields.EventnameCreate);

		//when
		runtimeService.StartProcessInstanceByKey(TASK_LISTENER_PROCESS);

		//then
		ITask task = taskService.CreateTaskQuery().First();
		Assert.That(task, null);
	  }

        [Test]
        public virtual void testCompletionIsPossibleOnAssignment()
	  {
		//given
		createProcessWithListener(TaskListenerFields.EventnameAssignment);

		//when
		runtimeService.StartProcessInstanceByKey(TASK_LISTENER_PROCESS);
		ITask task = taskService.CreateTaskQuery().First();
		taskService.SetAssignee(task.Id,"test assignee");

		//then
		task = taskService.CreateTaskQuery().First();
		Assert.That(task, null);//nullValue
        }


        [Test]
        [Deployment]
	  public virtual void testCompletionIsNotPossibleOnComplete()
	  {
		// expect
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage.Contains("invalid task state");
		//given
		createProcessWithListener(TaskListenerFields.EventnameComplete);

		//when
		runtimeService.StartProcessInstanceByKey(TASK_LISTENER_PROCESS);
		ITask task = taskService.CreateTaskQuery().First();

		taskService.Complete(task.Id);
	  }


        [Test]
        [Deployment]
        public virtual void testCompletionIsNotPossibleOnDelete()
	  {
		// expect
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage.Contains("invalid task state");

		//given
		createProcessWithListener(TaskListenerFields.EventnameDelete);

		//when
		IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey(TASK_LISTENER_PROCESS);
		runtimeService.DeleteProcessInstance(processInstance.Id,"test reason");
	  }

	  protected internal virtual void createProcessWithListener(string eventName)
	  {
		IBpmnModelInstance bpmnModelInstance = setupProcess(eventName);
        //Junit 暂时屏蔽
        //testHelper.Deploy(bpmnModelInstance);
	  }

	}

}