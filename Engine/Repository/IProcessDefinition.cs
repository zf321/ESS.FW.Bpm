using ESS.FW.Bpm.Engine.Runtime;

namespace ESS.FW.Bpm.Engine.Repository
{
    /// <summary>
    ///     An object structure representing an executable process composed of
    ///     activities and transitions.
    ///     Business processes are often created with graphical editors that store the
    ///     process definition in certain file format. These files can be added to a
    ///     <seealso cref="IDeployment" /> artifact, such as for example a Business Archive (.bar)
    ///     file.
    ///     At deploy time, the engine will then parse the process definition files to an
    ///     executable instance of this class, that can be used to start a <seealso cref="IProcessInstance" />.
    ///      
    ///      Joram Barez
    ///     
    /// </summary>
    public interface IProcessDefinition : IResourceDefinition
    {
        /// <summary>
        ///     description of this process
        /// </summary>
        string Description { get; }

        /// <summary>
        ///     暂停的
        ///     Returns true if the process definition is in suspended state.
        /// </summary>
        bool Suspended { get; }

        /// <summary>
        ///     Version tag of the process definition.
        /// </summary>
        string VersionTag { get; }

        /// <summary>
        ///     Does this process definition has a <seealso cref="IFormService#getStartFormData(String) start form key" />.
        /// </summary>
        bool GetHasStartFormKey();

        int SuspensionState { get; set; }
    }
}