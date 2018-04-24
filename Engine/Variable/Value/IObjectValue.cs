using ESS.FW.Bpm.Engine.Variable.Type;

namespace ESS.FW.Bpm.Engine.Variable.Value
{
    /// <summary>
    ///     <para>A typed value representing a Java Object.</para>
    /// </summary>
    public interface IObjectValue : ISerializableValue
    {
        /// <summary>
        ///     Returns the Class this object is an instance of.
        /// </summary>
        /// <returns> the Class this object is an instance of </returns>
        /// <exception cref="IllegalStateException">
        ///     in case the object is not deserialized. See
        ///     <seealso cref="#isDeserialized()" />.
        /// </exception>
        System.Type ObjectType { get; }

        /// <summary>
        ///     A String representation of the Object's type name.
        ///     Usually the canonical class name of the Java Class this object
        ///     is an instance of.
        /// </summary>
        /// <returns> the Object's type name. </returns>
        string ObjectTypeName { get; }

        /// <summary>
        ///     Returns the object provided by this VariableValue. Allows type-safe access to objects
        ///     by passing in the class.
        /// </summary>
        /// <param name="type"> the java class the value should be cast to </param>
        /// <returns> the object represented by this TypedValue. </returns>
        /// <exception cref="IllegalStateException">
        ///     in case the object is not deserialized. See
        ///     <seealso cref="#isDeserialized()" />.
        /// </exception>
        object GetValue(System.Type type);
    }
}