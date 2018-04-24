using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Autofac.Features.AttributeFilters;

using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.DB;
using ESS.FW.Bpm.Engine.Impl.Identity;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using Microsoft.Extensions.Logging;
using ESS.FW.DataAccess;
using ESS.FW.Common.Components;

namespace ESS.FW.Bpm.Engine.Persistence.Entity.Impl
{

    /// <summary>
    /// 
    /// </summary>
    [Component]
    public class AuthorizationManager : AbstractManagerNet<AuthorizationEntity>, IAuthorizationManager
    {

        protected internal static readonly EnginePersistenceLogger Log = ProcessEngineLogger.PersistenceLogger;

        // Used instead of Collections.emptyList() as mybatis uses reflection to call methods
        // like size() which can lead to problems as Collections.EmptyList is a private implementation
        protected internal static readonly IList<string> EmptyList = new List<string>();

        /// <summary>
        /// Group ids for which authorizations exist in the database.
        /// This is initialized once per command by the <seealso cref="#filterAuthenticatedGroupIds(List)"/> method. (Manager
        /// instances are command scoped).
        /// It is used to only check authorizations for groups for which authorizations exist. In other words,
        /// if for a given group no authorization exists in the DB, then auth checks are not performed for this group.
        /// </summary>
        protected internal ISet<string> AvailableAuthorizedGroupIds = null;

        protected internal bool? IsRevokeAuthCheckUsed = null;

        public AuthorizationManager(DbContext dbContex, ILoggerFactory loggerFactory, IDGenerator idGenerator) : base(dbContex, loggerFactory, idGenerator)
        {
        }

        public virtual PermissionCheck NewPermissionCheck()
        {
            return new PermissionCheck();
        }

        public virtual PermissionCheckBuilder NewPermissionCheckBuilder()
        {
            return new PermissionCheckBuilder();
        }

        public virtual IAuthorization CreateNewAuthorization(int type)
        {
            CheckAuthorization(Permissions.Create, Resources.Authorization, null);
            return new AuthorizationEntity(type);
        }

        public override AuthorizationEntity Add(AuthorizationEntity authorization)
        {
            CheckAuthorization(Permissions.Create, Resources.Authorization, null);
            return base.Add(authorization);
        }

        //public virtual IList<IAuthorization> SelectAuthorizationByQueryCriteria(AuthorizationQueryImpl authorizationQuery)
        //{
        //    ConfigureQuery(authorizationQuery, Resources.Authorization);
        //    //return ListExt.ConvertToListT<IAuthorization>(Find("selectAuthorizationByQueryCriteria", authorizationQuery));
        //    //return Find(m=>m.)
        //    throw new NotImplementedException();
        //}

        //public virtual long? SelectAuthorizationCountByQueryCriteria(AuthorizationQueryImpl authorizationQuery)
        //{
        //    ConfigureQuery(authorizationQuery, Resources.Authorization);
        //    //return (long?)First("selectAuthorizationCountByQueryCriteria", authorizationQuery);
        //    throw new NotImplementedException();
        //}

        public virtual AuthorizationEntity FindAuthorizationByUserIdAndResourceId(int type, string userId, Resources resource, string resourceId)
        {
            return FindAuthorization(type, userId, null, resource, resourceId);
        }

        public virtual AuthorizationEntity FindAuthorizationByGroupIdAndResourceId(int type, string groupId, Resources resource, string resourceId)
        {
            return FindAuthorization(type, null, groupId, resource, resourceId);
        }

        public virtual AuthorizationEntity FindAuthorization(int type, string userId, string groupId, Resources resource, string resourceId)
        {
            //IDictionary<string, object> @params = new Dictionary<string, object>();

            //@params["type"] = type;
            //@params["userId"] = userId;
            //@params["groupId"] = groupId;
            //@params["resourceId"] = resourceId;

            //if (resource != null)
            //{
            //    @params["resourceType"] = (int)resource;
            //}
            if (resource != null)
            {
                return First(m => m.AuthorizationType == type && m.UserId == userId && m.GroupId == groupId && m.ResourceId == resourceId && m.ResourceType == resource);
            }
            //return (AuthorizationEntity)First("selectAuthorizationByParameters", @params);
            return First(m => m.AuthorizationType == type && m.UserId == userId && m.GroupId == groupId && m.ResourceId == resourceId);
        }

        public override AuthorizationEntity Update(AuthorizationEntity authorization)
        {
            CheckAuthorization(Permissions.Update, Resources.Authorization, authorization.Id);
            return base.Update(authorization);
        }

        public override void Delete(AuthorizationEntity authorization)
        {
            CheckAuthorization(Permissions.Delete, Resources.Authorization, authorization.Id);
            DeleteAuthorizationsByResourceId(Resources.Authorization, authorization.Id);
            base.Delete(authorization);
        }

