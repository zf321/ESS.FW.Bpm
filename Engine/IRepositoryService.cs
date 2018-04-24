using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ESS.FW.Bpm.Engine.Application;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Dmn.Impl.Entity.Repository;
using ESS.FW.Bpm.Engine.exception;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Task;
using ESS.FW.Bpm.Model.Bpmn;
using ESS.FW.Bpm.Model.Cmmn;
using ESS.FW.Bpm.Model.Dmn;

namespace ESS.FW.Bpm.Engine
{
    


    /// <summary>
    /// Service providing access to the repository of process definitions and deployments.
    /// 
    /// </summary>
    public interface IRepositoryService
    {

        /// <summary>
        /// Starts creating a new deployment
        /// </summary>
        IDeploymentBuilder CreateDeployment();

        /// <summary>
        /// Starts creating a new <seealso cref="ProcessApplicationDeployment"/>.
        /// </summary>
        /// <seealso cref= ProcessApplicationDeploymentBuilder </seealso>
        IProcessApplicationDeploymentBuilder CreateDeployment(IProcessApplicationReference processApplication);

        /// <summary>
        /// Deletes the given deployment.
        /// </summary>
        /// <param name="deploymentId"> id of the deployment, cannot be null.
        /// </param>
        /// <exception cref="RuntimeException">
        ///          If there are still runtime or history process instances or jobs. </exception>
        /// <exception cref="AuthorizationException">
        ///          If the user has no <seealso cref="Permissions#DELETE"/> permission on <seealso cref="Resources#DEPLOYMENT"/>. </exception>
        void DeleteDeployment(string deploymentId);

        /// <summary>
        /// Deletes the given deployment and cascade deletion to process instances,
        /// history process instances and jobs.
        /// </summary>
        /// <param name="deploymentId"> id of the deployment, cannot be null.
        /// </param>
        /// <exception cref="AuthorizationException">
        ///          If the user has no <seealso cref="Permissions#DELETE"/> permission on <seealso cref="Resources#DEPLOYMENT"/>.
        /// </exception>
        /// @deprecated use <seealso cref="#deleteDeployment(String, boolean)"/>. This methods may be deleted from 5.3. 
        [Obsolete("use <seealso cref='deleteDeployment(String, boolean)'/>. This methods may be deleted from 5.3.")]
	    void DeleteDeploymentCascade(string deploymentId);

        /// <summary>
        /// Deletes the given deployment and cascade deletion to process instances,
        /// history process instances and jobs.
        /// </summary>
        /// <param name="deploymentId"> id of the deployment, cannot be null.
        /// </param>
        /// <exception cref="AuthorizationException">
        ///          If the user has no <seealso cref="Permissions#DELETE"/> permission on <seealso cref="Resources#DEPLOYMENT"/>. </exception>
        void DeleteDeployment(string deploymentId, bool cascade);

        /// <summary>
        /// Deletes the given deployment and cascade deletion to process instances,
        /// history process instances and jobs.
        /// </summary>
        /// <param name="deploymentId"> id of the deployment, cannot be null. </param>
        /// <param name="cascade"> if set to true, all process instances (including) history are deleted </param>
        /// <param name="skipCustomListeners"> if true, only the built-in <seealso cref="ExecutionListener"/>s
        /// are notified with the <seealso cref="ExecutionListener#EVENTNAME_END"/> event.
        /// </param>
        /// <exception cref="AuthorizationException">
        ///          If the user has no <seealso cref="Permissions#DELETE"/> permission on <seealso cref="Resources#DEPLOYMENT"/>. </exception>
        void DeleteDeployment(string deploymentId, bool cascade, bool skipCustomListeners);

        /// <summary>
        /// Deletes the given deployment and cascade deletion to process instances,
        /// history process instances and jobs.
        /// </summary>
        /// <param name="deploymentId"> id of the deployment, cannot be null. </param>
        /// <param name="cascade"> if set to true, all process instances (including) history are deleted </param>
        /// <param name="skipCustomListeners"> if true, only the built-in <seealso cref="ExecutionListener"/>s
        /// are notified with the <seealso cref="ExecutionListener#EVENTNAME_END"/> event. </param>
        /// <param name="skipIoMappings"> specifies whether input/output mappings for tasks should be invoked
        /// </param>
        /// <exception cref="AuthorizationException">
        ///          If the user has no <seealso cref="Permissions#DELETE"/> permission on <seealso cref="Resources#DEPLOYMENT"/>. </exception>
        void DeleteDeployment(string deploymentId, bool cascade, bool skipCustomListeners, bool skipIoMappings);


