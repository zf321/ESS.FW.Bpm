namespace ESS.FW.Bpm.Engine.Variable.Value
{
    /// <summary>
    ///     <para>Represents a serialization data format.</para>
    /// </summary>
    /// <seealso cref= SerializationDataFormats
    /// </seealso>
    public interface ISerializationDataFormat
    {
        /// <summary>
        ///     The name of the dataformat. Example: "application/json"
        /// </summary>
        /// <returns> the name of the dataformat. </returns>
        string Name { get; }
    }
}