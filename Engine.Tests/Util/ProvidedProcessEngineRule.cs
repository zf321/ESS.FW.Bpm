using System;
using ESS.FW.Bpm.Engine;

namespace Engine.Tests.Util
{
    public class ProvidedProcessEngineRule : ProcessEngineRule
    {
        protected internal Func<IProcessEngine> ProcessEngineProvider;

        public ProvidedProcessEngineRule() : base(PluggableProcessEngineTestCase.ProcessEngine, true)
        {
        }

        public ProvidedProcessEngineRule(ProcessEngineBootstrapRule bootstrapRule)
            : this(() => bootstrapRule.ProcessEngine)
        {
        }

        public ProvidedProcessEngineRule(Func<IProcessEngine> processEngineProvider) :base(false)//: base(true)
        {
            ProcessEngineProvider = processEngineProvider;
        }

        protected internal override void InitializeProcessEngine()
        {
            if (ProcessEngineProvider != null)
                try
                {
                    processEngine = ProcessEngineProvider();
                }
                catch (System.Exception e)
                {
                    throw new System.Exception("Could not get process engine", e);
                }
            else
                base.InitializeProcessEngine();
        }

    }
}