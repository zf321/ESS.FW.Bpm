using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.Batch.Impl.Job
{
    /// <summary>
    ///     
    /// </summary>
    public class SetJobRetriesBatchConfigurationJsonConverter
    {
        public const string JobIds = "jobIds";
        public const string Retries = "retries";

        public static readonly SetJobRetriesBatchConfigurationJsonConverter Instance =
            new SetJobRetriesBatchConfigurationJsonConverter();

        //public virtual JObject ToJsonObject(SetJobRetriesBatchConfiguration configuration)
        //{
        //    var ja = new JArray();
        //    foreach (var t in configuration.Ids)
        //        ja.Add(new JValue(t));
        //    var json = new JObject();
        //    json.Add(JobIds, ja);
        //    json.Add(Retries, configuration.Retries);
        //    return json;
        //}

        //public SetJobRetriesBatchConfiguration ToObject(JObject json)
        //{
        //    var configuration = new SetJobRetriesBatchConfiguration(ReadJobIds(json),
        //        int.Parse(json.GetValue(Retries).ToString()));

        //    return configuration;
        //}

        //protected internal virtual IList<string> ReadJobIds(JObject jsonObject)
        //{
        //    var objects = jsonObject.GetValue(JobIds);
        //    IList<string> jobIds = new List<string>();
        //    foreach (var @object in objects)
        //        jobIds.Add(@object.ToString());
        //    return jobIds;
        //}
    }
}