using ESS.FW.Bpm.Engine.Common.Logging;
using ESS.FW.Bpm.Engine.Dmn.engine.impl.hitpolicy;
using ESS.FW.Bpm.Engine.Dmn.engine.impl.transform;

namespace ESS.FW.Bpm.Engine.Dmn.engine.impl
{
    public class DmnLogger : BaseLogger
    {
        public const string PROJECT_CODE = "DMN";
        public const string PROJECT_LOGGER = "org.camunda.bpm.dmn";

        public static DmnEngineLogger ENGINE_LOGGER = CreateLogger<DmnEngineLogger>(typeof(DmnEngineLogger),
            PROJECT_CODE,
            PROJECT_LOGGER, "01");

        public static DmnTransformLogger TRANSFORM_LOGGER = CreateLogger<DmnTransformLogger>(
            typeof(DmnTransformLogger), PROJECT_CODE,
            PROJECT_LOGGER + ".transform", "02");

        public static DmnHitPolicyLogger HIT_POLICY_LOGGER = CreateLogger<DmnHitPolicyLogger>(
            typeof(DmnHitPolicyLogger), PROJECT_CODE,
            PROJECT_LOGGER + ".hitPolicy", "03");
    }
}