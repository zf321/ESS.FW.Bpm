using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESS.FW.Bpm.Model.Dmn.instance;
using ESS.FW.Bpm.Model.Xml;

namespace ESS.FW.Bpm.Model.Cmmn
{
    public interface ICmmnModelInstance:IModelInstance
    {
        IDefinitions Definitions { get; set; }
        /**
   * Copies the CMMN model instance but not the model. So only the wrapped DOM document is cloned.
   * Changes of the model are persistent between multiple model instances.
   *
   * @return the new CMMN model instance
   */
        ICmmnModelInstance Clone();
    }
}
