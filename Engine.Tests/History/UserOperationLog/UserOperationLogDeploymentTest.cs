using System;
using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Application.Impl;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;

namespace Engine.Tests.History.UserOperationLog
{


    /// <summary>
    /// 
    /// 
    /// </summary>
    [TestFixture]
    public class UserOperationLogDeploymentTest : AbstractUserOperationLogTest
	{

	  protected internal const string DEPLOYMENT_NAME = "my-deployment";
	  protected internal const string RESOURCE_NAME = "path/to/my/process.bpmn";
	  protected internal const string PROCESS_KEY = "process";


        [TearDown]
        protected internal void tearDown()
	  {
		base.TearDown();

	      IList<IDeployment> deployments = repositoryService.CreateDeploymentQuery()
	          .ToList();
		foreach (IDeployment deployment in deployments)
		{
		  repositoryService.DeleteDeployment(deployment.Id, true, true);
		}
	  }

        [Test]
        public virtual void testCreateDeployment()
	  {
		// when
		IDeployment deployment = repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).AddModelInstance(RESOURCE_NAME, createProcessWithServiceTask(PROCESS_KEY)).Deploy();

		// then
		IUserOperationLogEntry userOperationLogEntry = historyService.CreateUserOperationLogQuery().First();
		Assert.NotNull(userOperationLogEntry);

		Assert.AreEqual(EntityTypes.Deployment, userOperationLogEntry.EntityType);
		Assert.AreEqual(deployment.Id, userOperationLogEntry.DeploymentId);

		Assert.AreEqual(UserOperationLogEntryFields.OperationTypeCreate, userOperationLogEntry.OperationType);

		Assert.AreEqual("duplicateFilterEnabled", userOperationLogEntry.Property);
		Assert.IsNull(userOperationLogEntry.OrgValue);
		Assert.IsFalse(Convert.ToBoolean(userOperationLogEntry.NewValue));

		Assert.AreEqual(USER_ID, userOperationLogEntry.UserId);

		Assert.IsNull(userOperationLogEntry.JobDefinitionId);
		Assert.IsNull(userOperationLogEntry.ProcessInstanceId);
		Assert.IsNull(userOperationLogEntry.ProcessDefinitionId);
		Assert.IsNull(userOperationLogEntry.ProcessDefinitionKey);
		Assert.IsNull(userOperationLogEntry.CaseInstanceId);
		Assert.IsNull(userOperationLogEntry.CaseDefinitionId);
	  }

        [Test]
        public virtual void testCreateDeploymentPa()
	  {
		// given
		EmbeddedProcessApplication application = new EmbeddedProcessApplication();

		// when
		IDeployment deployment = repositoryService.CreateDeployment(application.Reference).Name(DEPLOYMENT_NAME).AddModelInstance(RESOURCE_NAME, createProcessWithServiceTask(PROCESS_KEY)).Deploy();

		// then
		IUserOperationLogEntry userOperationLogEntry = historyService.CreateUserOperationLogQuery().First();
		Assert.NotNull(userOperationLogEntry);

		Assert.AreEqual(EntityTypes.Deployment, userOperationLogEntry.EntityType);
		Assert.AreEqual(deployment.Id, userOperationLogEntry.DeploymentId);

		Assert.AreEqual(UserOperationLogEntryFields.OperationTypeCreate, userOperationLogEntry.OperationType);

		Assert.AreEqual("duplicateFilterEnabled", userOperationLogEntry.Property);
		Assert.IsNull(userOperationLogEntry.OrgValue);
		Assert.IsFalse(Convert.ToBoolean(userOperationLogEntry.NewValue));

		Assert.AreEqual(USER_ID, userOperationLogEntry.UserId);

		Assert.IsNull(userOperationLogEntry.JobDefinitionId);
		Assert.IsNull(userOperationLogEntry.ProcessInstanceId);
		Assert.IsNull(userOperationLogEntry.ProcessDefinitionId);
		Assert.IsNull(userOperationLogEntry.ProcessDefinitionKey);
		Assert.IsNull(userOperationLogEntry.CaseInstanceId);
		Assert.IsNull(userOperationLogEntry.CaseDefinitionId);
	  }

        [Test]
        public virtual void testPropertyDuplicateFiltering()
	  {
		// given
		IBpmnModelInstance model = createProcessWithServiceTask(PROCESS_KEY);

		// when
		IDeployment deployment = repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).AddModelInstance(RESOURCE_NAME, model).EnableDuplicateFiltering(false).Deploy();

		// then
		IQueryable<IUserOperationLogEntry> query = historyService.CreateUserOperationLogQuery();
		Assert.AreEqual(2, query.Count());

