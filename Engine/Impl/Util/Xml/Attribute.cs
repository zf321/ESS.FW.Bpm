namespace ESS.FW.Bpm.Engine.Impl.Util.xml
{
    /// <summary>
    ///     特性数据结构
    ///     
    /// </summary>
    public class Attribute
    {
        protected internal string name;

        protected internal string uri;

        protected internal string value;

        /// <summary>
        ///     特性构造函数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public Attribute(string name, string value)
        {
            this.name = name;
            this.value = value;
        }

        /// <summary>
        ///     特性构造函数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="uri"></param>
        public Attribute(string name, string value, string uri) : this(name, value)
        {
            this.uri = uri;
        }

        /// <summary>
        ///     名称(key)
        /// </summary>
        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        ///     值
        /// </summary>
        public virtual string Value
        {
            get { return value; }
            set { this.value = value; }
        }

        /// <summary>
        ///     资源地址
        /// </summary>
        public virtual string Uri
        {
            get { return uri; }
            set { uri = value; }
        }
    }
}