        // authorization checks ///////////////////////////////////////////

        public virtual void CheckAuthorization(params PermissionCheck[] permissionChecks)
        {
            EnsureUtil.EnsureNotNull("permissionChecks", permissionChecks);
            foreach (PermissionCheck permissionCheck in permissionChecks)
            {
                EnsureUtil.EnsureNotNull("permissionCheck", permissionCheck);
            }

            CheckAuthorization(permissionChecks.ToList());
        }

        public virtual void CheckAuthorization(CompositePermissionCheck compositePermissionCheck)
        {
            if (AuthCheckExecuted)
            {

                Authentication currentAuthentication = CurrentAuthentication;
                string userId = currentAuthentication.UserId;

                bool isAuthorized = IsAuthorized(compositePermissionCheck);
                if (!isAuthorized)
                {

                    IList<MissingAuthorization> missingAuthorizations = new List<MissingAuthorization>();

                    foreach (PermissionCheck check in compositePermissionCheck.AllPermissionChecks)
                    {
                        missingAuthorizations.Add(new MissingAuthorization(check.Permission.ToString(), check.Resource.ToString(), check.ResourceId));
                    }

                    throw new AuthorizationException(userId, missingAuthorizations);
                }
            }
        }

        public virtual void CheckAuthorization(IList<PermissionCheck> permissionChecks)
        {
            if (AuthCheckExecuted)
            {

                Authentication currentAuthentication = CurrentAuthentication;
                string userId = currentAuthentication.UserId;
                bool isAuthorized = IsAuthorized(userId, currentAuthentication.GroupIds, permissionChecks);
                if (!isAuthorized)
                {

                    IList<MissingAuthorization> missingAuthorizations = new List<MissingAuthorization>();

                    foreach (PermissionCheck check in permissionChecks)
                    {
                        missingAuthorizations.Add(new MissingAuthorization(check.Permission.ToString(), check.Resource.ToString(), check.ResourceId));
                    }

                    throw new AuthorizationException(userId, missingAuthorizations);
                }
            }
        }


        public virtual void CheckAuthorization(Permissions permission, Resources resource)
        {
            CheckAuthorization(permission, resource, null);
        }

        public  override void CheckAuthorization(Permissions permission, Resources resource, string resourceId)
        {
            if (AuthCheckExecuted)
            {
                Authentication currentAuthentication = CurrentAuthentication;
                bool isAuthorized = IsAuthorized(currentAuthentication.UserId, currentAuthentication.GroupIds, permission, resource, resourceId);
                if (!isAuthorized)
                {
                    throw new AuthorizationException(currentAuthentication.UserId, permission.ToString(), resource.ToString(), resourceId);
                }
            }

        }

        public virtual bool IsAuthorized(Permissions permission, Resources resource, string resourceId)
        {
            // this will be called by LdapIdentityProviderSession#isAuthorized() for executing LdapQueries.
            // to be backward compatible a check whether authorization has been enabled inside the given
            // command context will not be done.
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final org.camunda.bpm.engine.impl.identity.Authentication currentAuthentication = getCurrentAuthentication();
            Authentication currentAuthentication = CurrentAuthentication;

            if (AuthorizationEnabled && currentAuthentication != null && currentAuthentication.UserId != null)
            {
                return IsAuthorized(currentAuthentication.UserId, currentAuthentication.GroupIds, permission, resource, resourceId);

            }
            else
            {
                return true;

            }
        }

        public virtual bool IsAuthorized(string userId, IList<string> groupIds, Permissions permission, Resources resource, string resourceId)
        {
            PermissionCheck permCheck = NewPermissionCheck();
            permCheck.Permission = permission;
            permCheck.Resource = resource;
            permCheck.ResourceId = resourceId;

            List<PermissionCheck> permissionChecks = new List<PermissionCheck>();
            permissionChecks.Add(permCheck);

            return IsAuthorized(userId, groupIds, permissionChecks);
        }

        public virtual bool IsAuthorized(string userId, IList<string> groupIds, IList<PermissionCheck> permissionChecks)
        {
            if (!AuthorizationEnabled)
            {
                return true;
            }

            IList<string> filteredGroupIds = FilterAuthenticatedGroupIds(groupIds);

            bool isRevokeAuthorizationCheckEnabled = IsRevokeAuthCheckEnabled(userId, groupIds) ?? false;
            AuthorizationCheck authCheck = new AuthorizationCheck(userId, filteredGroupIds, permissionChecks, isRevokeAuthorizationCheckEnabled);
            //return DbEntityManager.SelectBoolean("isUserAuthorizedForResource", authCheck);
            throw new NotImplementedException();
        }

