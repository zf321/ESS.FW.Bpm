namespace ESS.FW.Bpm.Engine.Form.Impl.Engine
{
    /// <summary>
    ///      
    /// </summary>
    public interface IFormEngine
    {
        string Name { get; }
        object RenderStartForm(IStartFormData startForm);
        object RenderTaskForm(ITaskFormData taskForm);
    }
}