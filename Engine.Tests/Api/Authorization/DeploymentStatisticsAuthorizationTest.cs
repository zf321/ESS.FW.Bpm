using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Management;
using NUnit.Framework;

namespace Engine.Tests.Api.Authorization
{

    /// <summary>
    /// 
    /// 
    /// </summary>
    public class DeploymentStatisticsAuthorizationTest : AuthorizationTest
    {

        protected internal const string ONE_INCIDENT_PROCESS_KEY = "process";
        protected internal const string TIMER_START_PROCESS_KEY = "timerStartProcess";
        protected internal const string TIMER_BOUNDARY_PROCESS_KEY = "timerBoundaryProcess";

        protected internal string firstDeploymentId;
        protected internal string secondDeploymentId;
        protected internal string thirdDeploymentId;

        [SetUp]
        public void setUp()
        {
            firstDeploymentId = createDeployment("first", "resources/api/authorization/oneIncidentProcess.bpmn20.xml").Id;
            secondDeploymentId = createDeployment("second", "resources/api/authorization/timerStartEventProcess.bpmn20.xml").Id;
            thirdDeploymentId = createDeployment("third", "resources/api/authorization/timerBoundaryEventProcess.bpmn20.xml").Id;
            //base.SetUp();
        }

        [TearDown]
        public void tearDown()
        {
            base.TearDown();
            DeleteDeployment(firstDeploymentId);
            DeleteDeployment(secondDeploymentId);
            DeleteDeployment(thirdDeploymentId);
        }

        // deployment statistics query without process instance authorizations /////////////////////////////////////////////

        public virtual void testQueryWithoutAuthorization()
        {
            // given

            // when
            IQueryable<IDeploymentStatistics> query = managementService.CreateDeploymentStatisticsQuery();

            // then
            //verifyQueryResults(query, 0);
        }

        public virtual void testQueryWithReadPermissionOnDeployment()
        {
            // given
            createGrantAuthorization(Resources.Deployment, firstDeploymentId, userId, Permissions.Read);

            // when
            IQueryable<IDeploymentStatistics> query = managementService.CreateDeploymentStatisticsQuery();

            // then
            //verifyQueryResults(query, 1);

            IDeploymentStatistics statistics = query.First();
            verifyStatisticsResult(statistics, 0, 0, 0);
        }

        public virtual void testQueryWithMultiple()
        {
            // given
            createGrantAuthorization(Resources.Deployment, firstDeploymentId, userId, Permissions.Read);
            createGrantAuthorization(Resources.Deployment, AuthorizationFields.Any, userId, Permissions.Read);

            // when
            IQueryable<IDeploymentStatistics> query = managementService.CreateDeploymentStatisticsQuery();

            // then
            //verifyQueryResults(query, 3);
        }

        public virtual void testQueryWithReadPermissionOnAnyDeployment()
        {
            // given
            createGrantAuthorization(Resources.Deployment, AuthorizationFields.Any, userId, Permissions.Read);

            // when
            IQueryable<IDeploymentStatistics> query = managementService.CreateDeploymentStatisticsQuery();

            // then
            //verifyQueryResults(query, 3);

            IList<IDeploymentStatistics> result = query.ToList();
            foreach (IDeploymentStatistics statistics in result)
            {
                verifyStatisticsResult(statistics, 0, 0, 0);
            }
        }

        // deployment statistics query (including process instances) /////////////////////////////////////////////

        public virtual void testQueryWithReadPermissionOnProcessInstance()
        {
            // given
            createGrantAuthorization(Resources.Deployment, AuthorizationFields.Any, userId, Permissions.Read);

            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);

            StartProcessInstanceByKey(TIMER_START_PROCESS_KEY);
            StartProcessInstanceByKey(TIMER_START_PROCESS_KEY);
            StartProcessInstanceByKey(TIMER_START_PROCESS_KEY);

            StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
            StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Read);

            // when
            IQueryable<IDeploymentStatistics> query = managementService.CreateDeploymentStatisticsQuery();

            // then
            IList<IDeploymentStatistics> statistics = query.ToList();

