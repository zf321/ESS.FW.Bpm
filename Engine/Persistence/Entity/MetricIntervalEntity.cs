using System;
using ESS.FW.Bpm.Engine.Impl.DB;
using ESS.FW.Bpm.Engine.Management;
using ESS.Shared.Entities.Bpm;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{
    /// <summary>
    ///  
    /// </summary>
    [Serializable]
    public class MetricIntervalEntity : IMetricIntervalValue, IDbEntity
    {
        public MetricIntervalEntity(DateTime timestamp, string name, string reporter)
        {
            this.Timestamp = timestamp;
            this.Name = name;
            this.Reporter = reporter;
        }

        /// <summary>
        /// Ctor will be used by Mybatis
        /// </summary>
        /// <param name="timestamp"> </param>
        /// <param name="name"> </param>
        /// <param name="reporter"> </param>
        public MetricIntervalEntity(long? timestamp, string name, string reporter)
        {
            this.Timestamp = new DateTime((long)timestamp);
            this.Name = name;
            this.Reporter = reporter;
        }

        public DateTime GetTimestamp()
        {
            return Timestamp;
        }

        public virtual void SetTimestamp(DateTime timestamp)
        {
            this.Timestamp = timestamp;
        }

        public virtual string Name { get; set; }


        public virtual string Reporter { get; set; }

        public virtual DateTime Timestamp { get; set; }

        public virtual long Value { get; set; }


        public string Id
        {
            get
            {
                return Name + Reporter + Timestamp.Ticks.ToString();
            }
            set
            {
                throw new System.NotSupportedException("Not supported yet.");
            }
        }


        public object GetPersistentState()
        {
            return typeof(MetricIntervalEntity);
        }

        public override int GetHashCode()
        {
            int hash = 7;
            hash = 67 * hash + (this.Timestamp != null ? this.Timestamp.GetHashCode() : 0);
            hash = 67 * hash + (this.Name != null ? this.Name.GetHashCode() : 0);
            hash = 67 * hash + (this.Reporter != null ? this.Reporter.GetHashCode() : 0);
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
            {
                return true;
            }
            if (obj == null)
            {
                return false;
            }
            if (this.GetType() != obj.GetType())
            {
                return false;
            }
            MetricIntervalEntity other = (MetricIntervalEntity)obj;
            if ((this.Name == null) ? (other.Name != null) : !this.Name.Equals(other.Name))
            {
                return false;
            }
            if ((this.Reporter == null) ? (other.Reporter != null) : !this.Reporter.Equals(other.Reporter))
            {
                return false;
            }
            if (this.Timestamp != other.Timestamp && (this.Timestamp == null || !this.Timestamp.Equals(other.Timestamp)))
            {
                return false;
            }
            return true;
        }

    }

}