        protected internal virtual bool? IsRevokeAuthCheckEnabled(string userId, IList<string> groupIds)
        {
            bool? isRevokeAuthCheckEnabled = this.IsRevokeAuthCheckUsed;

            if (isRevokeAuthCheckEnabled == null)
            {
                string configuredMode = context.Impl.Context.ProcessEngineConfiguration.AuthorizationCheckRevokes;
                if (configuredMode != null)
                {
                    configuredMode = configuredMode.ToLower();
                }
                // if (ProcessEngineConfiguration.Resource.Authorization_CHECK_REVOKE_ALWAYS.Equals(configuredMode))
                // {
                //isRevokeAuthCheckEnabled = true;
                // }
                // else if (ProcessEngineConfiguration.Resource.Authorization_CHECK_REVOKE_NEVER.Equals(configuredMode))
                // {
                //isRevokeAuthCheckEnabled = false;
                // }
                else
                {
                    //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                    //ORIGINAL LINE: final java.Util.Map<String, Object> params = new java.Util.HashMap<String, Object>();
                    //IDictionary<string, object> @params = new Dictionary<string, object>();
                    //@params["userId"] = userId;
                    //@params["authGroupIds"] = FilterAuthenticatedGroupIds(groupIds);
                    //isRevokeAuthCheckEnabled = DbEntityManager.SelectBoolean("selectRevokeAuthorization", @params);
                   var authGroupIds= FilterAuthenticatedGroupIds(groupIds);
                    isRevokeAuthCheckEnabled = Count(m => m.UserId == userId && authGroupIds.Contains(m.GroupId)) > 0;
                }
                this.IsRevokeAuthCheckUsed = isRevokeAuthCheckEnabled;
            }

            return isRevokeAuthCheckEnabled;
        }

        public virtual bool IsAuthorized(string userId, IList<string> groupIds, CompositePermissionCheck compositePermissionCheck)
        {
            //IList<string> filteredGroupIds = FilterAuthenticatedGroupIds(groupIds);

            //bool isRevokeAuthorizationCheckEnabled = IsRevokeAuthCheckEnabled(userId, groupIds);
            //AuthorizationCheck authCheck = new AuthorizationCheck(userId, filteredGroupIds, compositePermissionCheck, isRevokeAuthorizationCheckEnabled);
            //return DbEntityManager.SelectBoolean("isUserAuthorizedForResource", authCheck);
            throw new NotImplementedException();
        }

        public virtual bool IsAuthorized(CompositePermissionCheck compositePermissionCheck)
        {
            Authentication currentAuthentication = CurrentAuthentication;

            if (currentAuthentication != null)
            {
                return IsAuthorized(currentAuthentication.UserId, currentAuthentication.GroupIds, compositePermissionCheck);
            }
            else
            {
                return true;
            }
        }

        // authorization checks on queries ////////////////////////////////

        //public virtual void ConfigureQuery(ListQueryParameterObject query)
        //{

        //    AuthorizationCheck authCheck = query.AuthCheck;
        //    authCheck.PermissionChecks.Clear();

        //    if (AuthCheckExecuted)
        //    {
        //        Authentication currentAuthentication = CurrentAuthentication;
        //        authCheck.AuthUserId = currentAuthentication.UserId;
        //        authCheck.AuthGroupIds = currentAuthentication.GroupIds;
        //        EnableQueryAuthCheck(authCheck);
        //    }
        //    else
        //    {
        //        authCheck.AuthorizationCheckEnabled = false;
        //        authCheck.AuthUserId = null;
        //        authCheck.AuthGroupIds = null;
        //    }
        //}

        public virtual void EnableQueryAuthCheck(AuthorizationCheck authCheck)
        {
            IList<string> authGroupIds = authCheck.AuthGroupIds;
            string authUserId = authCheck.AuthUserId;

            authCheck.AuthorizationCheckEnabled = true;
            authCheck.AuthGroupIds = FilterAuthenticatedGroupIds(authGroupIds);
            authCheck.RevokeAuthorizationCheckEnabled = IsRevokeAuthCheckEnabled(authUserId, authGroupIds) ?? false;
        }

        //protected internal override void ConfigureQuery(ListQueryParameterObject query, Resources resource)
        //{
        //    ConfigureQuery(query, resource, "RES.ID_");
        //}

        //public virtual void ConfigureQuery(ListQueryParameterObject query, Resources resource, string queryParam)
        //{
        //    ConfigureQuery(query, resource, queryParam, Permissions.Read);
        //}

        //public virtual void ConfigureQuery(ListQueryParameterObject query, Resources resource, string queryParam, Permissions permission)
        //{
        //    ConfigureQuery(query);
        //    AddPermissionCheck(query, resource, queryParam, permission);
        //}

