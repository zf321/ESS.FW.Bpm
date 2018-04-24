namespace ESS.FW.Bpm.Engine.Form
{
    /// <summary>
    ///     Used to indicate the type on a <seealso cref="IFormProperty" />.
    ///      
    /// </summary>
    public interface IFormType
    {
        /// <summary>
        ///     Name for the form type.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Retrieve type specific extra information like
        ///     the list of values for enum types or the format
        ///     for date types. Look in the userguide for
        ///     which extra information keys each type provides
        ///     and what return type they give.
        /// </summary>
        object GetInformation(string key);
    }
}