using System;
using System.Collections.Generic;
using System.Text;
using ESS.FW.Bpm.Engine.Application;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.exception;
using ESS.FW.Bpm.Engine.History.Impl;
using ESS.FW.Bpm.Engine.Impl.DB.EntityManager.Cache;
using ESS.FW.Bpm.Engine.Impl.DB.EntityManager.Operation;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Variable.Type;
using ESS.FW.Bpm.Engine.Variable.Value;
using ESS.FW.Bpm.Model.Xml.instance;
using ESS.Shared.Entities.Bpm;

//using JZTERP.Common.Shared.Entities.Bpm.Entities;


namespace ESS.FW.Bpm.Engine.Impl.DB
{
    
    

    /// <summary>
    ///     .
    /// </summary>
    public class EnginePersistenceLogger : ProcessEngineLogger
    {
        protected internal static readonly string HintText =
            "Hint: Set <property name=\"databaseSchemaUpdate\" to value=\"true\" or " +
            "value=\"create-drop\" (use create-drop for testing only!) in bean " +
            "processEngineConfiguration in camunda.cfg.xml for automatic schema creation";

        protected internal virtual string BuildStringFromList<T1>(ICollection<T1> list)
        {
            var message = new StringBuilder();
            message.Append("[");
            message.Append("\n");
            foreach (object @object in list)
            {
                message.Append("  ");
                message.Append(@object);
                message.Append("\n");
            }
            message.Append("]");

            return message.ToString();
        }

        private string BuildStringFromMap<T1>(IDictionary<string, T1> map)
        {
            var message = new StringBuilder();
            message.Append("[");
            message.Append("\n");
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: for(java.Util.Map.Entry<String, object> entry : map.entrySet())
            foreach (var entry in map)
            {
                message.Append("  ");
                message.Append(entry.Key);
                message.Append(": ");
                message.Append(entry.Value);
                message.Append("\n");
            }
            message.Append("]");
            return message.ToString();
        }

        public virtual ProcessEngineException EntityCacheLookupException<T>(Type type, string id, Type entity,
            System.Exception cause) where T : IDbEntity
        {
            return
                new ProcessEngineException(
                    ExceptionMessage("001",
                        "Could not lookup entity of type '{0}' and id '{1}': found entity of type '{2}'.", type, id, entity),
                    cause);
        }

        public virtual ProcessEngineException EntityCacheDuplicateEntryException(string currentState, string id,
            Type entityClass, DbEntityState foundState)
        {
            return
                new ProcessEngineException(ExceptionMessage("002",
                    "Cannot add {0} entity with id '{1}' and type '{2}' into cache. An entity with the same id and type is already in state '{3}'",
                    currentState, id, entityClass, foundState));
        }

        public virtual ProcessEngineException AlreadyMarkedEntityInEntityCacheException(string id, Type entityClass,
            DbEntityState state)
        {
            return
                new ProcessEngineException(ExceptionMessage("003",
                    "Inserting an entity with Id '{0}' and type '{1}' which is already marked with state '{2}'", id,
                    entityClass, state));
        }

        public virtual ProcessEngineException FlushDbOperationException(IList<DbOperation> operationsToFlush,
            DbOperation operation, System.Exception cause)
        {
            return
                new ProcessEngineException(
                    ExceptionMessage("004",
                        "Exception while executing Database Operation '{0}' with message '{1}'. Flush summary: \n {2}",
                        operation.ToString(), cause.Message, BuildStringFromList(operationsToFlush)), cause);
        }

        public virtual OptimisticLockingException ConcurrentUpdateDbEntityException(DbOperation operation)
        {
            return
                new OptimisticLockingException(ExceptionMessage("005",
                    "Execution of '{0}' failed. Entity was updated by another transaction concurrently.", operation));
        }

        public virtual void FlushedCacheState(IList<CachedDbEntity> cachedEntities)
        {
            if (DebugEnabled)
            {
                LogDebug("006", "Cache state after flush: {0}", BuildStringFromList(cachedEntities));
            }
        }

        public virtual ProcessEngineException MergeDbEntityException(IDbEntity entity)
        {
            return new ProcessEngineException(ExceptionMessage("007", "Cannot merge DbEntity '{0}' without id", entity));
        }

        public virtual void DatabaseFlushSummary(ICollection<DbOperation> operations)
        {
            if (DebugEnabled)
            {
                LogDebug("008", "Flush Summary: {0}", BuildStringFromList(operations));
            }
        }