		// (1): duplicate filter enabled property
		//IUserOperationLogEntry logDuplicateFilterEnabledProperty = query.Property("duplicateFilterEnabled").First();
		//Assert.NotNull(logDuplicateFilterEnabledProperty);

		//Assert.AreEqual(EntityTypes.Deployment, logDuplicateFilterEnabledProperty.EntityType);
		//Assert.AreEqual(deployment.Id, logDuplicateFilterEnabledProperty.DeploymentId);
		//Assert.AreEqual(UserOperationLogEntryFields.OperationTypeCreate, logDuplicateFilterEnabledProperty.OperationType);

		//Assert.AreEqual(USER_ID, logDuplicateFilterEnabledProperty.UserId);

		//Assert.AreEqual("duplicateFilterEnabled", logDuplicateFilterEnabledProperty.Property);
		//Assert.IsNull(logDuplicateFilterEnabledProperty.OrgValue);
		//Assert.True(Convert.ToBoolean(logDuplicateFilterEnabledProperty.NewValue));

		//// (2): deploy changed only
		//IUserOperationLogEntry logDeployChangedOnlyProperty = query.Property("deployChangedOnly").First();
		//Assert.NotNull(logDeployChangedOnlyProperty);

		//Assert.AreEqual(EntityTypes.Deployment, logDeployChangedOnlyProperty.EntityType);
		//Assert.AreEqual(deployment.Id, logDeployChangedOnlyProperty.DeploymentId);
		//Assert.AreEqual(UserOperationLogEntryFields.OperationTypeCreate, logDeployChangedOnlyProperty.OperationType);
		//Assert.AreEqual(USER_ID, logDeployChangedOnlyProperty.UserId);

		//Assert.AreEqual("deployChangedOnly", logDeployChangedOnlyProperty.Property);
		//Assert.IsNull(logDeployChangedOnlyProperty.OrgValue);
		//Assert.IsFalse(Convert.ToBoolean(logDeployChangedOnlyProperty.NewValue));

