namespace ESS.FW.Bpm.Engine.Impl.Pvm
{
    /// <summary>
    ///      
    /// </summary>
    public interface IPvmTransition : IPvmProcessElement
    {
        IPvmActivity Source { get; set; }

        IPvmActivity Destination { get; set; }
    }
}