        /// <summary>
        /// Deletes the process definition which belongs to the given process definition id.
        /// Same behavior as <seealso cref="RepositoryService#deleteProcessDefinition(java.lang.String, boolean, boolean)"/>
        /// Both boolean parameters of this method are per default false. The deletion is
        /// in this case not cascading.
        /// </summary>
        /// <param name="processDefinitionId"> the id, which corresponds to the process definition </param>
        /// <exception cref="ProcessEngineException">
        ///          If the process definition does not exist
        /// </exception>
        /// <exception cref="AuthorizationException">
        ///          If the user has no <seealso cref="Permissions#DELETE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>. </exception>
        /// <seealso cref= RepositoryService#deleteProcessDefinition(java.lang.String, boolean, boolean) </seealso>
        void DeleteProcessDefinition(string processDefinitionId);

        /// <summary>
        /// Deletes the process definition which belongs to the given process definition id.
        /// Cascades the deletion if the cascade is set to true.
        /// Same behavior as <seealso cref="RepositoryService#deleteProcessDefinition(java.lang.String, boolean, boolean)"/>
        /// The skipCustomListeners parameter is per default false. The custom listeners are called
        /// if the cascading flag is set to true and the process instances are deleted.
        /// </summary>
        /// <param name="processDefinitionId"> the id, which corresponds to the process definition </param>
        /// <param name="cascade"> if set to true, all process instances (including) history are deleted </param>
        /// <exception cref="ProcessEngineException">
        ///          If the process definition does not exist
        /// </exception>
        /// <exception cref="AuthorizationException">
        ///          If the user has no <seealso cref="Permissions#DELETE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>. </exception>
        /// <seealso cref= RepositoryService#deleteProcessDefinition(java.lang.String, boolean, boolean) </seealso>
        void DeleteProcessDefinition(string processDefinitionId, bool cascade);


        /// <summary>
        /// Deletes the process definition which belongs to the given process definition id.
        /// Cascades the deletion if the cascade is set to true the custom listener
        /// can be skipped if the third parameter is set to true.
        /// </summary>
        /// <param name="processDefinitionId"> the id, which corresponds to the process definition </param>
        /// <param name="cascade"> if set to true, all process instances (including) history are deleted </param>
        /// <param name="skipCustomListeners"> if true, only the built-in <seealso cref="ExecutionListener"/>s
        ///            are notified with the <seealso cref="ExecutionListener#EVENTNAME_END"/> event.
        ///            Is only used if cascade set to true.
        /// </param>
        /// <exception cref="ProcessEngineException">
        ///          If the process definition does not exist
        /// </exception>
        /// <exception cref="AuthorizationException">
        ///          If the user has no <seealso cref="Permissions#DELETE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>. </exception>
        void DeleteProcessDefinition(string processDefinitionId, bool cascade, bool skipCustomListeners);

        /// <summary>
        /// Retrieves a list of deployment resource names for the given deployment,
        /// ordered alphabetically.
        /// </summary>
        /// <param name="deploymentId"> id of the deployment, cannot be null.
        /// </param>
        /// <exception cref="AuthorizationException">
        ///          If the user has no <seealso cref="Permissions#READ"/> permission on <seealso cref="Resources#DEPLOYMENT"/>. </exception>
        IList<string> GetDeploymentResourceNames(string deploymentId);

        /// <summary>
        /// Retrieves a list of deployment resources for the given deployment,
        /// ordered alphabetically by name.
        /// </summary>
        /// <param name="deploymentId"> id of the deployment, cannot be null.
        /// </param>
        /// <exception cref="AuthorizationException">
        ///          If the user has no <seealso cref="Permissions#READ"/> permission on <seealso cref="Resources#DEPLOYMENT"/>. </exception>
        IList<IResource> GetDeploymentResources(string deploymentId);

        /// <summary>
        /// Gives access to a deployment resource through a stream of bytes.
        /// </summary>
        /// <param name="deploymentId"> id of the deployment, cannot be null. </param>
        /// <param name="resourceName"> name of the resource, cannot be null.
        /// </param>
        /// <exception cref="ProcessEngineException">
        ///          When the resource doesn't exist in the given deployment or when no deployment exists
        ///          for the given deploymentId. </exception>
        /// <exception cref="AuthorizationException">
        ///          If the user has no <seealso cref="Permissions#READ"/> permission on <seealso cref="Resources#DEPLOYMENT"/>. </exception>
        System.IO.Stream GetResourceAsStream(string deploymentId, string resourceName);

