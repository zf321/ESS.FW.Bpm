namespace ESS.FW.Bpm.Engine.Impl.Pvm.Process
{
    /// <summary>
    ///     矩形图形数据接口
    ///     Marks implementing class as having DI-information bounded by a rectangle
    ///     at a certain location.
    ///     
    /// </summary>
    public interface IHasDiBounds
    {
        int Width { get; set; }
        int Height { get; set; }
        int X { get; set; }
        int Y { get; set; }
    }
}