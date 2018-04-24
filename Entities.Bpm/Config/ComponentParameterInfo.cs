using System;

namespace ESS.Shared.Entities.Bpm.Config
{
    /// <summary>
    /// 组件方法参数信息
    /// </summary>
    [Serializable]
    public class ComponentParameterInfo : ComponentTypeInfo, IDbEntity, IRevision
    {
        /// <summary>
        /// 参数名称
        /// </summary>
        public string ParameterName { get; set; }

        /// <summary>
        /// 参数位置
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// 是否引用参数
        /// </summary>
        public bool IsByRef { get; set; }

        /// <summary>
        /// 是否输出参数
        /// </summary>
        public bool IsOut { get; set; }

        /// <summary>
        /// 是否可变数组参数
        /// </summary>
        public bool IsParamArray { get; set; }

        /// <summary>
        /// 是否可选参数
        /// </summary>
        public bool IsOptional { get; set; }

        /// <summary>
        /// 是否有默认值
        /// </summary>
        public bool HasDefaultValue { get; set; }

        /// <summary>
        /// 默认值
        /// </summary>
        public string DefaultValue { get; set; }

        /// <summary>
        /// 方法Id
        /// </summary>
        public string MethodId { get; set; }

        public int Revision { get; set; }
        public string Id { get; set; }

        public object GetPersistentState()
        {
            throw new NotImplementedException();
        }
    }
}
