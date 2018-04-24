using System.Collections.Generic;
using System.Text;
using ESS.FW.Bpm.Engine.Management.Impl;


namespace ESS.FW.Bpm.Engine.Persistence.Deploy.Cache
{



	/// <summary>
	///  
	/// </summary>
	public class CachePurgeReport : IPurgeReporting<ICollection<string>>
	{

	  public const string ProcessDefCache = "PROC_DEF_CACHE";
	  public const string BpmnModelInstCache = "BPMN_MODEL_INST_CACHE";
	  public const string CaseDefCache = "CASE_DEF_CACHE";
	  public const string CaseModelInstCache = "CASE_MODEL_INST_CACHE";
	  public const string DmnDefCache = "DMN_DEF_CACHE";
	  public const string DmnReqDefCache = "DMN_REQ_DEF_CACHE";
	  public const string DmnModelInstCache = "DMN_MODEL_INST_CACHE";

	  /// <summary>
	  /// Key: cache name
	  /// Value: valuesI
	  /// </summary>
	  internal IDictionary<string, ICollection<string>> DeletedCache = new Dictionary<string, ICollection<string>>();

	  public  void AddPurgeInformation(string key, ICollection<string> value)
	  {
		DeletedCache[key] = new HashSet<string>(value);
	  }

	  public  IDictionary<string, ICollection<string>> PurgeReport
	  {
		  get
		  {
			return DeletedCache;
		  }
	  }

	  public  string PurgeReportAsString
	  {
		  get
		  {
			StringBuilder builder = new StringBuilder();
			foreach (string key in DeletedCache.Keys)
			{
			  builder.Append("Cache: ").Append(key).Append(" contains: ").Append(GetReportValue(key)).Append("\n");
			}
			return builder.ToString();
		  }
	  }

	  public ICollection<string> GetReportValue(string key)
	  {
		return DeletedCache[key];
	  }

	  public  bool ContainsReport(string key)
	  {
		return DeletedCache.ContainsKey(key);
	  }

	  public virtual bool Empty
	  {
		  get
		  {
			return DeletedCache.Count == 0;
		  }
	  }
	}

}