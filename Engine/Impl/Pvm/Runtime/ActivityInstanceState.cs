
 

namespace ESS.FW.Bpm.Engine.Impl.Pvm.Runtime
{
    /// <summary>
    ///     Contains a predefined set of states activity instances may be in
    ///     during the execution of a process instance.
    ///     
    /// </summary>
    public interface IActivityInstanceState
    {
        int StateCode { get; }

        ///////////////////////////////////////////////////// default implementation
    }

    public static class ActivityInstanceStateFields
    {
        public static readonly IActivityInstanceState Default = new ActivityInstanceStateActivityInstanceStateImpl(0,
            "default");

        public static readonly IActivityInstanceState ScopeComplete =
            new ActivityInstanceStateActivityInstanceStateImpl(1, "scopeComplete");

        public static readonly IActivityInstanceState Canceled = new ActivityInstanceStateActivityInstanceStateImpl(2,
            "canceled");

        public static readonly IActivityInstanceState Starting = new ActivityInstanceStateActivityInstanceStateImpl(3,
            "starting");

        public static readonly IActivityInstanceState Ending = new ActivityInstanceStateActivityInstanceStateImpl(4,
            "ending");
    }

    public class ActivityInstanceStateActivityInstanceStateImpl : IActivityInstanceState
    {
        protected internal readonly string Name;

        public readonly int stateCode;

        public ActivityInstanceStateActivityInstanceStateImpl(int suspensionCode, string @string)
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
            var other = (ActivityInstanceStateActivityInstanceStateImpl) obj;
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