        /// <summary>
        /// Gives access to a deployment resource through a stream of bytes.
        /// </summary>
        /// <param name="deploymentId"> id of the deployment, cannot be null. </param>
        /// <param name="resourceId"> id of the resource, cannot be null.
        /// </param>
        /// <exception cref="ProcessEngineException">
        ///          When the resource doesn't exist in the given deployment or when no deployment exists
        ///          for the given deploymentId. </exception>
        /// <exception cref="AuthorizationException">
        ///          If the user has no <seealso cref="Permissions#READ"/> permission on <seealso cref="Resources#DEPLOYMENT"/>. </exception>
        System.IO.Stream GetResourceAsStreamById(string deploymentId, string resourceId);

        /// <summary>
        /// Query process definitions.
        /// </summary>
        IQueryable<IProcessDefinition> CreateProcessDefinitionQuery(Expression<Func<ProcessDefinitionEntity,bool>> expression = null);

        /// <summary>
        /// Query case definitions.
        /// </summary>
        IQueryable<ICaseDefinition> CreateCaseDefinitionQuery(Expression<Func<ICaseDefinition, bool>> expression =null);

        /// <summary>
        /// Query decision definitions.
        /// </summary>
        IQueryable<IDecisionDefinition> CreateDecisionDefinitionQuery(
            Expression<Func<DecisionDefinitionEntity, bool>> expression = null);

        /// <summary>
        /// Query decision requirements definition.
        /// </summary>
        IQueryable<IDecisionRequirementsDefinition> CreateDecisionRequirementsDefinitionQuery(Expression<Func<DecisionRequirementsDefinitionEntity, bool>> expression = null);

        /// <summary>
        /// Query process definitions.
        /// </summary>
        IQueryable<IDeployment> CreateDeploymentQuery(Expression<Func<DeploymentEntity, bool>> expression = null);

        /// <summary>
        /// Suspends the process definition with the given id.
        /// 
        /// If a process definition is in state suspended, it will not be possible to start new process instances
        /// based on the process definition.
        /// 
        /// <strong>Note: all the process instances of the process definition will still be active
        /// (ie. not suspended)!</strong>
        /// 
        /// <para>Note: for more complex suspend commands use <seealso cref="#updateProcessDefinitionSuspensionState()"/>.</para>
        /// </summary>
        /// <exception cref="ProcessEngineException">
        ///          If no such processDefinition can be found. </exception>
        /// <exception cref="AuthorizationException">
        ///          If the user has no <seealso cref="Permissions#UPDATE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>. </exception>
        void SuspendProcessDefinitionById(string processDefinitionId);

        /// <summary>
        /// Suspends the process definition with the given id.
        /// 
        /// If a process definition is in state suspended, it will not be possible to start new process instances
        /// based on the process definition.
        /// 
        /// <para>Note: for more complex suspend commands use <seealso cref="#updateProcessDefinitionSuspensionState()"/>.</para>
        /// </summary>
        /// <param name="suspendProcessInstances"> If true, all the process instances of the provided process definition
        ///                                will be suspended too. </param>
        /// <param name="suspensionDate"> The date on which the process definition will be suspended. If null, the
        ///                       process definition is suspended immediately.
        ///                       Note: The job executor needs to be active to use this!
        /// </param>
        /// <exception cref="ProcessEngineException">
        ///          If no such processDefinition can be found. </exception>
        /// <exception cref="AuthorizationException">
        ///          If the user has no <seealso cref="Permissions#UPDATE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>
        ///          and if <code>suspendProcessInstances</code> is set to <code>true</code> and the user have no
        ///          <seealso cref="Permissions#UPDATE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/> or no
        ///          <seealso cref="Permissions#UPDATE_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>.
        /// </exception>
        /// <seealso cref= RuntimeService#suspendProcessInstanceById(String) </seealso>
        void SuspendProcessDefinitionById(string processDefinitionId, bool suspendProcessInstances, DateTime suspensionDate);

