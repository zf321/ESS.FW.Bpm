
using ESS.FW.Bpm.Engine.Impl;

namespace Engine.Tests
{


    /// <summary>
    /// 
    /// 
    /// </summary>
    public class TestLogger : ProcessEngineLogger
    {
        public static readonly TestLogger Logger = CreateLogger<TestLogger>(
            typeof(TestLogger),
            ProjectCode, "bpmn.TestLogger", "01");
    }

}