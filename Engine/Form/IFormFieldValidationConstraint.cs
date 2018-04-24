namespace ESS.FW.Bpm.Engine.Form
{
    /// <summary>
    ///     
    /// </summary>
    public interface IFormFieldValidationConstraint
    {
        /// <returns> the name of the validation constraint </returns>
        string Name { get; }

        /// <returns> the configuration of the validation constraint. </returns>
        object Configuration { get; }
    }
}