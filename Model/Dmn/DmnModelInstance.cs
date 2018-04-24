

using ESS.FW.Bpm.Model.Dmn.instance;
using ESS.FW.Bpm.Model.Xml;

namespace ESS.FW.Bpm.Model.Dmn
{
    //using bpmn.instance;
    
    public interface IDmnModelInstance : IModelInstance
    {

        /// <returns> the <seealso cref="Definitions"/>, root element of the Dmn Model.
        ///  </returns>
        IDefinitions Definitions { get; set; }


        /// <summary>
        /// Copies the DMN model instance but not the model. So only the wrapped DOM document is cloned.
        /// Changes of the model are persistent between multiple model instances.
        /// </summary>
        /// <returns> the new DMN model instance </returns>
        //DmnModelInstance clone();
    }
}