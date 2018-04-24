using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Task;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;

namespace Engine.Tests.Api.MultiTenancy
{
    

	public class MultiTenancyMessageCorrelationTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyMessageCorrelationTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			testRule = new ProcessEngineTestRule(engineRule);
			//ruleChain = RuleChain.outerRule(engineRule).around(testRule);
		}


	  protected internal const string TENANT_ONE = "tenant1";
	  protected internal const string TENANT_TWO = "tenant2";

	  protected internal static readonly IBpmnModelInstance MESSAGE_START_PROCESS = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("messageStart").StartEvent().Message("message").UserTask().EndEvent().Done();

	  protected internal static readonly IBpmnModelInstance MESSAGE_CATCH_PROCESS = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("messageCatch").StartEvent().IntermediateCatchEvent().Message("message").UserTask().EndEvent().Done();

	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();

	  protected internal ProcessEngineTestRule testRule;


        [Test]
        public virtual void correlateMessageToStartEventNoTenantIdSetForNonTenant()
	  {
		testRule.Deploy(MESSAGE_START_PROCESS);

		engineRule.RuntimeService.CreateMessageCorrelation("message").Correlate();

		 IQueryable<IProcessInstance> query = engineRule.RuntimeService.CreateProcessInstanceQuery();
		Assert.That(query.Count(), Is.EqualTo(1L));
		Assert.That(query.First().TenantId, Is.EqualTo(null));
	  }


        [Test]
        public virtual void correlateMessageToStartEventNoTenantIdSetForTenant()
	  {
		testRule.DeployForTenant(TENANT_ONE, MESSAGE_START_PROCESS);

		engineRule.RuntimeService.CreateMessageCorrelation("message").Correlate();

		 IQueryable<IProcessInstance> query = engineRule.RuntimeService.CreateProcessInstanceQuery();
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
	  }


        [Test]
        public virtual void correlateMessageToStartEventWithoutTenantId()
	  {
		testRule.Deploy(MESSAGE_START_PROCESS);
		testRule.DeployForTenant(TENANT_ONE, MESSAGE_START_PROCESS);

		engineRule.RuntimeService.CreateMessageCorrelation("message" ).SetTenantId(null).Correlate();

		 IQueryable<IProcessInstance> query = engineRule.RuntimeService.CreateProcessInstanceQuery();
		Assert.That(query.Count(), Is.EqualTo(1L));
		Assert.That(query.First().TenantId, Is.EqualTo(null));
	  }


        [Test]
        public virtual void correlateMessageToStartEventWithTenantId()
	  {
		testRule.DeployForTenant(TENANT_ONE, MESSAGE_START_PROCESS);
		testRule.DeployForTenant(TENANT_TWO, MESSAGE_START_PROCESS);

		engineRule.RuntimeService.CreateMessageCorrelation("message").SetTenantId(TENANT_ONE).Correlate();

		 IQueryable<IProcessInstance> query = engineRule.RuntimeService.CreateProcessInstanceQuery();
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_TWO).Count(), Is.EqualTo(0L));
	  }


        [Test]
        public virtual void correlateMessageToIntermediateCatchEventNoTenantIdSetForNonTenant()
	  {
		testRule.Deploy(MESSAGE_CATCH_PROCESS);

		engineRule.RuntimeService.StartProcessInstanceByKey("messageCatch");

		engineRule.RuntimeService.CreateMessageCorrelation("message").Correlate();

		IQueryable<ITask> query = engineRule.TaskService.CreateTaskQuery();
		Assert.That(query.Count(), Is.EqualTo(1L));
	  }


        [Test]
        public virtual void correlateMessageToIntermediateCatchEventNoTenantIdSetForTenant()
	  {
		testRule.DeployForTenant(TENANT_ONE, MESSAGE_CATCH_PROCESS);

		engineRule.RuntimeService.StartProcessInstanceByKey("messageCatch");

		engineRule.RuntimeService.CreateMessageCorrelation("message").Correlate();

		IQueryable<ITask> query = engineRule.TaskService.CreateTaskQuery();
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
	  }


        [Test]
        public virtual void correlateMessageToIntermediateCatchEventWithoutTenantId()
	  {
		testRule.Deploy(MESSAGE_CATCH_PROCESS);
		testRule.DeployForTenant(TENANT_ONE, MESSAGE_CATCH_PROCESS);

		engineRule.RuntimeService.CreateProcessInstanceByKey("messageCatch").ProcessDefinitionWithoutTenantId().Execute();
		engineRule.RuntimeService.CreateProcessInstanceByKey("messageCatch").SetProcessDefinitionTenantId(TENANT_ONE).Execute();

		engineRule.RuntimeService.CreateMessageCorrelation("message" ).SetTenantId( null).Correlate();

		IQueryable<ITask> query = engineRule.TaskService.CreateTaskQuery();
		Assert.That(query.Count(), Is.EqualTo(1L));
		Assert.That(query.First().TenantId, Is.EqualTo(null));
	  }


        [Test]
        public virtual void correlateMessageToIntermediateCatchEventWithTenantId()
	  {
		testRule.DeployForTenant(TENANT_ONE, MESSAGE_CATCH_PROCESS);
		testRule.DeployForTenant(TENANT_TWO, MESSAGE_CATCH_PROCESS);

		engineRule.RuntimeService.CreateProcessInstanceByKey("messageCatch").SetProcessDefinitionTenantId(TENANT_ONE).Execute();
		engineRule.RuntimeService.CreateProcessInstanceByKey("messageCatch").SetProcessDefinitionTenantId(TENANT_TWO).Execute();

		engineRule.RuntimeService.CreateMessageCorrelation("message").SetTenantId(TENANT_ONE).Correlate();

		IQueryable<ITask> query = engineRule.TaskService.CreateTaskQuery();
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_TWO).Count(), Is.EqualTo(0L));
	  }


        [Test]
        public virtual void correlateMessageToStartAndIntermediateCatchEventWithoutTenantId()
	  {
		testRule.Deploy(MESSAGE_START_PROCESS, MESSAGE_CATCH_PROCESS);
		testRule.DeployForTenant(TENANT_ONE, MESSAGE_START_PROCESS, MESSAGE_CATCH_PROCESS);

		engineRule.RuntimeService.CreateProcessInstanceByKey("messageCatch").ProcessDefinitionWithoutTenantId().Execute();

		engineRule.RuntimeService.CreateMessageCorrelation("message" ).SetTenantId( null).CorrelateAll();

	      IList<ITask> tasks = engineRule.TaskService.CreateTaskQuery()
	          .ToList();
		Assert.That(tasks.Count, Is.EqualTo(2));
		Assert.That(tasks[0].TenantId, Is.EqualTo(null));
		Assert.That(tasks[1].TenantId, Is.EqualTo(null));
	  }


        [Test]
        public virtual void correlateMessageToStartAndIntermediateCatchEventWithTenantId()
	  {
		testRule.DeployForTenant(TENANT_ONE, MESSAGE_START_PROCESS, MESSAGE_CATCH_PROCESS);
		testRule.DeployForTenant(TENANT_TWO, MESSAGE_START_PROCESS, MESSAGE_CATCH_PROCESS);

		engineRule.RuntimeService.CreateProcessInstanceByKey("messageCatch").SetProcessDefinitionTenantId(TENANT_ONE).Execute();
		engineRule.RuntimeService.CreateProcessInstanceByKey("messageCatch").SetProcessDefinitionTenantId(TENANT_TWO).Execute();

		engineRule.RuntimeService.CreateMessageCorrelation("message").SetTenantId(TENANT_ONE).CorrelateAll();

		IQueryable<ITask> query = engineRule.TaskService.CreateTaskQuery();
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_TWO).Count(), Is.EqualTo(0L));
	  }


        [Test]
        public virtual void correlateMessageToMultipleIntermediateCatchEventsWithoutTenantId()
	  {
		testRule.Deploy(MESSAGE_CATCH_PROCESS);
		testRule.DeployForTenant(TENANT_ONE, MESSAGE_CATCH_PROCESS);

		engineRule.RuntimeService.CreateProcessInstanceByKey("messageCatch").ProcessDefinitionWithoutTenantId().Execute();
		engineRule.RuntimeService.CreateProcessInstanceByKey("messageCatch").ProcessDefinitionWithoutTenantId().Execute();

		engineRule.RuntimeService.CreateProcessInstanceByKey("messageCatch").SetProcessDefinitionTenantId(TENANT_ONE).Execute();
		engineRule.RuntimeService.CreateProcessInstanceByKey("messageCatch").SetProcessDefinitionTenantId(TENANT_ONE).Execute();

		engineRule.RuntimeService.CreateMessageCorrelation("message" ).SetTenantId( null).CorrelateAll();

		IList<ITask> tasks = engineRule.TaskService.CreateTaskQuery().ToList();
		Assert.That(tasks.Count, Is.EqualTo(2));
		Assert.That(tasks[0].TenantId, Is.EqualTo(null));
		Assert.That(tasks[1].TenantId, Is.EqualTo(null));
	  }


        [Test]
        public virtual void correlateMessageToMultipleIntermediateCatchEventsWithTenantId()
	  {
		testRule.DeployForTenant(TENANT_ONE, MESSAGE_CATCH_PROCESS);
		testRule.DeployForTenant(TENANT_TWO, MESSAGE_CATCH_PROCESS);

		engineRule.RuntimeService.CreateProcessInstanceByKey("messageCatch").SetProcessDefinitionTenantId(TENANT_ONE).Execute();
		engineRule.RuntimeService.CreateProcessInstanceByKey("messageCatch").SetProcessDefinitionTenantId(TENANT_ONE).Execute();

		engineRule.RuntimeService.CreateProcessInstanceByKey("messageCatch").SetProcessDefinitionTenantId(TENANT_TWO).Execute();
		engineRule.RuntimeService.CreateProcessInstanceByKey("messageCatch").SetProcessDefinitionTenantId(TENANT_TWO).Execute();

		engineRule.RuntimeService.CreateMessageCorrelation("message").SetTenantId(TENANT_ONE).CorrelateAll();

		IQueryable<ITask> query = engineRule.TaskService.CreateTaskQuery();
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_TWO).Count(), Is.EqualTo(0L));
	  }


        [Test]
        public virtual void correlateStartMessageWithoutTenantId()
	  {
		testRule.Deploy(MESSAGE_START_PROCESS);
		testRule.DeployForTenant(TENANT_ONE, MESSAGE_START_PROCESS);

		engineRule.RuntimeService.CreateMessageCorrelation("message" ).SetTenantId( null).CorrelateStartMessage();

		 IQueryable<IProcessInstance> query = engineRule.RuntimeService.CreateProcessInstanceQuery();
		Assert.That(query.Count(), Is.EqualTo(1L));
		Assert.That(query.First().TenantId, Is.EqualTo(null));
	  }


        [Test]
        public virtual void correlateStartMessageWithTenantId()
	  {
		testRule.DeployForTenant(TENANT_ONE, MESSAGE_START_PROCESS);
		testRule.DeployForTenant(TENANT_TWO, MESSAGE_START_PROCESS);

		engineRule.RuntimeService.CreateMessageCorrelation("message").SetTenantId(TENANT_ONE).CorrelateStartMessage();

		 IQueryable<IProcessInstance> query = engineRule.RuntimeService.CreateProcessInstanceQuery();
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_TWO).Count(), Is.EqualTo(0L));
	  }


        [Test]
        public virtual void correlateMessagesToStartEventsForMultipleTenants()
	  {
		testRule.DeployForTenant(TENANT_ONE, MESSAGE_START_PROCESS);
		testRule.DeployForTenant(TENANT_TWO, MESSAGE_START_PROCESS);

		engineRule.RuntimeService.CreateMessageCorrelation("message").CorrelateAll();

		 IQueryable<IProcessInstance> query = engineRule.RuntimeService.CreateProcessInstanceQuery();
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_TWO).Count(), Is.EqualTo(1L));
	  }


        [Test]
        public virtual void correlateMessagesToIntermediateCatchEventsForMultipleTenants()
	  {
		testRule.DeployForTenant(TENANT_ONE, MESSAGE_CATCH_PROCESS);
		testRule.DeployForTenant(TENANT_TWO, MESSAGE_CATCH_PROCESS);

		engineRule.RuntimeService.CreateProcessInstanceByKey("messageCatch").SetProcessDefinitionTenantId(TENANT_ONE).Execute();
		engineRule.RuntimeService.CreateProcessInstanceByKey("messageCatch").SetProcessDefinitionTenantId(TENANT_TWO).Execute();

		engineRule.RuntimeService.CreateMessageCorrelation("message").CorrelateAll();

		IQueryable<ITask> query = engineRule.TaskService.CreateTaskQuery();
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_TWO).Count(), Is.EqualTo(1L));
	  }


        [Test]
        public virtual void correlateMessagesToStartAndIntermediateCatchEventForMultipleTenants()
	  {
		testRule.DeployForTenant(TENANT_ONE, MESSAGE_START_PROCESS);
		testRule.DeployForTenant(TENANT_TWO, MESSAGE_CATCH_PROCESS);

		engineRule.RuntimeService.CreateProcessInstanceByKey("messageCatch").SetProcessDefinitionTenantId(TENANT_TWO).Execute();

		engineRule.RuntimeService.CreateMessageCorrelation("message").CorrelateAll();

		Assert.That(engineRule.RuntimeService.CreateProcessInstanceQuery(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
		Assert.That(engineRule.TaskService.CreateTaskQuery(c=>c.TenantId == TENANT_TWO).Count(), Is.EqualTo(1L));
	  }

        [Test]
        public virtual void failToCorrelateMessageToIntermediateCatchEventsForMultipleTenants()
	  {
		testRule.DeployForTenant(TENANT_ONE, MESSAGE_CATCH_PROCESS);
		testRule.DeployForTenant(TENANT_TWO, MESSAGE_CATCH_PROCESS);

		engineRule.RuntimeService.CreateProcessInstanceByKey("messageCatch").SetProcessDefinitionTenantId(TENANT_ONE).Execute();
		engineRule.RuntimeService.CreateProcessInstanceByKey("messageCatch").SetProcessDefinitionTenantId(TENANT_TWO).Execute();

		// declare expected exception
		//thrown.Expect(typeof(MismatchingMessageCorrelationException));
		//thrown.ExpectMessage("Cannot correlate a message with name 'message' to a single execution");

		engineRule.RuntimeService.CreateMessageCorrelation("message").Correlate();
	  }


        [Test]
        public virtual void failToCorrelateMessageToStartEventsForMultipleTenants()
	  {
		testRule.DeployForTenant(TENANT_ONE, MESSAGE_START_PROCESS);
		testRule.DeployForTenant(TENANT_TWO, MESSAGE_START_PROCESS);

		// declare expected exception
		//thrown.Expect(typeof(MismatchingMessageCorrelationException));
		//thrown.ExpectMessage("Cannot correlate a message with name 'message' to a single process definition");

		engineRule.RuntimeService.CreateMessageCorrelation("message").Correlate();
	  }


        [Test]
        public virtual void failToCorrelateStartMessageForMultipleTenants()
	  {
		testRule.DeployForTenant(TENANT_ONE, MESSAGE_START_PROCESS);
		testRule.DeployForTenant(TENANT_TWO, MESSAGE_START_PROCESS);

		// declare expected exception
		//thrown.Expect(typeof(MismatchingMessageCorrelationException));
		//thrown.ExpectMessage("Cannot correlate a message with name 'message' to a single process definition");

		engineRule.RuntimeService.CreateMessageCorrelation("message").CorrelateStartMessage();
	  }


        [Test]
        public virtual void failToCorrelateMessageByProcessInstanceIdWithoutTenantId()
	  {
		// declare expected exception
		//thrown.Expect(typeof(BadUserRequestException));
		//thrown.ExpectMessage("Cannot specify a tenant-id");

		engineRule.RuntimeService.CreateMessageCorrelation("message")
               // .Where(c=>c.ProcessInstanceId=="id")
               .WithoutTenantId()
                .Correlate();
	  }


        [Test]
        public virtual void failToCorrelateMessageByProcessInstanceIdAndTenantId()
	  {
		// declare expected exception
		//thrown.Expect(typeof(BadUserRequestException));
		//thrown.ExpectMessage("Cannot specify a tenant-id");

		engineRule.RuntimeService.CreateMessageCorrelation("message")
                //.Where(c=>c.ProcessInstanceId=="id")
                .SetTenantId(TENANT_ONE).Correlate();
	  }


        [Test]
        public virtual void failToCorrelateMessageByProcessDefinitionIdWithoutTenantId()
	  {
		// declare expected exception
		//thrown.Expect(typeof(BadUserRequestException));
		//thrown.ExpectMessage("Cannot specify a tenant-id");

		engineRule.RuntimeService.CreateMessageCorrelation("message").ProcessDefinitionId("id" ).SetTenantId( null).CorrelateStartMessage();
	  }


        [Test]
        public virtual void failToCorrelateMessageByProcessDefinitionIdAndTenantId()
	  {
		// declare expected exception
		//thrown.Expect(typeof(BadUserRequestException));
		//thrown.ExpectMessage("Cannot specify a tenant-id");

		engineRule.RuntimeService.CreateMessageCorrelation("message").ProcessDefinitionId("id").SetTenantId(TENANT_ONE).CorrelateStartMessage();
	  }

	}

}