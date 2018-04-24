namespace ESS.FW.Bpm.Engine.Runtime
{
    /// <summary>
    ///     
    /// </summary>
    public interface IProcessInstanceModificationInstantiationBuilder : IProcessInstanceModificationBuilder,
        IActivityInstantiationBuilder<IProcessInstanceModificationInstantiationBuilder>
    {
    }
}