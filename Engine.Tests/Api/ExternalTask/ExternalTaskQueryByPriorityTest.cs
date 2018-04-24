using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.Api.ExternalTask
{

    /// <summary>
    /// 
    /// </summary>
    public class ExternalTaskQueryByPriorityTest : PluggableProcessEngineTestCase
    {

        [Deployment(new string[] { "resources/api/externaltask/externalTaskPriorityExpression.bpmn20.xml" })]
        public virtual void testOrderByPriority()
        {
            // given five jobs with priorities from 1 to 5
            //each process has two external tasks - one with priority expression and one without priority
            IList<IProcessInstance> instances = new List<IProcessInstance>();

            for (int i = 0; i < 5; i++)
            {
                instances.Add(runtimeService.StartProcessInstanceByKey("twoExternalTaskWithPriorityProcess", ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables().PutValue("priority", i)));
            }

            // then querying and ordering by priority works
            // Todo.TestOrderingUtil.VerifySortingAndCount  &  TestOrderingUtil.externalTaskByPriority
            //TestOrderingUtil.VerifySortingAndCount(externalTaskService.CreateExternalTaskQuery().OrderByPriority()/*.Asc()*/, 10, TestOrderingUtil.externalTaskByPriority());
            //TestOrderingUtil.VerifySortingAndCount(externalTaskService.CreateExternalTaskQuery().OrderByPriority()/*.Desc()*/, 10, TestOrderingUtil.inverted(TestOrderingUtil.externalTaskByPriority()));
        }

        [Deployment(new string[] { "resources/api/externaltask/externalTaskPriorityExpression.bpmn20.xml" })]
        public virtual void testFilterByExternalTaskPriorityLowerThanOrEquals()
        {
            // given five jobs with priorities from 1 to 5
            //each process has two external tasks - one with priority expression and one without priority
            IList<IProcessInstance> instances = new List<IProcessInstance>();

            for (int i = 0; i < 5; i++)
            {
                instances.Add(runtimeService.StartProcessInstanceByKey("twoExternalTaskWithPriorityProcess", ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables().PutValue("priority", i)));
            }

            // when making a external task query and filtering by priority
            // then the correct external tasks are returned
            IList<ESS.FW.Bpm.Engine.Externaltask.IExternalTask> tasks = externalTaskService.CreateExternalTaskQuery(c=>c.Priority<=2)
                .ToList();
            Assert.AreEqual(8, tasks.Count);

            foreach (ESS.FW.Bpm.Engine.Externaltask.IExternalTask task in tasks)
            {
                Assert.True(task.Priority <= 2);
            }
        }

        [Deployment(new string[] { "resources/api/externaltask/externalTaskPriorityExpression.bpmn20.xml" })]
        public virtual void testFilterByExternalTaskPriorityLowerThanOrEqualsAndHigherThanOrEqual()
        {
            // given five jobs with priorities from 1 to 5
            IList<IProcessInstance> instances = new List<IProcessInstance>();

            for (int i = 0; i < 5; i++)
            {
                instances.Add(runtimeService.StartProcessInstanceByKey("twoExternalTaskWithPriorityProcess", ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables().PutValue("priority", i)));
            }

            // when making a external task query and filtering by disjunctive external task priority
            // then no external task are returned
            Assert.AreEqual(0, externalTaskService.CreateExternalTaskQuery(c=>c.Priority <=2&& c.Priority >=3).Count());

            // when making a external task query and filtering by external task priority >= 2 and <= 3
            // then two external task are returned
            Assert.AreEqual(2, externalTaskService.CreateExternalTaskQuery(c=>c.Priority >=2&& c.Priority <=3).Count());
        }

        [Deployment(new string[] { "resources/api/externaltask/externalTaskPriorityExpression.bpmn20.xml" })]
        public virtual void testFilterByExternalTaskPriorityHigherThanOrEquals()
        {
            // given five jobs with priorities from 1 to 5
            IList<IProcessInstance> instances = new List<IProcessInstance>();

            for (int i = 0; i < 5; i++)
            {
                instances.Add(runtimeService.StartProcessInstanceByKey("twoExternalTaskWithPriorityProcess", ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables().PutValue("priority", i)));
            }

            // when making a external task query and filtering by external task priority
            // then the correct external task are returned
            IList<ESS.FW.Bpm.Engine.Externaltask.IExternalTask> tasks = externalTaskService.CreateExternalTaskQuery()
                    //.PriorityHigherThanOrEquals(2L)
                    .ToList();
                ;Assert.AreEqual(3, tasks.Count);

            ISet<string> processInstanceIds = new HashSet<string>();
            processInstanceIds.Add(instances[2].Id);
            processInstanceIds.Add(instances[3].Id);
            processInstanceIds.Add(instances[4].Id);

            foreach (ESS.FW.Bpm.Engine.Externaltask.IExternalTask task in tasks)
            {
                Assert.True(task.Priority >= 2);
                Assert.True(processInstanceIds.Contains(task.ProcessInstanceId));
            }
        }

        [Deployment(new string[] { "resources/api/externaltask/externalTaskPriorityExpression.bpmn20.xml" })]
        public virtual void testFilterByExternalTaskPriorityLowerAndHigher()
        {
            // given five jobs with priorities from 1 to 5
            IList<IProcessInstance> instances = new List<IProcessInstance>();

            for (int i = 0; i < 5; i++)
            {
                instances.Add(runtimeService.StartProcessInstanceByKey("twoExternalTaskWithPriorityProcess", ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables().PutValue("priority", i)));
            }

            // when making a external task query and filtering by external task priority
            // then the correct external task is returned
            ESS.FW.Bpm.Engine.Externaltask.IExternalTask task = externalTaskService.CreateExternalTaskQuery(c=>c.Priority >=2L&& c.Priority <=2L).First();
            Assert.NotNull(task);
            Assert.AreEqual(2, task.Priority);
            Assert.AreEqual(instances[2].Id, task.ProcessInstanceId);
        }
    }

}