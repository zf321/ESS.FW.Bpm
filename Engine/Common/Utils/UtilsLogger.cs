using ESS.FW.Bpm.Engine.Common.Logging;

namespace ESS.FW.Bpm.Engine.Common.Utils
{
    /// <summary>
    ///     
    /// </summary>
    public class UtilsLogger : BaseLogger
    {
        public const string PROJECT_CODE = "UTILS";

        public static readonly IoUtilLogger IoUtilLogger = CreateLogger<IoUtilLogger>(typeof(IoUtilLogger),
            PROJECT_CODE,
            "IO", "01");

        public static readonly EnsureUtilLogger EnsureUtilLogger =
            CreateLogger<EnsureUtilLogger>(typeof(EnsureUtilLogger),
                PROJECT_CODE, "ensure", "02");
    }
}