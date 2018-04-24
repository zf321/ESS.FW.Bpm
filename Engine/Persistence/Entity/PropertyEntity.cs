using System;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Impl.DB;
using ESS.Shared.Entities.Bpm;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{
    /// <summary>
    ///  
    /// </summary>
    [Serializable]
    public class PropertyEntity : IDbEntity, IHasDbRevision
    {
        protected internal static readonly EnginePersistenceLogger Log = ProcessEngineLogger.PersistenceLogger;
        public PropertyEntity()
        {
        }

        public PropertyEntity(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }

        public virtual string Name { get; set; }


        public virtual int Revision { get; set; }


        public virtual string Value { get; set; }


        // persistent object methods ////////////////////////////////////////////////

        public virtual string Id { get; set; }

        public virtual object GetPersistentState()
        {
            return Value;
        }


        public virtual int RevisionNext
        {
            get
            {
                return Revision + 1;
            }
        }

        public override string ToString()
        {
            return this.GetType().Name + "[name=" + Name + ", revision=" + Revision + ", value=" + Value + "]";
        }
    }
}