        //protected internal virtual void AddPermissionCheck(ListQueryParameterObject query,Resources resource, string queryParam, Permissions permission)
        //{
        //    CommandContext commandContext = CommandContext;
        //    if (AuthorizationEnabled && CurrentAuthentication != null && commandContext.AuthorizationCheckEnabled)
        //    {
        //        PermissionCheck permCheck = NewPermissionCheck();
        //        permCheck.Resource = resource;
        //        permCheck.ResourceIdQueryParam = queryParam;
        //        permCheck.Permission = permission;

        //        query.AuthCheck.AddAtomicPermissionCheck(permCheck);
        //    }
        //}

        protected internal virtual void AddPermissionCheck(AuthorizationCheck authCheck, CompositePermissionCheck compositeCheck)
        {
            CommandContext commandContext = CommandContext;
            if (AuthorizationEnabled && CurrentAuthentication != null && commandContext.AuthorizationCheckEnabled)
            {
                authCheck.PermissionChecks = compositeCheck;
            }
        }

        // delete authorizations //////////////////////////////////////////////////

        public virtual void DeleteAuthorizationsByResourceId(Resources resource, string resourceId)
        {

            if (resourceId == null)
            {
                throw new System.ArgumentException("Resource id cannot be null");
            }

            if (AuthorizationEnabled)
            {
                //IDictionary<string, object> deleteParams = new Dictionary<string, object>();
                //deleteParams["resourceType"] = (int)resource;
                //deleteParams["resourceId"] = resourceId;
                //DbEntityManager.Delete(typeof(AuthorizationEntity), "deleteAuthorizationsForResourceId", deleteParams);
                Delete(m => m.ResourceType == resource && m.ResourceId == resourceId);
            }
        }

        public virtual void DeleteAuthorizationsByResourceIdAndUserId(Resources resource, string resourceId, string userId)
        {

            if (resourceId == null)
            {
                throw new System.ArgumentException("Resource id cannot be null");
            }

            if (AuthorizationEnabled)
            {
                IDictionary<string, object> deleteParams = new Dictionary<string, object>();
                deleteParams["resourceType"] = (int)resource;
                deleteParams["resourceId"] = resourceId;
                deleteParams["userId"] = userId;
                //DbEntityManager.Delete(typeof(AuthorizationEntity), "deleteAuthorizationsForResourceId", deleteParams);
                Delete(m => m.Resource == resource && m.ResourceId == resourceId && m.UserId == userId);
            }
        }

        public virtual void DeleteAuthorizationsByResourceIdAndGroupId(Resources resource, string resourceId, string groupId)
        {
            if (resourceId == null)
            {
                throw new System.ArgumentException("Resource id cannot be null");
            }

            if (AuthorizationEnabled)
            {
                IDictionary<string, object> deleteParams = new Dictionary<string, object>();
                deleteParams["resourceType"] = (int)resource;
                deleteParams["resourceId"] = resourceId;
                deleteParams["groupId"] = groupId;
                //DbEntityManager.Delete(typeof(AuthorizationEntity), "deleteAuthorizationsForResourceId", deleteParams);
                Delete(m => m.ResourceType == resource && m.ResourceId == resourceId && m.GroupId == groupId);
            }
        }

        // predefined authorization checks

        /* MEMBER OF CAMUNDA_ADMIN */

        /// <summary>
        /// Checks if the current authentication contains the group
        /// <seealso cref="IGroups#CAMUNDA_ADMIN"/>. The check is ignored if the authorization is
        /// disabled or no authentication exists.
        /// </summary>
        /// <exception cref="AuthorizationException"> </exception>
        public virtual void CheckCamundaAdmin()
        {
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final org.camunda.bpm.engine.impl.identity.Authentication currentAuthentication = getCurrentAuthentication();
            Authentication currentAuthentication = CurrentAuthentication;
            CommandContext commandContext = context.Impl.Context.CommandContext;

            if (AuthorizationEnabled && commandContext.AuthorizationCheckEnabled && currentAuthentication != null && !IsCamundaAdmin(currentAuthentication))
            {

                throw Log.RequiredCamundaAdminException();
            }
        }

        /// <param name="authentication">
        ///          authentication to check, cannot be <code>null</code> </param>
        /// <returns> <code>true</code> if the given authentication contains the group
        ///         <seealso cref="IGroups#CAMUNDA_ADMIN"/> </returns>
        public virtual bool IsCamundaAdmin(Authentication authentication)
        {
            IList<string> groupIds = authentication.GroupIds;
            if (groupIds != null)
            {
                return groupIds.Contains(GroupsFields.CamundaAdmin);
            }
            else
            {
                return false;
            }
        }

        /* QUERIES */

