using ESS.FW.Bpm.Engine.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESS.FW.Bpm.Engine.Dmn.Feel.Impl.Juel
{
    public class FeelLogger : BaseLogger
    {
        public static  String PROJECT_CODE = "FEEL";
    public static  String PROJECT_LOGGER = "org.camunda.bpm.dmn.feel";
    public static FeelEngineLogger ENGINE_LOGGER = CreateLogger<FeelEngineLogger>(typeof(FeelEngineLogger), "FEEL", "ESS.FW.Bpm.Engine.Dmn.Feel", "01");
    }
}
