using System;
using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Task;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.UserTask
{
    

	/// <summary>
	/// 
	/// 
	/// </summary>
	public class TaskFollowUpDateExtensionsTest : PluggableProcessEngineTestCase
	{
        
        [Test]
        [Deployment("resources/bpmn/usertask/TaskFollowUpDateExtensionsTest.TestUserTaskFollowUpDate.bpmn20.xml")]
        public virtual void testUserTaskFollowUpDateExtension()
	  {
            DateTime date = DateTime.Parse("2015-01-01 12:10:00");
            IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["dateVariable"] = date;

		// Start process-instance, passing date that should be used as followUpDate
		IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("process", variables);

		ITask task = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==processInstance.Id).First();

		Assert.NotNull(task.FollowUpDate);
		Assert.AreEqual(date, task.FollowUpDate);
	  }


        [Test]
        [Deployment("resources/bpmn/usertask/TaskFollowUpDateExtensionsTest.TestUserTaskFollowUpDate.bpmn20.xml")]
        public virtual void testUserTaskFollowUpDateStringExtension()
	  {

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["dateVariable"] = "2015-01-01T12:10:00";

		// Start process-instance, passing date that should be used as followUpDate
		IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("process", variables);

		ITask task = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==processInstance.Id).First();

		Assert.NotNull(task.FollowUpDate);
            DateTime date = DateTime.Parse("2015-01-01 12:10:00");
		Assert.AreEqual(date, task.FollowUpDate);
	  }

        [Test]
        [Deployment("resources/bpmn/usertask/TaskFollowUpDateExtensionsTest.TestUserTaskFollowUpDate.bpmn20.xml")]
        public virtual void testUserTaskRelativeFollowUpDate()
	    {
	        IDictionary<string, object> variables = new Dictionary<string, object>();
	        variables["dateVariable"] = "P2DT2H30M";

	        IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("process", variables);

	        ITask task = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==processInstance.Id).First();

	        DateTime? followUpDate = task.FollowUpDate;
	        Assert.NotNull(followUpDate);

	        var period = (followUpDate.Value -task.CreateTime  );
	        Assert.AreEqual(period.Days, 2);
	        Assert.AreEqual(period.Hours, 2);
	        Assert.AreEqual(period.Minutes, 30);
	    }

	}

}