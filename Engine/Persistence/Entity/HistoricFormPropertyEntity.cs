

using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.History.Impl.Event;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{


    /// <summary>
    ///  
    /// </summary>
    public class HistoricFormPropertyEntity : HistoricFormPropertyEventEntity, IHistoricFormProperty, IHistoricFormField
    {

        private const long SerialVersionUid = 1L;


        public virtual string FieldId
        {
            get
            {
                return PropertyId;
            }
            set
            {
                this.PropertyId = value;
            }
        }

        public virtual object FieldValue
        {
            get
            {
                return PropertyValue;
            }
            set
            {
                this.PropertyValue = (string)value;
            }
        }

        // public string UserOperationId { get; }
    }

}