        public virtual void ExecuteDatabaseOperation(string operationType, object parameter)
        {
            if (DebugEnabled)
            {
                string message;
                if (parameter != null)
                {
                    message = parameter.ToString();
                }
                else
                {
                    message = "null";
                }

                if (parameter is IDbEntity)
                {
                    var dbEntity = (IDbEntity) parameter;
                    message = ClassNameUtil.GetClassNameWithoutPackage(dbEntity) + "[id=" + dbEntity.Id + "]";
                }

                LogDebug("009", "SQL operation: '{0}'; Entity: '{1}'", operationType, message);
            }
        }

        public virtual void ExecuteDatabaseBulkOperation(string operationType, string statement, object parameter)
        {
            LogDebug("010", "SQL bulk operation: '{0}'; Statement: '{1}'; Parameter: '{2}'", operationType, statement,
                parameter);
        }

        public virtual void FetchDatabaseTables(string source, IList<string> tableNames)
        {
            if (DebugEnabled)
            {
                LogDebug("011", "Retrieving process engine tables from: '{0}'. Retrieved tables: {1}", source,
                    BuildStringFromList(tableNames));
            }
        }

        public virtual void MissingSchemaResource(string resourceName, string operation)
        {
            LogDebug("012", "There is no schema resource '{0}' for operation '{1}'.", resourceName, operation);
        }

        public virtual ProcessEngineException MissingSchemaResourceException(string resourceName, string operation)
        {
            return
                new ProcessEngineException(ExceptionMessage("013",
                    "There is no schema resource '{0}' for operation '{1}'.", resourceName, operation));
        }

        public virtual ProcessEngineException MissingSchemaResourceFileException(string fileName, System.Exception cause)
        {
            return
                new ProcessEngineException(
                    ExceptionMessage("014", "Cannot find schema resource file with name '{0}'", fileName), cause);
        }

        public virtual void FailedDatabaseOperation(string operation, string statement, System.Exception cause)
        {
            LogError("015", "Problem during schema operation '{0}' with statement '{1}'. Cause: '{2}'", operation,
                statement, cause.Message);
        }

        public virtual void PerformingDatabaseOperation(string operation, string component, string resourceName)
        {
            LogInfo("016", "Performing database operation '{0}' on component '{1}' with resource '{2}'", operation,
                component, resourceName);
        }

        public virtual void SuccessfulDatabaseOperation(string operation, string component)
        {
            LogDebug("Database schema operation '{0}' for component '{1}' was successful.", operation, component);
        }

        public virtual ProcessEngineException PerformDatabaseOperationException(string operation, string sql,
            System.Exception cause)
        {
            return
                new ProcessEngineException(
                    ExceptionMessage("017",
                        "Could not perform operation '{0}' on database schema for SQL Statement: '{1}'.", operation, sql),
                    cause);
        }

        public virtual ProcessEngineException CheckDatabaseTableException(System.Exception cause)
        {
            return
                new ProcessEngineException(
                    ExceptionMessage("018", "Could not check if tables are already present using metadata."), cause);
        }

        public virtual ProcessEngineException GetDatabaseTableNameException(System.Exception cause)
        {
            return new ProcessEngineException(ExceptionMessage("019", "Unable to fetch process engine table names."),
                cause);
        }

        public virtual ProcessEngineException MissingRelationMappingException(string relation)
        {
            return
                new ProcessEngineException(ExceptionMessage("020",
                    "There is no mapping for the relation '{0}' registered.", relation));
        }

        public virtual ProcessEngineException DatabaseHistoryLevelException(string level)
        {
            return
                new ProcessEngineException(ExceptionMessage("021",
                    "historyLevel '{0}' is higher then 'none' and dbHistoryUsed is set to false.", level));
        }

        public virtual ProcessEngineException InvokeSchemaResourceToolException(int length)
        {
            return
                new ProcessEngineException(ExceptionMessage("022",
                    "Schema resource tool was invoked with '{0}' parameters." +
                    "Schema resource tool must be invoked with exactly 2 parameters:" +
                    "\n - 1st parameter is the process engine configuration file," +
                    "\n - 2nd parameter is the schema resource file name", length));
        }

        public virtual ProcessEngineException LoadModelException(string type, string modelName, string id,
            System.Exception cause)
        {
            return
                new ProcessEngineException(
                    ExceptionMessage("023", "Could not load {0} Model for {1} definition with id '{2}'.", type, modelName,
                        id), cause);
        }

        public virtual void RemoveEntryFromDeploymentCacheFailure(string modelName, string id, System.Exception cause)
        {
            LogWarn("024", "Could not remove {0} definition with id '{1}' from the cache. Reason: '{2}'", modelName, id,
                cause.Message, cause);
        }


