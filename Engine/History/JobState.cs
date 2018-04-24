namespace ESS.FW.Bpm.Engine.History
{
    /// <summary>
    ///     
    /// </summary>
    public interface IJobState
    {
        int StateCode { get; }

        ///////////////////////////////////////////////////// default implementation
    }

    public static class JobStateFields
    {
        public static readonly IJobState Created = new JobStateJobStateImpl(0, "created");
        public static readonly IJobState Failed = new JobStateJobStateImpl(1, "failed");
        public static readonly IJobState Successful = new JobStateJobStateImpl(2, "successful");
        public static readonly IJobState Deleted = new JobStateJobStateImpl(3, "deleted");
    }

    public class JobStateJobStateImpl : IJobState
    {
        protected internal readonly string Name;

        public readonly int stateCode;

        public JobStateJobStateImpl(int stateCode, string @string)
        {
            this.stateCode = stateCode;
            Name = @string;
        }

        public virtual int StateCode
        {
            get { return stateCode; }
        }

        public override int GetHashCode()
        {
            const int prime = 31;
            var result = 1;
            result = prime*result + stateCode;
            return result;
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;
            if (obj == null)
                return false;
            if (GetType() != obj.GetType())
                return false;
            var other = (JobStateJobStateImpl) obj;
            if (stateCode != other.stateCode)
                return false;
            return true;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}