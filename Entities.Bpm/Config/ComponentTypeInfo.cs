using System;

namespace ESS.Shared.Entities.Bpm.Config
{
    /// <summary>
    /// 组件类型定义信息
    /// </summary>
    [Serializable]
    public class ComponentTypeInfo
    {
        /// <summary>
        /// 类名
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// 类全名
        /// </summary>
        public string TypeFullName { get; set; }

        /// <summary>
        /// 类命名空间
        /// </summary>
        public string TypeNamespace { get; set; }

        /// <summary>
        /// 程序集名
        /// </summary>
        public string AssemblyName { get; set; }

        /// <summary>
        /// 程序集全名
        /// </summary>
        public string AssemblyFullName { get; set; }

        /// <summary>
        /// 程序集限定名
        /// </summary>
        public string AssemblyQualifiedName { get; set; }

        /// <summary>
        /// 描述信息
        /// </summary>
        public string Description { get; set; }
    }
}