        /// <summary>
        /// Suspends the <strong>all</strong> process definitions with the given key (= id in the bpmn20.xml file).
        /// 
        /// If a process definition is in state suspended, it will not be possible to start new process instances
        /// based on the process definition.
        /// 
        /// <strong>Note: all the process instances of the process definition will still be active
        /// (ie. not suspended)!</strong>
        /// 
        /// <para>Note: for more complex suspend commands use <seealso cref="#updateProcessDefinitionSuspensionState()"/>.</para>
        /// </summary>
        /// <exception cref="ProcessEngineException">
        ///          If no such processDefinition can be found. </exception>
        /// <exception cref="AuthorizationException">
        ///          If the user has no <seealso cref="Permissions#UPDATE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>. </exception>
        void SuspendProcessDefinitionByKey(string processDefinitionKey);

        /// <summary>
        /// Suspends the <strong>all</strong> process definitions with the given key (= id in the bpmn20.xml file).
        /// 
        /// If a process definition is in state suspended, it will not be possible to start new process instances
        /// based on the process definition.
        /// 
        /// <para>Note: for more complex suspend commands use <seealso cref="#updateProcessDefinitionSuspensionState()"/>.</para>
        /// </summary>
        /// <param name="suspendProcessInstances"> If true, all the process instances of the provided process definition
        ///                                will be suspended too. </param>
        /// <param name="suspensionDate"> The date on which the process definition will be suspended. If null, the
        ///                       process definition is suspended immediately.
        ///                       Note: The job executor needs to be active to use this!
        /// </param>
        /// <exception cref="ProcessEngineException">
        ///          If no such processDefinition can be found. </exception>
        /// <exception cref="AuthorizationException">
        ///          If the user has no <seealso cref="Permissions#UPDATE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>
        ///          and if <code>suspendProcessInstances</code> is set to <code>true</code> and the user have no
        ///          <seealso cref="Permissions#UPDATE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/> or no
        ///          <seealso cref="Permissions#UPDATE_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>.
        /// </exception>
        /// <seealso cref= RuntimeService#suspendProcessInstanceById(String) </seealso>
        void SuspendProcessDefinitionByKey(string processDefinitionKey, bool suspendProcessInstances, DateTime suspensionDate);

        /// <summary>
        /// Activates the process definition with the given id.
        /// 
        /// <para>Note: for more complex activate commands use <seealso cref="#updateProcessDefinitionSuspensionState()"/>.</para>
        /// </summary>
        /// <exception cref="ProcessEngineException">
        ///          If no such processDefinition can be found or if the process definition is already in state active. </exception>
        /// <exception cref="AuthorizationException">
        ///          If the user has no <seealso cref="Permissions#UPDATE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>. </exception>
        void ActivateProcessDefinitionById(string processDefinitionId);

        /// <summary>
        /// Activates the process definition with the given id.
        /// 
        /// <para>Note: for more complex activate commands use <seealso cref="#updateProcessDefinitionSuspensionState()"/>.</para>
        /// </summary>
        /// <param name="suspendProcessInstances"> If true, all the process instances of the provided process definition
        ///                                will be activated too. </param>
        /// <param name="activationDate"> The date on which the process definition will be activated. If null, the
        ///                       process definition is suspended immediately.
        ///                       Note: The job executor needs to be active to use this!
        /// </param>
        /// <exception cref="ProcessEngineException">
        ///          If no such processDefinition can be found. </exception>
        /// <exception cref="AuthorizationException">
        ///          If the user has no <seealso cref="Permissions#UPDATE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>
        ///          and if <code>activateProcessInstances</code> is set to <code>true</code> and the user have no
        ///          <seealso cref="Permissions#UPDATE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/> or no
        ///          <seealso cref="Permissions#UPDATE_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>.
        /// </exception>
        /// <seealso cref= RuntimeService#activateProcessInstanceById(String) </seealso>
        void ActivateProcessDefinitionById(string processDefinitionId, bool activateProcessInstances, DateTime activationDate);

        /// <summary>
        /// Activates the process definition with the given key (=id in the bpmn20.xml file).
        /// 
        /// <para>Note: for more complex activate commands use <seealso cref="#updateProcessDefinitionSuspensionState()"/>.</para>
        /// </summary>
        /// <exception cref="ProcessEngineException">
        ///          If no such processDefinition can be found. </exception>
        /// <exception cref="AuthorizationException">
        ///          If the user has no <seealso cref="Permissions#UPDATE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>. </exception>
        void ActivateProcessDefinitionByKey(string processDefinitionKey);

