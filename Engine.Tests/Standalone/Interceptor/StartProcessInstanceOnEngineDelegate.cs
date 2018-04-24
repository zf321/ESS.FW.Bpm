using System.Collections.Generic;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Delegate;
using NUnit.Framework;

namespace Engine.Tests.Standalone.Interceptor
{



    /// <summary>
    /// 
    /// 
    /// </summary>
    [TestFixture]
    public class StartProcessInstanceOnEngineDelegate : IJavaDelegate
    {

        public static IDictionary<string, IProcessEngine> Engines = new Dictionary<string, IProcessEngine>();

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public void execute(delegate.IDelegateExecution execution) throws Exception
        public virtual void Execute(IBaseDelegateExecution execution)
        {

            string engineName = (string)execution.GetVariable("engineName");
            string processKeyName = (string)execution.GetVariable("processKey");

            Engines[engineName].RuntimeService.StartProcessInstanceByKey(processKeyName);

        }

    }

}