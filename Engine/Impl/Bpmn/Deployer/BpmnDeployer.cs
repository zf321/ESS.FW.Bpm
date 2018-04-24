using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Helper;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Parser;
using ESS.FW.Bpm.Engine.Impl.Cmd;
using ESS.FW.Bpm.Engine.Impl.Core.Model;
using ESS.FW.Bpm.Engine.Impl.DB.EntityManager;
using ESS.FW.Bpm.Engine.Impl.Event;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;
using ESS.FW.Bpm.Engine.Impl.Pvm.Runtime;
using ESS.FW.Bpm.Engine.Management;
using ESS.FW.Bpm.Engine.Persistence.Deploy.Cache;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.DB.EntityManager.Cache;
using Microsoft.EntityFrameworkCore;
using ESS.FW.Bpm.Engine.Persistence.Entity.Impl;
using ESS.FW.DataAccess;
using Autofac.Features.Metadata;
using System.Linq;


namespace ESS.FW.Bpm.Engine.Impl.Bpmn.Deployer
{


    /// <summary>
    /// <seealso cref="Deployer"/> responsible to parse BPMN 2.0 XML files and create the proper
    /// <seealso cref="ESS.FW.Bpm.Engine.Persistence.Entity.ProcessDefinitionEntity"/>s. Overwrite this class if you want to gain some control over
    /// this mechanism, e.g. setting different version numbers, or you want to use your own <seealso cref="BpmnParser"/>.
    /// 
    /// </summary>
    public class BpmnDeployer : AbstractDefinitionDeployer<ProcessDefinitionEntity>
    {
        public static BpmnParseLogger Log = ProcessEngineLogger.BpmnParseLogger;

        public static readonly string[] BpmnResourceSuffixes = new string[] { "bpmn20.xml", "bpmn" };

        protected internal static readonly PropertyMapKey<string, IList<IJobDeclaration>> JobDeclarationsProperty = new PropertyMapKey<string, IList<IJobDeclaration>>("JOB_DECLARATIONS_PROPERTY");

        protected internal EL.ExpressionManager expressionManager;
        protected internal BpmnParser bpmnParser;

        /// <summary>
        /// <!> DON'T KEEP DEPLOYMENT-SPECIFIC STATE <!> </summary>

        protected internal override string[] ResourcesSuffixes
        {
            get
            {
                return BpmnResourceSuffixes;
            }
        }
        //解析xml 生成ProcessDefinitionEntity
        protected internal override IList<ProcessDefinitionEntity> TransformDefinitions(DeploymentEntity deployment, ResourceEntity resource, Core.Model.Properties properties)
        {
            byte[] bytes = resource.Bytes;
            System.IO.MemoryStream inputStream = new System.IO.MemoryStream(bytes);

            BpmnParse bpmnParse = bpmnParser.CreateBpmnParse().SourceInputStream(inputStream).Deployment(deployment).SetName(resource.Name);

            if (!deployment.IsValidatingSchema)
            {
                bpmnParse.SchemaResource = null;
            }
            //执行实际解析
            bpmnParse.Execute();

            //TODO 解析后续行为
            if(!properties.Contains(JobDeclarationsProperty))
            {
                properties.Set(JobDeclarationsProperty, new Dictionary<string, IList<IJobDeclaration>>());
            }
            var oldProperties = properties.Get(JobDeclarationsProperty);
            foreach (var item in bpmnParse.GetJobDeclarations())
            {
                oldProperties[item.Key] = item.Value;
            }
            //foreach (var item in oldProperties)
            //{
            //    bpmnParse.JobDeclarations[item.Key] = item.Value;
            //}
            //properties.Set<string, IList<IJobDeclaration>>(JobDeclarationsProperty, bpmnParse.JobDeclarations);

            return bpmnParse.ProcessDefinitions;
        }

        protected internal override  ProcessDefinitionEntity FindDefinitionByDeploymentAndKey(string deploymentId, string definitionKey)
        {
            return ProcessDefinitionManager.FindProcessDefinitionByDeploymentAndKey(deploymentId, definitionKey);
        }

        protected internal override  ProcessDefinitionEntity FindLatestDefinitionByKeyAndTenantId(string definitionKey, string tenantId)
        {
            return ProcessDefinitionManager.FindLatestProcessDefinitionByKeyAndTenantId(definitionKey, tenantId);
        }

        protected internal override  void PersistDefinition(ProcessDefinitionEntity definition)
        {
            ProcessDefinitionManager.InsertProcessDefinition(definition);
        }