        /// <summary>
        /// Activates the process definition with the given key (=id in the bpmn20.xml file).
        /// 
        /// <para>Note: for more complex activate commands use <seealso cref="#updateProcessDefinitionSuspensionState()"/>.</para>
        /// </summary>
        /// <param name="suspendProcessInstances"> If true, all the process instances of the provided process definition
        ///                                will be activated too. </param>
        /// <param name="activationDate"> The date on which the process definition will be activated. If null, the
        ///                       process definition is suspended immediately.
        ///                       Note: The job executor needs to be active to use this!
        /// </param>
        /// <exception cref="ProcessEngineException">
        ///          If no such processDefinition can be found. </exception>
        /// <exception cref="AuthorizationException">
        ///          If the user has no <seealso cref="Permissions#UPDATE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>
        ///          and if <code>activateProcessInstances</code> is set to <code>true</code> and the user have no
        ///          <seealso cref="Permissions#UPDATE"/> permission on <seealso cref="Resources#PROCESS_INSTANCE"/> or no
        ///          <seealso cref="Permissions#UPDATE_INSTANCE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>.
        /// </exception>
        /// <seealso cref= RuntimeService#activateProcessInstanceById(String) </seealso>
        void ActivateProcessDefinitionByKey(string processDefinitionKey, bool activateProcessInstances, DateTime activationDate);

        /// <summary>
        /// Activate or suspend process definitions using a fluent builder. Specify the
        /// definitions by calling one of the <i>by</i> methods, like
        /// <i>byProcessDefinitionId</i>. To update the suspension state call
        /// <seealso cref="UpdateProcessDefinitionSuspensionStateBuilder#activate()"/> or
        /// <seealso cref="UpdateProcessDefinitionSuspensionStateBuilder#suspend()"/>.
        /// </summary>
        /// <returns> the builder to update the suspension state </returns>
        IUpdateProcessDefinitionSuspensionStateSelectBuilder UpdateProcessDefinitionSuspensionState();

        /// <summary>
        /// Updates time to live of process definition. The field is used within history cleanup process. </summary>
        /// <param name="processDefinitionId"> </param>
        /// <param name="historyTimeToLive"> </param>
        /// <exception cref="AuthorizationException">
        ///          If the user has no <seealso cref="Permissions#UPDATE"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>. </exception>
        void UpdateProcessDefinitionHistoryTimeToLive(string processDefinitionId, int? historyTimeToLive);

        /// <summary>
        /// Updates time to live of decision definition. The field is used within history cleanup process. </summary>
        /// <param name="decisionDefinitionId"> </param>
        /// <param name="historyTimeToLive"> </param>
        /// <exception cref="AuthorizationException">
        ///          If the user has no <seealso cref="Permissions#UPDATE"/> permission on <seealso cref="Resources#DECISION_DEFINITION"/>. </exception>
        void UpdateDecisionDefinitionHistoryTimeToLive(string decisionDefinitionId, int? historyTimeToLive);

        /// <summary>
        /// Updates time to live of case definition. The field is used within history cleanup process. </summary>
        /// <param name="caseDefinitionId"> </param>
        /// <param name="historyTimeToLive"> </param>
        void UpdateCaseDefinitionHistoryTimeToLive(string caseDefinitionId, int? historyTimeToLive);

        /// <summary>
        /// Gives access to a deployed process model, e.g., a BPMN 2.0 XML file,
        /// through a stream of bytes.
        /// </summary>
        /// <param name="processDefinitionId">
        ///          id of a <seealso cref="ProcessDefinition"/>, cannot be null.
        /// </param>
        /// <exception cref="ProcessEngineException">
        ///           when the process model doesn't exist. </exception>
        /// <exception cref="AuthorizationException">
        ///          If the user has no <seealso cref="Permissions#READ"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>. </exception>
        System.IO.Stream GetProcessModel(string processDefinitionId);

        /// <summary>
        /// Gives access to a deployed process diagram, e.g., a PNG image, through a
        /// stream of bytes.
        /// </summary>
        /// <param name="processDefinitionId">
        ///          id of a <seealso cref="ProcessDefinition"/>, cannot be null. </param>
        /// <returns> null when the diagram resource name of a <seealso cref="ProcessDefinition"/> is null.
        /// </returns>
        /// <exception cref="ProcessEngineException">
        ///           when the process diagram doesn't exist. </exception>
        /// <exception cref="AuthorizationException">
        ///          If the user has no <seealso cref="Permissions#READ"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>. </exception>
        System.IO.Stream GetProcessDiagram(string processDefinitionId);

