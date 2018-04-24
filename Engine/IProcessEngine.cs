using System.Text.RegularExpressions;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Task;

namespace ESS.FW.Bpm.Engine
{
    /// <summary>
    ///     Provides access to all the services that expose the BPM and workflow operations.
    ///     <ul>
    ///         <li>
    ///             <b><seealso cref="IRuntimeService" />: </b> Allows the creation of
    ///             <seealso cref="IDeployment" />s and the starting of and searching on
    ///             <seealso cref="org.camunda.bpm.engine.runtime.ProcessInstance" />s.
    ///         </li>
    ///         <li>
    ///             <b><seealso cref="ITaskService" />: </b> Exposes operations to manage human
    ///             (standalone) <seealso cref="ITask" />s, such as claiming, completing and
    ///             assigning tasks
    ///         </li>
    ///         <li>
    ///             <b><seealso cref="IIdentityService" />: </b> Used for managing
    ///             <seealso cref="org.camunda.bpm.engine.identity.User" />s,
    ///             <seealso cref="Group" />s and
    ///             the relations between them<</li>
    ///         <li>
    ///             <b><seealso cref="IManagementService" />: </b> Exposes engine admin and
    ///             maintenance operations
    ///         </li>
    ///         <li>
    ///             <b><seealso cref="IHistoryService" />: </b> Service exposing information about
    ///             ongoing and past process instances.
    ///         </li>
    ///         <li>
    ///             <b><seealso cref="IAuthorizationService" />:</b> Service allowing
    ///             to manage access permissions for users and groups.</b>
    ///     </ul>
    ///     Typically, there will be only one central ProcessEngine instance needed in a
    ///     end-user application. Building a ProcessEngine is done through a
    ///     <seealso cref="ProcessEngineConfiguration" /> instance and is a costly operation which should be
    ///     avoided. For that purpose, it is advised to store it in a static field or
    ///     JNDI location (or something similar). This is a thread-safe object, so no
    ///     special precautions need to be taken.
    ///      
    ///     
    ///     
    /// </summary>
    public interface IProcessEngine : IProcessEngineServices
    {
        /// <summary>
        ///     the version of the process engine library
        /// </summary>
        /// <summary>
        ///     The name as specified in 'process-engine-name' in
        ///     the camunda.cfg.xml configuration file.
        ///     The default name for a process engine is 'default
        /// </summary>
        string Name { get; }

        ProcessEngineConfiguration ProcessEngineConfiguration { get; }

        void Close();
    }

    public static class ProcessEngineFields
    {
        public const string Version = "fox";
    }
}