        protected internal override void AddDefinitionToDeploymentCache(DeploymentCache deploymentCache, ProcessDefinitionEntity definition)
        {
            deploymentCache.AddProcessDefinition(definition);
        }
        protected internal override void DefinitionAddedToDeploymentCache(DeploymentEntity deployment, ProcessDefinitionEntity definition, Core.Model.Properties properties)
        {
            IList<IJobDeclaration> declarations;
            properties.Get(JobDeclarationsProperty).TryGetValue(definition.Key, out declarations);

            //var obj = properties.Get(JobDeclarationsProperty);
            //IList<IJobDeclaration> declarations =obj.ContainsKey(definition.Key)?obj[definition.Key]:null;

            UpdateJobDeclarations(declarations, definition, deployment.IsNew);

            ProcessDefinitionEntity latestDefinition = FindLatestDefinitionByKeyAndTenantId(definition.Key, definition.TenantId);

            if (deployment.IsNew)
            {
                AdjustStartEventSubscriptions(definition, latestDefinition);
            }

            // add "authorizations"
            AddAuthorizations(definition);
        }

        protected internal override void PersistedDefinitionLoaded(DeploymentEntity deployment, ProcessDefinitionEntity definition, ProcessDefinitionEntity persistedDefinition)
        {
            
            definition.SuspensionState = persistedDefinition.SuspensionState;
        }

        protected internal override void HandlePersistedDefinition(ESS.FW.Bpm.Engine.Persistence.Entity.ProcessDefinitionEntity definition, ESS.FW.Bpm.Engine.Persistence.Entity.ProcessDefinitionEntity persistedDefinition, DeploymentEntity deployment, Core.Model.Properties properties)
        {
            //check if persisted definition is not null, since the process definition can be deleted by the user
            //in such cases we don't want to handle them
            //we can't do this in the parent method, since other siblings want to handle them like {@link DecisionDefinitionDeployer}
            if (persistedDefinition != null)
            {
                base.HandlePersistedDefinition(definition, persistedDefinition, deployment, properties);
            }
        }

        protected internal virtual void UpdateJobDeclarations(IList<IJobDeclaration> jobDeclarations, ProcessDefinitionEntity processDefinition, bool isNewDeployment)
        {

            if (jobDeclarations == null || jobDeclarations.Count == 0)
            {
                return;
            }
            
            IJobDefinitionManager jobDefinitionManager = JobDefinitionManager;

            if (isNewDeployment)
            {
                // create new job definitions:
                foreach (IJobDeclaration jobDeclaration in jobDeclarations)
                {
                    CreateJobDefinition(processDefinition, jobDeclaration);
                }

            }
            else
            {
                // query all job definitions and update the declarations with their Ids
                IList<JobDefinitionEntity> existingDefinitions = jobDefinitionManager.FindByProcessDefinitionId(processDefinition.Id);

                LegacyBehavior.MigrateMultiInstanceJobDefinitions(processDefinition, existingDefinitions);
                
                foreach (IJobDeclaration jobDeclaration in jobDeclarations)
                {
                    bool jobDefinitionExists = false;
                    foreach (JobDefinitionEntity jobDefinitionEntity in existingDefinitions)
                    {

                        // <!> Assumption: there can be only one job definition per activity and type
                        if (jobDeclaration.ActivityId.Equals(jobDefinitionEntity.ActivityId) && jobDeclaration.JobHandlerType.Equals(jobDefinitionEntity.JobType))
                        {
                            jobDeclaration.JobDefinitionId = jobDefinitionEntity.Id;
                            jobDefinitionExists = true;
                            break;
                        }
                    }

                    if (!jobDefinitionExists)
                    {
                        // not found: create new definition
                        CreateJobDefinition(processDefinition, jobDeclaration);
                    }

                }
            }

        }

        protected internal virtual void CreateJobDefinition(IProcessDefinition processDefinition, IJobDeclaration jobDeclaration)
        {
            IJobDefinitionManager jobDefinitionManager = JobDefinitionManager;

            JobDefinitionEntity jobDefinitionEntity = JobDefinitionEntity.CreateJobDefinitionEntity(jobDeclaration);
            jobDefinitionEntity.ProcessDefinitionId = processDefinition.Id;
            jobDefinitionEntity.ProcessDefinitionKey = processDefinition.Key;
            jobDefinitionEntity.TenantId = processDefinition.TenantId;
            jobDefinitionManager.Add(jobDefinitionEntity);
            jobDeclaration.JobDefinitionId = jobDefinitionEntity.Id;
        }