        /// <summary>
        /// Returns the <seealso cref="ProcessDefinition"/> including all BPMN information like additional
        /// Properties (e.g. documentation).
        /// </summary>
        /// <exception cref="AuthorizationException">
        ///          If the user has no <seealso cref="Permissions#READ"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>. </exception>
        IProcessDefinition GetProcessDefinition(string processDefinitionId);

        /// <summary>
        /// Provides positions and dimensions of elements in a process diagram as
        /// provided by <seealso cref="RepositoryService#getProcessDiagram(String)"/>.
        /// 
        /// This method requires a process model and a diagram image to be deployed.
        /// </summary>
        /// <param name="processDefinitionId"> id of a <seealso cref="ProcessDefinition"/>, cannot be null. </param>
        /// <returns> Map with process element ids as keys and positions and dimensions as values.
        /// </returns>
        /// <returns> null when the input stream of a process diagram is null.
        /// </returns>
        /// <exception cref="ProcessEngineException">
        ///          When the process model or diagram doesn't exist. </exception>
        /// <exception cref="AuthorizationException">
        ///          If the user has no <seealso cref="Permissions#READ"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>. </exception>
        DiagramLayout GetProcessDiagramLayout(string processDefinitionId);

        /// <summary>
        /// Returns the <seealso cref="BpmnModelInstance"/> for the given processDefinitionId.
        /// </summary>
        /// <param name="processDefinitionId"> the id of the Process Definition for which the <seealso cref="BpmnModelInstance"/>
        ///  should be retrieved.
        /// </param>
        /// <returns> the <seealso cref="BpmnModelInstance"/>
        /// </returns>
        /// <exception cref="AuthorizationException">
        ///          If the user has no <seealso cref="Permissions#READ"/> permission on <seealso cref="Resources#PROCESS_DEFINITION"/>. </exception>
        IBpmnModelInstance GetBpmnModelInstance(string processDefinitionId);

        /// <summary>
        /// Returns the <seealso cref="CmmnModelInstance"/> for the given caseDefinitionId.
        /// </summary>
        /// <param name="caseDefinitionId"> the id of the Case Definition for which the <seealso cref="CmmnModelInstance"/>
        ///  should be retrieved.
        /// </param>
        /// <returns> the <seealso cref="CmmnModelInstance"/>
        /// </returns>
        /// <exception cref="NotValidException"> when the given case definition id or deployment id or resource name is null </exception>
        /// <exception cref="NotFoundException"> when no CMMN model instance or deployment resource is found for the given
        ///     case definition id </exception>
        /// <exception cref="ProcessEngineException"> when an internal exception happens during the execution
        ///     of the command. </exception>
        ICmmnModelInstance GetCmmnModelInstance(string caseDefinitionId);

        /// <summary>
        /// Returns the <seealso cref="DmnModelInstance"/> for the given decisionDefinitionId.
        /// </summary>
        /// <param name="decisionDefinitionId"> the id of the Decision Definition for which the <seealso cref="DmnModelInstance"/>
        ///  should be retrieved.
        /// </param>
        /// <returns> the <seealso cref="DmnModelInstance"/>
        /// </returns>
        /// <exception cref="NotValidException"> when the given decision definition id or deployment id or resource name is null </exception>
        /// <exception cref="NotFoundException"> when no DMN model instance or deployment resource is found for the given
        ///     decision definition id </exception>
        /// <exception cref="ProcessEngineException"> when an internal exception happens during the execution of the command. </exception>
        /// <exception cref="AuthorizationException">
        ///          If the user has no <seealso cref="Permissions#READ"/> permission on <seealso cref="Resources#DECISION_DEFINITION"/>. </exception>
        IDmnModelInstance GetDmnModelInstance(string decisionDefinitionId);

        /// <summary>
        /// Authorizes a candidate user for a process definition.
        /// </summary>
        /// <param name="processDefinitionId"> id of the process definition, cannot be null. </param>
        /// <param name="userId"> id of the user involve, cannot be null.
        /// </param>
        /// <exception cref="ProcessEngineException">
        ///          When the process definition or user doesn't exist.
        /// </exception>
        /// @deprecated Use authorization mechanism instead.
        ///  
        [Obsolete("Use authorization mechanism instead.")]
        void AddCandidateStarterUser(string processDefinitionId, string userId);

