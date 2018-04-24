using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Task;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.Event.Message
{
    [TestFixture]
    public class MessageStartEventTest : PluggableProcessEngineTestCase
    {
        public MessageStartEventTest()
        {
            ClearDeploymentAll = true;
        }
        [Test]
        public virtual void testDeploymentCreatesSubscriptions()
        {
            string DeploymentId = repositoryService.CreateDeployment()
                .AddClasspathResource("resources/bpmn/event/message/MessageStartEventTest.testSingleMessageStartEvent.bpmn20.xml").Deploy().Id;

            IList<IEventSubscription> eventSubscriptions = runtimeService.CreateEventSubscriptionQuery().ToList();

            Assert.AreEqual(1, eventSubscriptions.Count);
            repositoryService.DeleteDeployment(DeploymentId);
        }
        [Test]
        public virtual void testSameMessageNameFails()
        {
            var id = repositoryService.CreateDeployment()
                .AddClasspathResource("resources/bpmn/event/message/MessageStartEventTest.testSingleMessageStartEvent.bpmn20.xml").Deploy().Id;
            try
            {
                repositoryService.CreateDeployment().AddClasspathResource("resources/bpmn/event/message/otherProcessWithNewInvoiceMessage.bpmn20.xml").Deploy();
                Assert.Fail("exception expected");
            }
            catch (System.Exception e)
            {
                Assert.AreEqual("ENGINE-01011 Cannot deploy process definition 'resources/bpmn/event/message/otherProcessWithNewInvoiceMessage.bpmn20.xml': there already is a message event subscription for the message with name 'newInvoiceMessage'.", e.Message);
                Assert.True(e.Message.IndexOf("there already is a message event subscription for the message with name")>-1);
            }
            finally
            {
                // clean db:
                IList<IDeployment> deployments = repositoryService.CreateDeploymentQuery().ToList();
                foreach (IDeployment deployment in deployments)
                {
                    repositoryService.DeleteDeployment(deployment.Id, true);
                }
                // Workaround for #CAM-4250: remove process definition of failed
                // deployment from deployment cache
                processEngineConfiguration.DeploymentCache.ProcessDefinitionCache.Clear();
            }
        }

        // SEE: https://app.Camunda.com/jira/browse/CAM-1448
        [Test]
        public virtual void testEmptyMessageNameFails()
        {
            try
            {
                repositoryService.CreateDeployment().AddClasspathResource("resources/bpmn/event/message/MessageStartEventTest.testEmptyMessageNameFails.bpmn20.xml").Deploy();
                Assert.Fail("exception expected");
            }
            catch (ProcessEngineException e)
            {
                Assert.True(e.Message.IndexOf("Cannot have a message event subscription with an empty or missing name") > -1);
            }
        }
        [Test]
        public virtual void testSameMessageNameInSameProcessFails()
        {
            try
            {
                repositoryService.CreateDeployment().AddClasspathResource("resources/bpmn/event/message/testSameMessageNameInSameProcessFails.bpmn20.xml").Deploy();
                Assert.Fail("exception expected");
            }
            catch (ProcessEngineException e)
            {
                Assert.True(e.Message.Contains("Cannot have more than one message event subscription with name 'newInvoiceMessage' for scope"));
            }
        }
        [Test]
        public virtual void testUpdateProcessVersionCancelsSubscriptions()
        {
            string DeploymentId = repositoryService.CreateDeployment().AddClasspathResource("resources/bpmn/event/message/MessageStartEventTest.testSingleMessageStartEvent.bpmn20.xml").Deploy().Id;

            IList<IEventSubscription> eventSubscriptions = runtimeService.CreateEventSubscriptionQuery().ToList();
            IList<IProcessDefinition> processDefinitions = repositoryService.CreateProcessDefinitionQuery().ToList();

            Assert.AreEqual(1, eventSubscriptions.Count);
            Assert.AreEqual(1, processDefinitions.Count);
            //无法多次插入 缺少互斥锁？ ENGINE-13022 No exclusive lock is aquired while deploying because it is disabled. This can lead to problems when multiple process engines use the same data source (i.e. in cluster mode).
            string newDeploymentId = repositoryService.CreateDeployment().AddClasspathResource("resources/bpmn/event/message/MessageStartEventTest.testSingleMessageStartEvent.bpmn20.xml").Deploy().Id;

            IList<IEventSubscription> newEventSubscriptions = runtimeService.CreateEventSubscriptionQuery().ToList();
            IList<IProcessDefinition> newProcessDefinitions = repositoryService.CreateProcessDefinitionQuery().ToList();

            Assert.AreEqual(1, newEventSubscriptions.Count);
            Assert.AreEqual(2, newProcessDefinitions.Count);
            foreach (IProcessDefinition processDefinition in newProcessDefinitions)
            {
                if (processDefinition.Version == 1)
                {
                    foreach (IEventSubscription subscription in newEventSubscriptions)
                    {
                        EventSubscriptionEntity subscriptionEntity = (EventSubscriptionEntity)subscription;
                        Assert.IsFalse(subscriptionEntity.Configuration.Equals(processDefinition.Id));
                    }
                }
                else
                {
                    foreach (IEventSubscription subscription in newEventSubscriptions)
                    {
                        EventSubscriptionEntity subscriptionEntity = (EventSubscriptionEntity)subscription;
                        Assert.True(subscriptionEntity.Configuration.Equals(processDefinition.Id));
                    }
                }
            }
            //JAVA TO C# CONVERTER WARNING: LINQ 'SequenceEqual' is not always identical to Java AbstractList 'equals':
            //ORIGINAL LINE: Assert.IsFalse(eventSubscriptions.equals(newEventSubscriptions));
            Assert.IsFalse(eventSubscriptions.SequenceEqual(newEventSubscriptions));

            repositoryService.DeleteDeployment(DeploymentId);
            repositoryService.DeleteDeployment(newDeploymentId);
        }

        [Test]
        [Deployment]
        public virtual void testSingleMessageStartEvent()
        {

            // using StartProcessInstanceByMessage triggers the message start event

            IProcessInstance processInstance = runtimeService.StartProcessInstanceByMessage("newInvoiceMessage");

            Assert.IsFalse(processInstance.IsEnded);

            ITask task = taskService.CreateTaskQuery().First();
            Assert.NotNull(task);

            taskService.Complete(task.Id);

            AssertProcessEnded(processInstance.Id);

            // using StartProcessInstanceByKey also triggers the message event, if there is a single start event

            processInstance = runtimeService.StartProcessInstanceByKey("singleMessageStartEvent");

            Assert.IsFalse(processInstance.IsEnded);

            task = taskService.CreateTaskQuery().First();
            Assert.NotNull(task);

            taskService.Complete(task.Id);

            AssertProcessEnded(processInstance.Id);

        }


        [Test]
        [Deployment]
        public virtual void testMessageStartEventAndNoneStartEvent()
        {

            // using StartProcessInstanceByKey triggers the none start event

            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("testProcess");

            Assert.IsFalse(processInstance.IsEnded);

            ITask task = taskService.CreateTaskQuery(c => c.TaskDefinitionKey == "taskAfterNoneStart").First();
            Assert.NotNull(task);

            taskService.Complete(task.Id);

            AssertProcessEnded(processInstance.Id);

            // using StartProcessInstanceByMessage triggers the message start event

            processInstance = runtimeService.StartProcessInstanceByMessage("newInvoiceMessage");

            Assert.IsFalse(processInstance.IsEnded);

            task = taskService.CreateTaskQuery(c => c.TaskDefinitionKey == "taskAfterMessageStart").First();
            Assert.NotNull(task);

            taskService.Complete(task.Id);

            AssertProcessEnded(processInstance.Id);

        }

        [Test]
        [Deployment]
        public virtual void testMultipleMessageStartEvents()
        {

            // sending newInvoiceMessage

            IProcessInstance processInstance = runtimeService.StartProcessInstanceByMessage("newInvoiceMessage");

            Assert.IsFalse(processInstance.IsEnded);

            ITask task = taskService.CreateTaskQuery(c => c.TaskDefinitionKey == "taskAfterMessageStart").FirstOrDefault();
            Assert.NotNull(task);

            taskService.Complete(task.Id);

            AssertProcessEnded(processInstance.Id);

            // sending newInvoiceMessage2

            processInstance = runtimeService.StartProcessInstanceByMessage("newInvoiceMessage2");

            Assert.IsFalse(processInstance.IsEnded);

            task = taskService.CreateTaskQuery(c => c.TaskDefinitionKey == "taskAfterMessageStart2").First();
            Assert.NotNull(task);

            taskService.Complete(task.Id);

            AssertProcessEnded(processInstance.Id);

            // starting the process using StartProcessInstanceByKey is not possible:
            try
            {
                runtimeService.StartProcessInstanceByKey("testProcess");
                Assert.Fail("exception expected");
            }
            //catch (ProcessEngineException e)
            catch (System.Exception e)
            {
                Assert.True(e.Message.Contains("has no default start activity"), "different exception expected, not " + e.Message);
            }

        }

        [Test]
        [Deployment]
        public virtual void testDeployStartAndIntermediateEventWithSameMessageInSameProcess()
        {
            IProcessInstance pi = null;
            try
            {
                runtimeService.StartProcessInstanceByMessage("message");
                pi = runtimeService.CreateProcessInstanceQuery().First();
                Assert.That(pi.IsEnded, Is.EqualTo(false));

                string DeploymentId = repositoryService.CreateDeployment().AddClasspathResource("resources/bpmn/event/message/MessageStartEventTest.testDeployStartAndIntermediateEventWithSameMessageInSameProcess.bpmn").Name("deployment2").DeployAndReturnDefinitions().Id;
                Assert.That(repositoryService.CreateDeploymentQuery(c => c.Id == DeploymentId).First(), Is.Not.Null);
            }
            finally
            {
                // clean db:
                runtimeService.DeleteProcessInstance(pi.Id, "failure");
                IList<IDeployment> deployments = repositoryService.CreateDeploymentQuery().ToList();
                foreach (IDeployment d in deployments)
                {
                    repositoryService.DeleteDeployment(d.Id, true);
                }
                // Workaround for #CAM-4250: remove process definition of failed
                // deployment from deployment cache

                processEngineConfiguration.DeploymentCache.ProcessDefinitionCache.Clear();
            }
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/message/MessageStartEventTest.testDeployStartAndIntermediateEventWithSameMessageDifferentProcesses.bpmn" })]
        public virtual void testDeployStartAndIntermediateEventWithSameMessageDifferentProcessesFirstStartEvent()
        {
            IProcessInstance pi = null;
            try
            {
                runtimeService.StartProcessInstanceByMessage("message");
                pi = runtimeService.CreateProcessInstanceQuery().First();
                Assert.That(pi.IsEnded, Is.EqualTo(false));

                string DeploymentId = repositoryService.CreateDeployment().AddClasspathResource("resources/bpmn/event/message/MessageStartEventTest.testDeployStartAndIntermediateEventWithSameMessageDifferentProcesses2.bpmn").Name("deployment2").Deploy().Id;
                Assert.That(repositoryService.CreateDeploymentQuery(c => c.Id == DeploymentId).First(), Is.Not.Null);
            }
            finally
            {
                // clean db:
                runtimeService.DeleteProcessInstance(pi.Id, "failure");
                IList<IDeployment> deployments = repositoryService.CreateDeploymentQuery().ToList();
                foreach (IDeployment d in deployments)
                {
                    repositoryService.DeleteDeployment(d.Id, true);
                }
                // Workaround for #CAM-4250: remove process definition of failed
                // deployment from deployment cache

                processEngineConfiguration.DeploymentCache.ProcessDefinitionCache.Clear();
            }
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/message/MessageStartEventTest.testDeployStartAndIntermediateEventWithSameMessageDifferentProcesses2.bpmn" })]
        public virtual void testDeployStartAndIntermediateEventWithSameMessageDifferentProcessesFirstIntermediateEvent()
        {
            IProcessInstance pi = null;
            try
            {
                runtimeService.StartProcessInstanceByKey("Process_2");
                pi = runtimeService.CreateProcessInstanceQuery().First();
                Assert.That(pi.IsEnded, Is.EqualTo(false));

                string DeploymentId = repositoryService.CreateDeployment().AddClasspathResource("resources/bpmn/event/message/MessageStartEventTest.testDeployStartAndIntermediateEventWithSameMessageDifferentProcesses.bpmn").Name("deployment2").Deploy().Id;
                Assert.That(repositoryService.CreateDeploymentQuery(c => c.Id == DeploymentId).First(), Is.Not.Null);
            }
            ////表达式未实现
            //catch(Exception e)
            //{
            //    throw e;
            //}
            finally
            {
                // clean db:
                runtimeService.DeleteProcessInstance(pi.Id, "failure");
                IList<IDeployment> deployments = repositoryService.CreateDeploymentQuery().ToList();
                foreach (IDeployment d in deployments)
                {
                    repositoryService.DeleteDeployment(d.Id, true);
                }
                // Workaround for #CAM-4250: remove process definition of failed
                // deployment from deployment cache

                processEngineConfiguration.DeploymentCache.ProcessDefinitionCache.Clear();
            }
        }
        [Test]
        public virtual void testUsingExpressionWithDollarTagInMessageStartEventNameThrowsException()
        {

            // given a process definition with a start message event that has a message Name which contains an expression
            string processDefinition = "resources/bpmn/event/message/" + "MessageStartEventTest.testUsingExpressionWithDollarTagInMessageStartEventNameThrowsException.bpmn20.xml";
            try
            {
                // when deploying the process
                repositoryService.CreateDeployment().AddClasspathResource(processDefinition).Deploy();
                Assert.Fail("exception expected");
            }
            catch (ProcessEngineException e)
            {
                // then a process engine exception should be thrown with a certain message
                Assert.True(e.Message.Contains("Invalid message name"));
                Assert.True(e.Message.Contains("expressions in the message start event name are not allowed!"));
            }
        }
        [Test]
        public virtual void testUsingExpressionWithHashTagInMessageStartEventNameThrowsException()
        {

            // given a process definition with a start message event that has a message Name which contains an expression
            string processDefinition = "resources/bpmn/event/message/" + "MessageStartEventTest.testUsingExpressionWithHashTagInMessageStartEventNameThrowsException.bpmn20.xml";
            try
            {
                // when deploying the process
                repositoryService.CreateDeployment().AddClasspathResource(processDefinition).Deploy();
                Assert.Fail("exception expected");
            }
            catch (ProcessEngineException e)
            {
                // then a process engine exception should be thrown with a certain message
                Assert.True(e.Message.Contains("Invalid message name"));
                Assert.True(e.Message.Contains("expressions in the message start event name are not allowed!"));
            }
        }

        [Test]
        [Deployment]
        public virtual void testSingleMessageStartEventByOtherProcess()
        {
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("RepariPriceBill");
            

            AssertProcessEnded(processInstance.Id);

        }
    }

}