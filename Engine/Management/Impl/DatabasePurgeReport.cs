using System;
using System.Collections.Generic;
using System.Text;

namespace ESS.FW.Bpm.Engine.Management.Impl
{
    /// <summary>
    ///      
    /// </summary>
    public class DatabasePurgeReport : IPurgeReporting<long>
    {
        /// <summary>
        ///     Key: table name
        ///     Value: entity count
        /// </summary>
        internal IDictionary<string, long> DeletedEntities = new Dictionary<string, long>();

        public void AddPurgeInformation(string key, long value)
        {
            throw new NotImplementedException();
        }

        public virtual IDictionary<string, long> PurgeReport
        {
            get { return DeletedEntities; }
        }

        public virtual string PurgeReportAsString
        {
            get
            {
                var builder = new StringBuilder();
                foreach (var key in DeletedEntities.Keys)
                    builder.Append("Table: ")
                        .Append(key)
                        .Append(" contains: ")
                        .Append(GetReportValue(key))
                        .Append(" rows\n");
                return builder.ToString();
            }
        }
        
        public virtual bool ContainsReport(string key)
        {
            return DeletedEntities.ContainsKey(key);
        }

        public virtual bool Empty
        {
            get { return DeletedEntities.Count == 0; }
        }

        public virtual void AddPurgeInformation(string key, long? value)
        {
            //deletedEntities[key] = value;
        }

        public virtual long GetReportValue(string key)
        {
            return DeletedEntities[key];
        }
    }
}