        /// <summary>
        /// Authorizes a candidate group for a process definition.
        /// </summary>
        /// <param name="processDefinitionId"> id of the process definition, cannot be null. </param>
        /// <param name="groupId"> id of the group involve, cannot be null.
        /// </param>
        /// <exception cref="ProcessEngineException">
        ///          When the process definition or group doesn't exist.
        /// </exception>
        /// @deprecated Use authorization mechanism instead.
        ///  
        [Obsolete("Use authorization mechanism instead.")]
        void AddCandidateStarterGroup(string processDefinitionId, string groupId);

        /// <summary>
        /// Removes the authorization of a candidate user for a process definition.
        /// </summary>
        /// <param name="processDefinitionId"> id of the process definition, cannot be null. </param>
        /// <param name="userId"> id of the user involve, cannot be null.
        /// </param>
        /// <exception cref="ProcessEngineException">
        ///          When the process definition or user doesn't exist.
        /// </exception>
        /// @deprecated Use authorization mechanism instead.
        ///  
        [Obsolete("Use authorization mechanism instead.")]
        void DeleteCandidateStarterUser(string processDefinitionId, string userId);

        /// <summary>
        /// Removes the authorization of a candidate group for a process definition.
        /// </summary>
        /// <param name="processDefinitionId"> id of the process definition, cannot be null. </param>
        /// <param name="groupId"> id of the group involve, cannot be null.
        /// </param>
        /// <exception cref="ProcessEngineException">
        ///          When the process definition or group doesn't exist.
        /// </exception>
        /// @deprecated Use authorization mechanism instead.
        ///  
        [Obsolete("Use authorization mechanism instead.")]
        void DeleteCandidateStarterGroup(string processDefinitionId, string groupId);

        /// <summary>
        /// Retrieves the <seealso cref="IdentityLink"/>s associated with the given process definition.
        /// Such an <seealso cref="IdentityLink"/> informs how a certain identity (eg. group or user)
        /// is authorized for a certain process definition
        /// </summary>
        /// @deprecated Use authorization mechanism instead.
        ///  
        [Obsolete("Use authorization mechanism instead.")]
        IList<IIdentityLink> GetIdentityLinksForProcessDefinition(string processDefinitionId);

        /// <summary>
        /// Returns the <seealso cref="CaseDefinition"/>.
        /// </summary>
        /// <exception cref="NotValidException"> when the given case definition id is null </exception>
        /// <exception cref="NotFoundException"> when no case definition is found for the given case definition id </exception>
        /// <exception cref="ProcessEngineException"> when an internal exception happens during the execution
        ///     of the command. </exception>
        ICaseDefinition GetCaseDefinition(string caseDefinitionId);

        /// <summary>
        /// Gives access to a deployed case model, e.g., a CMMN 1.0 XML file,
        /// through a stream of bytes.
        /// </summary>
        /// <param name="caseDefinitionId">
        ///          id of a <seealso cref="CaseDefinition"/>, cannot be null.
        /// </param>
        /// <exception cref="NotValidException"> when the given case definition id or deployment id or resource name is null </exception>
        /// <exception cref="NotFoundException"> when no case definition or deployment resource is found for the given case definition id </exception>
        /// <exception cref="ProcessEngineException"> when an internal exception happens during the execution of the command </exception>
        System.IO.Stream GetCaseModel(string caseDefinitionId);

        /// <summary>
        /// Gives access to a deployed case diagram, e.g., a PNG image, through a
        /// stream of bytes.
        /// </summary>
        /// <param name="caseDefinitionId"> id of a <seealso cref="CaseDefinition"/>, cannot be null. </param>
        /// <returns> null when the diagram resource name of a <seealso cref="CaseDefinition"/> is null. </returns>
        /// <exception cref="ProcessEngineException"> when the process diagram doesn't exist. </exception>
        System.IO.Stream GetCaseDiagram(string caseDefinitionId);

        /// <summary>
        /// Returns the <seealso cref="DecisionDefinition"/>.
        /// </summary>
        /// <exception cref="NotValidException"> when the given decision definition id is null </exception>
        /// <exception cref="NotFoundException"> when no decision definition is found for the given decision definition id </exception>
        /// <exception cref="ProcessEngineException"> when an internal exception happens during the execution of the command. </exception>
        /// <exception cref="AuthorizationException">
        ///          If the user has no <seealso cref="Permissions#READ"/> permission on <seealso cref="Resources#DECISION_DEFINITION"/>. </exception>
        IDecisionDefinition GetDecisionDefinition(string decisionDefinitionId);