        // deployment query ////////////////////////////////////////

        //public virtual void ConfigureDeploymentQuery(DeploymentQueryImpl query)
        //{
        //    ConfigureQuery(query, Resources.Deployment);
        //}

        // process definition query ////////////////////////////////

        //public virtual void ConfigureProcessDefinitionQuery(ProcessDefinitionQueryImpl query)
        //{
        //    ConfigureQuery(query, Resources.ProcessDefinition, "RES.KEY_");
        //}

        // execution/process instance query ////////////////////////

        //public virtual void ConfigureExecutionQuery(ListQueryParameterObject query)
        //{
        //    ConfigureQuery(query);
        //    AddPermissionCheck(query, Resources.ProcessInstance, "RES.PROC_INST_ID_",Permissions.Read);
        //    AddPermissionCheck(query, Resources.ProcessDefinition, "P.KEY_",Permissions.ReadInstance);
        //}

        // task query //////////////////////////////////////////////

        //public virtual void ConfigureTaskQuery(TaskQueryImpl query)
        //{
        //    ConfigureQuery(query);

        //    if (query.AuthCheck.AuthorizationCheckEnabled)
        //    {

        //        // necessary authorization check when the task is part of
        //        // a running process instance

        //        CompositePermissionCheck permissionCheck = (new PermissionCheckBuilder()).Disjunctive().AtomicCheck(Resources.Task, "RES.ID_", Permissions.Read).AtomicCheck(Resources.ProcessDefinition, "PROCDEF.KEY_", Permissions.ReadTask).Build();
        //        AddPermissionCheck(query.AuthCheck, permissionCheck);
        //    }
        //}

        // event subscription query //////////////////////////////

        //public virtual void ConfigureEventSubscriptionQuery(EventSubscriptionQueryImpl query)
        //{
        //    ConfigureQuery(query);
        //    AddPermissionCheck(query, Resources.ProcessInstance, "RES.PROC_INST_ID_", Permissions.Read);
        //    AddPermissionCheck(query, Resources.ProcessDefinition, "PROCDEF.KEY_", Permissions.ReadInstance);
        //}

        // incident query ///////////////////////////////////////

        //public virtual void ConfigureIncidentQuery(IncidentQueryImpl query)
        //{
        //    ConfigureQuery(query);
        //    AddPermissionCheck(query, Resources.ProcessInstance, "RES.PROC_INST_ID_", Permissions.Read);
        //    AddPermissionCheck(query, Resources.ProcessDefinition, "PROCDEF.KEY_", Permissions.ReadInstance);
        //}

        // variable instance query /////////////////////////////

        //protected internal virtual void ConfigureVariableInstanceQuery(VariableInstanceQueryImpl query)
        //{
        //    ConfigureQuery(query);

        //    if (query.AuthCheck.AuthorizationCheckEnabled)
        //    {
        //        CompositePermissionCheck permissionCheck = (new PermissionCheckBuilder()).Disjunctive().AtomicCheck(Resources.ProcessInstance, "RES.PROC_INST_ID_", Permissions.Read).AtomicCheck(Resources.ProcessDefinition, "PROCDEF.KEY_", Permissions.ReadInstance).AtomicCheck(Resources.Task, "RES.TASK_ID_", Permissions.Read).Build();
        //        AddPermissionCheck(query.AuthCheck, permissionCheck);
        //    }
        //}

        // job definition query ////////////////////////////////////////////////

        //public virtual void ConfigureJobDefinitionQuery(JobDefinitionQueryImpl query)
        //{
        //    ConfigureQuery(query, Resources.ProcessDefinition, "RES.PROC_DEF_KEY_");
        //}

        //// job query //////////////////////////////////////////////////////////

        //public virtual void ConfigureJobQuery(JobQueryImpl query)
        //{
        //    ConfigureQuery(query);
        //    AddPermissionCheck(query, Resources.ProcessInstance, "RES.PROCESS_INSTANCE_ID_", Permissions.Read);
        //    AddPermissionCheck(query, Resources.ProcessDefinition, "RES.PROCESS_DEF_KEY_", Permissions.ReadInstance);
        //}

        ///* HISTORY */

        //// historic process instance query ///////////////////////////////////

        //public virtual void ConfigureHistoricProcessInstanceQuery(HistoricProcessInstanceQueryImpl query)
        //{
        //    ConfigureQuery(query, Resources.ProcessDefinition, "SELF.PROC_DEF_KEY_", Permissions.ReadHistory);
        //}

        //// historic activity instance query /////////////////////////////////

        //public virtual void ConfigureHistoricActivityInstanceQuery(HistoricActivityInstanceQueryImpl query)
        //{
        //    ConfigureQuery(query, Resources.ProcessDefinition, "SELF.PROC_DEF_KEY_", Permissions.ReadHistory);
        //}

