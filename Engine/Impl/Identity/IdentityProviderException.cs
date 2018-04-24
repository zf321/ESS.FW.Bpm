namespace ESS.FW.Bpm.Engine.Impl.Identity
{
    /// <summary>
    ///     <para>Marks an exception triggered inside an identity provider implementation</para>
    ///     
    /// </summary>
    public class IdentityProviderException : ProcessEngineException
    {
        

        public IdentityProviderException(string message) : base(message)
        {
        }

        public IdentityProviderException(string message, System.Exception cause) : base(message, cause)
        {
        }
    }
}