        public virtual ProcessEngineException EngineAuthorizationTypeException(int usedType, int global, int grant,
            int revoke)
        {
            return
                new ProcessEngineException(ExceptionMessage("025",
                    "Unrecognized authorization type '{0}'. Must be one of ['{1}', '{2}', '{3}']", usedType, global, grant,
                    revoke));
        }

        public virtual InvalidOperationException PermissionStateException(string methodName, string type)
        {
            return
                new InvalidOperationException(ExceptionMessage("026",
                    "MethodInfo '{0}' cannot be used for authorization with type '{1}'.", methodName, type));
        }

        public virtual ProcessEngineException NotUsableGroupIdForGlobalAuthorizationException()
        {
            return new ProcessEngineException(ExceptionMessage("027", "Cannot use 'groupId' for GLOBAL authorization"));
        }

        public virtual ProcessEngineException IllegalValueForUserIdException(string id, string expected)
        {
            return
                new ProcessEngineException(ExceptionMessage("028",
                    "Illegal value '{0}' for userId for GLOBAL authorization. Must be '{1}'", id, expected));
        }

        public virtual AuthorizationException RequiredCamundaAdminException()
        {
            return
                new AuthorizationException(ExceptionMessage("029", "Required authenticated group '{0}'.",
                    GroupsFields.CamundaAdmin));
        }

        public virtual void CreateChildExecution(ExecutionEntity child, ExecutionEntity parent)
        {
            if (DebugEnabled)
            {
                LogDebug("030", "Child execution '{0}' created with parent '{1}'.", child.ToString(), parent.ToString());
            }
        }

        public virtual void InitializeExecution(ExecutionEntity entity)
        {
            LogDebug("031", "Initializing execution '{0}'", entity.ToString());
        }

        public virtual void InitializeTimerDeclaration(ExecutionEntity entity)
        {
            LogDebug("032", "Initializing timer declaration '{0}'", entity.ToString());
        }

        public virtual ProcessEngineException RequiredAsyncContinuationException(string id)
        {
            return
                new ProcessEngineException(ExceptionMessage("033",
                    "Asynchronous Continuation for activity with id '{0}' requires a message job declaration", id));
        }

        public virtual ProcessEngineException RestoreProcessInstanceException(ExecutionEntity entity)
        {
            return
                new ProcessEngineException(ExceptionMessage("034",
                    "Can only restore process instances. This method must be called on a process instance execution but was called on '{0}'",
                    entity.ToString()));
        }

        public virtual ProcessEngineException ExecutionNotFoundException(string id)
        {
            return new ProcessEngineException(ExceptionMessage("035", "Unable to find execution for id '{0}'", id));
        }

        public virtual ProcessEngineException CastModelInstanceException(IModelElementInstance instance,
            string toElement, string type, string @namespace, System.Exception cause)
        {
            return
                new ProcessEngineException(
                    ExceptionMessage("036", "Cannot cast '{0}' to '{1}'. Element is of type '{2}' with namespace '{3}'.",
                        instance, toElement, type, @namespace), cause);
        }

        public virtual BadUserRequestException RequestedProcessInstanceNotFoundException(string id)
        {
            return new BadUserRequestException(ExceptionMessage("037", "No process instance found for id '{0}'", id));
        }

        public virtual NotValidException QueryExtensionException(string extendedClassName, string extendingClassName)
        {
            return
                new NotValidException(ExceptionMessage("038",
                    "Unable to extend a query of class '{0}' by a query of class '{1}'.", extendedClassName,
                    extendingClassName));
        }

        public virtual ProcessEngineException UnsupportedResourceTypeException(string type)
        {
            return new ProcessEngineException(ExceptionMessage("039", "Unsupported resource type '{0}'", type));
        }

        public virtual ProcessEngineException SerializerNotDefinedException(object entity)
        {
            return
                new ProcessEngineException(ExceptionMessage("040", "No serializer defined for variable instance '{0}'",
                    entity));
        }

        public virtual ProcessEngineException SerializerOutOfContextException()
        {
            return
                new ProcessEngineException(ExceptionMessage("041",
                    "Cannot work with serializers outside of command context."));
        }

        public virtual ProcessEngineException TaskIsAlreadyAssignedException(string usedId, string foundId)
        {
            return
                new ProcessEngineException(ExceptionMessage("042",
                    "Cannot assign '{0}' to a ITask assignment that has already '{1}' set.", usedId, foundId));
        }

        public virtual SuspendedEntityInteractionException SuspendedEntityException(string type, string id)
        {
            return
                new SuspendedEntityInteractionException(ExceptionMessage("043", "{0} with id '{1}' is suspended.", type,
                    id));
        }

