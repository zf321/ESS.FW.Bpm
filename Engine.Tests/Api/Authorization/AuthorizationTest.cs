using System;
using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Identity;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Task;
using ESS.FW.Bpm.Engine.Variable;
using NUnit.Framework;

namespace Engine.Tests.Api.Authorization
{
    [TestFixture]
    public abstract class AuthorizationTest : PluggableProcessEngineTestCase
    {
        [SetUp]
        protected internal virtual void setUp()
        {
            user = createUser(userId);
            group = createGroup(groupId);

            identityService.CreateMembership(userId, groupId);

            identityService.SetAuthentication(userId, new[] {groupId});
            processEngineConfiguration.SetAuthorizationEnabled(true);
        }

        [TearDown]
        public void tearDown()
        {
            processEngineConfiguration.SetAuthorizationEnabled(false);
            foreach (var user in identityService.CreateUserQuery()
                .ToList())
                identityService.DeleteUser(user.Id);
            foreach (var group in identityService.CreateGroupQuery()
                .ToList())
                identityService.DeleteGroup(group.Id);
            foreach (var authorization in authorizationService.CreateAuthorizationQuery()
                .ToList())
                authorizationService.DeleteAuthorization(authorization.Id);
        }

        protected internal string userId = "test";
        protected internal string groupId = "accounting";
        protected internal IUser user;
        protected internal IGroup group;

        protected internal const string VARIABLE_NAME = "aVariableName";
        protected internal const string VARIABLE_VALUE = "aVariableValue";

        protected internal virtual T runWithoutAuthorization<T>(Func<T> runnable)
        {
            var authorizationEnabled = processEngineConfiguration.AuthorizationEnabled;
            try
            {
                disableAuthorization();
                return runnable();
            }
            catch (System.Exception e)
            {
                throw e;
            }
            finally
            {
                if (authorizationEnabled)
                    enableAuthorization();
            }
        }

        protected internal virtual void runWithoutAuthorization(Action runnable)
        {
            var authorizationEnabled = processEngineConfiguration.AuthorizationEnabled;
            try
            {
                disableAuthorization();
                runnable();
            }
            catch (System.Exception e)
            {
                throw e;
            }
            finally
            {
                if (authorizationEnabled)
                    enableAuthorization();
            }
        }


        // user ////////////////////////////////////////////////////////////////

        protected internal virtual IUser createUser(string userId)
        {
            var user = identityService.NewUser(userId);
            identityService.SaveUser(user);

            // give user all permission to manipulate authorizations
            var authorization = createGrantAuthorization(Resources.Authorization, AuthorizationFields.Any);
            authorization.UserId = userId;
            authorization.AddPermission(Permissions.All);
            saveAuthorization(authorization);

            // give user all permission to manipulate users
            authorization = createGrantAuthorization(Resources.User, AuthorizationFields.Any);
            authorization.UserId = userId;
            authorization.AddPermission(Permissions.All);
            saveAuthorization(authorization);

            return user;
        }


        protected internal virtual IGroup createGroup(string groupId)
        {
            return runWithoutAuthorization(() =>
            {
                var group = identityService.NewGroup(groupId);
                identityService.SaveGroup(group);
                return group;
            });
        }


        // authorization ///////////////////////////////////////////////////////

        protected internal virtual void createGrantAuthorization(Resources resource, string resourceId, string userId,
            params Permissions[] permissions)
        {
            var authorization = createGrantAuthorization(resource, resourceId);
            authorization.UserId = userId;
            foreach (var permission in permissions)
                authorization.AddPermission(permission);
            saveAuthorization(authorization);
        }

        protected internal virtual void createRevokeAuthorization(Resources resource, string resourceId, string userId,
            params Permissions[] permissions)
        {
            var authorization = createRevokeAuthorization(resource, resourceId);
            authorization.UserId = userId;
            foreach (var permission in permissions)
                authorization.RemovePermission(permission);
            saveAuthorization(authorization);
        }

        protected internal virtual IAuthorization createGlobalAuthorization(Resources resource, string resourceId)
        {
            var authorization = createAuthorization(AuthorizationFields.AuthTypeGlobal, resource, resourceId);
            return authorization;
        }

        protected internal virtual IAuthorization createGrantAuthorization(Resources resource, string resourceId)
        {
            var authorization = createAuthorization(AuthorizationFields.AuthTypeGrant, resource, resourceId);
            return authorization;
        }

        protected internal virtual IAuthorization createRevokeAuthorization(Resources resource, string resourceId)
        {
            var authorization = createAuthorization(AuthorizationFields.AuthTypeRevoke, resource, resourceId);
            return authorization;
        }

        protected internal virtual IAuthorization createAuthorization(int type, Resources resource, string resourceId)
        {
            var authorization = authorizationService.CreateNewAuthorization(type);

            authorization.Resource = resource;
            if (!ReferenceEquals(resourceId, null))
                authorization.ResourceId = resourceId;

            return authorization;
        }