        /// <summary>
        /// 调整负责启动流程实例的所有事件订阅（计时器启动事件、消息启动事件）。默认行为是删除旧订阅，并为新部署的流程定义添加新订阅。
        /// adjust all event subscriptions responsible to start process instances
        /// (timer start event, message start event). The default behavior is to remove the old
        /// subscriptions and add new ones for the new deployed process definitions.
        /// </summary>
        protected internal virtual void AdjustStartEventSubscriptions(ProcessDefinitionEntity newLatestProcessDefinition, ProcessDefinitionEntity oldLatestProcessDefinition)
        {
            RemoveObsoleteTimers(newLatestProcessDefinition);
            AddTimerDeclarations(newLatestProcessDefinition);

            RemoveObsoleteEventSubscriptions(newLatestProcessDefinition, oldLatestProcessDefinition);
            AddEventSubscriptions(newLatestProcessDefinition);
        }
        
        protected internal virtual void AddTimerDeclarations(ESS.FW.Bpm.Engine.Persistence.Entity.ProcessDefinitionEntity processDefinition)
        {
            IList<TimerDeclarationImpl> timerDeclarations = (IList<TimerDeclarationImpl>)processDefinition.GetProperty(BpmnParse.PropertynameStartTimer);
            if (timerDeclarations != null)
            {
                foreach (TimerDeclarationImpl timerDeclaration in timerDeclarations)
                {
                    string deploymentId = processDefinition.DeploymentId;
                    timerDeclaration.CreateStartTimerInstance(deploymentId);
                }
            }
        }

        protected internal virtual void RemoveObsoleteTimers(ESS.FW.Bpm.Engine.Persistence.Entity.ProcessDefinitionEntity processDefinition)
        {
            IList<JobEntity> jobsToDelete = JobManager.FindJobsByConfiguration(TimerStartEventJobHandler.TYPE, processDefinition.Key, processDefinition.TenantId);

            foreach (JobEntity job in jobsToDelete)
            {
                (new DeleteJobsCmd(job.Id)).Execute(Context.CommandContext);
            }
        }

        protected internal virtual void RemoveObsoleteEventSubscriptions(ESS.FW.Bpm.Engine.Persistence.Entity.ProcessDefinitionEntity processDefinition, ESS.FW.Bpm.Engine.Persistence.Entity.ProcessDefinitionEntity latestProcessDefinition)
        {
            // remove all subscriptions for the previous version
            if (latestProcessDefinition != null)
            {
                IEventSubscriptionManager eventSubscriptionManager = EventSubscriptionManager;

                IList<EventSubscriptionEntity> subscriptionsToDelete = new List<EventSubscriptionEntity>();

                IList<EventSubscriptionEntity> messageEventSubscriptions = eventSubscriptionManager.FindEventSubscriptionsByConfiguration(EventType.Message.Name, latestProcessDefinition.Id);
                ((List<EventSubscriptionEntity>)subscriptionsToDelete).AddRange(messageEventSubscriptions);

                IList<EventSubscriptionEntity> signalEventSubscriptions = eventSubscriptionManager.FindEventSubscriptionsByConfiguration(EventType.Signal.Name, latestProcessDefinition.Id);
                ((List<EventSubscriptionEntity>)subscriptionsToDelete).AddRange(signalEventSubscriptions);

                foreach (EventSubscriptionEntity eventSubscriptionEntity in subscriptionsToDelete)
                {
                    eventSubscriptionEntity.Delete();
                }
            }
        }

        protected internal virtual void AddEventSubscriptions(ESS.FW.Bpm.Engine.Persistence.Entity.ProcessDefinitionEntity processDefinition)
        {
            IDictionary<string, EventSubscriptionDeclaration> eventDefinitions = processDefinition.Properties.Get(BpmnProperties.EventSubscriptionDeclarations);
            foreach (EventSubscriptionDeclaration eventDefinition in eventDefinitions.Values)
            {
                AddEventSubscription(processDefinition, eventDefinition);
            }
        }

        protected internal virtual void AddEventSubscription(ESS.FW.Bpm.Engine.Persistence.Entity.ProcessDefinitionEntity processDefinition, EventSubscriptionDeclaration eventDefinition)
        {
            if (eventDefinition.StartEvent)
            {
                string eventType = eventDefinition.EventType;

                if (eventType==EventType.Message.Name)
                {
                    AddMessageStartEventSubscription(eventDefinition, processDefinition);
                }
                else if (eventType==EventType.Signal.Name)
                {
                    AddSignalStartEventSubscription(eventDefinition, processDefinition);
                }
            }
        }

