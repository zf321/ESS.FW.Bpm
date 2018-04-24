using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine
{
    /// <summary>
    ///     bpm源码所有setProperty方法实现的基类
    /// </summary>
    public abstract class SetPropertyBase
    {
        private IDictionary<string, object> _properties;

        /// <summary>
        ///     赋值,key重复直接覆盖
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetProperty<T>(string key, T value)
        {
            if (_properties == null)
                _properties = new Dictionary<string, object>();
            _properties[key] = value;
        }

        /// <summary>
        ///     没有为默认值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public T GetProperty<T>(string key)
        {
            if ((_properties == null) || !_properties.ContainsKey(key))
                return default(T);
            return (T) _properties[key];
        }
    }
}