            foreach (IDeploymentStatistics deploymentStatistics in statistics)
            {
                string id = deploymentStatistics.Id;
                if (id.Equals(firstDeploymentId))
                {
                    verifyStatisticsResult(deploymentStatistics, 0, 0, 0);
                }
                else if (id.Equals(secondDeploymentId))
                {
                    verifyStatisticsResult(deploymentStatistics, 0, 0, 0);
                }
                else if (id.Equals(thirdDeploymentId))
                {
                    verifyStatisticsResult(deploymentStatistics, 1, 0, 0);
                }
                else
                {
                    Assert.Fail("Unexpected deployment");
                }
            }
        }

        public virtual void testQueryWithReadPermissionOnAnyProcessInstance()
        {
            // given
            createGrantAuthorization(Resources.Deployment, AuthorizationFields.Any, userId, Permissions.Read);

            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);

            StartProcessInstanceByKey(TIMER_START_PROCESS_KEY);
            StartProcessInstanceByKey(TIMER_START_PROCESS_KEY);
            StartProcessInstanceByKey(TIMER_START_PROCESS_KEY);

            StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
            StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
            StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);

            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Read);

            // when
            IQueryable<IDeploymentStatistics> query = managementService.CreateDeploymentStatisticsQuery();

            // then
            IList<IDeploymentStatistics> statistics = query.ToList();

            foreach (IDeploymentStatistics deploymentStatistics in statistics)
            {
                string id = deploymentStatistics.Id;
                if (id.Equals(firstDeploymentId))
                {
                    verifyStatisticsResult(deploymentStatistics, 3, 0, 0);
                }
                else if (id.Equals(secondDeploymentId))
                {
                    verifyStatisticsResult(deploymentStatistics, 3, 0, 0);
                }
                else if (id.Equals(thirdDeploymentId))
                {
                    verifyStatisticsResult(deploymentStatistics, 3, 0, 0);
                }
                else
                {
                    Assert.Fail("Unexpected deployment");
                }
            }
        }

        public virtual void testQueryWithReadInstancePermissionOnProcessDefinition()
        {
            // given
            createGrantAuthorization(Resources.Deployment, AuthorizationFields.Any, userId, Permissions.Read);

            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);

            StartProcessInstanceByKey(TIMER_START_PROCESS_KEY);
            StartProcessInstanceByKey(TIMER_START_PROCESS_KEY);
            StartProcessInstanceByKey(TIMER_START_PROCESS_KEY);

            StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
            StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
            StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);

            createGrantAuthorization(Resources.ProcessDefinition, TIMER_BOUNDARY_PROCESS_KEY, userId, Permissions.ReadInstance);

            // when
            IQueryable<IDeploymentStatistics> query = managementService.CreateDeploymentStatisticsQuery();

            // then
            IList<IDeploymentStatistics> statistics = query.ToList();

            foreach (IDeploymentStatistics deploymentStatistics in statistics)
            {
                string id = deploymentStatistics.Id;
                if (id.Equals(firstDeploymentId))
                {
                    verifyStatisticsResult(deploymentStatistics, 0, 0, 0);
                }
                else if (id.Equals(secondDeploymentId))
                {
                    verifyStatisticsResult(deploymentStatistics, 0, 0, 0);
                }
                else if (id.Equals(thirdDeploymentId))
                {
                    verifyStatisticsResult(deploymentStatistics, 3, 0, 0);
                }
                else
                {
                    Assert.Fail("Unexpected deployment");
                }
            }
        }

        public virtual void testQueryWithReadInstancePermissionOnAnyProcessDefinition()
        {
            // given
            createGrantAuthorization(Resources.Deployment, AuthorizationFields.Any, userId, Permissions.Read);

            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);

            StartProcessInstanceByKey(TIMER_START_PROCESS_KEY);
            StartProcessInstanceByKey(TIMER_START_PROCESS_KEY);
            StartProcessInstanceByKey(TIMER_START_PROCESS_KEY);

            StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
            StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
            StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);

            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadInstance);

            // when
            IQueryable<IDeploymentStatistics> query = managementService.CreateDeploymentStatisticsQuery();

            // then
            IList<IDeploymentStatistics> statistics = query.ToList();

            foreach (IDeploymentStatistics deploymentStatistics in statistics)
            {
                string id = deploymentStatistics.Id;
                if (id.Equals(firstDeploymentId))
                {
                    verifyStatisticsResult(deploymentStatistics, 3, 0, 0);
                }
                else if (id.Equals(secondDeploymentId))
                {
                    verifyStatisticsResult(deploymentStatistics, 3, 0, 0);
                }
                else if (id.Equals(thirdDeploymentId))
                {
                    verifyStatisticsResult(deploymentStatistics, 3, 0, 0);
                }
                else
                {
                    Assert.Fail("Unexpected deployment");
                }
            }
        }

        // deployment statistics query (including failed jobs) /////////////////////////////////////////////

        public virtual void testQueryIncludingFailedJobsWithReadPermissionOnProcessInstance()
        {
            // given
            createGrantAuthorization(Resources.Deployment, AuthorizationFields.Any, userId, Permissions.Read);

            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
            string ProcessInstanceId = startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Read);

            StartProcessInstanceByKey(TIMER_START_PROCESS_KEY);
            StartProcessInstanceByKey(TIMER_START_PROCESS_KEY);
            StartProcessInstanceByKey(TIMER_START_PROCESS_KEY);

            StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
            StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
            StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);

            // when
            IQueryable<IDeploymentStatistics> query = managementService.CreateDeploymentStatisticsQuery()/*.IncludeFailedJobs()*/;

            // then
            IList<IDeploymentStatistics> statistics = query.ToList();

            foreach (IDeploymentStatistics deploymentStatistics in statistics)
            {
                string id = deploymentStatistics.Id;
                if (id.Equals(firstDeploymentId))
                {
                    verifyStatisticsResult(deploymentStatistics, 1, 1, 0);
                }
                else if (id.Equals(secondDeploymentId))
                {
                    verifyStatisticsResult(deploymentStatistics, 0, 0, 0);
                }
                else if (id.Equals(thirdDeploymentId))
                {
                    verifyStatisticsResult(deploymentStatistics, 0, 0, 0);
                }
                else
                {
                    Assert.Fail("Unexpected deployment");
                }
            }
        }

        public virtual void testQueryIncludingFailedJobsWithReadPermissionOnAnyProcessInstance()
        {
            // given
            createGrantAuthorization(Resources.Deployment, AuthorizationFields.Any, userId, Permissions.Read);

            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);

            StartProcessInstanceByKey(TIMER_START_PROCESS_KEY);
            StartProcessInstanceByKey(TIMER_START_PROCESS_KEY);
            StartProcessInstanceByKey(TIMER_START_PROCESS_KEY);

            StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
            StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
            StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);

            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Read);

            // when
            IQueryable<IDeploymentStatistics> query = managementService.CreateDeploymentStatisticsQuery()/*.IncludeFailedJobs()*/;

            // then
            IList<IDeploymentStatistics> statistics = query.ToList();

            foreach (IDeploymentStatistics deploymentStatistics in statistics)
            {
                string id = deploymentStatistics.Id;
                if (id.Equals(firstDeploymentId))
                {
                    verifyStatisticsResult(deploymentStatistics, 3, 3, 0);
                }
                else if (id.Equals(secondDeploymentId))
                {
                    verifyStatisticsResult(deploymentStatistics, 3, 0, 0);
                }
                else if (id.Equals(thirdDeploymentId))
                {
                    verifyStatisticsResult(deploymentStatistics, 3, 0, 0);
                }
                else
                {
                    Assert.Fail("Unexpected deployment");
                }
            }
        }

        public virtual void testQueryIncludingFailedJobsWithReadInstancePermissionOnProcessDefinition()
        {
            // given
            createGrantAuthorization(Resources.Deployment, AuthorizationFields.Any, userId, Permissions.Read);

            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);

            StartProcessInstanceByKey(TIMER_START_PROCESS_KEY);
            StartProcessInstanceByKey(TIMER_START_PROCESS_KEY);
            StartProcessInstanceByKey(TIMER_START_PROCESS_KEY);

            StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
            StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
            StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);

            createGrantAuthorization(Resources.ProcessDefinition, ONE_INCIDENT_PROCESS_KEY, userId, Permissions.ReadInstance);

            // when
            IQueryable<IDeploymentStatistics> query = managementService.CreateDeploymentStatisticsQuery()/*.IncludeFailedJobs()*/;

            // then
            IList<IDeploymentStatistics> statistics = query.ToList();

            foreach (IDeploymentStatistics deploymentStatistics in statistics)
            {
                string id = deploymentStatistics.Id;
                if (id.Equals(firstDeploymentId))
                {
                    verifyStatisticsResult(deploymentStatistics, 3, 3, 0);
                }
                else if (id.Equals(secondDeploymentId))
                {
                    verifyStatisticsResult(deploymentStatistics, 0, 0, 0);
                }
                else if (id.Equals(thirdDeploymentId))
                {
                    verifyStatisticsResult(deploymentStatistics, 0, 0, 0);
                }
                else
                {
                    Assert.Fail("Unexpected deployment");
                }
            }
        }

        public virtual void testQueryIncludingFailedJobsWithReadInstancePermissionOnAnyProcessDefinition()
        {
            // given
            createGrantAuthorization(Resources.Deployment, AuthorizationFields.Any, userId, Permissions.Read);

            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);

            StartProcessInstanceByKey(TIMER_START_PROCESS_KEY);
            StartProcessInstanceByKey(TIMER_START_PROCESS_KEY);
            StartProcessInstanceByKey(TIMER_START_PROCESS_KEY);

            StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
            StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
            StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);

            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadInstance);

            // when
            IQueryable<IDeploymentStatistics> query = managementService.CreateDeploymentStatisticsQuery()/*.IncludeFailedJobs()*/;

            // then
            IList<IDeploymentStatistics> statistics = query.ToList();

            foreach (IDeploymentStatistics deploymentStatistics in statistics)
            {
                string id = deploymentStatistics.Id;
                if (id.Equals(firstDeploymentId))
                {
                    verifyStatisticsResult(deploymentStatistics, 3, 3, 0);
                }
                else if (id.Equals(secondDeploymentId))
                {
                    verifyStatisticsResult(deploymentStatistics, 3, 0, 0);
                }
                else if (id.Equals(thirdDeploymentId))
                {
                    verifyStatisticsResult(deploymentStatistics, 3, 0, 0);
                }
                else
                {
                    Assert.Fail("Unexpected deployment");
                }
            }
        }

        // deployment statistics query (including incidents) /////////////////////////////////////////////

        public virtual void testQueryIncludingIncidentsWithReadPermissionOnProcessInstance()
        {
            // given
            createGrantAuthorization(Resources.Deployment, AuthorizationFields.Any, userId, Permissions.Read);

            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
            string ProcessInstanceId = startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Read);

            StartProcessInstanceByKey(TIMER_START_PROCESS_KEY);
            StartProcessInstanceByKey(TIMER_START_PROCESS_KEY);
            StartProcessInstanceByKey(TIMER_START_PROCESS_KEY);

            StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
            StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
            StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);

            // when
            IQueryable<IDeploymentStatistics> query = managementService.CreateDeploymentStatisticsQuery()/*.IncludeIncidents()*/;

            // then
            IList<IDeploymentStatistics> statistics = query.ToList();

            foreach (IDeploymentStatistics deploymentStatistics in statistics)
            {
                string id = deploymentStatistics.Id;
                if (id.Equals(firstDeploymentId))
                {
                    verifyStatisticsResult(deploymentStatistics, 1, 0, 1);
                }
                else if (id.Equals(secondDeploymentId))
                {
                    verifyStatisticsResult(deploymentStatistics, 0, 0, 0);
                }
                else if (id.Equals(thirdDeploymentId))
                {
                    verifyStatisticsResult(deploymentStatistics, 0, 0, 0);
                }
                else
                {
                    Assert.Fail("Unexpected deployment");
                }
            }
        }

        public virtual void testQueryIncludingIncidentsWithReadPermissionOnAnyProcessInstance()
        {
            // given
            createGrantAuthorization(Resources.Deployment, AuthorizationFields.Any, userId, Permissions.Read);

            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);

            StartProcessInstanceByKey(TIMER_START_PROCESS_KEY);
            StartProcessInstanceByKey(TIMER_START_PROCESS_KEY);
            StartProcessInstanceByKey(TIMER_START_PROCESS_KEY);

            StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
            StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
            StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);

            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Read);

            // when
            IQueryable<IDeploymentStatistics> query = managementService.CreateDeploymentStatisticsQuery()/*.IncludeIncidents()*/;

            // then
            IList<IDeploymentStatistics> statistics = query.ToList();

            foreach (IDeploymentStatistics deploymentStatistics in statistics)
            {
                string id = deploymentStatistics.Id;
                if (id.Equals(firstDeploymentId))
                {
                    verifyStatisticsResult(deploymentStatistics, 3, 0, 3);
                }
                else if (id.Equals(secondDeploymentId))
                {
                    verifyStatisticsResult(deploymentStatistics, 3, 0, 0);
                }
                else if (id.Equals(thirdDeploymentId))
                {
                    verifyStatisticsResult(deploymentStatistics, 3, 0, 0);
                }
                else
                {
                    Assert.Fail("Unexpected deployment");
                }
            }
        }

        public virtual void testQueryIncludingIncidentsWithReadInstancePermissionOnProcessDefinition()
        {
            // given
            createGrantAuthorization(Resources.Deployment, AuthorizationFields.Any, userId, Permissions.Read);

            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);

            StartProcessInstanceByKey(TIMER_START_PROCESS_KEY);
            StartProcessInstanceByKey(TIMER_START_PROCESS_KEY);
            StartProcessInstanceByKey(TIMER_START_PROCESS_KEY);

            StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
            StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
            StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);

            createGrantAuthorization(Resources.ProcessDefinition, ONE_INCIDENT_PROCESS_KEY, userId, Permissions.ReadInstance);

            // when
            IQueryable<IDeploymentStatistics> query = managementService.CreateDeploymentStatisticsQuery()/*.IncludeIncidents()*/;

            // then
            IList<IDeploymentStatistics> statistics = query.ToList();

            foreach (IDeploymentStatistics deploymentStatistics in statistics)
            {
                string id = deploymentStatistics.Id;
                if (id.Equals(firstDeploymentId))
                {
                    verifyStatisticsResult(deploymentStatistics, 3, 0, 3);
                }
                else if (id.Equals(secondDeploymentId))
                {
                    verifyStatisticsResult(deploymentStatistics, 0, 0, 0);
                }
                else if (id.Equals(thirdDeploymentId))
                {
                    verifyStatisticsResult(deploymentStatistics, 0, 0, 0);
                }
                else
                {
                    Assert.Fail("Unexpected deployment");
                }
            }
        }

        public virtual void testQueryIncludingIncidentsWithReadInstancePermissionOnAnyProcessDefinition()
        {
            // given
            createGrantAuthorization(Resources.Deployment, AuthorizationFields.Any, userId, Permissions.Read);

            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);

            StartProcessInstanceByKey(TIMER_START_PROCESS_KEY);
            StartProcessInstanceByKey(TIMER_START_PROCESS_KEY);
            StartProcessInstanceByKey(TIMER_START_PROCESS_KEY);

            StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
            StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
            StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);

            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadInstance);

            // when
            IQueryable<IDeploymentStatistics> query = managementService.CreateDeploymentStatisticsQuery()/*.IncludeIncidents()*/;

            // then
            IList<IDeploymentStatistics> statistics = query.ToList();

            foreach (IDeploymentStatistics deploymentStatistics in statistics)
            {
                string id = deploymentStatistics.Id;
                if (id.Equals(firstDeploymentId))
                {
                    verifyStatisticsResult(deploymentStatistics, 3, 0, 3);
                }
                else if (id.Equals(secondDeploymentId))
                {
                    verifyStatisticsResult(deploymentStatistics, 3, 0, 0);
                }
                else if (id.Equals(thirdDeploymentId))
                {
                    verifyStatisticsResult(deploymentStatistics, 3, 0, 0);
                }
                else
                {
                    Assert.Fail("Unexpected deployment");
                }
            }
        }

        // deployment statistics query (including failed jobs and incidents) /////////////////////////////////////////////

        public virtual void testQueryIncludingFailedJobsAndIncidentsWithReadPermissionOnProcessInstance()
        {
            // given
            createGrantAuthorization(Resources.Deployment, AuthorizationFields.Any, userId, Permissions.Read);

            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
            string ProcessInstanceId = startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Read);

            StartProcessInstanceByKey(TIMER_START_PROCESS_KEY);
            StartProcessInstanceByKey(TIMER_START_PROCESS_KEY);
            StartProcessInstanceByKey(TIMER_START_PROCESS_KEY);

            StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
            StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
            StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);

            // when
            IQueryable<IDeploymentStatistics> query = managementService.CreateDeploymentStatisticsQuery()/*.IncludeFailedJobs()*//*.IncludeIncidents()*/;

            // then
            IList<IDeploymentStatistics> statistics = query.ToList();

            foreach (IDeploymentStatistics deploymentStatistics in statistics)
            {
                string id = deploymentStatistics.Id;
                if (id.Equals(firstDeploymentId))
                {
                    verifyStatisticsResult(deploymentStatistics, 1, 1, 1);
                }
                else if (id.Equals(secondDeploymentId))
                {
                    verifyStatisticsResult(deploymentStatistics, 0, 0, 0);
                }
                else if (id.Equals(thirdDeploymentId))
                {
                    verifyStatisticsResult(deploymentStatistics, 0, 0, 0);
                }
                else
                {
                    Assert.Fail("Unexpected deployment");
                }
            }
        }

        public virtual void testQueryIncludingFailedJobsAndIncidentsWithReadPermissionOnAnyProcessInstance()
        {
            // given
            createGrantAuthorization(Resources.Deployment, AuthorizationFields.Any, userId, Permissions.Read);

            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);

            StartProcessInstanceByKey(TIMER_START_PROCESS_KEY);
            StartProcessInstanceByKey(TIMER_START_PROCESS_KEY);
            StartProcessInstanceByKey(TIMER_START_PROCESS_KEY);

            StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
            StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
            StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);

            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Read);

            // when
            IQueryable<IDeploymentStatistics> query = managementService.CreateDeploymentStatisticsQuery()/*.IncludeFailedJobs()*//*.IncludeIncidents()*/;

            // then
            IList<IDeploymentStatistics> statistics = query.ToList();

            foreach (IDeploymentStatistics deploymentStatistics in statistics)
            {
                string id = deploymentStatistics.Id;
                if (id.Equals(firstDeploymentId))
                {
                    verifyStatisticsResult(deploymentStatistics, 3, 3, 3);
                }
                else if (id.Equals(secondDeploymentId))
                {
                    verifyStatisticsResult(deploymentStatistics, 3, 0, 0);
                }
                else if (id.Equals(thirdDeploymentId))
                {
                    verifyStatisticsResult(deploymentStatistics, 3, 0, 0);
                }
                else
                {
                    Assert.Fail("Unexpected deployment");
                }
            }
        }

        public virtual void testQueryIncludingFailedJobsAndIncidentsWithReadInstancePermissionOnProcessDefinition()
        {
            // given
            createGrantAuthorization(Resources.Deployment, AuthorizationFields.Any, userId, Permissions.Read);

            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);

            StartProcessInstanceByKey(TIMER_START_PROCESS_KEY);
            StartProcessInstanceByKey(TIMER_START_PROCESS_KEY);
            StartProcessInstanceByKey(TIMER_START_PROCESS_KEY);

            StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
            StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
            StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);

            createGrantAuthorization(Resources.ProcessDefinition, ONE_INCIDENT_PROCESS_KEY, userId, Permissions.ReadInstance);

            // when
            IQueryable<IDeploymentStatistics> query = managementService.CreateDeploymentStatisticsQuery()/*.IncludeFailedJobs()*//*.IncludeIncidents()*/;

            // then
            IList<IDeploymentStatistics> statistics = query.ToList();

            foreach (IDeploymentStatistics deploymentStatistics in statistics)
            {
                string id = deploymentStatistics.Id;
                if (id.Equals(firstDeploymentId))
                {
                    verifyStatisticsResult(deploymentStatistics, 3, 3, 3);
                }
                else if (id.Equals(secondDeploymentId))
                {
                    verifyStatisticsResult(deploymentStatistics, 0, 0, 0);
                }
                else if (id.Equals(thirdDeploymentId))
                {
                    verifyStatisticsResult(deploymentStatistics, 0, 0, 0);
                }
                else
                {
                    Assert.Fail("Unexpected deployment");
                }
            }
        }

        public virtual void testQueryIncludingFailedJobsAndIncidentsWithReadInstancePermissionOnAnyProcessDefinition()
        {
            // given
            createGrantAuthorization(Resources.Deployment, AuthorizationFields.Any, userId, Permissions.Read);

            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);

            StartProcessInstanceByKey(TIMER_START_PROCESS_KEY);
            StartProcessInstanceByKey(TIMER_START_PROCESS_KEY);
            StartProcessInstanceByKey(TIMER_START_PROCESS_KEY);

            StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
            StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
            StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);

            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadInstance);

            // when
            IQueryable<IDeploymentStatistics> query = managementService.CreateDeploymentStatisticsQuery()/*.IncludeFailedJobs()*//*.IncludeIncidents()*/;

            // then
            IList<IDeploymentStatistics> statistics = query.ToList();

            foreach (IDeploymentStatistics deploymentStatistics in statistics)
            {
                string id = deploymentStatistics.Id;
                if (id.Equals(firstDeploymentId))
                {
                    verifyStatisticsResult(deploymentStatistics, 3, 3, 3);
                }
                else if (id.Equals(secondDeploymentId))
                {
                    verifyStatisticsResult(deploymentStatistics, 3, 0, 0);
                }
                else if (id.Equals(thirdDeploymentId))
                {
                    verifyStatisticsResult(deploymentStatistics, 3, 0, 0);
                }
                else
                {
                    Assert.Fail("Unexpected deployment");
                }
            }
        }

        // helper ///////////////////////////////////////////////////////////////////////////

        protected internal virtual void verifyQueryResults(IQueryable<IDeploymentStatistics> query, int countExpected)
        {
            //JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
            //ORIGINAL LINE: //verifyQueryResults((org.camunda.bpm.Engine.impl.AbstractQuery<?, ?>) query, countExpected);
            ////verifyQueryResults((AbstractQuery<object, object>)query, countExpected);
        }

        protected internal virtual void verifyStatisticsResult(IDeploymentStatistics statistics, int instances, int failedJobs, int incidents)
        {
            //Assert.AreEqual("Instances", instances, statistics.Instances);
            //Assert.AreEqual("Failed Jobs", failedJobs, statistics.FailedJobs);

            Assert.AreEqual(instances, statistics.Instances, "Instances");
            Assert.AreEqual(failedJobs, statistics.FailedJobs, "Failed Jobs");

            IList<IIncidentStatistics> incidentStatistics = statistics.IncidentStatistics;
            if (incidents == 0)
            {
                //Assert.True("Incidents supposed to be empty", incidentStatistics.Count == 0);
                Assert.True(incidentStatistics.Count == 0, "Incidents supposed to be empty");
            }
            else
            {
                // the test does have only one type of incidents
                //Assert.AreEqual("Incidents", incidents, incidentStatistics[0].IncidentCount);
                Assert.AreEqual(incidents, incidentStatistics[0].IncidentCount, "Incidents");
            }
        }

    }

}