		//// (3): operation id
		//Assert.AreEqual(logDuplicateFilterEnabledProperty.OperationId, logDeployChangedOnlyProperty.OperationId);
	  }

        [Test]
        public virtual void testPropertiesDuplicateFilteringAndDeployChangedOnly()
	  {
		// given
		IBpmnModelInstance model = createProcessWithServiceTask(PROCESS_KEY);

		// when
		IDeployment deployment = repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).AddModelInstance(RESOURCE_NAME, model).EnableDuplicateFiltering(true).Deploy();

		// then
		IQueryable<IUserOperationLogEntry> query = historyService.CreateUserOperationLogQuery();
		Assert.AreEqual(2, query.Count());

		// (1): duplicate filter enabled property
		//IUserOperationLogEntry logDuplicateFilterEnabledProperty = query.Property("duplicateFilterEnabled").First();
		//Assert.NotNull(logDuplicateFilterEnabledProperty);
		//Assert.AreEqual(EntityTypes.Deployment, logDuplicateFilterEnabledProperty.EntityType);
		//Assert.AreEqual(deployment.Id, logDuplicateFilterEnabledProperty.DeploymentId);
		//Assert.AreEqual(UserOperationLogEntryFields.OperationTypeCreate, logDuplicateFilterEnabledProperty.OperationType);
		//Assert.AreEqual(USER_ID, logDuplicateFilterEnabledProperty.UserId);

		//Assert.AreEqual("duplicateFilterEnabled", logDuplicateFilterEnabledProperty.Property);
		//Assert.IsNull(logDuplicateFilterEnabledProperty.OrgValue);
		//Assert.True(Convert.ToBoolean(logDuplicateFilterEnabledProperty.NewValue));

		//// (2): deploy changed only
		//IUserOperationLogEntry logDeployChangedOnlyProperty = query.Property("deployChangedOnly").First();
		//Assert.NotNull(logDeployChangedOnlyProperty);

		//Assert.AreEqual(EntityTypes.Deployment, logDeployChangedOnlyProperty.EntityType);
		//Assert.AreEqual(deployment.Id, logDeployChangedOnlyProperty.DeploymentId);
		//Assert.AreEqual(UserOperationLogEntryFields.OperationTypeCreate, logDeployChangedOnlyProperty.OperationType);
		//Assert.AreEqual(USER_ID, logDeployChangedOnlyProperty.UserId);

		//Assert.AreEqual("deployChangedOnly", logDeployChangedOnlyProperty.Property);
		//Assert.IsNull(logDeployChangedOnlyProperty.OrgValue);
		//Assert.True(Convert.ToBoolean(logDeployChangedOnlyProperty.NewValue));

		//// (3): operation id
		//Assert.AreEqual(logDuplicateFilterEnabledProperty.OperationId, logDeployChangedOnlyProperty.OperationId);
	  }

        [Test]
        public virtual void testDeleteDeploymentCascadingShouldKeepCreateUserOperationLog()
	  {
		// given
		IDeployment deployment = repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).AddModelInstance(RESOURCE_NAME, createProcessWithServiceTask(PROCESS_KEY)).Deploy();

		IQueryable<IUserOperationLogEntry> query = historyService.CreateUserOperationLogQuery(c=>c.OperationType ==UserOperationLogEntryFields.OperationTypeCreate);

		Assert.AreEqual(1, query.Count());

		// when
		repositoryService.DeleteDeployment(deployment.Id, true);

		// then
		Assert.AreEqual(1, query.Count());
	  }

	  public virtual void testDeleteDeploymentWithoutCascadingShouldKeepCreateUserOperationLog()
	  {
		// given
		IDeployment deployment = repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).AddModelInstance(RESOURCE_NAME, createProcessWithServiceTask(PROCESS_KEY)).Deploy();

		IQueryable<IUserOperationLogEntry> query = historyService.CreateUserOperationLogQuery(c=>c.OperationType ==UserOperationLogEntryFields.OperationTypeCreate);

		Assert.AreEqual(1, query.Count());

		// when
		repositoryService.DeleteDeployment(deployment.Id, false);

		// then
		Assert.AreEqual(1, query.Count());
	  }

        [Test]
        public virtual void testDeleteDeployment()
	  {
		// given
		IDeployment deployment = repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).AddModelInstance(RESOURCE_NAME, createProcessWithServiceTask(PROCESS_KEY)).Deploy();

		IQueryable<IUserOperationLogEntry> query = historyService.CreateUserOperationLogQuery(c=>c.OperationType ==UserOperationLogEntryFields.OperationTypeDelete);

		// when
		repositoryService.DeleteDeployment(deployment.Id, false);

		// then
		Assert.AreEqual(1, query.Count());

		IUserOperationLogEntry log = query.First();
		Assert.NotNull(log);

		Assert.AreEqual(EntityTypes.Deployment, log.EntityType);
		Assert.AreEqual(deployment.Id, log.DeploymentId);

		Assert.AreEqual(UserOperationLogEntryFields.OperationTypeDelete, log.OperationType);

		Assert.AreEqual("cascade", log.Property);
		Assert.IsNull(log.OrgValue);
		Assert.IsFalse(Convert.ToBoolean(log.NewValue));

		Assert.AreEqual(USER_ID, log.UserId);

		Assert.IsNull(log.JobDefinitionId);
		Assert.IsNull(log.ProcessInstanceId);
		Assert.IsNull(log.ProcessDefinitionId);
		Assert.IsNull(log.ProcessDefinitionKey);
		Assert.IsNull(log.CaseInstanceId);
		Assert.IsNull(log.CaseDefinitionId);
	  }

        [Test]
        public virtual void testDeleteDeploymentCascading()
	  {
		// given
		IDeployment deployment = repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).AddModelInstance(RESOURCE_NAME, createProcessWithServiceTask(PROCESS_KEY)).Deploy();

		IQueryable<IUserOperationLogEntry> query = historyService.CreateUserOperationLogQuery(c=>c.OperationType ==UserOperationLogEntryFields.OperationTypeDelete);

		// when
		repositoryService.DeleteDeployment(deployment.Id, true);

		// then
		Assert.AreEqual(1, query.Count());

		IUserOperationLogEntry log = query.First();
		Assert.NotNull(log);

		Assert.AreEqual(EntityTypes.Deployment, log.EntityType);
		Assert.AreEqual(deployment.Id, log.DeploymentId);

		Assert.AreEqual(UserOperationLogEntryFields.OperationTypeDelete, log.OperationType);

		Assert.AreEqual("cascade", log.Property);
		Assert.IsNull(log.OrgValue);
		Assert.True(Convert.ToBoolean(log.NewValue));

		Assert.AreEqual(USER_ID, log.UserId);

		Assert.IsNull(log.JobDefinitionId);
		Assert.IsNull(log.ProcessInstanceId);
		Assert.IsNull(log.ProcessDefinitionId);
		Assert.IsNull(log.ProcessDefinitionKey);
		Assert.IsNull(log.CaseInstanceId);
		Assert.IsNull(log.CaseDefinitionId);
	  }


        [Test]
        public virtual void testDeleteProcessDefinitionCascadingShouldKeepCreateUserOperationLog()
	  {
		// given
		IDeployment deployment = repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).AddModelInstance(RESOURCE_NAME, createProcessWithServiceTask(PROCESS_KEY)).Deploy();

		IProcessDefinition procDef = repositoryService.CreateProcessDefinitionQuery(c=> c.DeploymentId == deployment.Id).First();

		IQueryable<IUserOperationLogEntry> query = historyService.CreateUserOperationLogQuery(c=>c.OperationType ==UserOperationLogEntryFields.OperationTypeCreate);

		Assert.AreEqual(1, query.Count());

		// when
		repositoryService.DeleteProcessDefinition(procDef.Id, true);

		// then
		Assert.AreEqual(1, query.Count());
	  }

        [Test]
        public virtual void testDeleteProcessDefinitiontWithoutCascadingShouldKeepCreateUserOperationLog()
	  {
		// given
		IDeployment deployment = repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).AddModelInstance(RESOURCE_NAME, createProcessWithServiceTask(PROCESS_KEY)).Deploy();

		IProcessDefinition procDef = repositoryService.CreateProcessDefinitionQuery(c=> c.DeploymentId == deployment.Id).First();

		IQueryable<IUserOperationLogEntry> query = historyService.CreateUserOperationLogQuery(c=>c.OperationType ==UserOperationLogEntryFields.OperationTypeCreate);

		Assert.AreEqual(1, query.Count());

		// when
		repositoryService.DeleteProcessDefinition(procDef.Id, true);

		// then
		Assert.AreEqual(1, query.Count());
	  }

        [Test]
        public virtual void testDeleteProcessDefinition()
	  {
		// given
		IDeployment deployment = repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).AddModelInstance(RESOURCE_NAME, createProcessWithServiceTask(PROCESS_KEY)).Deploy();

		IProcessDefinition procDef = repositoryService.CreateProcessDefinitionQuery(c=> c.DeploymentId == deployment.Id).First();

		IQueryable<IUserOperationLogEntry> query = historyService.CreateUserOperationLogQuery(c=>c.OperationType ==UserOperationLogEntryFields.OperationTypeDelete);

		// when
		repositoryService.DeleteProcessDefinition(procDef.Id, false);

		// then
		Assert.AreEqual(1, query.Count());

		IUserOperationLogEntry log = query.First();
		Assert.NotNull(log);

		Assert.AreEqual(EntityTypes.ProcessDefinition, log.EntityType);
		Assert.AreEqual(procDef.Id, log.ProcessDefinitionId);
		Assert.AreEqual(procDef.Key, log.ProcessDefinitionKey);
		Assert.AreEqual(deployment.Id, log.DeploymentId);

		Assert.AreEqual(UserOperationLogEntryFields.OperationTypeDelete, log.OperationType);

		Assert.AreEqual("cascade", log.Property);
		Assert.IsNull(log.OrgValue);
		Assert.IsFalse(Convert.ToBoolean(log.NewValue));

		Assert.AreEqual(USER_ID, log.UserId);

		Assert.IsNull(log.JobDefinitionId);
		Assert.IsNull(log.ProcessInstanceId);
		Assert.IsNull(log.CaseInstanceId);
		Assert.IsNull(log.CaseDefinitionId);
	  }

        [Test]
        public virtual void testDeleteProcessDefinitionCascading()
	  {
		// given
		IDeployment deployment = repositoryService.CreateDeployment().Name(DEPLOYMENT_NAME).AddModelInstance(RESOURCE_NAME, createProcessWithServiceTask(PROCESS_KEY)).Deploy();

		IProcessDefinition procDef = repositoryService.CreateProcessDefinitionQuery(c=> c.DeploymentId == deployment.Id).First();

		IQueryable<IUserOperationLogEntry> query = historyService.CreateUserOperationLogQuery(c=>c.OperationType ==UserOperationLogEntryFields.OperationTypeDelete);

		// when
		repositoryService.DeleteProcessDefinition(procDef.Id, true);

		// then
		Assert.AreEqual(1, query.Count());

		IUserOperationLogEntry log = query.First();
		Assert.NotNull(log);

		Assert.AreEqual(EntityTypes.ProcessDefinition, log.EntityType);
		Assert.AreEqual(procDef.Id, log.ProcessDefinitionId);
		Assert.AreEqual(procDef.Key, log.ProcessDefinitionKey);
		Assert.AreEqual(deployment.Id, log.DeploymentId);

		Assert.AreEqual(UserOperationLogEntryFields.OperationTypeDelete, log.OperationType);

		Assert.AreEqual("cascade", log.Property);
		Assert.IsNull(log.OrgValue);
		Assert.True(Convert.ToBoolean(log.NewValue));

		Assert.AreEqual(USER_ID, log.UserId);

		Assert.IsNull(log.JobDefinitionId);
		Assert.IsNull(log.ProcessInstanceId);
		Assert.IsNull(log.CaseInstanceId);
		Assert.IsNull(log.CaseDefinitionId);
	  }

	  protected internal virtual IBpmnModelInstance createProcessWithServiceTask(string key)
	  {
		return ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(key).StartEvent().ServiceTask().CamundaExpression("${true}").EndEvent().Done();
	  }

	}

}