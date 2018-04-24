using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.Impl
{
    /// <summary>
    ///      
    /// </summary>
    public class Direction
    {
        private static readonly IDictionary<string, Direction> Directions = new Dictionary<string, Direction>();

        public static readonly Direction Ascending = new Direction("asc");
        public static readonly Direction Descending = new Direction("desc");

        private readonly string _name;

        public Direction(string name)
        {
            this._name = name;
            Directions[name] = this;
        }

        public virtual string Name
        {
            get { return _name; }
        }

        public override string ToString()
        {
            return "Direction[" + "name=" + _name + "]";
        }

        public static Direction FindByName(string directionName)
        {
            return Directions[directionName];
        }
    }
}