        protected internal virtual void saveAuthorization(IAuthorization authorization)
        {
            authorizationService.SaveAuthorization(authorization);
        }

        // enable/disable authorization //////////////////////////////////////////////

        protected internal virtual void enableAuthorization()
        {
            processEngineConfiguration.SetAuthorizationEnabled(true);
        }

        protected internal virtual void disableAuthorization()
        {
            processEngineConfiguration.SetAuthorizationEnabled(false);
        }

        // actions (executed without authorization) ///////////////////////////////////

        protected internal virtual IProcessInstance StartProcessInstanceByKey(string key)
        {
            return StartProcessInstanceByKey(key, null);
        }


        protected internal virtual IProcessInstance StartProcessInstanceByKey(string key,
            IDictionary<string, object> variables)
        {
            return runWithoutAuthorization(() => runtimeService.StartProcessInstanceByKey(key, variables));
        }


        public override void ExecuteAvailableJobs()
        {
            runWithoutAuthorization(() => ExecuteAvailableJobs());
        }


        protected internal virtual ICaseInstance createCaseInstanceByKey(string key)
        {
            return createCaseInstanceByKey(key, null);
        }

        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: protected ICaseInstance createCaseInstanceByKey(final String key, final java.util.Map<String, Object> variables)
        protected internal virtual ICaseInstance createCaseInstanceByKey(string key,
            IDictionary<string, object> variables)
        {
            return runWithoutAuthorization(() => caseService.CreateCaseInstanceByKey(key, variables));
        }


        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: protected void createTask(final String taskId)
        protected internal virtual void createTask(string taskId)
        {
            runWithoutAuthorization(() =>
            {
                var task = taskService.NewTask(taskId);
                taskService.SaveTask(task);
            });
        }


        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: protected void deleteTask(final String taskId, final boolean cascade)
        protected internal virtual void deleteTask(string taskId, bool cascade)
        {
            runWithoutAuthorization(() => taskService.DeleteTask(taskId, cascade));
        }


        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: protected void addCandidateUser(final String taskId, final String user)
        protected internal virtual void addCandidateUser(string taskId, string user)
        {
            runWithoutAuthorization(() => taskService.AddCandidateUser(taskId, user));
        }

        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: protected void addCandidateGroup(final String taskId, final String group)
        protected internal virtual void addCandidateGroup(string taskId, string group)
        {
            runWithoutAuthorization(() => taskService.AddCandidateGroup(taskId, group));
        }

        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: protected void setAssignee(final String taskId, final String userId)
        protected internal virtual void setAssignee(string taskId, string userId)
        {
            runWithoutAuthorization(() => { taskService.SetAssignee(taskId, userId); });
        }


        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: protected void delegateTask(final String taskId, final String userId)
        protected internal virtual void delegateTask(string taskId, string userId)
        {
            runWithoutAuthorization(() =>
                taskService.DelegateTask(taskId, userId)
            );
        }


        protected internal virtual ITask selectSingleTask()
        {
            return runWithoutAuthorization(() => taskService.CreateTaskQuery()
                .First());
        }


        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: protected void setTaskVariables(final String taskId, final java.util.Map<String, ? extends Object> variables)
        protected internal virtual void setTaskVariables(string taskId, IDictionary<string, object> variables)
        {
            runWithoutAuthorization(() => taskService.SetVariables(taskId, variables));
        }


        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: protected void setTaskVariablesLocal(final String taskId, final java.util.Map<String, ? extends Object> variables)
        protected internal virtual void setTaskVariablesLocal(string taskId, IDictionary<string, object> variables)
        {
            runWithoutAuthorization(() => taskService.SetVariablesLocal(taskId, variables));
        }


        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: protected void setTaskVariable(final String taskId, final String name, final Object value)
        protected internal virtual void setTaskVariable(string taskId, string name, object value)
        {
            runWithoutAuthorization(() => taskService.SetVariable(taskId, name, value));
        }


        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: protected void setTaskVariableLocal(final String taskId, final String name, final Object value)
        protected internal virtual void setTaskVariableLocal(string taskId, string name, object value)
        {
            runWithoutAuthorization(() => taskService.SetVariableLocal(taskId, name, value));
        }


        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: protected void setExecutionVariable(final String executionId, final String name, final Object value)
        protected internal virtual void setExecutionVariable(string executionId, string name, object value)
        {
            runWithoutAuthorization(() => runtimeService.SetVariable(executionId, name, value));
        }


        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: protected void setExecutionVariableLocal(final String executionId, final String name, final Object value)
        protected internal virtual void setExecutionVariableLocal(string executionId, string name, object value)
        {
            runWithoutAuthorization(() => runtimeService.SetVariableLocal(executionId, name, value));
        }


        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: protected void setCaseVariable(final String caseExecution, final String name, final Object value)
        protected internal virtual void setCaseVariable(string caseExecution, string name, object value)
        {
            runWithoutAuthorization(() => caseService.SetVariable(caseExecution, name, value));
        }


        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: protected void setCaseVariableLocal(final String caseExecution, final String name, final Object value)
        protected internal virtual void setCaseVariableLocal(string caseExecution, string name, object value)
        {
            runWithoutAuthorization(() => caseService.SetVariable(caseExecution, name, value));
        }


        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: protected IProcessDefinition selectProcessDefinitionByKey(final String processDefinitionKey)
        protected internal virtual IProcessDefinition selectProcessDefinitionByKey(string processDefinitionKey)
        {
            return
                runWithoutAuthorization(
                    () => repositoryService.CreateProcessDefinitionQuery(c => c.Key == processDefinitionKey)
                        .First());
        }


