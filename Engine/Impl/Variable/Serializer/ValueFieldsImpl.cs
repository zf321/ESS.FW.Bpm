using ESS.FW.Bpm.Engine.Variable.Serializer;

namespace ESS.FW.Bpm.Engine.Impl.Variable.Serializer
{
    /// <summary>
    /// </summary>
    public class ValueFieldsImpl : IValueFields
    {
        public virtual string Name { get; } = null;

        public virtual string TextValue { get; set; }


        public virtual string TextValue2 { get; set; }


        public virtual long? LongValue { get; set; }


        public virtual double? DoubleValue { get; set; }


        public virtual byte[] ByteArrayValue { get; set; }
        public string ByteArrayId { get; set; }
    }
}