        //// historic task instance query ////////////////////////////////////

        //public virtual void ConfigureHistoricTaskInstanceQuery(HistoricTaskInstanceQueryImpl query)
        //{
        //    ConfigureQuery(query, Resources.ProcessDefinition, "SELF.PROC_DEF_KEY_", Permissions.ReadHistory);
        //}

        //// historic variable instance query ////////////////////////////////

        //public virtual void ConfigureHistoricVariableInstanceQuery(HistoricVariableInstanceQueryImpl query)
        //{
        //    ConfigureQuery(query, Resources.ProcessDefinition, "SELF.PROC_DEF_KEY_", Permissions.ReadHistory);
        //}

        //// historic detail query ////////////////////////////////

        //public virtual void ConfigureHistoricDetailQuery(HistoricDetailQueryImpl query)
        //{
        //    ConfigureQuery(query, Resources.ProcessDefinition, "SELF.PROC_DEF_KEY_", Permissions.ReadHistory);
        //}

        //// historic job log query ////////////////////////////////

        //public virtual void ConfigureHistoricJobLogQuery(HistoricJobLogQueryImpl query)
        //{
        //    ConfigureQuery(query, Resources.ProcessDefinition, "SELF.PROCESS_DEF_KEY_", Permissions.ReadHistory);
        //}

        //// historic incident query ////////////////////////////////

        //public virtual void ConfigureHistoricIncidentQuery(HistoricIncidentQueryImpl query)
        //{
        //    ConfigureQuery(query, Resources.ProcessDefinition, "SELF.PROC_DEF_KEY_", Permissions.ReadHistory);
        //}

        ////historic identity link query ////////////////////////////////

        //public virtual void ConfigureHistoricIdentityLinkQuery(HistoricIdentityLinkLogQueryImpl query)
        //{
        //    ConfigureQuery(query, Resources.ProcessDefinition, "SELF.PROC_DEF_KEY_", Permissions.ReadHistory);
        //}

        //public virtual void ConfigureHistoricDecisionInstanceQuery(HistoricDecisionInstanceQueryImpl query)
        //{
        //    ConfigureQuery(query, Resources.DecisionDefinition, "SELF.DEC_DEF_KEY_", Permissions.ReadHistory);
        //}

        //// historic external task log query /////////////////////////////////

        //public virtual void ConfigureHistoricExternalTaskLogQuery(HistoricExternalTaskLogQueryImpl query)
        //{
        //    ConfigureQuery(query, Resources.ProcessDefinition, "SELF.PROC_DEF_KEY_", Permissions.ReadHistory);
        //}

        //// user operation log query ///////////////////////////////

        //public virtual void ConfigureUserOperationLogQuery(UserOperationLogQueryImpl query)
        //{
        //    ConfigureQuery(query, Resources.ProcessDefinition, "SELF.PROC_DEF_KEY_", Permissions.ReadHistory);
        //}

        //// batch

        //public virtual void ConfigureHistoricBatchQuery(HistoricBatchQueryImpl query)
        //{
        //    ConfigureQuery(query, Resources.Batch, "RES.ID_", Permissions.ReadHistory);
        //}

        ///* STATISTICS QUERY */

        //public virtual void ConfigureDeploymentStatisticsQuery(DeploymentStatisticsQueryImpl query)
        //{
        //    ConfigureQuery(query, Resources.Deployment, "RES.ID_");

        //    query.ProcessInstancePermissionChecks.Clear();
        //    query.JobPermissionChecks.Clear();
        //    query.IncidentPermissionChecks.Clear();

        //    if (query.AuthCheck.AuthorizationCheckEnabled)
        //    {

        //        PermissionCheck firstProcessInstancePermissionCheck = NewPermissionCheck();
        //        firstProcessInstancePermissionCheck.Resource = Resources.ProcessInstance;
        //        firstProcessInstancePermissionCheck.Permission = Permissions.Read;
        //        firstProcessInstancePermissionCheck.ResourceIdQueryParam = "EXECUTION.PROC_INST_ID_";

        //        PermissionCheck secondProcessInstancePermissionCheck = NewPermissionCheck();
        //        secondProcessInstancePermissionCheck.Resource = Resources.ProcessDefinition;
        //        secondProcessInstancePermissionCheck.Permission = Permissions.ReadInstance;
        //        secondProcessInstancePermissionCheck.ResourceIdQueryParam = "PROCDEF.KEY_";
        //        secondProcessInstancePermissionCheck.AuthorizationNotFoundReturnValue = 0L;

        //        query.AddProcessInstancePermissionCheck(firstProcessInstancePermissionCheck);
        //        query.AddProcessInstancePermissionCheck(secondProcessInstancePermissionCheck);

