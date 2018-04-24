namespace ESS.FW.Bpm.Engine.Impl.Digest
{
    public class Default16ByteSaltGenerator : Base64EncodedSaltGenerator
    {
        protected internal override int? SaltLengthInByte { get; } = 16;
    }
}