        public virtual void LogUpdateUnrelatedProcessDefinitionEntity(string thisKey, string thatKey,
            string thisDeploymentId, string thatDeploymentId)
        {
            LogDebug("044",
                "Cannot update entity from an unrelated process definition: this key '{0}', that key '{1}', this deploymentId '{2}', that deploymentId '{3}'",
                thisKey, thatKey, thisDeploymentId, thatDeploymentId);
        }

        public virtual ProcessEngineException ToManyProcessDefinitionsException(int count, string key, int? version,
            string tenantId)
        {
            return
                new ProcessEngineException(ExceptionMessage("045",
                    "There are '{0}' results for a process definition with key '{1}', version '{2}' and tenant-id '{3}'.",
                    count, key, version,tenantId));
        }

        public virtual ProcessEngineException NotAllowedIdException(string id)
        {
            return
                new ProcessEngineException(ExceptionMessage("046",
                    "Cannot set id '{0}'. Only the provided id generation is allowed for properties.", id));
        }

        public virtual void CountRowsPerProcessEngineTable(IDictionary<string, long> map)
        {
            if (DebugEnabled)
            {
                LogDebug("047", "decimal of rows per process engine table: {0}", BuildStringFromMap(map));
            }
        }

        public virtual ProcessEngineException CountTableRowsException(System.Exception cause)
        {
            return new ProcessEngineException(ExceptionMessage("048", "Could not fetch table counts."), cause);
        }

        public virtual void SelectTableCountForTable(string name)
        {
            LogDebug("049", "Selecting table count for table with name '{0}'", name);
        }

        public virtual ProcessEngineException RetrieveMetadataException(System.Exception cause)
        {
            return
                new ProcessEngineException(
                    ExceptionMessage("050", "Could not retrieve database metadata. Reason: '{0}'", cause.Message), cause);
        }

        public virtual ProcessEngineException InvokeTaskListenerException(System.Exception cause)
        {
            return
                new ProcessEngineException(
                    ExceptionMessage("051", "There was an exception while invoking the TaskListener. Message: '{0}'",
                        cause.Message), cause);
        }

        public virtual BadUserRequestException UninitializedFormKeyException()
        {
            return
                new BadUserRequestException(ExceptionMessage("052",
                    "The form key is not initialized. You must call initializeFormKeys() on the ITask query before you can " +
                    "retrieve the form key."));
        }

        public virtual ProcessEngineException DisabledHistoryException()
        {
            return new ProcessEngineException(ExceptionMessage("053", "History is not enabled."));
        }

        public virtual ProcessEngineException InstantiateSessionException(string name, System.Exception cause)
        {
            return
                new ProcessEngineException(
                    ExceptionMessage("054", "Could not instantiate class '{0}'. Message: '{1}'", name, cause.Message),
                    cause);
        }

        public virtual WrongDbException WrongDbVersionException(string version, string dbVersion)
        {
            return
                new WrongDbException(
                    ExceptionMessage("055",
                        "Version mismatch: Camunda library version is '{0}' and db version is '{1}'. " + HintText,
                        version, dbVersion), version, dbVersion);
        }

        public virtual ProcessEngineException MissingTableException(IList<string> components)
        {
            return
                new ProcessEngineException(ExceptionMessage("056", "Tables are missing for the following components: {0}",
                    BuildStringFromList(components)));
        }

        public virtual ProcessEngineException MissingActivitiTablesException()
        {
            return
                new ProcessEngineException(ExceptionMessage("057",
                    "There are no Camunda tables in the database. " + HintText));
        }

        public virtual ProcessEngineException UnableToFetchDbSchemaVersion(System.Exception cause)
        {
            return new ProcessEngineException(ExceptionMessage("058", "Could not fetch the database schema version."),
                cause);
        }

        public virtual void FailedTofetchVariableValue(System.Exception cause)
        {
            LogDebug("059", "Could not fetch value for variable.", cause);
        }

        public virtual ProcessEngineException HistoricDecisionInputInstancesNotFetchedException()
        {
            return
                new ProcessEngineException(ExceptionMessage("060",
                    "The input instances for the historic decision instance are not fetched. You must call 'includeInputs()' on the query to enable fetching."));
        }

        public virtual ProcessEngineException HistoricDecisionOutputInstancesNotFetchedException()
        {
            return
                new ProcessEngineException(ExceptionMessage("061",
                    "The output instances for the historic decision instance are not fetched. You must call 'includeOutputs()' on the query to enable fetching."));
        }