        /// <summary>
        /// Returns the <seealso cref="DecisionRequirementsDefinition"/>.
        /// </summary>
        /// <exception cref="NotValidException"> when the given decision requirements definition id is null </exception>
        /// <exception cref="NotFoundException"> when no decision requirements definition is found for the given decision requirements definition id </exception>
        /// <exception cref="ProcessEngineException"> when an internal exception happens during the execution of the command. </exception>
        /// <exception cref="AuthorizationException">
        ///          If the user has no <seealso cref="Permissions#READ"/> permission on <seealso cref="Resources#DECISION_REQUIREMENTS_DEFINITION"/>. </exception>
        IDecisionRequirementsDefinition GetDecisionRequirementsDefinition(string decisionRequirementsDefinitionId);

        /// <summary>
        /// Gives access to a deployed decision model, e.g., a DMN 1.1 XML file,
        /// through a stream of bytes.
        /// </summary>
        /// <param name="decisionDefinitionId">
        ///          id of a <seealso cref="DecisionDefinition"/>, cannot be null.
        /// </param>
        /// <exception cref="NotValidException"> when the given decision definition id or deployment id or resource name is null </exception>
        /// <exception cref="NotFoundException"> when no decision definition or deployment resource is found for the given decision definition id </exception>
        /// <exception cref="ProcessEngineException"> when an internal exception happens during the execution of the command </exception>
        /// <exception cref="AuthorizationException">
        ///          If the user has no <seealso cref="Permissions#READ"/> permission on <seealso cref="Resources#DECISION_DEFINITION"/>. </exception>
        System.IO.Stream GetDecisionModel(string decisionDefinitionId);

        /// <summary>
        /// Gives access to a deployed decision requirements model, e.g., a DMN 1.1 XML file,
        /// through a stream of bytes.
        /// </summary>
        /// <param name="decisionRequirementsDefinitionId">
        ///          id of a <seealso cref="DecisionRequirementsDefinition"/>, cannot be null.
        /// </param>
        /// <exception cref="NotValidException"> when the given decision requirements definition id or deployment id or resource name is null </exception>
        /// <exception cref="NotFoundException"> when no decision requirements definition or deployment resource is found for the given decision requirements definition id </exception>
        /// <exception cref="ProcessEngineException"> when an internal exception happens during the execution of the command </exception>
        /// <exception cref="AuthorizationException">
        ///          If the user has no <seealso cref="Permissions#READ"/> permission on <seealso cref="Resources#DECISION_REQUIREMENTS_DEFINITION"/>. </exception>
        System.IO.Stream GetDecisionRequirementsModel(string decisionRequirementsDefinitionId);

        /// <summary>
        /// Gives access to a deployed decision diagram, e.g., a PNG image, through a
        /// stream of bytes.
        /// </summary>
        /// <param name="decisionDefinitionId"> id of a <seealso cref="DecisionDefinition"/>, cannot be null. </param>
        /// <returns> null when the diagram resource name of a <seealso cref="DecisionDefinition"/> is null. </returns>
        /// <exception cref="ProcessEngineException"> when the decision diagram doesn't exist. </exception>
        /// <exception cref="AuthorizationException">
        ///          If the user has no <seealso cref="Permissions#READ"/> permission on <seealso cref="Resources#DECISION_DEFINITION"/>. </exception>
        System.IO.Stream GetDecisionDiagram(string decisionDefinitionId);

        /// <summary>
        /// Gives access to a deployed decision requirements diagram, e.g., a PNG image, through a
        /// stream of bytes.
        /// </summary>
        /// <param name="decisionRequirementsDefinitionId"> id of a <seealso cref="DecisionRequirementsDefinition"/>, cannot be null. </param>
        /// <returns> null when the diagram resource name of a <seealso cref="DecisionRequirementsDefinition"/> is null. </returns>
        /// <exception cref="ProcessEngineException"> when the decision requirements diagram doesn't exist. </exception>
        /// <exception cref="AuthorizationException">
        ///          If the user has no <seealso cref="Permissions#READ"/> permission on <seealso cref="Resources#DECISION_REQUIREMENTS_DEFINITION"/>. </exception>
        System.IO.Stream GetDecisionRequirementsDiagram(string decisionRequirementsDefinitionId);

    }

}
