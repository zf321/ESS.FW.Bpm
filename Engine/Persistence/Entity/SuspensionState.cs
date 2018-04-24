namespace ESS.FW.Bpm.Engine.Persistence.Entity
{
    /// <summary>
    /// Contains a predefined set of states for process definitions and process instances
    /// 
    /// 
    /// </summary>
    public interface ISuspensionState
    {

        int StateCode { get; }

        string Name { get; }

        ///////////////////////////////////////////////////// default implementation
    }

    public static class SuspensionStateFields
    {
        public static readonly ISuspensionState Active = new SuspensionStateSuspensionStateImpl(1, "active");
        public static readonly ISuspensionState Suspended = new SuspensionStateSuspensionStateImpl(2, "suspended");
    }

    public class SuspensionStateSuspensionStateImpl : ISuspensionState
    {

        public readonly int stateCode;
        protected internal readonly string name;

        public SuspensionStateSuspensionStateImpl(int suspensionCode, string @string)
        {
            this.stateCode = suspensionCode;
            this.name = @string;
        }

        public virtual int StateCode
        {
            get
            {
                return stateCode;
            }
        }

        public override int GetHashCode()
        {
            const int prime = 31;
            int result = 1;
            result = prime * result + stateCode;
            return result;
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
            SuspensionStateSuspensionStateImpl other = (SuspensionStateSuspensionStateImpl)obj;
            if (stateCode != other.stateCode)
            {
                return false;
            }
            return true;
        }

        public override string ToString()
        {
            return name;
        }

        public string Name
        {
            get
            {
                return name;
            }
        }
    }

}