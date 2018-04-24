using System;
using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Task;
using NUnit.Framework;

namespace Engine.Tests.History.UserOperationLog
{


    /// <summary>
    /// 
    /// </summary>
    [TestFixture]
    public class UserOperationLogTaskServiceAndBeanTest : AbstractUserOperationLogTest
	{

	  protected internal ITask task;

        [TearDown]
        protected internal void tearDown()
	  {
		base.TearDown();

		if (task != null)
		{
		  taskService.DeleteTask(task.Id, true);
		}
	  }

        [Test]
        public virtual void testBeanPropertyChanges()
	  {
		TaskEntity entity = new TaskEntity();

		// assign and validate changes
		entity.Assignee = "icke";
		IDictionary<string, PropertyChange> changes = entity.PropertyChanges;
		Assert.AreEqual(1, changes.Count);
		Assert.IsNull(changes[TaskEntity.ASSIGNEE].OrgValue);
		Assert.AreEqual("icke", changes[TaskEntity.ASSIGNEE].NewValue);

		// assign it again
		entity.Assignee = "er";
		changes = entity.PropertyChanges;
		Assert.AreEqual(1, changes.Count);

		// original value is still null because the task was not saved
		Assert.IsNull(changes[TaskEntity.ASSIGNEE].OrgValue);
		Assert.AreEqual("er", changes[TaskEntity.ASSIGNEE].NewValue);

		// set a due date
		entity.DueDate = DateTime.Now;
		changes = entity.PropertyChanges;
		Assert.AreEqual(2, changes.Count);
	  }

        [Test]
        public virtual void testNotTrackChangeToTheSameValue()
	  {
		TaskEntity entity = new TaskEntity();

		// get and set a properties
		entity.Priority = entity.Priority;
		entity.Owner = entity.Owner;
		entity.FollowUpDate = entity.FollowUpDate;

		// should not track this change
		Assert.False(entity.PropertyChanges.Any());
	  }

        [Test]
        public virtual void testRemoveChangeWhenSetBackToTheOrgValue()
	  {
		TaskEntity entity = new TaskEntity();

		// set an owner (default is null)
		entity.Owner = "icke";

		// should track this change
		Assert.True(entity.PropertyChanges.Any());

		// reset the owner
		entity.Owner = null;

		// the change is removed
		Assert.False(entity.PropertyChanges.Any());
	  }

        [Test]
        public virtual void testAllTrackedProperties()
	  {
		DateTime yesterday = new DateTime((DateTime.Now).Ticks - 86400000);
		DateTime tomorrow = new DateTime((DateTime.Now).Ticks + 86400000);

		TaskEntity entity = new TaskEntity();

		// call all tracked setter methods
		entity.Assignee = "er";
		entity.DelegationState = DelegationState.Pending;
		entity.Deleted = true;
		entity.Description = "a description";
		entity.DueDate = tomorrow;
		entity.FollowUpDate = yesterday;
		entity.Name = "to do";
		entity.Owner = "icke";
		entity.ParentTaskId = "parent";
		entity.Priority = 73;

		// and validate the change list
		IDictionary<string, PropertyChange> changes = entity.PropertyChanges;
		Assert.AreEqual("er", changes[TaskEntity.ASSIGNEE].NewValue);
		Assert.AreSame(DelegationState.Pending, changes[TaskEntity.Delegation].NewValue);
		Assert.True((bool) changes[TaskEntity.delete].NewValue);
		Assert.AreEqual("a description", changes[TaskEntity.DESCRIPTION].NewValue);
		Assert.AreEqual(tomorrow, changes[TaskEntity.DUE_DATE].NewValue);
		Assert.AreEqual(yesterday, changes[TaskEntity.FOLLOW_UP_DATE].NewValue);
		Assert.AreEqual("to do", changes[TaskEntity.NAME].NewValue);
		Assert.AreEqual("icke", changes[TaskEntity.OWNER].NewValue);
		Assert.AreEqual("parent", changes[TaskEntity.PARENT_TASK].NewValue);
		Assert.AreEqual(73, changes[TaskEntity.PRIORITY].NewValue);
	  }

        [Test]
        public virtual void testDeleteTask()
	  {
		// given: a single task
		task = taskService.NewTask();
		taskService.SaveTask(task);

		// then: Delete the task
		taskService.DeleteTask(task.Id, "duplicated");

		// expect: one entry for the deletion
		IQueryable<IUserOperationLogEntry> query = queryOperationDetails(UserOperationLogEntryFields.OperationTypeDelete);
		Assert.AreEqual(1, query.Count());

		// Assert: details
		IUserOperationLogEntry Delete = query.First();
		Assert.AreEqual(Permissions.Delete, Delete.Property);
		Assert.IsFalse(bool.Parse(Delete.OrgValue));
		Assert.True(bool.Parse(Delete.NewValue));
	  }

