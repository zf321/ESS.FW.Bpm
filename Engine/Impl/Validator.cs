namespace ESS.FW.Bpm.Engine.Impl
{
    /// <summary>
    ///     Validators must be equal (in terms of <seealso cref="#equals(Object)" />) if they validate
    ///     the exact same thing.
    ///     
    /// </summary>
    public interface IValidator<T>
    {
        void Validate(T obj);
    }
}