        protected internal virtual IProcessInstance selectSingleProcessInstance()
        {
            return runWithoutAuthorization(() => runtimeService.CreateProcessInstanceQuery()
                .First());
        }


        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: protected void suspendProcessDefinitionByKey(final String processDefinitionKey)
        protected internal virtual void suspendProcessDefinitionByKey(string processDefinitionKey)
        {
            runWithoutAuthorization(() => repositoryService.SuspendProcessDefinitionByKey(processDefinitionKey));
        }


        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: protected void suspendProcessDefinitionById(final String processDefinitionId)
        protected internal virtual void suspendProcessDefinitionById(string processDefinitionId)
        {
            runWithoutAuthorization(() => repositoryService.SuspendProcessDefinitionById(processDefinitionId));
        }


        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: protected void suspendProcessInstanceById(final String ProcessInstanceId)
        protected internal virtual void suspendProcessInstanceById(string ProcessInstanceId)
        {
            runWithoutAuthorization(() => runtimeService.SuspendProcessInstanceById(ProcessInstanceId));
        }


        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: protected void suspendJobById(final String jobId)
        protected internal virtual void suspendJobById(string jobId)
        {
            runWithoutAuthorization(() => managementService.SuspendJobById(jobId));
        }

        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: protected void suspendJobByProcessInstanceId(final String ProcessInstanceId)
        protected internal virtual void suspendJobByProcessInstanceId(string processInstanceId)
        {
            runWithoutAuthorization(() => managementService.SuspendJobByProcessInstanceId(processInstanceId));
        }


        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: protected void suspendJobByJobDefinitionId(final String jobDefinitionId)
        protected internal virtual void suspendJobByJobDefinitionId(string jobDefinitionId)
        {
            runWithoutAuthorization(() => managementService.SuspendJobByJobDefinitionId(jobDefinitionId));
        }


        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: protected void suspendJobByProcessDefinitionId(final String processDefinitionId)
        protected internal virtual void suspendJobByProcessDefinitionId(string processDefinitionId)
        {
            runWithoutAuthorization(() => managementService.SuspendJobByProcessDefinitionId(processDefinitionId));
        }


        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: protected void suspendJobByProcessDefinitionKey(final String processDefinitionKey)
        protected internal virtual void suspendJobByProcessDefinitionKey(string processDefinitionKey)
        {
            runWithoutAuthorization(() => managementService.SuspendJobByProcessDefinitionKey(processDefinitionKey));
        }


        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: protected void SuspendJobDefinitionById(final String jobDefinitionId)
        protected internal virtual void SuspendJobDefinitionById(string jobDefinitionId)
        {
            runWithoutAuthorization(() => managementService.SuspendJobDefinitionById(jobDefinitionId));
        }

        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: protected void SuspendJobDefinitionByProcessDefinitionId(final String processDefinitionId)
        protected internal virtual void SuspendJobDefinitionByProcessDefinitionId(string processDefinitionId)
        {
            runWithoutAuthorization(
                () => managementService.SuspendJobDefinitionByProcessDefinitionId(processDefinitionId));
        }


        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: protected void SuspendJobDefinitionByProcessDefinitionKey(final String processDefinitionKey)
        protected internal virtual void SuspendJobDefinitionByProcessDefinitionKey(string processDefinitionKey)
        {
            runWithoutAuthorization(
                () => managementService.SuspendJobDefinitionByProcessDefinitionKey(processDefinitionKey));
        }


        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: protected void suspendJobDefinitionIncludingJobsById(final String jobDefinitionId)
        protected internal virtual void suspendJobDefinitionIncludingJobsById(string jobDefinitionId)
        {
            runWithoutAuthorization(() => managementService.SuspendJobDefinitionById(jobDefinitionId, true));
        }


        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: protected void suspendJobDefinitionIncludingJobsByProcessDefinitionId(final String processDefinitionId)
        protected internal virtual void suspendJobDefinitionIncludingJobsByProcessDefinitionId(
            string processDefinitionId)
        {
            runWithoutAuthorization(
                () => managementService.SuspendJobDefinitionByProcessDefinitionId(processDefinitionId, true));
        }


        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: protected void suspendJobDefinitionIncludingJobsByProcessDefinitionKey(final String processDefinitionKey)
        protected internal virtual void suspendJobDefinitionIncludingJobsByProcessDefinitionKey(
            string processDefinitionKey)
        {
            runWithoutAuthorization(
                () => managementService.SuspendJobDefinitionByProcessDefinitionKey(processDefinitionKey, true));
        }


        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: protected Deployment createDeployment(final String name, final String.. resources)
        protected internal virtual IDeployment createDeployment(string name, params string[] resources)
        {
            return runWithoutAuthorization(() =>
            {
                var builder = repositoryService.CreateDeployment();
                foreach (var resource in resources)
                    builder.AddClasspathResource(resource);
                return builder.Deploy();
            });
        }


