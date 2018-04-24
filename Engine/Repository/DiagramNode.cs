using System;

namespace ESS.FW.Bpm.Engine.Repository
{
    /// <summary>
    ///     Stores position and dimensions of a diagram node.
    ///     
    /// </summary>
    [Serializable]
    public class DiagramNode : DiagramElement
    {
        private const long SerialVersionUid = 1L;
        private double? _height;
        private double? _width;

        private double? _x;
        private double? _y;

        public DiagramNode()
        {
        }

        public DiagramNode(string id) : base(id)
        {
        }

        public DiagramNode(string id, double? x, double? y, double? width, double? height) : base(id)
        {
            this._x = x;
            this._y = y;
            this._width = width;
            this._height = height;
        }

        public virtual double? X
        {
            get { return _x; }
            set { _x = value; }
        }


        public virtual double? Y
        {
            get { return _y; }
            set { _y = value; }
        }


        public virtual double? Width
        {
            get { return _width; }
            set { _width = value; }
        }


        public virtual double? Height
        {
            get { return _height; }
            set { _height = value; }
        }

        public override bool Node
        {
            get { return true; }
        }

        public override bool Edge
        {
            get { return false; }
        }


        public override string ToString()
        {
            return base.ToString() + ", x=" + X + ", y=" + Y + ", width=" + Width + ", height=" + Height;
        }
    }
}