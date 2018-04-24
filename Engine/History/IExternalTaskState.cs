namespace ESS.FW.Bpm.Engine.History
{
    public interface IExternalTaskState
    {
        int StateCode { get; }

        ///////////////////////////////////////////////////// default implementation
    }

    public static class ExternalTaskStateFields
    {
        public static readonly IExternalTaskState Created = new ExternalTaskStateExternalTaskStateImpl(0, "created");
        public static readonly IExternalTaskState Failed = new ExternalTaskStateExternalTaskStateImpl(1, "failed");

        public static readonly IExternalTaskState Successful = new ExternalTaskStateExternalTaskStateImpl(2,
            "successful");

        public static readonly IExternalTaskState Deleted = new ExternalTaskStateExternalTaskStateImpl(3, "deleted");
    }

    public class ExternalTaskStateExternalTaskStateImpl : IExternalTaskState
    {
        protected internal readonly string Name;

        public readonly int stateCode;

        public ExternalTaskStateExternalTaskStateImpl(int stateCode, string @string)
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
            var other = (ExternalTaskStateExternalTaskStateImpl) obj;
            return stateCode == other.stateCode;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}