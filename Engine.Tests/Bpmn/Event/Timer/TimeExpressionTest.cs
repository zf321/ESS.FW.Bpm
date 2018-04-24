using System;
using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.Event.Timer
{
    [TestFixture]
    public class TimeExpressionTest : PluggableProcessEngineTestCase
    {

        private DateTime testExpression(string timeExpression)
        {
            // Set the clock fixed
            Dictionary<string, object> variables1 = new Dictionary<string, object>();
            variables1["dueDate"] = timeExpression;

            // After process start, there should be timer created    
            IProcessInstance pi1 = runtimeService.StartProcessInstanceByKey("intermediateTimerEventExample", variables1);
            Assert.AreEqual(1, managementService.CreateJobQuery(c=>c.ProcessInstanceId == pi1.Id).Count());


            IList<IJob> jobs = managementService.CreateJobQuery()/*.Executable()*/.ToList();
            Assert.AreEqual(1, jobs.Count);
            return (DateTime)jobs[0].Duedate;
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/timer/IntermediateTimerEventTest.testExpression.bpmn20.xml" })]
        public virtual void testTimeExpressionComplete()
        {
            DateTime dt = DateTime.Now;

            DateTime dueDate = testExpression(dt.ToString("yyyy-MM-dd HH:mm:ss"));
            Assert.AreEqual(dt.ToString("yyyy-MM-dd HH:mm:ss"), dueDate.ToString("yyyy-MM-dd HH:mm:ss"));
            
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/timer/IntermediateTimerEventTest.testExpression.bpmn20.xml" })]
        public virtual void testTimeExpressionWithoutSeconds()
        {
            DateTime dt = DateTime.Now;

            DateTime dueDate = testExpression(dt.ToString("yyyy-MM-dd'T'HH:mm"));
            Assert.AreEqual(dt.ToString("yyyy-MM-dd'T'HH:mm"), dueDate.ToString("yyyy-MM-dd'T'HH:mm"));
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/timer/IntermediateTimerEventTest.testExpression.bpmn20.xml" })]
        public virtual void testTimeExpressionWithoutMinutes()
        {
            DateTime dt = DateTime.Now;
            //.net 不支持这样反向解析 DateTImeUtil 24
            DateTime dueDate = testExpression(DateTime.Now.ToString("yyyy-MM-dd'T'HH"));
            Assert.AreEqual(dt.ToString("yyyy-MM-dd'T'HH"), dueDate.ToString("yyyy-MM-dd'T'HH"));
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/timer/IntermediateTimerEventTest.testExpression.bpmn20.xml" })]
        public virtual void testTimeExpressionWithoutTime()
        {
            DateTime dt = DateTime.Now;

            DateTime dueDate = testExpression(DateTime.Now.ToString("yyyy-MM-dd"));
            Assert.AreEqual(dt.ToString("yyyy-MM-dd"), dueDate.ToString("yyyy-MM-dd"));
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/timer/IntermediateTimerEventTest.testExpression.bpmn20.xml" })]
        public virtual void testTimeExpressionWithoutDay()
        {
            DateTime dt = DateTime.Now;

            DateTime dueDate = testExpression(DateTime.Now.ToString("yyyy-MM"));
            Assert.AreEqual(dt.ToString("yyyy-MM"), dueDate.ToString("yyyy-MM"));
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/timer/IntermediateTimerEventTest.testExpression.bpmn20.xml" })]
        public virtual void testTimeExpressionWithoutMonth()
        {
            DateTime dt = DateTime.Now;
            //.net 不支持这样反向解析 DateTImeUtil 24
            DateTime dueDate = testExpression(DateTime.Now.ToString("yyyy"));
            Assert.AreEqual(dt.ToString("yyyy"), dueDate.ToString("yyyy"));
        }
    }

}