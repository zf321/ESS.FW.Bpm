using ESS.FW.Bpm.Engine.Impl.Core.Model;

namespace ESS.FW.Bpm.Engine.Impl.Pvm
{
    /// <summary>
    ///      
    /// </summary>
    public interface IPvmProcessElement
    {
        /// <summary>
        ///     The id of the element
        /// </summary>
        /// <returns> the id </returns>
        string Id { get; }

        /// <summary>
        ///     The process definition scope, root of the scope hierarchy.
        ///     @return
        /// </summary>
        IPvmProcessDefinition ProcessDefinition { get; }

        /// <summary>
        ///     Returns the properties of the element.
        /// </summary>
        /// <returns> the properties </returns>
        Core.Model.Properties Properties { get; }

        object GetProperty(string name);
    }
}