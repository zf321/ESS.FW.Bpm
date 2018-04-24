using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.Impl.oplog
{
    /// <summary>
    ///     <para>Provides information about user operations.</para>
    ///     <para>
    ///         One context object can contain many entries. An entry represents one operation on a set of
    ///         resources of the same type. One such operation can change multiple properties on these entities.
    ///         For example, more than one entry is needed when a cascading command is logged. Then there is an entry
    ///         for the changes performed on the addressed resource type as well as entries for those resource types that
    ///         are affected by the cascading behavior.
    ///     </para>
    ///     
    ///     
    /// </summary>
    public class UserOperationLogContext
    {
        protected internal IList<UserOperationLogContextEntry> entries;

        protected internal string operationId;
        protected internal string userId;

        public UserOperationLogContext()
        {
            entries = new List<UserOperationLogContextEntry>();
        }

        public virtual string UserId
        {
            get { return userId; }
            set { userId = value; }
        }


        public virtual string OperationId
        {
            get { return operationId; }
            set { operationId = value; }
        }

        public virtual IList<UserOperationLogContextEntry> Entries
        {
            get { return entries; }
        }


        public virtual void AddEntry(UserOperationLogContextEntry entry)
        {
            entries.Add(entry);
        }
    }
}