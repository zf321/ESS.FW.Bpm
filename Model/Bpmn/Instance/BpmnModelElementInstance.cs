

using ESS.FW.Bpm.Model.Bpmn.builder;
using ESS.FW.Bpm.Model.Xml.instance;

namespace ESS.FW.Bpm.Model.Bpmn.instance
{
    /// <summary>
    /// Interface implemented by all elements in a BPMN Model
    /// 
    /// 
    /// </summary>
    public interface IBpmnModelElementInstance : IModelElementInstance,IBuilder
    {

        /// <summary>
        /// Returns a new fluent builder for the element if implemented.
        /// </summary>
        /// <returns> the builder object </returns>
        //IBaseElementBuilder Builder();

        /// <summary>
        /// Tests if the element is a scope like process or sub-process.
        /// </summary>
        /// <returns> true if element is scope, otherwise false </returns>
        bool IsScope { get; }

        /// <summary>
        /// Gets the element which is the scope of this element. Like
        /// the parent process or sub-process.
        /// </summary>
        /// <returns> the scope element or null if non is found </returns>
        IBpmnModelElementInstance Scope { get; }
    }
}