        public virtual void ExecutingDdl(IList<string> logLines)
        {
            if (DebugEnabled)
            {
                LogDebug("062", "Executing Schmema DDL {0}", BuildStringFromList(logLines));
            }
        }

        public virtual ProcessEngineException CollectResultValueOfUnsupportedTypeException(ITypedValue collectResultValue)
        {
            return
                new ProcessEngineException(ExceptionMessage("063",
                    "The collect result value '{0}' of the decision table result is not of type integer, long or double.",
                    collectResultValue));
        }

        public virtual ProcessEngineException UpdateTransientVariableException(string variableName)
        {
            return
                new ProcessEngineException(ExceptionMessage("064",
                    "The variable with name '{0}' can not be updated because it is transient and read-only.",
                    variableName));
        }

        public virtual void CreatingHistoryLevelPropertyInDatabase(IHistoryLevel historyLevel)
        {
            LogInfo("065", "Creating historyLevel property in database for level: {0}", historyLevel);
        }

        public virtual void CouldNotSelectHistoryLevel(string message)
        {
            LogWarn("066", "Could not select history level property: {0}", message);
        }

        public virtual void NoHistoryLevelPropertyFound()
        {
            LogInfo("067", "No history level property found in database");
        }

        public virtual void NoDeploymentLockPropertyFound()
        {
            LogError("068", "No deployment lock property found in databse");
        }

        public virtual void DebugJobExecuted(JobEntity jobEntity)
        {
            LogDebug("069", "Job executed, deleting it", jobEntity);
        }

        public virtual ProcessEngineException MultipleTenantsForProcessDefinitionKeyException(
            string processDefinitionKey)
        {
            return
                new ProcessEngineException(ExceptionMessage("070",
                    "Cannot resolve a unique process definition for key '{0}' because it exists for multiple tenants.",
                    processDefinitionKey));
        }

        //public virtual ProcessEngineException CannotDeterminePaDataformats(ProcessApplicationUnavailableException e)
        //{
        //    return
        //        new ProcessEngineException(
        //            ExceptionMessage("071",
        //                "Cannot determine process application variable serializers. Context Process Application is unavailable."),
        //            e);
        //}
        public virtual ProcessEngineException CannotDeterminePaDataformats(System.Exception e)
        {
            return
                new ProcessEngineException(
                    ExceptionMessage("071",
                        "Cannot determine process application variable serializers. Context Process Application is unavailable."),
                    e);
        }

        public virtual ProcessEngineException CannotChangeTenantIdOfTask(string taskId, string currentTenantId,
            string tenantIdToSet)
        {
            return
                new ProcessEngineException(ExceptionMessage("072",
                    "Cannot change tenantId of ITask '{0}'. Current tenant id '{1}', Tenant id to set '{2}'", taskId,
                    currentTenantId, tenantIdToSet));
        }

        public virtual ProcessEngineException CannotSetDifferentTenantIdOnSubtask(string parentTaskId, string tenantId,
            string tenantIdToSet)
        {
            return
                new ProcessEngineException(ExceptionMessage("073",
                    "Cannot set different tenantId on subtask than on parent ITask. Parent taskId: '{0}', tenantId: '{1}', tenant id to set '{2}'",
                    parentTaskId, tenantId, tenantIdToSet));
        }

        public virtual ProcessEngineException MultipleTenantsForDecisionDefinitionKeyException(
            string decisionDefinitionKey)
        {
            return
                new ProcessEngineException(ExceptionMessage("074",
                    "Cannot resolve a unique decision definition for key '{0}' because it exists for multiple tenants.",
                    decisionDefinitionKey));
        }

        public virtual ProcessEngineException MultipleTenantsForCaseDefinitionKeyException(string caseDefinitionKey)
        {
            return
                new ProcessEngineException(ExceptionMessage("075",
                    "Cannot resolve a unique case definition for key '{0}' because it exists for multiple tenants.",
                    caseDefinitionKey));
        }

        public virtual ProcessEngineException DeleteProcessDefinitionWithProcessInstancesException(
            string processDefinitionId, long? processInstanceCount)
        {
            return
                new ProcessEngineException(ExceptionMessage("076",
                    "Deletion of process definition without cascading failed. Process definition with id: {0} can't be deleted, since there exists {1} dependening process instances.",
                    processDefinitionId, processInstanceCount));
        }

        public virtual ProcessEngineException ResolveParentOfExecutionFailedException(string parentId,
            string executionId)
        {
            return
                new ProcessEngineException(ExceptionMessage("077",
                    $"Cannot resolve parent with id '{parentId}' of execution '{executionId}', perhaps it was deleted in the meantime",
                    parentId, executionId));
        }
    }
}