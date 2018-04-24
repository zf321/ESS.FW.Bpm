using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Variable.Serializer
{
    /// <summary>
    /// </summary>
    public interface IValueFields : INameable
    {
        string ByteArrayId { get; set; }
        string TextValue { get; set; }

        string TextValue2 { get; set; }

        long? LongValue { get; set; }

        double? DoubleValue { get; set; }

        byte[] ByteArrayValue { get; set; }
    }
}