namespace ESS.FW.Bpm.Engine.Impl.Pvm.Process
{
    /// <summary>
    ///     Object indicating that a <seealso cref="ProcessDefinitionImpl" /> is a participant in a collaboration (pool).
    ///     Currently only used to store graphical information and the pool name.
    ///     
    /// </summary>
    [System.Serializable]
    public class ParticipantProcess : IHasDiBounds
    {
        public virtual string Id { set; get; }


        public virtual string Name { get; set; }


        public virtual int X { get; set; } = -1;


        public virtual int Y { get; set; } = -1;


        public virtual int Width { get; set; } = -1;


        public virtual int Height { get; set; } = -1;
    }
}