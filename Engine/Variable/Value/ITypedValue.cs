using ESS.FW.Bpm.Engine.Variable.Type;

namespace ESS.FW.Bpm.Engine.Variable.Value
{
    /// <summary>
    ///     <para>
    ///         A <seealso cref="ITypedValue" /> is a value with additional type information (the <seealso cref="IValueType" />
    ///         ).
    ///         TypedValues are used for representing variable values.
    ///     </para>
    /// </summary>
    public interface ITypedValue
    {
        /// <summary>
        ///     The actual value. May be null in case the value is null.
        /// </summary>
        /// <returns> the value </returns>
        object Value { get; }

        /// <summary>
        ///     The type of the value. See ValueType for a list of built-in ValueTypes.
        /// </summary>
        /// <returns> the type of the value. </returns>
        IValueType Type { get; }
    }
}