namespace ESS.FW.Bpm.Engine.Impl.Pvm
{
    /// <summary>
    ///      
    ///     
    /// </summary>
    public interface IReadOnlyProcessDefinition : IPvmScope
    {
        string Name { get; }

        string Description { get; }

        IPvmActivity Initial { get; }

        string DiagramResourceName { get; }
    }
}