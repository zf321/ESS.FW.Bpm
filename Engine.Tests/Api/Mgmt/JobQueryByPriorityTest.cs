using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.Api.Mgmt
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class JobQueryByPriorityTest : PluggableProcessEngineTestCase
    {
        [Test]
        [Deployment("resources/api/mgmt/jobPrioExpressionProcess.bpmn20.xml")]
        public virtual void testFilterByJobPriorityHigherThanOrEquals()
        {
            // given five jobs with priorities from 1 to 5
            IList<IProcessInstance> instances = new List<IProcessInstance>();

            for (var i = 0; i < 5; i++)
                instances.Add(runtimeService.StartProcessInstanceByKey("jobPrioExpressionProcess",
                    ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                        .PutValue("priority", i)));

            // when making a job query and filtering by job priority
            // then the correct jobs are returned
            var jobs = managementService.CreateJobQuery(c=>c.Priority>=2L)
                
                .ToList();
            Assert.AreEqual(3, jobs.Count);

            ISet<string> processInstanceIds = new HashSet<string>();
            processInstanceIds.Add(instances[2].Id);
            processInstanceIds.Add(instances[3].Id);
            processInstanceIds.Add(instances[4].Id);

            foreach (var job in jobs)
            {
                Assert.True(job.Priority >= 2);
                Assert.True(processInstanceIds.Contains(job.ProcessInstanceId));
            }
        }

        [Test]
        [Deployment("resources/api/mgmt/jobPrioExpressionProcess.bpmn20.xml")]
        public virtual void testFilterByJobPriorityLowerAndHigher()
        {
            // given five jobs with priorities from 1 to 5
            IList<IProcessInstance> instances = new List<IProcessInstance>();

            for (var i = 0; i < 5; i++)
                instances.Add(runtimeService.StartProcessInstanceByKey("jobPrioExpressionProcess",
                    ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                        .PutValue("priority", i)));

            // when making a job query and filtering by job priority
            // then the correct job is returned
            var job = managementService.CreateJobQuery(c=>c.Priority>=2L && c.Priority<=2L)
                .First();
            Assert.NotNull(job);
            Assert.AreEqual(2, job.Priority);
            Assert.AreEqual(instances[2].Id, job.ProcessInstanceId);
        }

        [Test]
        [Deployment("resources/api/mgmt/jobPrioExpressionProcess.bpmn20.xml")]
        public virtual void testFilterByJobPriorityLowerThanOrEquals()
        {
            // given five jobs with priorities from 1 to 5
            IList<IProcessInstance> instances = new List<IProcessInstance>();

            for (var i = 0; i < 5; i++)
                instances.Add(runtimeService.StartProcessInstanceByKey("jobPrioExpressionProcess",
                    ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                        .PutValue("priority", i)));

            // when making a job query and filtering by job priority
            // then the correct jobs are returned
            var jobs = managementService.CreateJobQuery(c=>c.Priority<=2)
                
                .ToList();
            Assert.AreEqual(3, jobs.Count);

            ISet<string> processInstanceIds = new HashSet<string>();
            processInstanceIds.Add(instances[0].Id);
            processInstanceIds.Add(instances[1].Id);
            processInstanceIds.Add(instances[2].Id);

            foreach (var job in jobs)
            {
                Assert.True(job.Priority <= 2);
                Assert.True(processInstanceIds.Contains(job.ProcessInstanceId));
            }
        }

        [Test]
        [Deployment("resources/api/mgmt/jobPrioExpressionProcess.bpmn20.xml")]
        public virtual void testFilterByJobPriorityLowerThanOrEqualsAndHigherThanOrEqual()
        {
            // given five jobs with priorities from 1 to 5
            IList<IProcessInstance> instances = new List<IProcessInstance>();

            for (var i = 0; i < 5; i++)
                instances.Add(runtimeService.StartProcessInstanceByKey("jobPrioExpressionProcess",
                    ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                        .PutValue("priority", i)));

            // when making a job query and filtering by disjunctive job priority
            // then the no jobs are returned
            Assert.AreEqual(0, managementService.CreateJobQuery(c=>c.Priority<=2)
                //.PriorityHigherThanOrEquals(3)
                .Count());
        }

        [Test]
        [Deployment("resources/api/mgmt/jobPrioExpressionProcess.bpmn20.xml")]
        public virtual void testOrderByPriority()
        {
            // given five jobs with priorities from 1 to 5
            IList<IProcessInstance> instances = new List<IProcessInstance>();

            for (var i = 0; i < 5; i++)
                instances.Add(runtimeService.StartProcessInstanceByKey("jobPrioExpressionProcess",
                    ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                        .PutValue("priority", i)));

            // then querying and ordering by priority works
            //VerifySortingAndCount(managementService.CreateJobQuery().OrderByJobPriority()/*.Asc()*/, 5, jobByPriority());
            //VerifySortingAndCount(managementService.CreateJobQuery().OrderByJobPriority()/*.Desc()*/, 5, inverted(jobByPriority()));
        }
    }
}