        //        if (query.FailedJobsToInclude)
        //        {
        //            PermissionCheck firstJobPermissionCheck = NewPermissionCheck();
        //            firstJobPermissionCheck.Resource = Resources.ProcessInstance;
        //            firstJobPermissionCheck.Permission = Permissions.Read;
        //            firstJobPermissionCheck.ResourceIdQueryParam = "JOB.PROCESS_INSTANCE_ID_";

        //            PermissionCheck secondJobPermissionCheck = NewPermissionCheck();
        //            secondJobPermissionCheck.Resource = Resources.ProcessDefinition;
        //            secondJobPermissionCheck.Permission = Permissions.ReadInstance;
        //            secondJobPermissionCheck.ResourceIdQueryParam = "JOB.PROCESS_DEF_KEY_";
        //            secondJobPermissionCheck.AuthorizationNotFoundReturnValue = 0L;

        //            query.AddJobPermissionCheck(firstJobPermissionCheck);
        //            query.AddJobPermissionCheck(secondJobPermissionCheck);
        //        }

        //        if (query.IncidentsToInclude)
        //        {
        //            PermissionCheck firstIncidentPermissionCheck = NewPermissionCheck();
        //            firstIncidentPermissionCheck.Resource = Resources.ProcessInstance;
        //            firstIncidentPermissionCheck.Permission = Permissions.Read;
        //            firstIncidentPermissionCheck.ResourceIdQueryParam = "INC.PROC_INST_ID_";

        //            PermissionCheck secondIncidentPermissionCheck = NewPermissionCheck();
        //            secondIncidentPermissionCheck.Resource = Resources.ProcessDefinition;
        //            secondIncidentPermissionCheck.Permission = Permissions.ReadInstance;
        //            secondIncidentPermissionCheck.ResourceIdQueryParam = "PROCDEF.KEY_";
        //            secondIncidentPermissionCheck.AuthorizationNotFoundReturnValue = 0L;

        //            query.AddIncidentPermissionCheck(firstIncidentPermissionCheck);
        //            query.AddIncidentPermissionCheck(secondIncidentPermissionCheck);

        //        }
        //    }
        //}

        //public virtual void ConfigureProcessDefinitionStatisticsQuery(ProcessDefinitionStatisticsQueryImpl query)
        //{
        //    ConfigureQuery(query, Resources.ProcessDefinition, "RES.KEY_");
        //}

        //public virtual void ConfigureActivityStatisticsQuery(ActivityStatisticsQueryImpl query)
        //{
        //    ConfigureQuery(query);

        //    query.ProcessInstancePermissionChecks.Clear();
        //    query.JobPermissionChecks.Clear();
        //    query.IncidentPermissionChecks.Clear();

        //    if (query.AuthCheck.AuthorizationCheckEnabled)
        //    {

        //        PermissionCheck firstProcessInstancePermissionCheck = NewPermissionCheck();
        //        firstProcessInstancePermissionCheck.Resource = Resources.ProcessInstance;
        //        firstProcessInstancePermissionCheck.Permission = Permissions.Read;
        //        firstProcessInstancePermissionCheck.ResourceIdQueryParam = "E.PROC_INST_ID_";

        //        PermissionCheck secondProcessInstancePermissionCheck = NewPermissionCheck();
        //        secondProcessInstancePermissionCheck.Resource = Resources.ProcessDefinition;
        //        secondProcessInstancePermissionCheck.Permission = Permissions.ReadInstance;
        //        secondProcessInstancePermissionCheck.ResourceIdQueryParam = "P.KEY_";
        //        secondProcessInstancePermissionCheck.AuthorizationNotFoundReturnValue = 0L;

        //        query.AddProcessInstancePermissionCheck(firstProcessInstancePermissionCheck);
        //        query.AddProcessInstancePermissionCheck(secondProcessInstancePermissionCheck);

        //        if (query.FailedJobsToInclude)
        //        {
        //            PermissionCheck firstJobPermissionCheck = NewPermissionCheck();
        //            firstJobPermissionCheck.Resource =Resources.ProcessInstance;
        //            firstJobPermissionCheck.Permission = Permissions.Read;
        //            firstJobPermissionCheck.ResourceIdQueryParam = "JOB.PROCESS_INSTANCE_ID_";

        //            PermissionCheck secondJobPermissionCheck = NewPermissionCheck();
        //            secondJobPermissionCheck.Resource = Resources.ProcessDefinition;
        //            secondJobPermissionCheck.Permission = Permissions.ReadInstance;
        //            secondJobPermissionCheck.ResourceIdQueryParam = "JOB.PROCESS_DEF_KEY_";
        //            secondJobPermissionCheck.AuthorizationNotFoundReturnValue = 0L;

