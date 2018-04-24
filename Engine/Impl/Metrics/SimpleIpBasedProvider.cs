namespace ESS.FW.Bpm.Engine.Impl.Metrics
{
    /// <summary>
    ///     
    /// </summary>
    public class SimpleIpBasedProvider : IMetricsReporterIdProvider
    {
        private static readonly MetricsLogger Log = ProcessEngineLogger.MetricsLogger;

        public virtual string ProvideId(IProcessEngine processEngine)
        {
            var localIp = "";
            try
            {
                //localIp = InetAddress.LocalHost.HostAddress;
            }
            catch (System.Exception e)
            {
                // do not throw an exception; failure to determine an IP should not prevent from using the engine
                Log.CouldNotDetermineIp(e);
            }

            return CreateId(localIp, processEngine.Name);
        }

        public static string CreateId(string ip, string engineName)
        {
            return ip + "$" + engineName;
        }
    }
}