using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Parser;
using ESS.FW.Bpm.Engine.Impl.Util.xml;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace Engine.Tests.Standalone.Deploy
{


    /// <summary>
    /// 
    /// </summary>
    public class TestBPMNParseListener : AbstractBpmnParseListener
    {


        public override void ParseRootElement(Element rootElement, IList<ProcessDefinitionEntity> processDefinitions)
        {
            // Change the key of all deployed process-definitions
            foreach (ProcessDefinitionEntity entity in processDefinitions)
            {
                entity.Key = entity.Key + "-modified";
            }
        }

    }

}