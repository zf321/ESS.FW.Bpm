using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.UserTask
{
    /// <summary>
    /// </summary>
    public class TaskDueDateExtensionsTest : PluggableProcessEngineTestCase
    {
        [Test]
        [Deployment]
        public virtual void testDueDateExtension()
        {
            var date = DateTime.Parse("1986-07-06 12:10:00");
            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["dateVariable"] = date;

            // Start process-instance, passing date that should be used as dueDate
            var processInstance = runtimeService.StartProcessInstanceByKey("dueDateExtension", variables);

            var task = taskService.CreateTaskQuery(c => c.ProcessInstanceId == processInstance.Id)
                .First();

            Assert.NotNull(task.DueDate);
            Assert.AreEqual(date, task.DueDate);
        }

        [Test]
        [Deployment]
        public virtual void testDueDateStringExtension()
        {
            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["dateVariable"] = "1986-07-06T12:10:00";

            // Start process-instance, passing date that should be used as dueDate
            var processInstance = runtimeService.StartProcessInstanceByKey("dueDateExtension", variables);

            var task = taskService.CreateTaskQuery(c => c.ProcessInstanceId == processInstance.Id)
                .First();

            Assert.NotNull(task.DueDate);
            var date = DateTime.Parse("1986-07-06 12:10:00");
            Assert.AreEqual(date, task.DueDate);
        }


        [Test]
        [Deployment]
        public virtual void testRelativeDueDate()
        {
            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["dateVariable"] = "P2DT2H30M";

            var processInstance = runtimeService.StartProcessInstanceByKey("dueDateExtension", variables);

            var task = taskService.CreateTaskQuery(c => c.ProcessInstanceId == processInstance.Id)
                .First();


            var dueDate = task.DueDate;
            Assert.NotNull(dueDate);

            var period = dueDate- task.CreateTime  ;
            Assert.AreEqual(period.Value.Days, 2);
            Assert.AreEqual(period.Value.Hours, 2);
            Assert.AreEqual(period.Value.Minutes, 30);
        }
    }
}