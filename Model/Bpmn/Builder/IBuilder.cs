using ESS.FW.Bpm.Model.Bpmn.instance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESS.FW.Bpm.Model.Bpmn.builder
{
    public interface IBuilder/*<out TB,in TE> where TB: IBpmnModelElementBuilder<TB, TE> where TE:IBpmnModelElementInstance*/
    {
        TB Builder<TB,TE>() where TB : IBpmnModelElementBuilder<TE> where TE : IBpmnModelElementInstance;
    }
}