        protected internal virtual void AddMessageStartEventSubscription(EventSubscriptionDeclaration messageEventDefinition, ESS.FW.Bpm.Engine.Persistence.Entity.ProcessDefinitionEntity processDefinition)
        {

            string tenantId = processDefinition.TenantId;

            if (IsSameMessageEventSubscriptionAlreadyPresent(messageEventDefinition, tenantId))
            {
                throw Log.MessageEventSubscriptionWithSameNameExists(processDefinition.ResourceName, messageEventDefinition.UnresolvedEventName);
            }
            EventSubscriptionEntity newSubscription = messageEventDefinition.CreateSubscriptionForStartEvent(processDefinition);
            newSubscription.Insert();

        }

        protected internal virtual bool IsSameMessageEventSubscriptionAlreadyPresent(EventSubscriptionDeclaration eventSubscription, string tenantId)
        {
            // look for subscriptions for the same name in db:
            IList<EventSubscriptionEntity> subscriptionsForSameMessageName = EventSubscriptionManager.FindEventSubscriptionsByNameAndTenantId(EventType.Message.Name, eventSubscription.UnresolvedEventName, tenantId);
            //TODO 缓存差异
            // also look for subscriptions created in the session: 源码有值
            IList<EventSubscriptionEntity> cachedSubscriptions = DbEntityCache.GetEntitiesByType<EventSubscriptionEntity>(typeof(EventSubscriptionEntity));

            foreach (EventSubscriptionEntity cachedSubscription in cachedSubscriptions)
            {
                //TODO HasTenantId subscriptions
                if (eventSubscription.UnresolvedEventName.Equals(cachedSubscription.EventName) && HasTenantId(cachedSubscription, tenantId) && !subscriptionsForSameMessageName.Contains(cachedSubscription))
                {

                    subscriptionsForSameMessageName.Add(cachedSubscription);
                }
            }

            // remove subscriptions deleted in the same command
            //subscriptionsForSameMessageName = DbEntityManager.PruneDeletedEntities(subscriptionsForSameMessageName);
            subscriptionsForSameMessageName = Context.CommandContext.PruneDeletedEntities<EventSubscriptionEntity>(subscriptionsForSameMessageName);
            
            // remove subscriptions for different type of event (i.e. remove intermediate message event subscriptions)
            subscriptionsForSameMessageName = FilterSubscriptionsOfDifferentType(eventSubscription, subscriptionsForSameMessageName);

            return subscriptionsForSameMessageName.Count > 0;
        }

        protected internal virtual bool HasTenantId(EventSubscriptionEntity cachedSubscription, string tenantId)
        {
            if (string.ReferenceEquals(tenantId, null))
            {
                return string.ReferenceEquals(cachedSubscription.TenantId, null);
            }
            else
            {
                return tenantId.Equals(cachedSubscription.TenantId);
            }
        }

        /// <summary>
        /// It is possible to deploy a process containing a start and intermediate
        /// message event that wait for the same message or to have two processes, one
        /// with a message start event and the other one with a message intermediate
        /// event, that subscribe for the same message. Therefore we have to find out
        /// if there are subscriptions for the other type of event and remove those.
        /// </summary>
        /// <param name="eventSubscription"> </param>
        /// <param name="subscriptionsForSameMessageName"> </param>
        protected internal virtual IList<EventSubscriptionEntity> FilterSubscriptionsOfDifferentType(EventSubscriptionDeclaration eventSubscription, IList<EventSubscriptionEntity> subscriptionsForSameMessageName)
        {
            List<EventSubscriptionEntity> filteredSubscriptions = new List<EventSubscriptionEntity>(subscriptionsForSameMessageName);

            foreach (EventSubscriptionEntity subscriptionEntity in new List<EventSubscriptionEntity>(subscriptionsForSameMessageName))
            {

                if (IsSubscriptionOfDifferentTypeAsDeclaration(subscriptionEntity, eventSubscription))
                {
                    filteredSubscriptions.Remove(subscriptionEntity);
                }
            }

            return filteredSubscriptions;
        }

        protected internal virtual bool IsSubscriptionOfDifferentTypeAsDeclaration(EventSubscriptionEntity subscriptionEntity, EventSubscriptionDeclaration declaration)
        {

            return (declaration.StartEvent && IsSubscriptionForIntermediateEvent(subscriptionEntity)) || (!declaration.StartEvent && IsSubscriptionForStartEvent(subscriptionEntity));
        }