        [Test]
        public virtual void testCompositeBeanInteraction()
	  {
		// given: a manually created task
		task = taskService.NewTask();

		// then: save the task without any property change
		taskService.SaveTask(task);

		// expect: no entry
		IQueryable<IUserOperationLogEntry> query = queryOperationDetails(UserOperationLogEntryFields.OperationTypeCreate);
		IUserOperationLogEntry create = query.First();
		Assert.NotNull(create);
		Assert.AreEqual(EntityTypes.Task, create.EntityType);
		Assert.IsNull(create.OrgValue);
		Assert.IsNull(create.NewValue);
		Assert.IsNull(create.Property);

		task.Assignee = "icke";
		task.Name = "to do";

		// then: save the task again
		taskService.SaveTask(task);

		// expect: two update entries with the same operation id
	      IList<IUserOperationLogEntry> entries = queryOperationDetails(UserOperationLogEntryFields.OperationTypeUpdate)
	          .ToList();
		Assert.AreEqual(2, entries.Count);
		Assert.AreEqual(entries[0].OperationId, entries[1].OperationId);
	  }

        [Test]
        public virtual void testMultipleValueChange()
	  {
		// given: a single task
		task = taskService.NewTask();
		taskService.SaveTask(task);

		// then: change a property twice
		task.Name = "a task";
		task.Name = "to do";
		taskService.SaveTask(task);
		IUserOperationLogEntry update = queryOperationDetails(UserOperationLogEntryFields.OperationTypeUpdate).First();
		Assert.IsNull(update.OrgValue);
		Assert.AreEqual("to do", update.NewValue);
	  }

        [Test]
        public virtual void testSetDateProperty()
	  {
		// given: a single task
		task = taskService.NewTask();
		DateTime now = ClockUtil.CurrentTime;
		task.DueDate = now;
		taskService.SaveTask(task);

		IUserOperationLogEntry logEntry = historyService.CreateUserOperationLogQuery().First();
		Assert.AreEqual(now.Ticks.ToString(), logEntry.NewValue);
	  }

        [Test]
        public virtual void testResetChange()
	  {
		// given: a single task
		task = taskService.NewTask();
		taskService.SaveTask(task);

		// then: change the name
		string name = "a task";
		task.Name = name;
		taskService.SaveTask(task);
		IUserOperationLogEntry update = queryOperationDetails(UserOperationLogEntryFields.OperationTypeUpdate).First();
		Assert.IsNull(update.OrgValue);
		Assert.AreEqual(name, update.NewValue);

		// then: change the name some times and set it back to the original value
		task.Name = "to do 1";
		task.Name = "to do 2";
		task.Name = name;
		taskService.SaveTask(task);

		// expect: there is no additional change tracked
		update = queryOperationDetails(UserOperationLogEntryFields.OperationTypeUpdate).First();
		Assert.IsNull(update.OrgValue);
		Assert.AreEqual(name, update.NewValue);
	  }

        [Test]
        public virtual void testConcurrentTaskChange()
	  {
		// create a task
		task = taskService.NewTask();
		taskService.SaveTask(task);

		// change the bean property
		task.Assignee = "icke";

		// use the service method to do an other assignment
		taskService.SetAssignee(task.Id, "er");

		try
		{ // now try to save the task and overwrite the change
		  taskService.SaveTask(task);
		}
		catch (System.Exception e)
		{
		  Assert.NotNull(e); // concurrent modification
		}
	  }

        [Test]
        public virtual void testCaseInstanceId()
	  {
		// create new task
		task = taskService.NewTask();
		taskService.SaveTask(task);

		IQueryable<IUserOperationLogEntry> query = queryOperationDetails(UserOperationLogEntryFields.OperationTypeUpdate);
		Assert.AreEqual(0, query.Count());

		// set case instance id and save task
		task.CaseInstanceId = "aCaseInstanceId";
		taskService.SaveTask(task);

		Assert.AreEqual(1, query.Count());

		IUserOperationLogEntry entry = query.First();
		Assert.NotNull(entry);

		Assert.IsNull(entry.OrgValue);
		Assert.AreEqual("aCaseInstanceId", entry.NewValue);
		Assert.AreEqual(TaskEntity.CASE_INSTANCE_ID, entry.Property);

		// change case instance id and save task
		task.CaseInstanceId = "anotherCaseInstanceId";
		taskService.SaveTask(task);

		Assert.AreEqual(2, query.Count());

	      IList<IUserOperationLogEntry> entries = query.ToList();
		Assert.AreEqual(2, entries.Count);

		foreach (IUserOperationLogEntry currentEntry in entries)
		{
		  if (!currentEntry.Id.Equals(entry.Id))
		  {
			Assert.AreEqual("aCaseInstanceId", currentEntry.OrgValue);
			Assert.AreEqual("anotherCaseInstanceId", currentEntry.NewValue);
			Assert.AreEqual(TaskEntity.CASE_INSTANCE_ID, currentEntry.Property);
		  }
		}
	  }

	  private IQueryable<IUserOperationLogEntry> queryOperationDetails(string type)
	  {
		return historyService.CreateUserOperationLogQuery(c=>c.OperationType==type);
	  }

	}

}