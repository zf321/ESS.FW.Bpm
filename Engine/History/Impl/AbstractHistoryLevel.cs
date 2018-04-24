using ESS.FW.Bpm.Engine.History.Impl.Event;

namespace ESS.FW.Bpm.Engine.History.Impl
{
    /// <summary>
    ///     
    /// </summary>
    public abstract class AbstractHistoryLevel : IHistoryLevel
    {
        public abstract bool IsHistoryEventProduced(HistoryEventTypes eventType, object entity);
        public abstract string Name { get; }
        public abstract int Id { get; }

        public override int GetHashCode()
        {
            const int prime = 31;
            var result = 1;
            result = prime*result + Id;
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
            var other = (AbstractHistoryLevel) obj;
            if (Id != other.Id)
                return false;
            return true;
        }

        public override string ToString()
        {
            return string.Format("{0}(name={1}, id={2:D})", GetType().Name, Name, Id);
        }
    }
}