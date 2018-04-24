namespace ESS.FW.Bpm.Engine.History
{
    /// <summary>
    ///     
    /// </summary>
    public interface IIncidentState
    {
        int StateCode { get; }

        ///////////////////////////////////////////////////// default implementation
    }

    public static class IncidentStateFields
    {
        public static readonly IIncidentState Default = new IncidentStateIncidentStateImpl(0, "open");
        public static readonly IIncidentState Resolved = new IncidentStateIncidentStateImpl(1, "resolved");
        public static readonly IIncidentState Deleted = new IncidentStateIncidentStateImpl(2, "deleted");
    }

    public class IncidentStateIncidentStateImpl : IIncidentState
    {
        protected internal readonly string Name;

        public readonly int stateCode;

        public IncidentStateIncidentStateImpl(int suspensionCode, string @string)
        {
            stateCode = suspensionCode;
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
            var other = (IncidentStateIncidentStateImpl) obj;
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