namespace ESS.FW.Bpm.Engine.Variable.Value
{
    /// <summary>
    ///     A <seealso cref="ITypedValue" /> for which a serialized value can be obtained and specified
    /// </summary>
    public interface ISerializableValue : ITypedValue
    {
        /// <summary>
        ///     Returns true in case the value is deserialized. If this method returns true,
        ///     it is safe to call the <seealso cref="#getValue()" /> method
        /// </summary>
        /// <returns> true if the object is deserialized. </returns>
        bool IsDeserialized { get; }

        ///// <summary>
        ///// Returns the value or null in case the value is null.
        ///// </summary>
        ///// <returns> the value represented by this TypedValue. </returns>
        ///// <exception cref="IllegalStateException"> in case the value is not deserialized. See <seealso cref="#isDeserialized()"/>. </exception>
        //object Value {get;}

        /// <summary>
        ///     Returns the serialized value. In case the serializaton data format
        ///     (as returned by <seealso cref="#getSerializationDataFormat()" />) is not text based,
        ///     a base 64 encoded representation of the value is returned
        ///     The serialized value is a snapshot of the state of the value as it is
        ///     serialized to the process engine database.
        /// </summary>
        string ValueSerialized { get; }

        /// <summary>
        ///     The serialization format used to serialize this value.
        /// </summary>
        /// <returns> the serialization format used to serialize this variable. </returns>
        string SerializationDataFormat { get; }

        //ISerializableValueType Type {get;}
    }
}