        protected internal virtual void DeleteDeployment(string deploymentId)
        {
            DeleteDeployment(deploymentId, true);
        }

        protected internal virtual void DeleteDeployment(string deploymentId, bool cascade)
        {
            var authentication = identityService.CurrentAuthentication;
            try
            {
                identityService.ClearAuthentication();
                runWithoutAuthorization(() => repositoryService.DeleteDeployment(deploymentId, cascade));
            }
            finally
            {
                if (authentication != null)
                    identityService.Authentication = authentication;
            }
        }

        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: protected IProcessInstance startProcessAndExecuteJob(final String key)
        protected internal virtual IProcessInstance startProcessAndExecuteJob(string key)
        {
            return runWithoutAuthorization(() =>
            {
                var processInstance = StartProcessInstanceByKey(key);
                executeAvailableJobs(key);
                return processInstance;
            });
        }


        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: protected void executeAvailableJobs(final String key)
        protected internal virtual void executeAvailableJobs(string key)
        {
            runWithoutAuthorization(() =>
            {
                IList<ESS.FW.Bpm.Engine.Runtime.IJob> jobs =
                    managementService.CreateJobQuery(c => c.ProcessDefinitionKey == key && c.Retries > 0)
                        .ToList();

                if (jobs.Count == 0)
                    enableAuthorization();

                foreach (var job in jobs)
                    try
                    {
                        managementService.ExecuteJob(job.Id);
                    }
                    catch (System.Exception)
                    {
                    }

                executeAvailableJobs(key);
            });
        }


        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: protected IDecisionDefinition selectDecisionDefinitionByKey(final String decisionDefinitionKey)
        protected internal virtual IDecisionDefinition selectDecisionDefinitionByKey(string decisionDefinitionKey)
        {
            return
                runWithoutAuthorization(
                    () =>
                        repositoryService.CreateDecisionDefinitionQuery(
                                c => c.PreviousDecisionDefinitionId == decisionDefinitionKey)
                            .First());
        }

        // verify query results ////////////////////////////////////////////////////////

        protected internal virtual void verifyQueryResults(IQueryable<IHistoricDecisionInstance> query,
            int countExpected)
        {
        }

        protected internal virtual void verifyQueryResults(IQueryable<IDecisionDefinition> query, int countExpected)
        {
        }

        //protected internal virtual void verifyQueryResults<T1, T2>(IQuery<T1, T2> query, int countExpected)
        //{
        //    Assert.AreEqual(countExpected, query.Count());
        //    Assert.AreEqual(countExpected, query.Count());

        //    if (countExpected == 1)
        //    {
        //        Assert.NotNull(query.First());
        //    }
        //    else if (countExpected > 1)
        //    {
        //        verifySingleResultFails(query);
        //    }
        //    else if (countExpected == 0)
        //    {
        //        Assert.IsNull(query.First());
        //    }
        //}

        //protected internal virtual void verifySingleResultFails<T1, T2>(IQuery<T1, T2> query)
        //{
        //    try
        //    {
        //        query.First();
        //        Assert.Fail();
        //    }
        //    catch (ProcessEngineException)
        //    {
        //    }
        //}

        public virtual Permissions DefaultTaskPermissionForUser
        {
            get
            {
                // get the default task assignee permission
                var processEngineConfiguration =
                    (ProcessEngineConfigurationImpl) ProcessEngine.ProcessEngineConfiguration;

                return processEngineConfiguration.DefaultUserPermissionForTask;
            }
        }

        // helper ////////////////////////////////////////////////////////////////////

        protected internal virtual IVariableMap Variables
        {
            get
            {
                return ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                    .PutValue(VARIABLE_NAME, VARIABLE_VALUE);
            }
        }
    }
}