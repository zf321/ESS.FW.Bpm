using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.Batch.Impl.Deletion
{
    /// <summary>
    ///     Configuration object that is passed to the Job that will actually perform execution of
    ///     deletion.
    ///     <para>
    ///         This object will be serialized and persisted as run will be performed asynchronously.
    ///         
    ///     </para>
    /// </summary>
    /// <seealso cref= org.camunda.bpm.engine.impl.batch.deletion.DeleteProcessInstanceBatchConfigurationJsonConverter
    /// </seealso>
    public class DeleteProcessInstanceBatchConfiguration : BatchConfiguration
    {
        protected internal string deleteReason;

        public DeleteProcessInstanceBatchConfiguration(IList<string> ids) : base(ids)
        {
        }

        public DeleteProcessInstanceBatchConfiguration(IList<string> ids, string deleteReason) : base(ids)
        {
            this.deleteReason = deleteReason;
        }

        public virtual string DeleteReason
        {
            get { return deleteReason; }
            set { deleteReason = value; }
        }
    }
}