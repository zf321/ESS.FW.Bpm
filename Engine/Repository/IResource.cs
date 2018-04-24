namespace ESS.FW.Bpm.Engine.Repository
{
    /// <summary>
    ///     
    /// </summary>
    public interface IResource
    {
        string Id { get; }

        string Name { get; }

        string DeploymentId { get; }
    }
}