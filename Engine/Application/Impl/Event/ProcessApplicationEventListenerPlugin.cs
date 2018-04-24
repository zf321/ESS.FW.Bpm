using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Parser;
using ESS.FW.Bpm.Engine.Impl.Cfg;

namespace ESS.FW.Bpm.Engine.Application.Impl.Event
{
    /// <summary>
    ///     <para><seealso cref="ProcessEnginePlugin" /> enabling the process application event listener support.</para>
    ///     
    /// </summary>
    public class ProcessApplicationEventListenerPlugin : AbstractProcessEnginePlugin
    {
        public override void PreInit(ProcessEngineConfigurationImpl processEngineConfiguration)
        {
            var preParseListeners = processEngineConfiguration.CustomPreBpmnParseListeners;
            if (preParseListeners == null)
            {
                preParseListeners = new List<IBpmnParseListener>();
                processEngineConfiguration.CustomPreBpmnParseListeners = preParseListeners;
            }
            preParseListeners.Add(new ProcessApplicationEventParseListener());
        }
    }
}