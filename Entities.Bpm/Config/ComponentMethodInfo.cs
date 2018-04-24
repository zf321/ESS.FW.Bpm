using System;
using System.Collections.Generic;

namespace ESS.Shared.Entities.Bpm.Config
{
    /// <summary>
    /// 组件方法信息
    /// </summary>
    [Serializable]
    public class ComponentMethodInfo : ComponentTypeInfo, IDbEntity, IRevision
    {
        /// <summary>
        /// 方法名称-仅名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 方法定义名
        /// </summary>
        public string MethodName { get; set; }

        /// <summary>
        /// 方法签名
        /// </summary>
        public string MethodSignature { get; set; }

        /// <summary>
        /// 是否静态方法
        /// </summary>
        public bool IsStatic { get; set; }

        /// <summary>
        /// 返回参数
        /// </summary>
        public ComponentParameterInfo ReturnParameter { get; set; }

        /// <summary>
        /// 参数列表
        /// </summary>
        public virtual ICollection<ComponentParameterInfo> Parameters { get; set; }

        /// <summary>
        /// 自定义标签
        /// </summary>
        public string Tags { get; set; }

        public string Id { get; set; }
        public int Revision { get; set; }

        public object GetPersistentState()
        {
            throw new NotImplementedException();
        }
    }
}