        protected internal virtual bool IsSubscriptionForStartEvent(EventSubscriptionEntity subscriptionEntity)
        {
            return string.ReferenceEquals(subscriptionEntity.ExecutionId, null);
        }

        protected internal virtual bool IsSubscriptionForIntermediateEvent(EventSubscriptionEntity subscriptionEntity)
        {
            return !string.ReferenceEquals(subscriptionEntity.ExecutionId, null);
        }
        /// <summary>
        /// EventSubscriptionEntity
        /// </summary>
        /// <param name="signalEventDefinition"></param>
        /// <param name="processDefinition"></param>
        protected internal virtual void AddSignalStartEventSubscription(EventSubscriptionDeclaration signalEventDefinition, ESS.FW.Bpm.Engine.Persistence.Entity.ProcessDefinitionEntity processDefinition)
        {
            EventSubscriptionEntity newSubscription = signalEventDefinition.CreateSubscriptionForStartEvent(processDefinition);

            newSubscription.Insert();
        }

        public enum ExprType
        {
            User,
            Group

        }

        public virtual void AddAuthorizationsFromIterator(ISet<IExpression> exprSet, ESS.FW.Bpm.Engine.Persistence.Entity.ProcessDefinitionEntity processDefinition, ExprType exprType)
        {
            
            if (exprSet != null)
            {
                foreach (IExpression expr in exprSet)
                {

                    IdentityLinkEntity identityLink = new IdentityLinkEntity();
                    identityLink.ProcessDef = processDefinition;
                    if (exprType.Equals(ExprType.User))
                    {
                        identityLink.UserId = expr.ToString();
                    }
                    else if (exprType.Equals(ExprType.Group))
                    {
                        identityLink.GroupId = expr.ToString();
                    }
                    identityLink.Type = ESS.FW.Bpm.Engine.Task.IdentityLinkType.Candidate;
                    identityLink.Insert();
                }
            }
        }

        protected internal virtual void AddAuthorizations(ESS.FW.Bpm.Engine.Persistence.Entity.ProcessDefinitionEntity processDefinition)
        {
            
            AddAuthorizationsFromIterator(processDefinition.CandidateStarterUserIdExpressions, processDefinition, ExprType.User);
            AddAuthorizationsFromIterator(processDefinition.CandidateStarterGroupIdExpressions, processDefinition, ExprType.Group);
        }

        protected internal virtual void CreateResource(string name, byte[] bytes, DeploymentEntity deploymentEntity)
        {
            ResourceEntity resource = new ResourceEntity();
            resource.Name = name;
            resource.Bytes = bytes;
            resource.DeploymentId = deploymentEntity.Id;

            // Mark the resource as 'generated'
            resource.Generated = true;
            
            ResourceRepository.Add(resource);
        }



        // context ///////////////////////////////////////////////////////////////////////////////////////////
        
        protected IRepository<ResourceEntity,string> ResourceRepository
        {
            get
            {
                var reps = CommandContext.Scope.Resolve<IRepository<ResourceEntity, string>>();
                return reps;
            }
        }
        protected IRepository<JobDefinitionEntity,string> DbEntityManager
        {
            get { return CommandContext.GetDbEntityManager<JobDefinitionEntity>(); }
        }
        protected DbEntityCache DbEntityCache
        {
            get { return CommandContext.DbEntityCache; }
        }

        protected internal virtual IJobManager JobManager
        {
            get
            {
                return CommandContext.JobManager;
            }
        }

        protected internal virtual IJobDefinitionManager JobDefinitionManager
        {
            get
            {
                return CommandContext.JobDefinitionManager;
            }
        }

        protected internal virtual IEventSubscriptionManager EventSubscriptionManager
        {
            get
            {
                return CommandContext.EventSubscriptionManager;
            }
        }

        protected internal virtual IProcessDefinitionManager ProcessDefinitionManager
        {
            get
            {
                return CommandContext.ProcessDefinitionManager;
            }
        }

        // getters/setters ///////////////////////////////////////////////////////////////////////////////////

        public virtual EL.ExpressionManager ExpressionManager
        {
            get
            {
                return expressionManager;
            }
            set
            {
                this.expressionManager = value;
            }
        }


        public virtual BpmnParser BpmnParser
        {
            get
            {
                return bpmnParser;
            }
            set
            {
                this.bpmnParser = value;
            }
        }
    }
}