        //            query.AddJobPermissionCheck(firstJobPermissionCheck);
        //            query.AddJobPermissionCheck(secondJobPermissionCheck);
        //        }

        //        if (query.IncidentsToInclude)
        //        {
        //            PermissionCheck firstIncidentPermissionCheck = NewPermissionCheck();
        //            firstIncidentPermissionCheck.Resource = Resources.ProcessInstance;
        //            firstIncidentPermissionCheck.Permission = Permissions.Read;
        //            firstIncidentPermissionCheck.ResourceIdQueryParam = "I.PROC_INST_ID_";

        //            PermissionCheck secondIncidentPermissionCheck = NewPermissionCheck();
        //            secondIncidentPermissionCheck.Resource = Resources.ProcessDefinition;
        //            secondIncidentPermissionCheck.Permission = Permissions.ReadInstance;
        //            secondIncidentPermissionCheck.ResourceIdQueryParam = "PROCDEF.KEY_";
        //            secondIncidentPermissionCheck.AuthorizationNotFoundReturnValue = 0L;

        //            query.AddIncidentPermissionCheck(firstIncidentPermissionCheck);
        //            query.AddIncidentPermissionCheck(secondIncidentPermissionCheck);

        //        }
        //    }
        //}

        //public virtual void ConfigureExternalTaskQuery(ExternalTaskQueryImpl query)
        //{
        //    ConfigureQuery(query);
        //    AddPermissionCheck(query,Resources.ProcessInstance, "RES.PROC_INST_ID_", Permissions.Read);
        //    AddPermissionCheck(query, Resources.ProcessDefinition, "RES.PROC_DEF_KEY_", Permissions.ReadInstance);
        //}

        //public virtual void ConfigureExternalTaskFetch(ListQueryParameterObject parameter)
        //{
        //    ConfigureQuery(parameter);

        //    CompositePermissionCheck permissionCheck = NewPermissionCheckBuilder().Conjunctive().Composite().Disjunctive().AtomicCheck(Resources.ProcessInstance, "RES.PROC_INST_ID_",Permissions.Read).AtomicCheck(Resources.ProcessDefinition, "RES.PROC_DEF_KEY_", Permissions.ReadInstance).Done().Composite().Disjunctive().AtomicCheck(Resources.ProcessInstance, "RES.PROC_INST_ID_", Permissions.Update).AtomicCheck(Resources.ProcessDefinition, "RES.PROC_DEF_KEY_", Permissions.UpdateInstance).Done().Build();

        //    AddPermissionCheck(parameter.AuthCheck, permissionCheck);
        //}

        //public virtual void ConfigureDecisionDefinitionQuery(DecisionDefinitionQueryImpl query)
        //{
        //    ConfigureQuery(query, Resources.DecisionDefinition, "RES.KEY_");
        //}

        //public virtual void ConfigureDecisionRequirementsDefinitionQuery(DecisionRequirementsDefinitionQueryImpl query)
        //{
        //    ConfigureQuery(query, Resources.DecisionRequirementsDefinition , "RES.KEY_");
        //}

        //public virtual void ConfigureBatchQuery(BatchQueryImpl query)
        //{
        //    ConfigureQuery(query);
        //    AddPermissionCheck(query,Resources.Batch, "RES.ID_",Permissions.Read);
        //}

        //public virtual void ConfigureBatchStatisticsQuery(BatchStatisticsQueryImpl query)
        //{
        //    ConfigureQuery(query);
        //    AddPermissionCheck(query, Resources.Batch, "RES.ID_", Permissions.Read);
        //}

        public virtual IList<string> FilterAuthenticatedGroupIds(IList<string> authenticatedGroupIds)
        {
            throw new NotImplementedException();
            //if (authenticatedGroupIds == null || authenticatedGroupIds.Count == 0)
            //{
            //    return EmptyList;
            //}
            //else
            //{
            //    if (AvailableAuthorizedGroupIds == null)
            //    {
            //        AvailableAuthorizedGroupIds = new HashSet<string>(Find("selectAuthorizedGroupIds"));
            //    }
            //    ISet<string> copy = new HashSet<string>(AvailableAuthorizedGroupIds);
            //    copy.retainAll(authenticatedGroupIds);
            //    return new List<string>(copy);
            //}
        }

        protected internal virtual bool AuthCheckExecuted
        {
            get
            {
                Authentication currentAuthentication = CurrentAuthentication;
                CommandContext commandContext = context.Impl.Context.CommandContext;

                return AuthorizationEnabled && commandContext.AuthorizationCheckEnabled && currentAuthentication != null && currentAuthentication.UserId != null;

            }
        }

    }

}